using IdentificationValidationLib.Abstractions;
using IdentificationValidationLib.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IdentificationValidationLib
{
    public class VerfiyMeService : IVerifyMeService
    {
        private readonly INetworkService _networkService;
        public VerfiyMeService(INetworkService networkService)
        {
            _networkService = networkService;
        }
        public async Task<(bool isSuccess, string msg, object data)> Validate(string firstName, string middleName, string lastName, string idNumber, DateTime dateOfBirth, Validation.DocumentType docType, Validation.DocumentServiceType documentServiceType)
        {
            switch(docType)
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
            throw new NotImplementedException();
        }
    }
}
