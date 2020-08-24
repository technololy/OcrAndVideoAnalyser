using System;
using System.Collections.Generic;

namespace ReadTextFromImageConsole.Models
{
    public partial class IbscardRequestResponse
    {
        public Guid Id { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ReferenceId { get; set; }
        public string RequestType { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseText { get; set; }
    }
}
