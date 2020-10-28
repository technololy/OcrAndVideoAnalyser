using System;
using System.Reflection;
using System.Threading.Tasks;
using IdentificationValidationLib.Models;
using log4net;
using Microsoft.Extensions.Configuration;

namespace IdentificationValidationLib
{


    public interface IExternalImageValidationService
    {
        public Task<(bool isSuccess, string msg)> ValidateDoc(Validation v, Models.Camudatafield camudatafield);
    }


    public class ExternalImageValidationService : IExternalImageValidationService
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IConfiguration configuration;
        private readonly IAPI aPI;

        public ExternalImageValidationService(IConfiguration configuration, IAPI aPI)
        {
            this.configuration = configuration;
            this.aPI = aPI;
        }



        public async Task<(bool isSuccess, string msg)> ValidateDoc(Validation v, Models.Camudatafield camudatafield)
        {
            string url = GetURLByDocType(v.docType);
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
                log.Info(result.failedObj);
                return (result.isSuccess, result.failedObj.message);
                // Console.WriteLine(result.returnedStringContent);

            }

        }
        private string GetURLByDocType(Validation.DocumentType docType)
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

    }
}
