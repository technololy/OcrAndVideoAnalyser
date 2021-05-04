using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AcctOpeningImageValidationAPI.Models;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.Extensions.Hosting;

namespace AcctOpeningImageValidationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
       
        private static IFaceClient client;

        private readonly double _headPitchMinThreshold = -9; //-15

        private readonly double _headYawMinThreshold = -20;

        private readonly double _headRollMinThreshold = -20;

        private readonly static int activeFrames = 14;

        private readonly IHostingEnvironment _env;

        public WeatherForecastController(IHostingEnvironment env)
        {
            _env = env;
            client = new FaceClient(new ApiKeyServiceClientCredentials("e8ef40efa4704769860e661c210a0fc5")) { Endpoint = "https://eastus.api.cognitive.microsoft.com" };
        }

        /// <summary>
        /// This Method Processes The Video File For Liveness Check
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ProcessVideoFile([FromBody] FaceRequest req)
        {
            try
            {
                var fileName = "test.mp4";

                byte[] imageBytes = Convert.FromBase64String(req.VideoFile);

                string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "filess");

                Directory.CreateDirectory(FilePath);

                System.IO.File.WriteAllBytes(Path.Combine(FilePath, fileName), imageBytes);

                if (!System.IO.File.Exists(FilePath))
                {
                    if (!Directory.Exists(FilePath))
                    {
                        Directory.CreateDirectory(FilePath);
                        System.IO.File.WriteAllBytes(Path.Combine(FilePath, fileName), imageBytes);
                    }
                }

                // Extract
                ExtractFrameFromVideo(FilePath, fileName);

                //Convert Image to stream
                var headPoseResult = await RunHeadGestureOnImageFrame(FilePath);

                var response = new LivenessCheckResponse
                {
                    HeadNodingDetected = headPoseResult.Item1,
                    HeadShakingDetected = headPoseResult.Item2,
                    HeadRollingDetected = headPoseResult.Item3
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Extraction of video frame is being done here
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="fiileName"></param>
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
                RotateImage(outputFile.Filename);
                i++;
            }
        }

        /// <summary>
        /// This method flip image orientation by 90 degree
        /// Please DO NOT ALTER this method!!!
        /// </summary>
        /// <param name="path"></param>
        private void RotateImage(string path)
        {
            Image image = Image.FromFile(path);
            image.RotateFlip(RotateFlipType.Rotate90FlipY);
            System.IO.File.Delete(path);
            image.Save(path);
            //image.Dispose();
        }

        /// <summary>
        /// Converts Image From Byte To Stream - For later use
        /// </summary>
        /// <param name="imagesInBytes"></param>
        /// <returns></returns>
        private Stream ConvertImageFromByteToStream(byte[] imagesInBytes)
        {
            var ms = new MemoryStream(imagesInBytes);
            return ConvertImageFromImageToStream(Image.FromStream(ms));
        }

        /// <summary>
        /// Convert Image From Image To Stream
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private Stream ConvertImageFromImageToStream(Image image)
        {
            var memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Jpeg);
            return memoryStream;
        }

        /// <summary>
        /// Help Detect Gesture for Head Pose
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private async Task<Tuple<bool, bool, bool>> RunHeadGestureOnImageFrame(string filePath)
        {
            var headGestureResult = string.Empty;
            bool runStepOne = true;
            bool runStepTwo = true;
            bool runStepThree = true;
            bool stepOneComplete = false;
            bool stepTwoComplete = false;
            bool stepThreeComplete = false;

            var buffPitch = new List<double>();
            var buffYaw = new List<double>();
            var buffRoll = new List<double>();

            var files = Directory.GetFiles(filePath);

            var items = files.Reverse();

            foreach (var item in items)
            {
                if (item.EndsWith("mp4"))
                {
                    continue;
                }

                var fileName = item.Split('\\').Last();

                var imageName = fileName.Split('.').First();

                //UPLOAD IMAGE TO FIREBASE 
                // var baseString = GetBaseStringFromImagePath(item);
                byte[] imageArray = System.IO.File.ReadAllBytes(item);
                 
                Stream stream = new MemoryStream(imageArray);

                // Submit image to API. 
                var attrs = new List<FaceAttributeType> { FaceAttributeType.HeadPose };

                //TODO: USE IMAGE URL OF NETWORK
                //var faces = await client.Face.DetectWithUrlWithHttpMessagesAsync(uploadedContent, returnFaceId: false, returnFaceAttributes: attrs);
                var faces = await client.Face.DetectWithStreamWithHttpMessagesAsync(stream, returnFaceId: false, returnFaceAttributes: attrs);
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
                    headGestureResult = StepOne(buffPitch, pitch);
                    if (!string.IsNullOrEmpty(headGestureResult))
                    {
                        runStepOne = false;
                        stepOneComplete = true;
                    }
                }

                if (runStepTwo)
                {
                    headGestureResult = StepTwo(buffYaw, yaw);
                    if (!string.IsNullOrEmpty(headGestureResult))
                    {
                        runStepTwo = false;
                        stepTwoComplete = true;
                    }
                }

                if (runStepThree)
                {
                    headGestureResult = StepThree(buffRoll, roll);
                    if (!string.IsNullOrEmpty(headGestureResult))
                    {
                        runStepThree = false;
                        stepThreeComplete = true;
                    }

                }
            }
            return new Tuple<bool, bool, bool>(stepOneComplete, stepTwoComplete, stepThreeComplete);
        }

        /// <summary>
        /// Step One Gesture
        /// </summary>
        /// <param name="buffPitch"></param>
        /// <param name="pitch"></param>
        /// <returns></returns>
        private string StepOne(List<double> buffPitch, double pitch)
        {
            buffPitch.Add(pitch);
            if (buffPitch.Count > activeFrames)
            {
                buffPitch.RemoveAt(0);
            }

            var max = buffPitch.Max();
            var min = buffPitch.Min();

            if (min < _headPitchMinThreshold)// && max > headPitchMaxThreshold
            {
                return "Nodding Detected success.";
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Step Two Gesture
        /// </summary>
        /// <param name="buffYaw"></param>
        /// <param name="yaw"></param>
        /// <returns></returns>
        private string StepTwo(List<double> buffYaw, double yaw)
        {
            buffYaw.Add(yaw);
            if (buffYaw.Count > activeFrames)
            {
                buffYaw.RemoveAt(0);
            }

            var max = buffYaw.Max();
            var min = buffYaw.Min();

            if (min < _headYawMinThreshold)// && max > headYawMaxThreshold
            {
                return "Shaking Detected success.";
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Step Three Gesture
        /// </summary>
        /// <param name="buffRoll"></param>
        /// <param name="roll"></param>
        /// <returns></returns>
        private string StepThree(List<double> buffRoll, double roll)
        {
            buffRoll.Add(roll);
            if (buffRoll.Count > activeFrames)
            {
                buffRoll.RemoveAt(0);
            }

            var max = buffRoll.Max();
            var min = buffRoll.Min();

            if (min < _headRollMinThreshold) //  && max > headRollMaxThreshold
            {
                Console.WriteLine("All head pose detection finished.");
                return "Rolling Detected success.";
            }
            else
            {
                return null;
            }
        }
    }
}
