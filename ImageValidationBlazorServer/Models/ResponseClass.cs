using System;
namespace ImageValidationBlazorServer.Models
{
    public class ResponseClass
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        public class Data
        {
            public int id { get; set; }
            public object documentType { get; set; }
            public string fullName { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string middleName { get; set; }
            public string idType { get; set; }
            public string idNumber { get; set; }
            public string issueDate { get; set; }
            public string dateOfBirth { get; set; }
            public string expiryDate { get; set; }
            public string formerIDNumber { get; set; }
            public object idClass { get; set; }
            public object bvn { get; set; }
            public string email { get; set; }
            public object bloodGroup { get; set; }
            public object height { get; set; }
            public string issuingAuthority { get; set; }
            public string address { get; set; }
            public object occupation { get; set; }
            public object nextOfKin { get; set; }
            public object firstIssueState { get; set; }
            public object delim { get; set; }
            public string gender { get; set; }
            public DateTime dateInserted { get; set; }
        }

        public class Root
        {
            public bool status { get; set; }
            public string message { get; set; }
            public Data data { get; set; }
        }


        public ResponseClass()
        {
        }
    }
}
