using System;
namespace HelperLib
{
    public class FacialValidationModel
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        public class Data
        {
            public int id { get; set; }
            public object bvn { get; set; }
            public object email { get; set; }
            public object accessories { get; set; }
            public object facialHair { get; set; }
            public string hair { get; set; }
            public string emotion { get; set; }
            public string smile { get; set; }
            public string age { get; set; }
            public object headPose { get; set; }
            public string gender { get; set; }
            public object occlusion { get; set; }
            public DateTime dateInserted { get; set; }
        }

        public class Root
        {
            public bool status { get; set; }
            public string message { get; set; }
            public Data data { get; set; }
        }
    }
}
