using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AcctOpeningImageValidationAPI.Models
{
    public class MultipleFormRequest
    {
        public IFormFile[] Files { get; set; }
        public string UserIdentification { get; set; }
    }
}
