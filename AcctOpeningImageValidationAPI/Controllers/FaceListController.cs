using AcctOpeningImageValidationAPI.Models;
using AcctOpeningImageValidationAPI.Repository.Abstraction;
using AcctOpeningImageValidationAPI.Repository.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<IActionResult> AddFace([FromBody] FaceListRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ResponseViewModel<PersistedFace>.Failed("All fields are required"));
                }

                var result = await _faceRepository.AddFaceToFaceList(model.File, model.Name);

                return Ok(ResponseViewModel<PersistedFace>.Ok(result));

            } catch(Exception e)
            {
                return Ok(ResponseViewModel<PersistedFace>.Failed(e.Message));
            }
        }

        [HttpPost]
        [Route("face-list/verify-face")]
        public async Task<IActionResult> VerifyFace(string guid)
        {
            try
            {
                var result = await _faceRepository.VerifyFaceToFaceList(new Guid(guid));

                return Ok(result);
            }
            catch (Exception e)
            {
                return Ok(ResponseViewModel<PersistedFace>.Failed(e.Message));
            }
        }
    }
}
