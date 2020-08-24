using System;
using System.Collections.Generic;

namespace ReadTextFromImageConsole.Models
{
    public partial class Camudatafield
    {
        public Guid Id { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Folio { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string BranchCode { get; set; }
        public string Bvn { get; set; }
        public string Source { get; set; }
        public string DisContactPersonAdd { get; set; }
        public string DisContactPersonMobile { get; set; }
        public string DisContactPersonName { get; set; }
        public string Dob { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string MobileNumber { get; set; }
        public string ResidentialAddress { get; set; }
        public string UrlmeansOfId { get; set; }
        public string Urlother { get; set; }
        public string UrlphotoId { get; set; }
        public string AddState { get; set; }
        public string Urlreference1 { get; set; }
        public string Urlreference2 { get; set; }
        public string Urlmandate { get; set; }
        public string Idno { get; set; }
        public string Idtype { get; set; }
        public string Issuedate { get; set; }
        public string Iexpirydate { get; set; }
        public string Response { get; set; }
        public bool? IsIdentificationImageChecked { get; set; }
        public DateTime? DateIdentificationImageIsChecked { get; set; }
        public string IdentificationImageExtractedTextFromImage { get; set; }
        public string IdentificationImageCheckResponse { get; set; }
        public string IdentificationImageCheckResponseJson { get; set; }
        public bool? IsFacialImageChecked { get; set; }
        public DateTime? DateFacialImageIsChecked { get; set; }
        public string FacialImageExtractedTextFromImage { get; set; }
        public string FacialImageCheckResponse { get; set; }
        public string FacialImageCheckResponseJson { get; set; }
        public string Landmark { get; set; }
        public string NearestBusStop { get; set; }
    }
}
