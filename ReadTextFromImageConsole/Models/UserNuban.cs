using System;
using System.Collections.Generic;

namespace ReadTextFromImageConsole.Models
{
    public partial class UserNuban
    {
        public Guid Id { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public Guid UserId { get; set; }
        public string CustomerId { get; set; }
        public string AccountName { get; set; }
        public string Currency { get; set; }
        public string PassportPhoto { get; set; }
        public string Reference { get; set; }
        public string Signature { get; set; }
        public string ValidId { get; set; }
        public string ValidIdNumber { get; set; }
        public string ValidIdType { get; set; }
        public int AccountCategory { get; set; }
        public Guid? AccountLinkCode { get; set; }
        public string BvnFirstname { get; set; }
        public string BvnLastname { get; set; }
        public double? JointAccountTransferLimit { get; set; }
        public int SqNo { get; set; }
        public string IncorpDate { get; set; }
        public string RcNumber { get; set; }
        public string Residence { get; set; }
        public string ShortTitle { get; set; }
        public string Tin { get; set; }
        public string Mode { get; set; }
        public bool NotificationStatus { get; set; }
    }
}
