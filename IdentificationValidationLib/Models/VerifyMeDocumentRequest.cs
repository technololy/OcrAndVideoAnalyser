using System;
using System.Collections.Generic;
using System.Text;

namespace IdentificationValidationLib.Models
{
    public class VerifyMeDocumentRequest
    {
        public string IDNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
    }
}

