using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using AcctOpeningImageValidationAPI.Helpers;
using AcctOpeningImageValidationAPI.Models;
using AcctOpeningImageValidationAPI.Repository.Abstraction;
using AcctOpeningImageValidationAPI.Repository.Response;
using AcctOpeningImageValidationAPI.Repository.Services.Implementation;
using AcctOpeningImageValidationAPI.Repository.Services.Request;
using HelperLib.Exceptions;
using IdentificationValidationLib;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ReadAttributesFromFacialImage;
using static IdentificationValidationLib.Validation;
using static ModelLib.Passport;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AcctOpeningImageValidationAPI.Controllers
{
    public class ValidationController : ControllerBase
    {

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IComputerVision computerVision;
        private readonly IExternalImageValidationService externalImageValidationService;
        private readonly IFaceValidation faceValidation;
        private readonly Models.SterlingOnebankIDCardsContext context;
        private IConfiguration Configuration { get; set; }
        private readonly IOCRRepository _ocrRepository;
        private readonly RestClientService _restClientService;
        private readonly AppSettings _appSettings;

        public ValidationController(IConfiguration configuration,
            IComputerVision _computerVision,
            IExternalImageValidationService externalImageValidationService,
            ReadAttributesFromFacialImage.IFaceValidation faceValidation,
            Models.SterlingOnebankIDCardsContext _context,
            RestClientService restClientService,
            IOptions<AppSettings> option,
            IOCRRepository ocrRepository)
        {
            Configuration = configuration;
            this.computerVision = _computerVision;
            this.externalImageValidationService = externalImageValidationService;
            this.faceValidation = faceValidation;
            this.context = _context;
            _ocrRepository = ocrRepository;
            _appSettings = option.Value;
            _restClientService = restClientService;
        }

        /// <summary>
        /// This method is obsolete
        /// </summary>
        /// <param name="ImageURL"></param>
        /// <param name="camudatafield"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ValidateIdentificationImage")]
        [Obsolete]
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
            var response2 = await computerVision.PerformOcrWithAzureAI(ImageURL, null);
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

        [HttpPost]
        [Route("ValidateIDCard")]
        public async Task<IActionResult> ValidateIDCard([FromBody] ValidateInputModel validate)
        {

            try
            {
                OCRUsage ocrUsage = OCRUsage.Empty;
                context.RequestLog.Add(new RequestLogs { Email = validate.Email, Description = validate.Base64Encoded.Substring(0, 10), FileName = "Image validation" });
                //context.SaveChanges();

                var result = await _restClientService.UploadDocument(new DocumentUploadRequest
                {
                    FolderName = _appSettings.AzureContentFolderName,
                    Base64String = validate.Base64Encoded,
                    FileName = validate.UserName
                });

                try
                {
                    ocrUsage = _ocrRepository.ValidateUsage(result.Url);
                }
                catch (MaximumOCRUsageException e)
                {
                    //TODO: Return a base response from here
                    return BadRequest(ResponseViewModel<String>.Failed(ResponseMessageViewModel.UNSUCCESSFUL));
                }

                var bypass = Configuration.GetSection("AppSettings").GetSection("ByPassIdCards").Value;
                if (bypass.ToLower() == "true")
                {
                    log.Info($"bypass set to true for {result.Url}");
                    //return new OkObjectResult("success");
                    return new OkObjectResult(HelperLib.ReponseClass.ReponseMethod("success", true));
                }

                var response = await computerVision.PerformOcrWithAzureAI(result.Url, null);
                //var response = await computerVision.ReadText(ImageURL);
                if (!response.isSuccess)
                {
                    return new UnprocessableEntityObjectResult(HelperLib.ReponseClass.ReponseMethod(response.message, false));
                }

                //log json
                await context.OCRResponses.AddAsync(new OCRResponse { BVN = validate.Email, JsonResponse = response.message });
                await context.SaveChangesAsync();

                Root documentRoot = JsonSerializer.Deserialize<Root>(response.message);

                if (documentRoot == null || documentRoot.analyzeResult == null || documentRoot.analyzeResult.documentResults == null || documentRoot.analyzeResult.documentResults.FirstOrDefault().docType == null)
                {
                    return new UnprocessableEntityObjectResult(HelperLib.ReponseClass.ReponseMethod("The file is not valid. Its type is not valid", false));
                }

                var scannedIDCardDetails = ProcessScannedIDJsonToObject.ProcessJsonToObject(documentRoot, response.message);

                scannedIDCardDetails.Email = validate.Email; scannedIDCardDetails.OCRUsageId = ocrUsage.Id;

                var count = context.ScannedIDCardDetail.Count(x => x.FormerIDNumber == scannedIDCardDetails.FormerIDNumber);

                if (count == 0)
                {
                    await context.ScannedIDCardDetail.AddAsync(scannedIDCardDetails);
                    await context.SaveChangesAsync();
                }

                //return Ok(scannedIDCardDetails);
                return new OkObjectResult(HelperLib.ReponseClass.ReponseMethodGeneric<ScannedIDCardDetails>("Successful", scannedIDCardDetails, true));


            }
            catch (Exception ex)
            {
                return new UnprocessableEntityObjectResult(HelperLib.ReponseClass.ReponseMethod(ex.ToString(), false));
            }
        }

        [HttpPost]
        [Route("ValidateNigerianIDCards")]
        public async Task<IActionResult> ValidateNigerianIDCards([Required] string ImageURL, [Required] string UserEmail, bool validateWithExternalService)
        {
            // var test = Configuration.GetSection("AppSettings").GetSection("subscriptionKey").Value;

            //TODO: 1 Call Core Banking Content Server, Content Server return absolute url path
            //TODO: 2 Returned Scanned Document Object
            context.RequestLog.Add(new RequestLogs { Email = UserEmail, Description = ImageURL, FileName = "Image validation" });
            context.SaveChanges();

            try
            {
                _ocrRepository.ValidateUsage(UserEmail);

            }
            catch (MaximumOCRUsageException e)
            {
                Debug.WriteLine(e);
                //TODO: Return a base response from here
            }

            var bypass = Configuration.GetSection("AppSettings").GetSection("ByPassIdCards").Value;
            if (bypass.ToLower() == "true")
            {
                log.Info($"bypass set to true for {ImageURL}");
                return new OkObjectResult("success");

            }

            var response = await computerVision.PerformOcrWithAzureAI(ImageURL, null);
            //var response = await computerVision.ReadText(ImageURL);
            if (!response.isSuccess)
            {
                return new UnprocessableEntityObjectResult(response.message);
            }

            Root documentRoot = JsonSerializer.Deserialize<Root>(response.message);
            //QuickType.VotersCard.Root votersCard = new QuickType.VotersCard.Root();
            //QuickType.Passport.InternationalPassportRoot internationalPassport = new QuickType.Passport.InternationalPassportRoot();
            //QuickType.DriversLicenseRoot driversLicense = new QuickType.DriversLicenseRoot();
            //QuickType.NationalID.NationalIdRoot nationalId = new QuickType.NationalID.NationalIdRoot();
            DocumentType docType = new DocumentType();

            (bool isSuccess, string msg, object data) appruv;


            var scannedIDCardDetails = ProcessScannedIDJsonToObject.ProcessJsonToObject(documentRoot, response.message);

            scannedIDCardDetails.Email = UserEmail;
            var card = scannedIDCardDetails.DocumentType.ToLower();
            if (card.Contains("driver"))
            {
                docType = DocumentType.DriversLicense;
            }
            else if (card.Contains("voter"))
            {
                docType = DocumentType.VotersCard;

            }
            else if (card.Contains("international"))
            {
                docType = DocumentType.InternationalPassport;

            }
            else if (card.Contains("national"))
            {
                docType = DocumentType.nationalId;

            }


            //};
            await context.ScannedIDCardDetail.AddAsync(scannedIDCardDetails);
            await context.SaveChangesAsync();

            if (validateWithExternalService)
            {
                // Entry Point For Validation Here
                // appruv = await this.externalImageValidationService.ValidateDoc(firstName, middleName, lastName, idNumber, dateOfBirth, docType);
                appruv = await this.externalImageValidationService.ValidateDoc(scannedIDCardDetails.FirstName, scannedIDCardDetails.MiddleName, scannedIDCardDetails.LastName, scannedIDCardDetails.IDNumber, Convert.ToDateTime(scannedIDCardDetails.DateOfBirth), docType);
                if (appruv.isSuccess)
                {
                    await context.AppruvResponses.AddAsync(new Models.AppruvResponse { StatusOfRequest = "success", Email = UserEmail });
                    await context.SaveChangesAsync();
                    return new OkObjectResult("success");

                }
                else
                {
                    return new UnprocessableEntityObjectResult(appruv.msg);

                }
            } else
            {
                return new OkObjectResult("success");
            }
        }


        [HttpPost]
        [Route("ReadOutNigerianIDCards")]
        public async Task<IActionResult> ReadOutNigerianIDCards([Required] string ImageURL, [Required] string UserEmail)
        {
            // var test = Configuration.GetSection("AppSettings").GetSection("subscriptionKey").Value;

            context.RequestLog.Add(new RequestLogs { Email = UserEmail, Description = ImageURL, FileName = "Image validation" });
            context.SaveChanges();

            var bypass = Configuration.GetSection("AppSettings").GetSection("ByPassIdCards").Value;
            if (bypass.ToLower() == "true")
            {
                log.Info($"bypass set to true for {ImageURL}");
                return new OkObjectResult("success");

            }
            var response = await computerVision.PerformOcrWithAzureAI(ImageURL, null);
            //var response = await computerVision.ReadText(ImageURL);
            if (!response.isSuccess)
            {
                return new UnprocessableEntityObjectResult(response.message);
            }

            //log json
            await context.OCRResponses.AddAsync(new OCRResponse { BVN = UserEmail, JsonResponse = response.message });
            await context.SaveChangesAsync();

            Root documentRoot = JsonSerializer.Deserialize<Root>(response.message);
            //QuickType.VotersCard.Root votersCard = new QuickType.VotersCard.Root();
            //QuickType.Passport.InternationalPassportRoot internationalPassport = new QuickType.Passport.InternationalPassportRoot();
            //QuickType.DriversLicenseRoot driversLicense = new QuickType.DriversLicenseRoot();
            //QuickType.NationalID.NationalIdRoot nationalId = new QuickType.NationalID.NationalIdRoot();

            (bool isSuccess, string msg, object data) appruv;
            DocumentType docType = new DocumentType();


            var scannedIDCardDetails = ProcessScannedIDJsonToObject.ProcessJsonToObject(documentRoot, response.message);

            scannedIDCardDetails.Email = UserEmail;

            await context.ScannedIDCardDetail.AddAsync(scannedIDCardDetails);
            await context.SaveChangesAsync();
            return new OkObjectResult(HelperLib.ReponseClass.ReponseMethodGeneric<ScannedIDCardDetails>("Successful", scannedIDCardDetails, true));
            var card = scannedIDCardDetails.DocumentType.ToLower();
            if (card.Contains("driver"))
            {
                docType = DocumentType.DriversLicense;
            }
            else if (card.Contains("voter"))
            {
                docType = DocumentType.VotersCard;

            }
            else if (card.Contains("international"))
            {
                docType = DocumentType.InternationalPassport;

            }
            else if (card.Contains("national"))
            {
                docType = DocumentType.nationalId;

            }
            appruv = await this.externalImageValidationService.ValidateDoc(scannedIDCardDetails.FirstName, scannedIDCardDetails.MiddleName, scannedIDCardDetails.LastName, scannedIDCardDetails.IDNumber, Convert.ToDateTime(scannedIDCardDetails.DateOfBirth), docType);
            if (appruv.isSuccess)
            {
                await context.AppruvResponses.AddAsync(new Models.AppruvResponse { StatusOfRequest = "success", Email = UserEmail });
                await context.SaveChangesAsync();
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


            var bypass = Configuration.GetSection("AppSettings").GetSection("ByPassFacial").Value;
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
                    var faceAttributes = result.faces.FirstOrDefault().FaceAttributes;
                    Models.FacialValidation facialValidation = new Models.FacialValidation()
                    {
                        FacialHair = faceAttributes.FacialHair != null ? faceAttributes.FacialHair.ToString() : null,
                        Hair = faceAttributes.Hair != null ? faceAttributes.Hair.ToString() : null,
                        Accessories = faceAttributes.Accessories != null ? faceAttributes.Accessories.FirstOrDefault().ToString() : null,
                        Age = faceAttributes.Age != null ? faceAttributes.Age.ToString() : null,
                        Gender = faceAttributes.Gender != null ? faceAttributes.Gender.Value.ToString() : null,
                        Emotion = faceAttributes.Emotion != null ? faceAttributes.Emotion.ToString() : null,
                        Smile = faceAttributes.Smile != null ? faceAttributes.Smile.Value.ToString() : null,
                        Occlusion = faceAttributes.Occlusion != null ? faceAttributes.Occlusion.ToString() : null,
                    };
                    await context.FacialValidations.AddAsync(facialValidation);
                    //await context.SaveChangesAsync();
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

        /// <summary>
        /// Returns facial attribute and confidence level from static client images for lients to take decision. Automatically converts image byte to url
        /// </summary>
        /// <param name="validate"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ValidateStaticFaceImage")]

        public async Task<IActionResult> ValidateStaticFaceImage([FromBody] ValidateInputModel validate)
        {
            var ocrUsage = OCRUsage.Empty;
            context.RequestLog.Add(new RequestLogs { Email = validate.Email, Description = validate.Base64Encoded.Substring(0, 10), FileName = "Face Image validation" });
            context.SaveChanges();

            var ImageURL = await _restClientService.UploadDocument(new DocumentUploadRequest
            {
                FolderName = _appSettings.AzureContentFolderName,
                Base64String = validate.Base64Encoded,
                FileName = validate.UserName
            });

            try
            {
                ocrUsage = _ocrRepository.ValidateUsage(validate.Email);
            }
            catch (MaximumOCRUsageException e)
            {
                //TODO: Return a base response from here
                return BadRequest(ResponseViewModel<String>.Failed(ResponseMessageViewModel.UNSUCCESSFUL));
            }

            var bypass = Configuration.GetSection("AppSettings").GetSection("ByPassFacial").Value;
            if (bypass.ToLower() == "true")
            {
                log.Info($"bypass set to true for {validate.Email}");
                return new OkObjectResult("success");

            }

            try
            {
                var result = await faceValidation.PerformFaceValidationAsync(ImageURL.Url);
                log.Info($"source:{ImageURL}, result:{result.faces}");
                if (result.IsSuccess)
                {
                    var faceAttributes = result.faces.FirstOrDefault().FaceAttributes;
                    Models.FacialValidation facialValidation = new Models.FacialValidation()
                    {
                        FacialHair = faceAttributes.FacialHair?.Beard.ToString(),
                        Hair = faceAttributes.Hair?.HairColor?.FirstOrDefault().Confidence.ToString(),
                        Accessories = faceAttributes.Accessories?.FirstOrDefault().Confidence.ToString(),
                        Age = faceAttributes.Age?.ToString(),
                        Gender = faceAttributes.Gender.Value.ToString(),
                        Emotion = $"Neutral {faceAttributes.Emotion?.Neutral.ToString()}, Sadness {faceAttributes.Emotion?.Sadness.ToString()}",
                        Smile = faceAttributes.Smile.Value.ToString(),
                        Occlusion = faceAttributes.Occlusion?.ToString(),
                        OCRUsageId = ocrUsage.Id
                    };
                    await context.FacialValidations.AddAsync(facialValidation);
                    await context.SaveChangesAsync();

                    return new OkObjectResult(HelperLib.ReponseClass.ReponseMethodGeneric<Microsoft.Azure.CognitiveServices.Vision.Face.Models.FaceAttributes>("Successful", faceAttributes, true));

                }
                else
                {
                    return new UnprocessableEntityObjectResult(HelperLib.ReponseClass.ReponseMethod("Face not seen", false));
                    // return new UnprocessableEntityObjectResult(result.faces);

                }
            }
            catch (Exception ex)
            {
                log.Error($"source:{ImageURL}, error {ex}");
                return new BadRequestObjectResult(HelperLib.ReponseClass.ReponseMethod(ex.ToString(), false));
            }
        }


        /// <summary>
        /// compare faces using byte array of images
        /// </summary>
        /// <param name="validate"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CompareFaceUsingImageByte")]

        public async Task<IActionResult> CompareFaceUsingImageByte([FromBody] ValidateInputModel validate)
        {

            OCRUsage ocr;
            context.RequestLog.Add(new RequestLogs { Email = validate.Email, Description = validate.Base64Encoded.Substring(0, 10), FileName = "Face Image validation" });
            context.SaveChanges();

            var sourceImageURL = await _restClientService.UploadDocument(new DocumentUploadRequest
            {
                FolderName = _appSettings.AzureContentFolderName,
                Base64String = validate.Base64Encoded,
                FileName = validate.UserName
            });

            var targetImageURL = await _restClientService.UploadDocument(new DocumentUploadRequest
            {
                FolderName = _appSettings.AzureContentFolderName,
                Base64String = validate.Base64EncodedTarget,
                FileName = validate.UserName
            });
            if (sourceImageURL == null || targetImageURL == null)
            {
                return new UnprocessableEntityObjectResult(HelperLib.ReponseClass.ReponseMethod("source or image obj after upload document is null", false));

            }

            try
            {
                ocr = _ocrRepository.ValidateUsage(sourceImageURL.Url);
                // _ocrRepository.ValidateUsage(targetImageURL.Url);
                context.ImageScanneds.Add(new ImagesScanned { OcrUsageId = ocr.Id, ImageURL = sourceImageURL.Url });
                context.ImageScanneds.Add(new ImagesScanned { OcrUsageId = ocr.Id, ImageURL = targetImageURL.Url });
                await context.SaveChangesAsync();


            }
            catch (MaximumOCRUsageException e)
            {
                Debug.WriteLine(e);
                //TODO: Return a base response from here
                return BadRequest(ResponseViewModel<String>.Failed(ResponseMessageViewModel.UNSUCCESSFUL));
            }

            var bypass = Configuration.GetSection("AppSettings").GetSection("ByPassFacial").Value;
            if (bypass.ToLower() == "true")
            {
                log.Info($"bypass set to true for {validate.Email}");
                return new OkObjectResult("success");

            }

            try
            {
                var result = await faceValidation.FindSimilarFaces(new List<string>() { targetImageURL.Url }, sourceImageURL.Url);
                if (result.IsSuccess)
                {
                    var similar = result.faces.FirstOrDefault();
                    await context.SimilarFacesRecord.AddAsync(new SimilarFace { Confidence = similar.Confidence, FaceId = similar.FaceId, PersistedFaceId = similar.PersistedFaceId, OCRUsage = ocr });
                    await context.SaveChangesAsync();


                    return new OkObjectResult(HelperLib.ReponseClass.ReponseMethodGeneric("Successful", similar, true));

                }
                else
                {
                    return new UnprocessableEntityObjectResult(HelperLib.ReponseClass.ReponseMethod("Face not seen", false));
                    // return new UnprocessableEntityObjectResult(result.faces);

                }
            }
            catch (Exception ex)
            {
                log.Error($"source:{sourceImageURL} target {targetImageURL}, error {ex}");
                return new BadRequestObjectResult(HelperLib.ReponseClass.ReponseMethod(ex.ToString(), false));
            }
        }






        /// <summary>
        /// compare faces using byte array of images
        /// </summary>
        /// <param name="validate"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("VerifyTwoFaces")]

        public async Task<IActionResult> VerifyTwoFaces([FromBody] ValidateInputModel validate)
        {

            OCRUsage ocr;
            context.RequestLog.Add(new RequestLogs { Email = validate.Email, Description = validate.Base64Encoded.Substring(0, 10), FileName = "Face Image validation" });
            context.SaveChanges();

            var sourceImageURL = await _restClientService.UploadDocument(new DocumentUploadRequest
            {
                FolderName = _appSettings.AzureContentFolderName,
                Base64String = validate.Base64Encoded,
                FileName = validate.UserName + "_source"
            });

            var targetImageURL = await _restClientService.UploadDocument(new DocumentUploadRequest
            {
                FolderName = _appSettings.AzureContentFolderName,
                Base64String = validate.Base64EncodedTarget,
                FileName = validate.UserName + "_target"
            });
            if (sourceImageURL == null || targetImageURL == null)
            {
                return new UnprocessableEntityObjectResult(HelperLib.ReponseClass.ReponseMethod("source or image obj after upload document is null", false));

            }

            try
            {
                ocr = _ocrRepository.ValidateUsage(sourceImageURL.Url);
                // _ocrRepository.ValidateUsage(targetImageURL.Url);
                context.ImageScanneds.Add(new ImagesScanned { OcrUsageId = ocr.Id, ImageURL = sourceImageURL.Url });
                context.ImageScanneds.Add(new ImagesScanned { OcrUsageId = ocr.Id, ImageURL = targetImageURL.Url });
                await context.SaveChangesAsync();


            }
            catch (MaximumOCRUsageException e)
            {
                Debug.WriteLine(e);
                //TODO: Return a base response from here
                return BadRequest(ResponseViewModel<String>.Failed(ResponseMessageViewModel.UNSUCCESSFUL));
            }

            var bypass = Configuration.GetSection("AppSettings").GetSection("ByPassFacial").Value;
            if (bypass.ToLower() == "true")
            {
                log.Info($"bypass set to true for {validate.Email}");
                return new OkObjectResult("success");

            }

            try
            {
                var result = await faceValidation.VerifySimilarFaces(new List<string>() { targetImageURL.Url }, sourceImageURL.Url);
                if (result.IsSuccess)
                {
                    var similar = result.faces;
                    await context.SimilarFacesRecord.AddAsync(new SimilarFace { Confidence = similar.Confidence, FaceId = Guid.NewGuid(), PersistedFaceId = Guid.NewGuid(), OCRUsage = ocr }); ;
                    await context.SaveChangesAsync();


                    return new OkObjectResult(HelperLib.ReponseClass.ReponseMethodGeneric("Successful", similar, true));

                }
                else
                {
                    return new UnprocessableEntityObjectResult(HelperLib.ReponseClass.ReponseMethod("Face not seen", false));
                    // return new UnprocessableEntityObjectResult(result.faces);

                }
            }
            catch (Exception ex)
            {
                log.Error($"source:{sourceImageURL} target {targetImageURL}, error {ex}");
                return new BadRequestObjectResult(HelperLib.ReponseClass.ReponseMethod(ex.ToString(), false));
            }
        }





        [HttpGet]
        [Route("QaWork")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult QaWork([Required] string identifier)
        {


            var connString = Configuration.GetConnectionString("OneBankConn");
            connString = "";
            connString = "Server=localhost;Initial Catalog=KMNDB; Integrated Security=false;user id=sa;password=reallyStrongPwd123";

            try
            {
                SqlDataClientLib.Class1 c = new SqlDataClientLib.Class1();
                var code = c.ReturnSingle(connString, "SELECT [Code] FROM [KMNDB].[dbo].[OTP_table] where id=1", "");
                if (!string.IsNullOrEmpty(code))
                {
                    var query = $"update [KMNDB].[dbo].[OTP_table] set userid=2 where code={code}";
                    var num = c.ExecuteDbAction(connString, query, "");
                    //return new OkObjectResult(result.faces);
                    if (num > 0)
                    {
                        return new OkObjectResult("success");

                    }
                    else
                    {
                        return new UnprocessableEntityObjectResult("");

                    }

                }
                else
                {
                    return new UnprocessableEntityObjectResult("");

                }
            }
            catch (Exception ex)
            {
                log.Error($"source:{identifier}, error {ex}");
                return new BadRequestObjectResult(ex);
            }
        }

        [HttpGet]
        [Route("NewToWorkWork")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult NewToWorkWork(string identifier, string name)
        {


            var connString = Configuration.GetConnectionString("OneBankConn");
            //connString = "";
            connString = "Server=10.0.41.101;Initial Catalog=SterlingMobile;Integrated Security=False;User ID=sa;Password=tylent;MultipleActiveResultSets=True;pooling=true;Max Pool Size=200;";

            try
            {
                SqlDataClientLib.Class1 c = new SqlDataClientLib.Class1();
                var getuserid = $"select userid FROM [SterlingMobile].[dbo].[User] where UserName = @username";
                var id = c.ReturnSingle(connString, getuserid, name);
                if (string.IsNullOrEmpty(id))
                {

                    return new UnprocessableEntityObjectResult($"cant get id because {c.errorMessage}");
                }


                var query = $"delete  from [SterlingMobile].[dbo].userdeviceinfo where UserId =@param;";
                var num = c.ExecuteDbAction(connString, query, id);

                var query2 = $"delete  from [SterlingMobile].[dbo].Beneficiary where UserId =@param;";
                var num2 = c.ExecuteDbAction(connString, query2, id);

                var query3 = $"delete  from [SterlingMobile].[dbo].[User] where username =@param;";
                var num3 = c.ExecuteDbAction(connString, query3, name);
                //return new OkObjectResult(result.faces);
                if (num > 0 && num3 > 0)
                {
                    return new OkObjectResult("success");

                }
                else
                {
                    return new UnprocessableEntityObjectResult("cant delete");

                }




            }
            catch (Exception ex)
            {
                log.Error($"source:{identifier}, error {ex}");
                return new BadRequestObjectResult(ex);
            }
        }




    }
}
