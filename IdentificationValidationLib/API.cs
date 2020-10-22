using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace IdentificationValidationLib
{


    public interface IAPI
    {
        public Task<(bool isSuccess, string returnedStringContent, T1 SuccessObj, T2 failedObj)> Post<T1, T2>(object model, string endPoint);

        public Task<(bool isSuccess, string returnedStringContent, T1 SuccessObj)> PostAny<T1>(object model, string endPoint);
    }

    public class API : IAPI
    {
        HttpClient client;
        private readonly IConfiguration configuration;

        public API(IConfiguration configuration)
        {
            client = new HttpClient();
            //string accessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJqdGkiOiI5YTZkZDc3NS0zODc4LTQ1M2UtYmM4Ny1lZjU4MTA3YjRhYjEiLCJhdWQiOiI3ZmRmNTVmNC1hNTRhLTQ1ZWItOWRjZS00ZThlM2IwMWRmNmEiLCJzdWIiOiJjZjRiYWMxNi0zOTA3LTQ4MDAtYTY5Mi04YTQzZDhjMzkwYjAiLCJuYmYiOjAsInNjb3BlcyI6WyJ2ZXJpZmljYXRpb25fdmlldyIsInZlcmlmaWNhdGlvbl9saXN0IiwidmVyaWZpY2F0aW9uX2RvY3VtZW50IiwidmVyaWZpY2F0aW9uX2lkZW50aXR5Il0sImV4cCI6MzE3Mjk1NTMyMCwiaWF0IjoxNTk1MTE4NTIwfQ.5qGsVsB35RJPl3UsgCvdfWOlBqRP28YEMjrO43Up3tM";

            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer ", accessToken);

            this.configuration = configuration;
            string accessToken = configuration.GetSection("AppSettings").GetSection("AppruveToken").Value; ;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
        }

        public API(bool acctOpening)
        {
            client = new HttpClient();

            ////client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer ", accessToken);
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //if (acctOpening)
            //{
            //    var key = Program.config.GetSection("AppSettings").GetSection("CamsKey").Value;
            //    //client.DefaultRequestHeaders.Add("ApiKey", "wrqewtreyrutyterewrtretre");
            //    client.DefaultRequestHeaders.Add("ApiKey", key);

            //}
        }

        public async Task<(bool isSuccess, string returnedStringContent, T1 SuccessObj, T2 failedObj)> Post<T1, T2>(object model, string endPoint)//T1 is success mode, T2 is failure model
        {
            try
            {
                await Task.Delay(1);
                HttpResponseMessage response;

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                response = client.PostAsync(endPoint, content).Result;
                var result = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    var successObj = Newtonsoft.Json.JsonConvert.DeserializeObject<T1>(result);
                    return (true, result, successObj, default);
                }
                else
                {
                    var failedObj = Newtonsoft.Json.JsonConvert.DeserializeObject<T2>(result);
                    return (false, result, default, failedObj);

                }


            }
            catch (Exception ex)
            {
                return (false, ex.ToString(), default, default);

            }
        }


        public async Task<(bool isSuccess, string returnedStringContent, T1 SuccessObj)> PostAny<T1>(object model, string endPoint)//T1 is success mode, T2 is failure model
        {
            try
            {
                await Task.Delay(1);
                HttpResponseMessage response;

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                response = client.PostAsync(endPoint, content).Result;
                var result = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    var successObj = Newtonsoft.Json.JsonConvert.DeserializeObject<T1>(result);
                    return (true, result, successObj);
                }
                else
                {

                    return (false, result, default);

                }


            }
            catch (Exception ex)
            {
                return (false, ex.ToString(), default);

            }
        }




    }
}
