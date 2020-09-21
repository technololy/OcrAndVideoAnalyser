using System;
using System.Collections.Generic;

namespace SendImageToOneExpress.Models
{
    public partial class TblRecordsDone
    {
        public double? Nuban { get; set; }
        public string AccountName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Bvn { get; set; }
        public double? WorkingBalance { get; set; }
        public string RestrictionCode { get; set; }
        public string Restriction { get; set; }
        public string DateOpen { get; set; }
        public string UrlOfImageUploaded { get; set; }
        public int Id { get; set; }
    }
}
