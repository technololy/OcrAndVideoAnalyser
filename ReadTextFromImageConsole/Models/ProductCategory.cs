using System;
using System.Collections.Generic;

namespace ReadTextFromImageConsole.Models
{
    public partial class ProductCategory
    {
        public Guid Id { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public bool IsActive { get; set; }
    }
}
