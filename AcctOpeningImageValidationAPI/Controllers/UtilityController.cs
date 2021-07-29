using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AcctOpeningImageValidationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UtilityController : ControllerBase
    {
        private readonly ILogger<UtilityController> _logger;

        public UtilityController(ILogger<UtilityController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("convert-image-base64")]
        public IActionResult ConvertImageToBase64([FromForm] ImageFile imageFile)
        {
            _logger.LogInformation("Hello, Sample Logging!");

           if(!ModelState.IsValid)
           {
               return BadRequest();
           }

            var base64Encoded = string.Empty;

            using (var ms = new MemoryStream())
            {
                imageFile.File.CopyTo(ms);

                //Convert the images from Base64 to VideoBytes
                byte[] imageBytes = ms.ToArray();

                base64Encoded = Convert.ToBase64String(imageBytes);

                ms.Flush(); ms.Close();
            }

            return Ok(base64Encoded);
        }
    }

    public class ImageFile
    {
        [Required]
        public IFormFile File { get; set; }
    }
}
