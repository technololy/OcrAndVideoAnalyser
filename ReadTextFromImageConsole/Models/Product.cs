using System;
using System.Collections.Generic;

namespace ReadTextFromImageConsole.Models
{
    public partial class Product
    {
        public Guid Id { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public bool IsActive { get; set; }
        public int CategoryId { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public string FriendlyName { get; set; }
    }
}
