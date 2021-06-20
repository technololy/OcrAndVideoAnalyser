using System;
using System.IO;
using System.Threading.Tasks;
using AcctOpeningImageValidationAPI.Models;
using AcctOpeningImageValidationAPI.Repository.Abstraction;
using AcctOpeningImageValidationAPI.Services;
using IdentificationValidationLib;
using IdentificationValidationLib.Abstractions;
using IdentificationValidationLib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace AcctOpeningImageValidationAPI.Controllers
{
    public class FaceLivenessController : ControllerBase
    {
        /// <summary>
        /// IFaceRepository _faceRepository
        /// </summary>
        private readonly IFaceRepository _faceRepository;
        private readonly INetworkService _networkService;
        private readonly IHubContext<NotificationHub> _hub;

        /// <summary>
        /// AppSettings | Production or Development
        /// </summary>
        private readonly AppSettings _setting;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public FaceLivenessController(IFaceRepository faceRepository, IOptions<AppSettings> options, INetworkService networkService, IHubContext<NotificationHub> hub)
        {
            _faceRepository = faceRepository;
            _setting = options.Value;
            _networkService = networkService;
            _hub = hub;
        }

        [HttpPost]
        [Route("signalr/send-message")]
        public async Task<IActionResult> SendSignalR([FromBody] SignalModel model)
        {
            if(!ModelState.IsValid)
            {
                return new OkObjectResult(HelperLib.ReponseClass.ReponseMethodGeneric("Please pass the message", false));
            }

            await _hub.Clients.All.SendAsync("NewItem", "Hello From The Mars!!!");

            return new OkObjectResult(HelperLib.ReponseClass.ReponseMethodGeneric("Successful", true));
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
                    return new UnprocessableEntityObjectResult(HelperLib.ReponseClass.ReponseMethodGeneric<LivenessCheckResponse>("All fields are required", null, false));
                }

                //Set File Name
                var fileName = $"{model.UserIdentification}.{_setting.LivenessVideoFormat}";

                //Convert the images from Base64 to VideoBytes
                byte[] videoBytes = Convert.FromBase64String(model.VideoFile);

                //Get File Path from the root liveness directory
                string FilePath = Path.Combine(Directory.GetCurrentDirectory(), _setting.LivenessRootFolder);

                FilePath = Path.Combine(FilePath, model.UserIdentification, DateTime.Now.ToShortDateString(), DateTime.Now.Ticks.ToString());

                //Check if Directory Not Exists
                if(!Directory.Exists(FilePath))
                {
                    //Create directory always
                    Directory.CreateDirectory(FilePath);
                }

                //Create Video File
                System.IO.File.WriteAllBytes(Path.Combine(FilePath, fileName), videoBytes);

                // Extract Frames From Video
                var (status, message) = _faceRepository.ExtractFrameFromVideo(FilePath, fileName);

                if(!status)
                {
                    return new OkObjectResult(HelperLib.ReponseClass.ReponseMethod(message, status));
                }

                //Convert Image to stream
                var headPoseResult = await _faceRepository.RunHeadGestureOnImageFrame(FilePath, model.UserIdentification);

                //Return Response
                var response = new LivenessCheckResponse
                {
                    HeadNodingDetected = headPoseResult.Item1,
                    HeadShakingDetected = headPoseResult.Item2,
                    HeadRollingDetected = headPoseResult.Item3,
                    HasFaceSmile = headPoseResult.Item4,
                    EncodedResult = headPoseResult.Item5,
                    FaceImageUrl = headPoseResult.Item6
                };

                return new OkObjectResult(HelperLib.ReponseClass.ReponseMethodGeneric("Successful", response, true));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("liveness/multiple-images")]
        public async Task<IActionResult> ProcessVideoImages ([FromBody] FaceRequestImages model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            //Get File Path from the root liveness directory
            string FilePath = Path.Combine(Directory.GetCurrentDirectory(), _setting.LivenessRootFolder);

            //Create directory always
            Directory.CreateDirectory(FilePath);

            var i = 0;

            while(i <= model.Images.Length)
            {
                var fileName = string.Format("{0}\\image-{1}.jpeg", Path.Combine(FilePath), i);

                _faceRepository.SaveImageToDisk(model.Images[i], fileName);

                i++;
            }

            //Convert Image to stream
            var headPoseResult = await _faceRepository.RunHeadGestureOnImageFrame(FilePath, model.UserIdentification);

            //Return Response
            var response = new LivenessCheckResponse
            {
                HeadNodingDetected = headPoseResult.Item1,
                HeadShakingDetected = headPoseResult.Item2,
                HeadRollingDetected = headPoseResult.Item3,
                HasFaceSmile = headPoseResult.Item4
            };

            return new OkObjectResult(HelperLib.ReponseClass.ReponseMethodGeneric("Successful", response, true));
        }

        [HttpPost]
        [Route("liveness/form")]
        public async Task<IActionResult> ProcessVideoFormMultiPart([FromForm] FaceRequestForm model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new UnprocessableEntityObjectResult(HelperLib.ReponseClass.ReponseMethodGeneric<LivenessCheckResponse>("All fields are required", null, false));
                }

                using(var ms = new MemoryStream())
                {
                    model.File.CopyTo(ms);
                    
                    //Set File Name
                    var fileName = $"FormVideo.{_setting.LivenessVideoFormat}";

                    //Convert the images from Base64 to VideoBytes
                    byte[] videoBytes = ms.ToArray();

                    ms.Flush(); ms.Close();

                    //Get File Path from the root liveness directory
                    string FilePath = Path.Combine(Directory.GetCurrentDirectory(), _setting.LivenessRootFolder);

                    FilePath = Path.Combine(FilePath, model.UserIdentification, DateTime.Now.ToShortDateString(), DateTime.Now.Ticks.ToString());

                    //Check if Directory Not Exists
                    if (!Directory.Exists(FilePath))
                    {
                        //Create directory always
                        Directory.CreateDirectory(FilePath);
                    }

                    //Create Video File
                    System.IO.File.WriteAllBytes(Path.Combine(FilePath, fileName), videoBytes);

                    // Extract Frames From Video
                    var (status, message) = _faceRepository.ExtractFrameFromVideo(FilePath, fileName);

                    if (!status)
                    {
                        return new OkObjectResult(HelperLib.ReponseClass.ReponseMethod(message, status));
                    }
                    //Convert Image to stream
                    var headPoseResult = await _faceRepository.RunHeadGestureOnImageFrame(FilePath, model.UserIdentification);

                    //Return Response
                    var response = new LivenessCheckResponse
                    {
                        HeadNodingDetected = headPoseResult.Item1,
                        HeadShakingDetected = headPoseResult.Item2,
                        HeadRollingDetected = headPoseResult.Item3,
                        HasFaceSmile = headPoseResult.Item4,
                        EncodedResult = headPoseResult.Item5,
                        FaceImageUrl = headPoseResult.Item6
                    };

                    return new OkObjectResult(HelperLib.ReponseClass.ReponseMethodGeneric("Successful", response, true));
                }
              
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("liveness/blink/form")]
        public async Task<IActionResult> ProcessBlinkAlgorithm ([FromForm] FaceRequestForm model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new UnprocessableEntityObjectResult(HelperLib.ReponseClass.ReponseMethodGeneric<LivenessCheckResponse>("All fields are required", null, false));
                }

                using (var ms = new MemoryStream())
                {
                    model.File.CopyTo(ms);

                    //Set File Name
                    var fileName = $"FormVideo.{_setting.LivenessVideoFormat}";

                    //Convert the images from Base64 to VideoBytes
                    byte[] videoBytes = ms.ToArray();

                    ms.Flush(); ms.Close();

                    //Get File Path from the root liveness directory
                    string FilePath = Path.Combine(Directory.GetCurrentDirectory(), _setting.LivenessRootFolder);

                    FilePath = Path.Combine(FilePath, model.UserIdentification, DateTime.Now.ToShortDateString(), DateTime.Now.Ticks.ToString());

                    //Check if Directory Not Exists
                    if (!Directory.Exists(FilePath))
                    {
                        //Create directory always
                        Directory.CreateDirectory(FilePath);
                    }

                    //Create Video File
                    System.IO.File.WriteAllBytes(Path.Combine(FilePath, fileName), videoBytes);

                    // Extract Frames From Video
                    var (status, message) = _faceRepository.ExtractFrameFromVideo(FilePath, fileName);

                    if (!status)
                    {
                        return new OkObjectResult(HelperLib.ReponseClass.ReponseMethod(message, status));
                    }

                    //Run Blink Algorithm
                    var headPoseResult = await _faceRepository.RunEyeBlinkAlgorithm(FilePath, model.UserIdentification);

                    return new OkObjectResult(HelperLib.ReponseClass.ReponseMethodGeneric("Successful", headPoseResult, true));
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
