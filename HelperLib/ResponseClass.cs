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
}
