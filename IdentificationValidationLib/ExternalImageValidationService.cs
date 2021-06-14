using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using IdentificationValidationLib.Abstractions;
using IdentificationValidationLib.Models;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static IdentificationValidationLib.Validation;

namespace IdentificationValidationLib
{


    public interface IExternalImageValidationService
    {
        public Task<(bool isSuccess, string msg)> ValidateDoc(Validation v, Models.Camudatafield camudatafield);
        public Task<(bool isSuccess, string msg)> ValidateDoc(string firstName, string middleName, string lastName, string idNumber, DateTime dateOfBirth, DocumentType docType);
        public Task<(bool isSuccess, string msg, object data)> ValidateDoc(string firstName, string middleName, string lastName, string idNumber, DateTime dateOfBirth, DocumentType docType, DocumentServiceType documentServiceType);
    }


    public class ExternalImageValidationService : IExternalImageValidationService
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IConfiguration configuration;
        private readonly IAPI aPI;
        private readonly AppSettings _setting;
        private readonly INetworkService _networkService;

        public ExternalImageValidationService(IConfiguration configuration, IAPI aPI, IOptions<AppSettings> options, INetworkService networkService)
        {
            this.configuration = configuration;
            this.aPI = aPI;
            _setting = options.Value;
            _networkService = networkService;
        }



        public async Task<(bool isSuccess, string msg)> ValidateDoc(Validation v, Models.Camudatafield camudatafield)
        {
            string url = GetAppruvURLByDocType(v.docType);
            var model = new { id = v.idNumber, first_name = v.firstName ?? camudatafield.FirstName, last_name = v.lastName ?? camudatafield.LastName, date_of_birth = v.dateOfBirth ?? Convert.ToDateTime(camudatafield.Dob).ToString("yyyy-MM-dd") };
#if DEBUG
            //if (v.docType == Validation.DocumentType.DriversLicense)
            //{
            //    model = new { id = "ABC00578AA2", first_name = "Henry", last_name = "Nwandicne", date_of_birth = "1976-04-15" };

            //}
            //else if (v.docType == Validation.DocumentType.VotersCard)
            //{
            //    model = new { id = "90F5B0407E2960502637", first_name = "Nwabia", last_name = "Chidozie", date_of_birth = "1998-01-10" };

            //}
            //else if (v.docType == Validation.DocumentType.InternationalPassport)
            //{
            //    model = new { id = "A50013320", first_name = "Sunday", last_name = "Obafemi", date_of_birth = "1975-04-25" };

            //}
            //else if (v.docType == Validation.DocumentType.nationalId)
            //{
            //    model = new { id = "AKW06968AA2", first_name = "Michael", last_name = "Olugbenga", date_of_birth = "1982-05-20" };

            //}
#endif



            var result = await aPI.Post<AppruveResponseModelSuccess, AppruveResponseModelFailure>(model, url);
            if (result.isSuccess)
            {
                //got success from appruve
                //UpdateCamuDataOfAppruveResponse(result, camudatafield);
                log.Info(result.returnedStringContent);
                return (result.isSuccess, result.returnedStringContent);
            }
            else
            {
                //failure from appruve
                Console.WriteLine(result.failedObj.message);
                //UpdateCamuDataOfAppruveResponse(result, camudatafield);
                log.Info($"{result.failedObj} for {Newtonsoft.Json.JsonConvert.SerializeObject(v)} and {Newtonsoft.Json.JsonConvert.SerializeObject(camudatafield)} ");
                return (result.isSuccess, result.failedObj.message + $"{Environment.NewLine}Our partners at the validation agency were not able to validate the identification document with the ID:{v.idNumber},First Name:{camudatafield.FirstName},Last Name:{camudatafield.LastName}, and Date of Birth:{camudatafield.Dob}");
                // Console.WriteLine(result.returnedStringContent);

            }

        }
        private string GetAppruvURLByDocType(Validation.DocumentType docType)
        {
            string categoryEndPoint = "";
            switch (docType)
            {
                case Validation.DocumentType.DriversLicense:
                    categoryEndPoint = AppruveCurl.driver_license;
                    break;
                case Validation.DocumentType.InternationalPassport:
                    categoryEndPoint = AppruveCurl.passport;
                    break;
                case Validation.DocumentType.VotersCard:
                    categoryEndPoint = AppruveCurl.voter;
                    break;
                case Validation.DocumentType.nationalId:
                    categoryEndPoint = AppruveCurl.national_id;
                    break;
                default:
                    break;
            }
            string url = $"{configuration.GetSection("AppSettings").GetSection("AppruveBaseURL").Value}/{categoryEndPoint}";
            return url;
        }


