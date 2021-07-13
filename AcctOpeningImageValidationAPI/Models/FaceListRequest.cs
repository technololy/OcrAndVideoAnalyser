using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AcctOpeningImageValidationAPI.Models
{
    public class FaceListRequest
    {
        [Required]
        public IFormFile File { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
