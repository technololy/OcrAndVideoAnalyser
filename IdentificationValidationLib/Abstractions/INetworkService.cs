using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IdentificationValidationLib.Abstractions
{
    public interface INetworkService
    {
        Task<T> GetAsync<T>(string path, AuthType authType, T data);
        Task<T> PostAsync<T>(string path, AuthType authType, T data);
        Task<T> PutAsync<T>(string path, AuthType authType, T data);
    }

    public enum AuthType
    {
        BASIC,
        BEARER,
        NONE
    }

    public enum AuthRequestType
    {
        GET,
        POST,
        PUT,
        DELETE,
        OPTIONS
    }
}
