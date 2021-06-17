using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IdentificationValidationLib.Models
{
    public class FaceRequestImages
    {
        [Required]
        public string [] Images { get; set; }

        [Required]
        public string UserIdentification { get; set; }
    }

    public class FaceRequestForm
    {
        [Required]
        public IFormFile File { get; set; }

        [Required]
        public string UserIdentification { get; set; }
    }
}
