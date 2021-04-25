﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcctOpeningImageValidationAPI.Helpers;
using AcctOpeningImageValidationAPI.Models;
using AcctOpeningImageValidationAPI.Repository.Services.Implementation;
using AcctOpeningImageValidationAPI.Repository.Services.Request;
using AcctOpeningImageValidationAPI.Repository.Services.Response;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AcctOpeningImageValidationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LivenessController : ControllerBase
    {
        private readonly RestClientService _restClientService;
        private readonly AppSettings _appSettings;

        private static IFaceClient client;
        private readonly double _headPitchMaxThreshold = 25;

        private readonly double _headPitchMinThreshold = -15;

        private readonly double _headYawMaxThreshold = 20;

        private readonly double _headYawMinThreshold = -20;

        private readonly double _headRollMaxThreshold = 20;

        private readonly double _headRollMinThreshold = -20;
        private static bool processIdel = true;

        private static bool firstInProcess = true;

        private static int processStep = 1;


        private readonly static int activeFrames = 14;

        private readonly IHostingEnvironment _env;

        public LivenessController(IHostingEnvironment env,
                                  IOptions<AppSettings> option,
                                  RestClientService restClientService)
        {
            _env = env;
            client = new FaceClient(new ApiKeyServiceClientCredentials(option.Value.subscriptionKey)) //"e8ef40efa4704769860e661c210a0fc5"
            {
                Endpoint = option.Value.AzureFacialBaseUrl
            };
            _restClientService = restClientService;
            _appSettings = option.Value;
        }

        [HttpPost]
        [Route("/liveness/test-encryption")]
        public async Task<IActionResult> TestVideoEncrypted([FromBody] ImageRequest model)
        {
            var encryption = await Encryption.Encryption.Decrypt(model.Body, _appSettings.EncryptionKey, _appSettings.EncryptionIV);

            return Ok(encryption);
        }

        [HttpPost]
        [Route("/liveness")]
        public async Task<IActionResult> ProcessVideoFile([FromBody] ImageRequest model)
        {
            try
            {
                //TODO: Decrypt Data
                var encryption = await Encryption.Encryption.Decrypt(model.Body, _appSettings.EncryptionKey, _appSettings.EncryptionIV);

                var req = JsonConvert.DeserializeObject<FaceRequest>(model.Body);

                var fileName = "test.mp4";

                byte[] imageBytes = Convert.FromBase64String(req.VideoFile);

                string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "filess");

                if (!System.IO.File.Exists(FilePath))
                {
                    if (!Directory.Exists(FilePath))
                    {
                        Directory.CreateDirectory(FilePath);
                        System.IO.File.WriteAllBytes(Path.Combine(FilePath, fileName), imageBytes);
                    }
                }

                //Extract
                ExtractFrameFromVideo(FilePath, fileName);

                //Convert Image to stream
                var headPoseResult = await RunHeadGestureOnImageFrame(FilePath);
                var response = new Response
                {
                    HeadNodingDetected = headPoseResult.Item1,
                    HeadRollingDetected = headPoseResult.Item2,
                    HeadShakingDetected = headPoseResult.Item3
                };

                var encryptedValue = await Encryption.Encryption.Decrypt(JsonConvert.SerializeObject(response), _appSettings.EncryptionKey, _appSettings.EncryptionIV);

                return new OkObjectResult(HelperLib.ReponseClass.ReponseMethodGeneric("success", new FaceResponse { Body = encryptedValue }, true));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new UnprocessableEntityObjectResult(HelperLib.ReponseClass.ReponseMethod("Unable to validate face, please try again!", false));
            }
        }

        private void ExtractFrameFromVideo(string directory, string fiileName)
        {
            var mp4 = new MediaFile { Filename = Path.Combine(directory, fiileName) };
            using var engine = new Engine();


            engine.GetMetadata(mp4);

            var i = 0;
            while (i < mp4.Metadata.Duration.Seconds)
            {
                var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(i), };
                var outputFile = new MediaFile { Filename = string.Format("{0}\\image-{1}.jpeg", Path.Combine(directory), i) };
                engine.GetThumbnail(mp4, outputFile, options);
                i++;
            }
        }

        private async Task<Tuple<bool, bool, bool>> RunHeadGestureOnImageFrame(string filePath)
        {
            var headGestureResult = "";
            bool runStepOne = true;
            bool runStepTwo = true;
            bool runStepThree = true;
            bool stepOneComplete = false;
            bool stepTwoComplete = false;
            bool stepThreeComplete = false;

            var buff = new List<double>();

            var files = Directory.GetFiles(filePath);
            foreach (var item in files)
            {
                if (item.EndsWith("mp4"))
                {
                    continue;
                }
                var fileName = item.Split('\\').Last();
                var imageName = fileName.Split('.').First();

                //UPLOAD IMAGE TO FIREBASE 
                //var baseString = GetBaseStringFromImagePath(item);
                byte[] imageArray = System.IO.File.ReadAllBytes(item);
                //var uploadedContent = await FireBase.UploadDocumentAsync(fileName, imageName, item);

                DocumentUploadResponse uploadContent = new DocumentUploadResponse();

                using (var stream = new FileStream(item, FileMode.Open))
                {
                    uploadContent = await _restClientService.UploadDocument(new DocumentUploadRequest
                    {
                        FolderName = _appSettings.AzureContentFolderName,
                        Base64String = ConvertToBase64(stream),
                        FileName = "image-001"
                    });
                }

                // Submit image to API. 
                var attrs = new List<FaceAttributeType> { FaceAttributeType.HeadPose };

                //TODO: USE IMAGE URL OF NETWORK
                var faces = await client.Face.DetectWithUrlWithHttpMessagesAsync(uploadContent.Url, returnFaceId: false, returnFaceAttributes: attrs);
                if (faces.Body.Count <= 0)
                {
                    continue;
                }
                var headPose = faces.Body.First().FaceAttributes?.HeadPose;

                var pitch = headPose.Pitch;
                var roll = headPose.Roll;
                var yaw = headPose.Yaw;


                if (runStepOne)
                {
                    headGestureResult = StepOne(buff, pitch);
                    if (!string.IsNullOrEmpty(headGestureResult))
                    {
                        runStepOne = false;
                        stepOneComplete = true;
                    }
                }

                if (runStepTwo)
                {
                    headGestureResult = StepTwo(buff, pitch);
                    if (!string.IsNullOrEmpty(headGestureResult))
                    {
                        runStepTwo = false;
                        stepTwoComplete = true;
                    }
                }

                if (runStepThree)
                {
                    headGestureResult = StepThree(buff, pitch);
                    if (!string.IsNullOrEmpty(headGestureResult))
                    {
                        runStepThree = false;
                        stepThreeComplete = true;
                    }

                }
            }
            return new Tuple<bool, bool, bool>(stepOneComplete, stepTwoComplete, stepThreeComplete);
        }

        private string StepOne(List<double> buff, double pitch)
        {
            buff.Add(pitch);
            if (buff.Count > activeFrames)
            {
                buff.RemoveAt(0);
            }

            var max = buff.Max();
            var min = buff.Min();

            if (max > _headPitchMaxThreshold && min < _headPitchMinThreshold)
            {
                return "Nodding Detected success.";
            }
            else
            {
                return null;
            }
        }

        private string StepTwo(List<double> buff, double yaw)
        {
            buff.Add(yaw);
            if (buff.Count > activeFrames)
            {
                buff.RemoveAt(0);
            }

            var max = buff.Max();
            var min = buff.Min();

            if (min < _headYawMinThreshold && max > _headYawMaxThreshold)
            {
                return "Shaking Detected success.";
            }
            else
            {
                return null;
            }
        }

        private string StepThree(List<double> buff, double roll)
        {
            buff.Add(roll);
            if (buff.Count > activeFrames)
            {
                buff.RemoveAt(0);
            }

            var max = buff.Max();
            var min = buff.Min();

            if (min < _headRollMinThreshold && max > _headRollMaxThreshold)
            {
                Console.WriteLine("All head pose detection finished.");
                return "Rolling Detected success.";
            }
            else
            {
                return null;
            }
        }

        public string ConvertToBase64(Stream stream)
        {
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            string base64 = Convert.ToBase64String(bytes);
            return base64;
        }
    }
}


public class ImageRequest
{
    public string Body { get; set; }
}

public class LiveCameraResult
{
    public DetectedFace[] Faces { get; set; } = null;
}


public class Response
{
    public bool HeadNodingDetected { get; set; }
    public bool HeadShakingDetected { get; set; }
    public bool HeadRollingDetected { get; set; }
}


