using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AcctOpeningImageValidationAPI.Models
{
    public class SignalModel
    {
        [Required]
        public string Message { get; set; }
    }
}
