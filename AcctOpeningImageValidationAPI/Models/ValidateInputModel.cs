using System;
using System.ComponentModel.DataAnnotations;

namespace AcctOpeningImageValidationAPI.Models
{
    public class ValidateInputModel
    {
        public string Email { get; set; }

        [Required]
        public string Base64Encoded { get; set; }

        public string UserName { get; set; }
        public string Base64EncodedTarget { get; set; }

    }
}
