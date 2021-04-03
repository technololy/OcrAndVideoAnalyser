using System;
using Newtonsoft.Json;

namespace HelperLib.Verify
{
    public class VerifyResult
    {

        public string Status { get; set; }
        public string Message { get; set; }
        public Data data { get; set; }
    }
    public class Data
    {
        [JsonProperty(PropertyName = "isIdentical")]
        public bool IsIdentical
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "confidence")]
        public double Confidence
        {
            get;
            set;
        }






    }
}


