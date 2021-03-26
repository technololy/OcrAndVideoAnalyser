using System;
namespace HelperLib
{
    public static class ReponseClass
    {
        public static BaseResponse ReponseMethod(string message, bool status)
        {
            return new BaseResponse
            {

                Message = message,
                Status = status

            };
        }


        public static BaseResponse<T> ReponseMethodGeneric<T>(string message, T resultResponse, bool status = false)
        {
            return new BaseResponse<T>
            {
                Data = (T)resultResponse,

                Message = message,
                Status = status
            };
        }
    }


    public class FacialValidation
    {
        public int Id { get; set; }

        public string BVN { get; set; }
        public string Email { get; set; }
        public string Accessories { get; set; }
        public string FacialHair { get; set; }
        public string Hair { get; set; }
        public string Emotion { get; set; }
        public string Smile { get; set; }
        public string Age { get; set; }
        public string HeadPose { get; set; }
        public string Gender { get; set; }
        public string Occlusion { get; set; }
        public DateTime? DateInserted { get; set; }
    }
}
