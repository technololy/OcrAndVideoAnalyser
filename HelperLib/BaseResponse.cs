using System;
namespace HelperLib
{
    public class BaseResponse<T>
    {
        public BaseResponse()
        {
        }

        public bool Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

    }

    public class BaseResponse
    {
        public BaseResponse()
        {
        }

        public bool Status { get; set; }
        public string Message { get; set; }


    }
}
