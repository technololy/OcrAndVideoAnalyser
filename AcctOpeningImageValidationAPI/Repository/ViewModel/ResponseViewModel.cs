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

        /// <summary>
        /// Making the constructor to be private
        /// So that it will disable instatiation
        /// </summary>
        private ResponseViewModel() { }

        private ResponseViewModel(bool Status)
        {
            this.Status = Status;
        }

        private ResponseViewModel(bool Status, string Message)
        {
            this.Status = Status;
            this.Message = Message;
        }

        private ResponseViewModel(bool Status, string Message, T Data)
        {
            this.Status = Status;
            this.Message = Message;
            this.Data = Data;
        }

        public ResponseViewModel<T> Ok(string message)
        {
            return this;
        }


    }
}
