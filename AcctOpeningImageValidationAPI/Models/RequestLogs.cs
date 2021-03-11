using System;
namespace AcctOpeningImageValidationAPI.Models
{
    public class RequestLogs : BaseEntity
    {
        public RequestLogs()
        {
        }
        public string Email { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
    }
}