        public class AppruveCurl
        {
            public const string passport = "passport";
            public const string voter = "voter";
            public const string driver_license = "driver_license";
            public const string national_id = "national_id";
        }


        public async Task<(bool isSuccess, string msg)> ValidateDoc(string firstName, string middleName, string lastName, string idNumber, DateTime dateOfBirth, DocumentType docType)
        {
            string url = GetAppruvURLByDocType(docType);
            var model = new { id = idNumber, first_name = firstName, last_name = lastName, date_of_birth = dateOfBirth.ToString("yyyy-MM-dd") };
#if DEBUG
            //if (v.docType == Validation.DocumentType.DriversLicense)
            //{
            //    model = new { id = "ABC00578AA2", first_name = "Henry", last_name = "Nwandicne", date_of_birth = "1976-04-15" };

            //}
            //else if (v.docType == Validation.DocumentType.VotersCard)
            //{
            //    model = new { id = "90F5B0407E2960502637", first_name = "Nwabia", last_name = "Chidozie", date_of_birth = "1998-01-10" };

            //}
            //else if (v.docType == Validation.DocumentType.InternationalPassport)
            //{
            //    model = new { id = "A50013320", first_name = "Sunday", last_name = "Obafemi", date_of_birth = "1975-04-25" };

            //}
            //else if (v.docType == Validation.DocumentType.nationalId)
            //{
            //    model = new { id = "AKW06968AA2", first_name = "Michael", last_name = "Olugbenga", date_of_birth = "1982-05-20" };

            //}
#endif



            var result = await aPI.Post<AppruveResponseModelSuccess, AppruveResponseModelFailure>(model, url);
            if (result.isSuccess)
            {
                //got success from appruve
                //UpdateCamuDataOfAppruveResponse(result, camudatafield);
                log.Info(result.returnedStringContent);
                return (result.isSuccess, result.returnedStringContent);
            }
            else
            {
                //failure from appruve
                Console.WriteLine(result.failedObj.message);
                //UpdateCamuDataOfAppruveResponse(result, camudatafield);
                log.Info($"{result.failedObj} for {firstName} {lastName} {middleName} {idNumber}");
                return (result.isSuccess, result.failedObj.message + $"{Environment.NewLine}Our partners at Appruv, the validation agency, were not able to validate the identification document with the ID:{idNumber},First Name:{firstName},Last Name:{lastName}, and Date of Birth:{dateOfBirth}");
                // Console.WriteLine(result.returnedStringContent);

            }

        }

        public async Task<(bool isSuccess, string msg, object data)> ValidateDoc(string firstName, string middleName, string lastName, string idNumber, DateTime dateOfBirth, DocumentType docType, DocumentServiceType documentServiceType)
        {
            object result = documentServiceType switch
            {
                DocumentServiceType.APPRUV => await ValidateDoc(firstName, middleName, lastName, idNumber, dateOfBirth, docType),
                DocumentServiceType.VERIFY_ME => await _networkService.PostAsync<DriverLicenseResponse, DriverLicenseRequest>("/frsc", AuthType.BASIC, new DriverLicenseRequest { }),
                _ => await ValidateDoc(firstName, middleName, lastName, idNumber, dateOfBirth, docType),
            };

            return (true, string.Empty, new { });
        }
    }
}
