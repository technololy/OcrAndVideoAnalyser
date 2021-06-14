using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IdentificationValidationLib.Abstractions
{
    public interface INetworkService
    {
        Task<T> GetAsync<T, R>(string path, AuthType authType, R data);
        Task<T> PostAsync<T, R>(string path, AuthType authType, R data);
        Task<T> PutAsync<T, R>(string path, AuthType authType, R data);
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
