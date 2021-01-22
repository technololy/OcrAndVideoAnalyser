using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ReadTextFromImageConsole
{
    public class API
    {
        HttpClient client;

        public API()
        {
            client = new HttpClient();
     
            string accessToken = Program.config.GetSection("AppSettings").GetSection("AppruveToken").Value; ;

            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer ", accessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
        }
         public API(bool isOneExpress,string useIt)
        {
            client = new HttpClient();
     
          

            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer ", accessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
       
        }

        public API(bool acctOpening)
        {
            client = new HttpClient();

            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer ", accessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (acctOpening)
            {
                var key = Program.config?.GetSection("AppSettings").GetSection("CamsKey").Value ?? LoggingLib.GlobalConfig.CamsKey;
                //client.DefaultRequestHeaders.Add("ApiKey", "wrqewtreyrutyterewrtretre");
                client.DefaultRequestHeaders.Add("ApiKey", key);

            }
        }

        internal async Task<(bool isSuccess, string returnedStringContent, T1 SuccessObj, T2 failedObj)> Post<T1, T2>(object model, string endPoint)//T1 is success mode, T2 is failure model
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
                Console.WriteLine($" url requested is {endPoint}");
                response = client.PostAsync(endPoint, content).Result;
                var result = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine($" response received is {result}");

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
