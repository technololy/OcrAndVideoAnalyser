using IdentificationValidationLib.Abstractions;
using IdentificationValidationLib.Models;
using System;
using System.Threading.Tasks;

namespace IdentificationValidationLib
{
    public class VerifyMeService : IVerifyMeService
    {
        private readonly INetworkService _networkService;
        public VerifyMeService(INetworkService networkService)
        {
            _networkService = networkService;
        }
        public async Task<(bool isSuccess, string msg, object data)> Validate(string firstName, string middleName, string lastName, string idNumber, DateTime dateOfBirth, Validation.DocumentType docType, Validation.DocumentServiceType documentServiceType)
        {
            try
            {
                switch (docType)
                {
                    case Validation.DocumentType.DriversLicense:
                        var frscResult = await _networkService.PostAsync<DriverLicenseResponse, VerifyMeVerificationRequest>("/frsc", AuthType.BASIC,
                            new VerifyMeVerificationRequest
                            {
                                firstname = firstName,
                                lastname = lastName,
                                dob = dateOfBirth.ToString("dd-MM-yyyy"),
                                idNumber = idNumber
                            });
                        return (true, frscResult.dataResponse.status, frscResult.dataResponse.data);
                    default:
                        var result = await _networkService.PostAsync<NINResponse, VerifyMeVerificationRequest>("/nin", AuthType.BASIC,
                           new VerifyMeVerificationRequest
                           {
                               firstname = firstName,
                               lastname = lastName,
                               dob = dateOfBirth.ToString("dd-MM-yyyy"),
                               idNumber = idNumber
                           });
                        return (true, result.dataResponse.status, result.dataResponse.data);
                }
                
            }catch(Exception e)
            {
                return (false, $"Unable to verify, please try again. Reason : {e.Message}", null);
            }
        }
    }
}
