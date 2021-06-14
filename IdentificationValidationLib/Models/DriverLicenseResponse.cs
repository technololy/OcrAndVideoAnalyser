using System;
using System.Collections.Generic;
using System.Text;

namespace IdentificationValidationLib.Models
{
    public class FieldMatches
    {
        public bool lastname { get; set; }
    }

    public class DriverLicense
    {
        public string licenseNo { get; set; }
        public string firstname { get; set; }
        public string middlename { get; set; }
        public string lastname { get; set; }
        public string issuedDate { get; set; }
        public string expiryDate { get; set; }
        public string stateOfIssue { get; set; }
        public string gender { get; set; }
        public string birthdate { get; set; }
        public string photo { get; set; }
        public FieldMatches fieldMatches { get; set; }
    }

    public class DriverLicenseData
    {
        public string status { get; set; }
        public DriverLicense data { get; set; }
    }

    public class DriverLicenseResponse
    {
        public string responseCode { get; set; }
        public DriverLicenseData dataResponse { get; set; }
    }

    public class DriverLicenseRequest
    {
        public string idNumber { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string dob { get; set; }
    }

}
