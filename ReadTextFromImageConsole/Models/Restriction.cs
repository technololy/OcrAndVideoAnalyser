using System;
namespace ReadTextFromImageConsole.Models
{
    public class Restriction
    {
        public Restriction()
        {
        }


        // Root m   yDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        public class RestrictionResponse
        {
            public string responseCode { get; set; }
            public string responseDescription { get; set; }
        }

        public class RestrictionResponseResponse
        {
            public RestrictionResponse restrictionResponse { get; set; }
        }



        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        public class RemoveRestriction
        {
            public string branchcode { get; set; }
            public string account { get; set; }
            public string accountsType { get; set; }
            public string restriction_code { get; set; }
        }

        public class RestrictionResponseRequest
        {
            public RemoveRestriction removeRestriction { get; set; }
        }



    }
}
