using System;
namespace HelperLib
{

    public class SimilarFaces
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public Guid? FaceId
        {
            get;
            set;
        }

        public Guid? PersistedFaceId
        {
            get;
            set;
        }

        public double confidence
        {
            get;
            set;
        }



    }
}
