using IdentificationValidationLib.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace IdentificationValidationLib
{
    public class NetworkService : INetworkService
    {
        /// <summary>
        /// Properties
        /// </summary>
        private readonly AppSettings _settings;

        /// <summary>
        /// Network Service Constructor
        /// </summary>
        /// <param name="options"></param>
        public NetworkService(IOptions<AppSettings> options)
        {
            _settings = options.Value;
        }

        /// <summary>
        /// Get Asynchronous HttpClient
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="authType"></param>
        /// <param name="authValue"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T, R>(string path, AuthType authType, R data)
        {
            return await CreateHttpRequestMessageAsync<T,R>(AuthRequestType.POST, authType, data, path);
        }

        /// <summary>
        /// Post Asynchronous HttpClient
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="authType"></param>
        /// <param name="authValue"></param>
        /// <returns></returns>
        public async Task<T> PostAsync<T, R>(string path, AuthType authType, R data)
        {
            return await CreateHttpRequestMessageAsync<T, R>(AuthRequestType.POST, authType, data, path);
        }

        /// <summary>
        /// Put Asynchronous HttpClient
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="authType"></param>
        /// <param name="authValue"></param>
        /// <returns></returns>
        public async Task<T> PutAsync<T, R>(string path, AuthType authType, R data)
        {
            return await CreateHttpRequestMessageAsync<T, R>(AuthRequestType.PUT, authType, data, path);
        }

        /// <summary>
        /// Create Http Request Message
        /// </summary>
        /// <param name="type"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<T> CreateHttpRequestMessageAsync<T, R> (AuthRequestType type, AuthType authType, R data, string path)
        {
            try
            {
                var httpClient = CreateHttpClient($"{_settings.VerifyMeConfig.BASE_URL}{path}", authType, _settings.VerifyMeConfig.AUTH_TOKEN);

                HttpRequestMessage request = new HttpRequestMessage(type switch
                {
                    AuthRequestType.GET => new HttpMethod("GET"),
                    AuthRequestType.POST => new HttpMethod("POST"),
                    AuthRequestType.PUT => new HttpMethod("PUT"),
                    AuthRequestType.DELETE => new HttpMethod("DELETE"),
                    AuthRequestType.OPTIONS => new HttpMethod("OPTIONS"),
                    _ => new HttpMethod("GET"),
                }, $"{_settings.VerifyMeConfig.BASE_URL}{path}");

                if (type != AuthRequestType.GET)
                {
                    var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    request.Content = content;
                }

                string jsonResult = string.Empty;

                var responseMessage = await Policy
                    .Handle<WebException>(ex =>
                    {
                        Debug.WriteLine($"{ex.GetType().Name + " : " + ex.Message}");
                        return true;
                    })
                    .WaitAndRetryAsync
                    (
                        5,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                    )
                    .ExecuteAsync(async () => await httpClient.SendAsync(request));

                if (responseMessage.IsSuccessStatusCode)
                {
                    jsonResult = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var json = JsonConvert.DeserializeObject<T>(jsonResult);
                    return json;
                }

                if (responseMessage.StatusCode == HttpStatusCode.Forbidden ||
                     responseMessage.StatusCode == HttpStatusCode.BadRequest ||
                    responseMessage.StatusCode == HttpStatusCode.InternalServerError ||
                    responseMessage.StatusCode == HttpStatusCode.Unauthorized)
                {
                    jsonResult = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var json = JsonConvert.DeserializeObject<T>(jsonResult);
                    return json;
                }

                throw new HttpRequestException(jsonResult);
            }  
            catch (Exception e)
            {
                Debug.WriteLine($"{ e.GetType().Name + " : " + e.Message}");
                throw;
            }
        }

        /// <summary>
        /// Create Http Client
        /// </summary>
        /// <param name="url"></param>
        /// <param name="authType"></param>
        /// <param name="authValue"></param>
        /// <returns></returns>
        private HttpClient CreateHttpClient (string url, AuthType authType, string authValue = "")
        {
            var httpClient = new HttpClient { BaseAddress = new Uri($"{url}") };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = authType switch
            {
                AuthType.BASIC => new AuthenticationHeaderValue("Basic", authValue),
                _ => new AuthenticationHeaderValue("Bearer", authValue),
            };
            return httpClient;
        }
    }
}
