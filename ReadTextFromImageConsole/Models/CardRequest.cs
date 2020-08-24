using System;
using System.Collections.Generic;

namespace ReadTextFromImageConsole.Models
{
    public partial class CardRequest
    {
        public Guid Id { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string AccountId { get; set; }
        public string IsMasterCard { get; set; }
        public string IsVerveCard { get; set; }
        public string SessionKey { get; set; }
        public string EmailAddress { get; set; }
    }
}
