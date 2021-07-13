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

        /// <summary>
        /// Constructor Overload Method For Message
        /// </summary>
        /// <param name="message"></param>
        private ResponseViewModel(string message) {

            Message = message;
        }

        /// <summary>
        /// Method Overloads
        /// </summary>
        /// <param name="Status"></param>
        private ResponseViewModel(bool status)
        {
            Status = status;
        }

        /// <summary>
        /// Method Overloads
        /// </summary>
        /// <param name="Status"></param>
        private ResponseViewModel(bool status, string message)
        {
            Status = status;
            Message = message;
        }

        /// <summary>
        /// Method Overloads
        /// </summary>
        /// <param name="Status"></param>
        private ResponseViewModel(bool status, string message, T data)
        {
            Status = status;
            Message = message;
            Data = data;
        }

        /// <summary>
        /// OK - Success Method that returns true
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResponseViewModel<T> Ok(string message)
        {
            return new ResponseViewModel<T>(true, message);
        }

        /// <summary>
        /// Method overload for Ok-Data only
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ResponseViewModel<T> Ok(T data)
        {
            return new ResponseViewModel<T>(true, ResponseMessageViewModel.OK, data);
        }

        /// <summary>
        /// Ok - Success Method With Data
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ResponseViewModel<T> Ok(string message, T data)
        {
            return new ResponseViewModel<T>(true, message, data);
        }

        /// <summary>
        /// Unsuccessful without message, default-message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResponseViewModel<T> Failed()
        {
            return new ResponseViewModel<T>(false, ResponseMessageViewModel.UNSUCCESSFUL);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResponseViewModel<T> Failed(string message)
        {
            return new ResponseViewModel<T>(false, message);
        }
    }

    public static class ResponseMessageViewModel
    {
        public static readonly string OK = "Successful";
        public static readonly string UNSUCCESSFUL = "Unsuccessful";


    }
}
