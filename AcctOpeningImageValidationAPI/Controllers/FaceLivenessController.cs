using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AcctOpeningImageValidationAPI.Models;
using AcctOpeningImageValidationAPI.Repository.Abstraction;
using AcctOpeningImageValidationAPI.Services;
using FluentScheduler;
using IdentificationValidationLib;
using IdentificationValidationLib.Abstractions;
using IdentificationValidationLib.Models;
using Microsoft.AspNetCore.Http;
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
        private readonly SterlingOnebankIDCardsContext _context;

        /// <summary>
        /// AppSettings | Production or Development
        /// </summary>
        private readonly AppSettings _setting;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public FaceLivenessController(IFaceRepository faceRepository,
                                      IOptions<AppSettings> options,
                                      INetworkService networkService,
                                      SterlingOnebankIDCardsContext context,
                                      IHubContext<NotificationHub> hub)
        {
            _faceRepository = faceRepository;
            _setting = options.Value;
            _networkService = networkService;
            _hub = hub;
            _context = context;
        }

        /// <summary>
        /// Provide logs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("liveness/logs")]
        public IActionResult GetLogs()
        {
            return Ok(_context.RequestLog.ToList().OrderByDescending(x => x.Id));
        }
        /// <summary>
        /// Submit Processing Method For SignalR Possible Implementation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("liveness/submit-processing")]
        public IActionResult SubmitProcessing([FromForm] FaceRequestForm model)
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

                    //TODO: Hand over to a scheduler or queuing system to handle
                    Task.Run(() =>
                    {
                        // Extract Frames From Video
                        var (status, message) = _faceRepository.ExtractFrameFromVideo(FilePath, fileName);

                        _context.RequestLog.Add(new RequestLogs
                        {
                            Email = model.UserIdentification,
                            FileName = fileName,
                            Name = model.UserIdentification,
                            Description = $"Extraction Message : {message} | Extraction Status : {status}"
                        });

                        _context.SaveChanges();

                        _faceRepository.RunEyeBlinkAlgorithm(FilePath, model.UserIdentification, action: async response =>
                        {

                            await _hub.Clients.All.SendAsync(_setting.SignalrEventName, "Hello, testing after video is done");

                            _context.RequestLog.Add(new RequestLogs
                            {
                                Email = model.UserIdentification,
                                FileName = fileName,
                                Name = model.UserIdentification,
                                Description = $"==== Call Back From Video Analysis || Result Below : ====="
                            });

                            _context.SaveChanges();

                            var result = Newtonsoft.Json.JsonConvert.SerializeObject(response);

                            _context.RequestLog.Add(new RequestLogs
                            {
                                Email = model.UserIdentification,
                                FileName = fileName,
                                Name = model.UserIdentification,
                                Description = $"${result}"
                            });

                            _context.SaveChanges();

                            await _hub.Clients.All.SendAsync(_setting.SignalrEventName, response);

                            _context.RequestLog.Add(new RequestLogs
                            {
                                Email = model.UserIdentification,
                                FileName = fileName,
                                Name = model.UserIdentification,
                                Description = $"==== After Sending Notification ====="
                            });

                            _context.SaveChanges();
                        });

                        //if (!status)
                        //{
                        //    return new OkObjectResult(HelperLib.ReponseClass.ReponseMethod(message, status));
                        //}
                    });

                    return new OkObjectResult(HelperLib.ReponseClass.ReponseMethod("Successful", true));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("signalr/send-message")]
        public async Task<IActionResult> SendSignalR([FromBody] SignalModel model)
        {
            if (!ModelState.IsValid)
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
                if (!ModelState.IsValid)
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("liveness/multiple-form-images")]
        public async Task<IActionResult> ProcessVideoImages([FromForm] MultipleFormRequest model)
        {
            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(HelperLib.ReponseClass.ReponseMethodGeneric<LivenessCheckResponse>("All fields are required", null, false));
            }

            try
            {
                if(model.Files.Length == 0)
                {
                    return new UnprocessableEntityObjectResult(HelperLib.ReponseClass.ReponseMethodGeneric<LivenessCheckResponse>("No image being passed, please send image", null, false));
                }

                //Get File Path from the root liveness directory
                string FilePath = Path.Combine(Directory.GetCurrentDirectory(), _setting.LivenessRootFolder);

                //Create directory always
                Directory.CreateDirectory(FilePath);

                var i = 0; var shortDate = DateTime.Now.ToShortDateString(); var tick = DateTime.Now.Ticks.ToString();

                while (i < model.Files.Length)
                {
                    SaveFormFile(model.UserIdentification, model.Files[i], shortDate, tick, i);
                    i++;
                }

                //Combine the Path finally
                FilePath = Path.Combine(FilePath, model.UserIdentification, shortDate, tick);

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
            } catch(Exception e)
            {
                return new UnprocessableEntityObjectResult(HelperLib.ReponseClass.ReponseMethodGeneric<LivenessCheckResponse>(e.Message, null, false));
            }
        }

        [HttpPost]
        [Route("liveness/multiple-images")]
        public async Task<IActionResult> ProcessVideoImages([FromBody] FaceRequestImages model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            //Get File Path from the root liveness directory
            string FilePath = Path.Combine(Directory.GetCurrentDirectory(), _setting.LivenessRootFolder);

            //Create directory always
            Directory.CreateDirectory(FilePath);

            var i = 0;

            while (i <= model.Images.Length)
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
        public async Task<IActionResult> ProcessBlinkAlgorithm([FromForm] FaceRequestForm model)
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

        private string SaveFormFile (string UserIdentification, IFormFile File, string shortDate, string tick, int index)
        {
            using (var ms = new MemoryStream())
            {
                File.CopyTo(ms);

                string FilePath = Path.Combine(Directory.GetCurrentDirectory(), _setting.LivenessRootFolder);

                //Get File Path from the root liveness directory
                FilePath = Path.Combine(FilePath, UserIdentification, shortDate, tick);//DateTime.Now.ToShortDateString() DateTime.Now.Ticks.ToString()

                //Set File Name
                var fileName = string.Format("{0}\\image-{1}.jpeg", FilePath, index);

                //Convert the images from Base64 to VideoBytes
                byte[] videoBytes = ms.ToArray();

                ms.Flush(); ms.Close();

                //Check if Directory Not Exists
                if (!Directory.Exists(FilePath))
                {
                    //Create directory always
                    Directory.CreateDirectory(FilePath);
                }

                //Create Video File
                System.IO.File.WriteAllBytes(fileName, videoBytes);

                return fileName;
            }
        }
    }
}
