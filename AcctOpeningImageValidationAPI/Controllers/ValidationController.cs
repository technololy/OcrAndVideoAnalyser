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

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AcctOpeningImageValidationAPI.Controllers
{
    public class ValidationController : ControllerBase
    {

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IComputerVision computerVision;
        private readonly IExternalImageValidationService externalImageValidationService;

        private IConfiguration Configuration { get; set; }


        public ValidationController(IConfiguration configuration, IComputerVision _computerVision, IExternalImageValidationService externalImageValidationService)
        {
            Configuration = configuration;
            this.computerVision = _computerVision;
            this.externalImageValidationService = externalImageValidationService;
        }
        [HttpPost]
        [Route("ValidateImage")]

        public async Task<IActionResult> ValidateImage([Required] string ImageURL, IdentificationValidationLib.Models.Camudatafield camudatafield)
        {
            // var test = Configuration.GetSection("AppSettings").GetSection("subscriptionKey").Value;
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

                return new UnprocessableEntityObjectResult("can not see/extract text from image");

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
                return new UnprocessableEntityObjectResult("image details does not match submitted details");

            }

            await this.externalImageValidationService.ValidateDoc(v, camudatafield);

            return new OkResult();

        }





    }
}
