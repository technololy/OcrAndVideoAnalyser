using System;
using System.Collections.Generic;

namespace ReadTextFromImageConsole.Models
{
    public partial class CamuCorpData
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string Bvn { get; set; }
        public string CustomerId { get; set; }
        public string DateOfIncorp { get; set; }
        public string Industry { get; set; }
        public string InternalRef { get; set; }
        public string ProductName { get; set; }
        public string RcNumber { get; set; }
        public string RefereeAccountNumber1 { get; set; }
        public string RefereeAccountNumber2 { get; set; }
        public string RefereeBank1 { get; set; }
        public string RefereeBank2 { get; set; }
        public string Sector { get; set; }
        public string Source { get; set; }
        public string Tin { get; set; }
        public string ExistingAccount { get; set; }
        public string CorporateAddress { get; set; }
        public string InternalReference { get; set; }
        public string Chequerequired { get; set; }
        public string Response { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
