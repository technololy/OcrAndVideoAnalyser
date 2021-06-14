using System;
using System.IO;
using System.Threading.Tasks;
using AcctOpeningImageValidationAPI.Repository.Abstraction;
using IdentificationValidationLib;
using IdentificationValidationLib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AcctOpeningImageValidationAPI.Controllers
{
    public class FaceLivenessController : ControllerBase
    {
        /// <summary>
        /// IFaceRepository _faceRepository
        /// </summary>
        private readonly IFaceRepository _faceRepository;

        /// <summary>
        /// AppSettings | Production or Development
        /// </summary>
        private readonly AppSettings _setting;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public FaceLivenessController(IFaceRepository faceRepository, IOptions<AppSettings> options)
        {
            _faceRepository = faceRepository;
            _setting = options.Value;
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
                _faceRepository.ExtractFrameFromVideo(FilePath, fileName);

                //Convert Image to stream
                var headPoseResult = await _faceRepository.RunHeadGestureOnImageFrame(FilePath);

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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
