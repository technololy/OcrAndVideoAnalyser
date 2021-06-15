using IdentificationValidationLib.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static IdentificationValidationLib.Validation;

namespace IdentificationValidationLib.Abstractions
{
    public interface IVerifyMeService
    {
        public Task<(bool isSuccess, string msg, object data)> Validate (string firstName, string middleName, string lastName, string idNumber, DateTime dateOfBirth, DocumentType docType, DocumentServiceType documentServiceType);
    }
}
