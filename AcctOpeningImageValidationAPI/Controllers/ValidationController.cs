using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IdentificationValidationLib;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ReadAttributesFromFacialImage;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AcctOpeningImageValidationAPI.Controllers
{
    public class ValidationController : ControllerBase
    {

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IComputerVision computerVision;
        private readonly IExternalImageValidationService externalImageValidationService;
        private readonly IFaceValidation faceValidation;

        private IConfiguration Configuration { get; set; }


        public ValidationController(IConfiguration configuration,
            IComputerVision _computerVision,
            IExternalImageValidationService externalImageValidationService,
            ReadAttributesFromFacialImage.IFaceValidation faceValidation


            )
        {
            Configuration = configuration;
            this.computerVision = _computerVision;
            this.externalImageValidationService = externalImageValidationService;
            this.faceValidation = faceValidation;
        }
        [HttpPost]
        [Route("ValidateIdentificationImage")]

        public async Task<IActionResult> ValidateImage([Required] string ImageURL, [FromBody] IdentificationValidationLib.Models.Camudatafield camudatafield)
        {
            // var test = Configuration.GetSection("AppSettings").GetSection("subscriptionKey").Value;
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(camudatafield);

            var bypass = Configuration.GetSection("AppSettings").GetSection("ByPass").Value;
            if (bypass.ToLower() == "true")
            {
                log.Info($"bypass set to true for {json}");
                return new OkObjectResult("success");

            }

            var response = await computerVision.ReadText(ImageURL);
            if (!response.isSuccess)
            {
                return new UnprocessableEntityObjectResult(response.message);
            }

            //HelperServices helper = new HelperServices();

            var returnedExtractedTextFromImages = HelperServices.ExtractWordIntoLists(response.message);
            if (returnedExtractedTextFromImages == null || !returnedExtractedTextFromImages.Any())
            {
                //can not extract text from image. exit

                return new UnprocessableEntityObjectResult("can not see/extract any text from image submitted");

            }


            //go to extract the biodata from the images 
            Validation v = new Validation();
            v.ExtractDocKeyDetails
                (returnedExtractedTextFromImages, new IdentificationValidationLib.Models.Camudatafield());
            if (!v.IsExtractionOk)
            {
                return new UnprocessableEntityObjectResult(v.exception);
            }
            var compare = HelperServices.Compare(v, camudatafield);
            if (!compare)
            {
                return new UnprocessableEntityObjectResult($"ID Number extracted from image submitted does not match submitted ID details. The extracted ID number is {v.idNumber} and the submitted one is {camudatafield.Idno}");

            }

            var appruv = await this.externalImageValidationService.ValidateDoc(v, camudatafield);
            if (appruv.isSuccess)
            {
                // return new OkObjectResult(appruv.msg);
                return new OkObjectResult("success");

            }
            else
            {
                return new UnprocessableEntityObjectResult(appruv.msg);

            }

        }

        [HttpGet]
        [Route("ValidateFaceImage")]

        public async Task<IActionResult> ValidateFaceImage([Required] string ImageURL)
        {


            var bypass = Configuration.GetSection("AppSettings").GetSection("ByPass").Value;
            if (bypass.ToLower() == "true")
            {
                log.Info($"bypass set to true for {ImageURL}");
                return new OkObjectResult("success");

            }

            try
            {
                var result = await faceValidation.PerformFaceValidationAsync(ImageURL);
                log.Info($"source:{ImageURL}, result:{result.faces}");
                if (result.IsSuccess)
                {
                    //return new OkObjectResult(result.faces);
                    return new OkObjectResult("success");

                }
                else
                {
                    return new UnprocessableEntityObjectResult(result.faces);

                }
            }
            catch (Exception ex)
            {
                log.Error($"source:{ImageURL}, error {ex}");
                return new BadRequestObjectResult(ex);
            }
        }



    }
}
