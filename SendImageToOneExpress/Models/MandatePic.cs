using System;
using System.Collections.Generic;

namespace SendImageToOneExpress.Models
{
    public partial class MandatePic
    {
        public string Nuban { get; set; }
        public string AccountName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Bvn { get; set; }
        public string WorkingBalance { get; set; }
        public byte? RestrictionCode { get; set; }
        public string Restriction { get; set; }
        public DateTime? DateOpen { get; set; }
    }
}
