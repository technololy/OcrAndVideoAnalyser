using System;
using System.ComponentModel.DataAnnotations;

namespace AcctOpeningImageValidationAPI.Models
{
    public class FaceRequest
    {
        [Required]
        public string VideoFile { get; set; }
    }
}
