using System;
using System.Collections.Generic;

namespace ReadTextFromImageConsole.Models
{
    public partial class CamuDocument
    {
        public Guid Id { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string AccountNumber { get; set; }
        public string Folio { get; set; }
        public string Source { get; set; }
        public string MeansOfIdbase64 { get; set; }
        public string OtherBase64 { get; set; }
        public string PhotoIdbase64 { get; set; }
        public string AddState { get; set; }
        public string Urlreference1Base64 { get; set; }
        public string Urlreference2Base64 { get; set; }
        public string PhotoMandateBase64 { get; set; }
        public string SignatureMandateBase64 { get; set; }
        public string Idno { get; set; }
        public string Idtype { get; set; }
        public string Issuedate { get; set; }
        public string Iexpirydate { get; set; }
        public string Status { get; set; }
    }
}
