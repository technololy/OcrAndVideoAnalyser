using AcctOpeningImageValidationAPI.Models;
using AcctOpeningImageValidationAPI.Repository.Abstraction;
using AcctOpeningImageValidationAPI.Repository.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Threading.Tasks;

namespace AcctOpeningImageValidationAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FaceListController : ControllerBase
    {
        private readonly IFaceRepository _faceRepository;
        public FaceListController(IFaceRepository faceRepository)
        {
            _faceRepository = faceRepository;
        }

        [HttpGet]
        [Route("face-list/create-list")]
        public async Task<IActionResult> CreateList()
        {
            try
            {
                await _faceRepository.CreateFaceList();

                return Ok(ResponseViewModel<PersistedFace>.Ok("Successful"));
            }
            catch (Exception e)
            {
                return Ok(ResponseViewModel<PersistedFace>.Failed(e.Message));
            }
        }

        [HttpPost]
        [Route("face-list/create-face")]
        public async Task<IActionResult> AddFace([FromForm] FaceListRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ResponseViewModel<PersistedFace>.Failed("All fields are required"));
                }

                var result = await _faceRepository.AddFaceToFaceList(model.File, Guid.NewGuid().ToString());

                return Ok(ResponseViewModel<PersistedFace>.Ok(result));

            } catch(Exception e)
            {
                return Ok(ResponseViewModel<PersistedFace>.Failed(e.Message));
            }
        }

        [HttpGet]
        [Route("face-list/verify-face")]
        public async Task<IActionResult> VerifyFace(string guid, string name)
        {
            try
            {
                var result = await _faceRepository.VerifyFaceToFaceList(new Guid(guid), name);

                return Ok(result);
            }
            catch (Exception e)
            {
                return Ok(ResponseViewModel<PersistedFace>.Failed(e.Message));
            }
        }
    }
}
