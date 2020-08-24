using System;
using System.Collections.Generic;

namespace ReadTextFromImageConsole.Models
{
    public partial class Ereference
    {
        public Guid Id { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string AccountCurrency { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string BranchCode { get; set; }
        public string ProductType { get; set; }
        public string CustomerId { get; set; }
        public string DateAccountOpened { get; set; }
        public string RefereeBank { get; set; }
        public string RefereeAccountNumber { get; set; }
        public string RefereeName { get; set; }
    }
}
