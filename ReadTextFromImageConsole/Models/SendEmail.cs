using System;
namespace ReadTextFromImageConsole.Models
{
    public class SendEmail
    {
        public string subj { get; set; }
        public string destinationEmail { get; set; }
        public string recieverFirstName { get; set; }
        public string base64Attachment { get; set; }
        public string fileName { get; set; }
        public string addressesToCopy { get; set; }
        public string customerName { get; set; }
        public string fromEmail { get; set; }
        public string body { get; set; }
    }
}
