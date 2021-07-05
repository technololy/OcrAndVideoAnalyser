using AcctOpeningImageValidationAPI.Repository.Abstraction;
using AcctOpeningImageValidationAPI.Repository.Services.Implementation;
using AcctOpeningImageValidationAPI.Repository.Services.Request;
using IdentificationValidationLib;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.Extensions.Options;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AcctOpeningImageValidationAPI.Repository
{
    public class FaceRepository : IFaceRepository
    {
        /// <summary>
        /// Properties
        /// </summary>
        private readonly double _headPitchMinThreshold = -15; //-15
        private readonly double _headYawMinThreshold = -20; 
        private readonly double _headRollMinThreshold = -20;
        private readonly static int activeFrames = 14;
        private static IFaceClient client;
        private readonly AppSettings _setting;
        private readonly RestClientService _restClientService;
        private readonly double _eyeLeftCloseValue = 20;
        private readonly double _eyeRightCloseValue = 20;
        public EyeBlinkResult blinkResult = new EyeBlinkResult();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public FaceRepository(IOptions<AppSettings> options, RestClientService restClientService)
        {
            _setting = options.Value;
            _restClientService = restClientService;
            client = new FaceClient(new ApiKeyServiceClientCredentials(_setting.subscriptionKey)) { Endpoint = _setting.AzureFacialBaseUrl };
        }
        /// <summary>
        /// Extraction of video frame is being done here
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="fiileName"></param>
        public Tuple<bool, string> ExtractFrameFromVideo(string directory, string fiileName)
        {
            //Initiatlize Medial Took Kit to Extracting image(frame) from video
            var mp4 = new MediaFile { Filename = Path.Combine(directory, fiileName) };
            using var engine = new Engine();

            //Getting Meta Data
            engine.GetMetadata(mp4);

            //Initializing Seek to One
            var i = 0;

            if(mp4.Metadata.Duration.TotalSeconds < 1)
            {
                return new Tuple<bool, string>(false, "Face capturing must be greater than 1 second long, please try again");
            }
            //Looping through
            //TODO: This should be limited to 9 Seconds video
            while (i < mp4.Metadata.Duration.Seconds)
            {
                //Conversion Options Settings
                var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(i), };

                //Constructing the output file
                var outputFile = new MediaFile { Filename = string.Format("{0}\\image-{1}.jpeg", Path.Combine(directory), i) };

                //This Method is native to Window OS, this line will fail on (i.e. Macintosh & Docker Container)
                //Actual extraction of image
                engine.GetThumbnail(mp4, outputFile, options);

                //This is weird, but we've got to rotate it
                //Discalimer: DO NOT COMMENT, THIS IS THE PILLAR OF THE ENTIRE PROCESS!!!
                RotateImage(outputFile.Filename);
                i++;
            }

            return new Tuple<bool, string>(true, "Video analysis was successful");
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
            File.Delete(path);
            image.Save(path);
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

        public async Task<Tuple<bool, bool, bool, bool>> RunHeadGestureOnImageFrame(string[] images)
        {
            throw new NotImplementedException();
        }

        public bool SaveImageToDisk (string base64String, string path)
        {
            //Check if directory exist
            if (Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            byte[] imageBytes = Convert.FromBase64String(base64String);

            File.WriteAllBytes(path, imageBytes);

            return true;
        }

        public async Task<EyeBlinkResult> RunEyeBlinkAlgorithm (string filePath, string userIdentification)
        {
            var files = Directory.GetFiles(filePath); 
            var items = files.Reverse(); 
            var base64Encoded = string.Empty; 
            var index = 0;

            foreach (var item in items)
            {
                //Neglect any file ending .mp4
                if (item.EndsWith("mp4")) { continue; }

                //Extract file name and extension
                var fileName = item.Split('\\').Last(); var imageName = fileName.Split('.').First();

                // var baseString = GetBaseStringFromImagePath(item);
                byte[] imageArray = System.IO.File.ReadAllBytes(item);

                //Convert Image From Byte Array To Stream
                Stream stream = new MemoryStream(imageArray);

                // Submit image to API. 
                var attrs = new List<FaceAttributeType> { FaceAttributeType.HeadPose, FaceAttributeType.Smile };

                //var faces = await client.Face.DetectWithUrlWithHttpMessagesAsync(url, returnFaceId: false, returnFaceAttributes: attrs);
                HttpOperationResponse<IList<DetectedFace>> faces = await client.Face.DetectWithStreamWithHttpMessagesAsync(stream, returnFaceId: true, returnFaceAttributes: attrs, returnFaceLandmarks: true);

                //Check if Face is a Human face
                if (faces.Body.Count <= 0)
                {
                    continue;
                }

                base64Encoded = Convert.ToBase64String(imageArray);

                var result = await _restClientService.UploadDocument(new DocumentUploadRequest
                {
                    FolderName = $"{_setting.AzureContentFolderName}/{userIdentification}",
                    Base64String = base64Encoded, 
                    FileName = imageName
                });

                stream.Flush();  stream.Close();

                //Analyze Face Land Mark For Eye Blinking
                AnalyzeFaceLandMark(faces.Body.First().FaceLandmarks, result.Url, index, faces.Body.First().FaceAttributes?.HeadPose); index++;
            }

            return blinkResult;
        }

        /// <summary>
        /// Help Detect Gesture for Head Pose
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task<Tuple<bool, bool, bool, bool, string, string>> RunHeadGestureOnImageFrame(string filePath, string userIdentification)
        {
            var isSmiled = false;

            var headGestureResult = string.Empty; bool runStepOne = true; bool runStepTwo = true; bool runStepThree = true;

            bool stepOneComplete = false; bool stepTwoComplete = false; bool stepThreeComplete = false;

            var buffPitch = new List<double>(); var buffYaw = new List<double>(); var buffRoll = new List<double>();

            var files = Directory.GetFiles(filePath); var items = files.Reverse(); var base64Encoded = string.Empty; var index = 0;

            foreach (var item in items)
            {
                //Neglect any file ending .mp4
                if (item.EndsWith("mp4")) { continue; }

                //Extract file name and extension
                var fileName = item.Split('\\').Last(); var imageName = fileName.Split('.').First();

                // var baseString = GetBaseStringFromImagePath(item);
                byte[] imageArray = System.IO.File.ReadAllBytes(item);

                //Convert Image From Byte Array To Stream
                Stream stream = new MemoryStream(imageArray);

                // Submit image to API. 
                var attrs = new List<FaceAttributeType> { FaceAttributeType.HeadPose, FaceAttributeType.Smile };

                //var faces = await client.Face.DetectWithUrlWithHttpMessagesAsync(url, returnFaceId: false, returnFaceAttributes: attrs);
                HttpOperationResponse<IList<DetectedFace>> faces = await client.Face.DetectWithStreamWithHttpMessagesAsync(stream, returnFaceId: true, returnFaceAttributes: attrs, returnFaceLandmarks : true);

                
                //Check if Face is a Human face
                if (faces.Body.Count <= 0)
                {
                    continue;
                }

                //Analyze Face Land Mark For Eye Blinking
                var faceLandMark = AnalyzeFaceLandMark(faces.Body.First().FaceLandmarks, string.Empty, index, faces.Body.First().FaceAttributes?.HeadPose);

                //Get Head Pose (For Liveness Algorithm Check) Object
                var headPose = faces.Body.First().FaceAttributes?.HeadPose;

                //Check if this person has smile
                var smilePose = faces.Body.First().FaceAttributes?.Smile;

                //Smile. The smile expression of the given face. This value is between zero for no smile and one for a clear smile.
                if (smilePose > 0)
                {
                    isSmiled = true;
                }

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
                        base64Encoded = Convert.ToBase64String(imageArray);
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
                        base64Encoded = Convert.ToBase64String(imageArray);
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
                        base64Encoded = Convert.ToBase64String(imageArray);
                    }
                }

                index++;
            }

            //TODO: Upload Image To Blob Server.
            var result = await _restClientService.UploadDocument(new DocumentUploadRequest
            {
                FolderName = _setting.AzureContentFolderName,
                Base64String = base64Encoded,
                FileName = userIdentification
            });
            
            return new Tuple<bool, bool, bool, bool, string, string>(stepOneComplete, stepTwoComplete, stepThreeComplete, isSmiled, base64Encoded, result.Url ?? string.Empty);
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

        /// <summary>
        /// Eye Blnking Algorithm
        /// </summary>
        /// <param name="faceLandMark"></param>
        /// <returns></returns>
        public Tuple<bool, bool> AnalyzeFaceLandMark (FaceLandmarks faceLandMark)
        {
            if (faceLandMark != null)
            {
                int eyeLeftValue, eyeRightValue;
                bool eyeLeftBlinked = false, eyeRightBlinked = false;

                eyeLeftValue = Convert.ToInt32(faceLandMark.EyeLeftBottom.Y) - Convert.ToInt32(faceLandMark.EyeLeftTop.Y);

                if (eyeLeftValue < _eyeLeftCloseValue)
                {
                    eyeLeftBlinked = true;
                }

                eyeRightValue = Convert.ToInt32(faceLandMark.EyeRightBottom.Y) - Convert.ToInt32(faceLandMark.EyeRightTop.Y);

                if (eyeRightValue < _eyeRightCloseValue)
                {
                    eyeRightBlinked = true;
                }
                return new Tuple<bool, bool>(eyeLeftBlinked, eyeRightBlinked);
            }
            return new Tuple<bool, bool>(false, false);
        }

        /// <summary>
        /// Analyze Face Land Mark
        /// </summary>
        /// <param name="faceLandMark"></param>
        /// <param name="path"></param>
        /// <param name="index"></param>
        /// <param name="headPose"></param>
        /// <returns></returns>
        public EyeBlinkResult AnalyzeFaceLandMark(FaceLandmarks faceLandMark, string path, int index, HeadPose headPose)
        {
            if (faceLandMark != null)
            {
                if(blinkResult.EyeBlinks == null)
                {
                    blinkResult.EyeBlinks = new List<EyeBlink>();
                }

                int eyeLeftValue, eyeRightValue; bool eyeLeftBlinked = false, eyeRightBlinked = false;

                eyeLeftValue = Convert.ToInt32(faceLandMark.EyeLeftBottom.Y) - Convert.ToInt32(faceLandMark.EyeLeftTop.Y);

                if (eyeLeftValue > _eyeLeftCloseValue)
                {
                    eyeLeftBlinked = true;
                }

                eyeRightValue = Convert.ToInt32(faceLandMark.EyeRightBottom.Y) - Convert.ToInt32(faceLandMark.EyeRightTop.Y);

                if (eyeRightValue > _eyeRightCloseValue)
                {
                    eyeRightBlinked = true;
                }

                blinkResult.EyeBlinks.Add(new EyeBlink
                {
                    ImageIndex = index,
                    ImageUrl = path,
                    HeadPose = headPose,
                    Left = eyeLeftValue,
                    Right = eyeRightValue,
                    EyeStatus = (eyeLeftBlinked == true || eyeRightBlinked == true ) ? "EYE_CLOSED" : "EYE_OPENED",
                    RightEyeStatus = eyeRightBlinked ? "CLOSED" : "OPENED",
                    LeftEyeStatus = eyeLeftBlinked ? "CLOSED" : "OPENED"
                });

                if (blinkResult.IsHuman == false)
                {
                    if (eyeLeftBlinked == true || eyeRightBlinked == true)
                    {
                        blinkResult.IsHuman = true;
                    }
                }
            }

            return blinkResult;
        }

        public async Task RunHeadGestureOnImageFrame(string filePath, string userIdentification, Action<Tuple<bool, bool, bool, bool, string, string>> action)
        {
             var result = await RunHeadGestureOnImageFrame(filePath, userIdentification);
             action(result);
        }

        public async Task RunEyeBlinkAlgorithm(string filePath, string userIdentification, Action<EyeBlinkResult> action)
        {
            var result = await RunEyeBlinkAlgorithm(filePath, userIdentification);
            action(result);
        }
    }
}

public class EyeBlink
{
    public string RightEyeStatus { get; set; }
    public string LeftEyeStatus { get; set; }
    public string EyeStatus { get; set; }
    public string ImageUrl { get; set; }
    public int ImageIndex { get; set; }
    public int Left { get; set; }
    public int Right { get; set; }
    public HeadPose HeadPose { get; set; }
}

public class EyeBlinkResult
{
    public List<EyeBlink> EyeBlinks { get; set; }
    public bool IsHuman { get; set; }
}