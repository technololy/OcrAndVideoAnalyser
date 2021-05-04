using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AcctOpeningImageValidationAPI.Helpers;
using AcctOpeningImageValidationAPI.Models;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.Extensions.Options;

namespace AcctOpeningImageValidationAPI.Controllers
{
    public class FaceLivenessController : ControllerBase
    {

        /// <summary>
        /// Properties
        /// </summary>
        private static IFaceClient client;

        private readonly double _headPitchMinThreshold = -9; //-15

        private readonly double _headYawMinThreshold = -20;

        private readonly double _headRollMinThreshold = -20;

        private readonly static int activeFrames = 14;

        private AppSettings _setting;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public FaceLivenessController(IOptions<AppSettings> options)
        {
            _setting = options.Value;

            client = new FaceClient(new ApiKeyServiceClientCredentials(_setting.subscriptionKey)) { Endpoint = _setting.AzureFacialBaseUrl };
        }

        /// <summary>
        /// This Method Processes The Video File For Liveness Check
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("liveness")]
        public async Task<IActionResult> ProcessVideoFile([FromBody] FaceRequest model)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return new OkObjectResult(HelperLib.ReponseClass.ReponseMethodGeneric<LivenessCheckResponse>("All fields are required", null, false));
                }

                //Set File Name
                var fileName = $"{model.UserIdentification}.${_setting.LivenessVideoFormat}";

                //Convert the images from Base64 to VideoBytes
                byte[] videoBytes = Convert.FromBase64String(model.VideoFile);

                //Get File Path from the root liveness directory
                string FilePath = Path.Combine(Directory.GetCurrentDirectory(), _setting.LivenessRootFolder);

                //Create directory always
                Directory.CreateDirectory(FilePath);

                //Create Video File
                System.IO.File.WriteAllBytes(Path.Combine(FilePath, fileName), videoBytes);

                // Extract Frames From Video
                ExtractFrameFromVideo(FilePath, fileName);

                //Convert Image to stream
                var headPoseResult = await RunHeadGestureOnImageFrame(FilePath);

                //Return Response
                var response = new LivenessCheckResponse
                {
                    HeadNodingDetected = headPoseResult.Item1,
                    HeadShakingDetected = headPoseResult.Item2,
                    HeadRollingDetected = headPoseResult.Item3
                };

                return new OkObjectResult(HelperLib.ReponseClass.ReponseMethodGeneric("Successful", response, true));
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
            //Initiatlize Medial Took Kit to Extracting image(frame) from video
            var mp4 = new MediaFile { Filename = Path.Combine(directory, fiileName) };
            using var engine = new Engine();

            //Getting Meta Data
            engine.GetMetadata(mp4);

            //Initializing Seek to One
            var i = 0;

            //Looping through
            //TODO: This should be limited to 9 Seconds video
            while (i < mp4.Metadata.Duration.Seconds)
            {
                //Conversion Options Settings
                var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(i), };

                //Constructing the output file
                var outputFile = new MediaFile { Filename = string.Format("{0}\\image-{1}.jpeg", Path.Combine(directory), i) };

                //Actual extraction of image
                engine.GetThumbnail(mp4, outputFile, options);

                //This is weird, but we've got to rotate it
                //Discalimer: DO NOT COMMENT, THIS IS THE PILLAR OF THE ENTIRE PROCESS!!!
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
                //Neglect any file ending .mp4
                if (item.EndsWith("mp4"))
                {
                    continue;
                }

                //Extract file name and extension
                var fileName = item.Split('\\').Last(); var imageName = fileName.Split('.').First();

                // var baseString = GetBaseStringFromImagePath(item);
                byte[] imageArray = System.IO.File.ReadAllBytes(item);

                //Convert Image From Byte Array To Stream
                Stream stream = new MemoryStream(imageArray);

                // Submit image to API. 
                var attrs = new List<FaceAttributeType> { FaceAttributeType.HeadPose };

                //var faces = await client.Face.DetectWithUrlWithHttpMessagesAsync(uploadedContent, returnFaceId: false, returnFaceAttributes: attrs);
                var faces = await client.Face.DetectWithStreamWithHttpMessagesAsync(stream, returnFaceId: false, returnFaceAttributes: attrs);

                //Check if Face is a Human face
                if (faces.Body.Count <= 0)
                {
                    continue;
                }

                //Get Head Pose (For Liveness Algorithm Check) Object
                var headPose = faces.Body.First().FaceAttributes?.HeadPose;

                //Get Pitch, Roll and Yaw values as a determinant for each Head Pose
                var pitch = headPose.Pitch; var roll = headPose.Roll; var yaw = headPose.Yaw;

                //Run Step One, Pitch Algorithm and Value Calculation
                if (runStepOne)
                {
                    headGestureResult = StepOne(buffPitch, pitch);
                    if (!string.IsNullOrEmpty(headGestureResult))
                    {
                        runStepOne = false;
                        stepOneComplete = true;
                    }
                }

                //Run Step Two, Yaw Algorithm and Value Calculation
                if (runStepTwo)
                {
                    headGestureResult = StepTwo(buffYaw, yaw);
                    if (!string.IsNullOrEmpty(headGestureResult))
                    {
                        runStepTwo = false;
                        stepTwoComplete = true;
                    }
                }

                //Run Step Three, Roll Algorithm and Value Calculation
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
        /// Step One Gesture - Pitch
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
        /// Step Two Gesture - Yaw
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
        /// Step Three Gesture - Roll
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
