using System;
namespace AcctOpeningImageValidationAPI.Repository.Response
{
    public class ResponseViewModel<T>
    {
        /// <summary>
        /// Status (value : true or false)
        /// </summary>
        private bool Status { get; set; }

        /// <summary>
        /// Response Message
        /// </summary>
        private string Message { get; set; }

        /// <summary>
        /// Response Data
        /// Type of T
        /// </summary>
        private T Data { get; set; }

        /// <summary>
        /// Response Status Code
        /// </summary>
        private string StatusCode { get; set; }
    }
}
