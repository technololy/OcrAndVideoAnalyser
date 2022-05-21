using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using IdentificationValidationLib.Models;
using Newtonsoft.Json;

namespace EncryptionConsole
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var request = new Request();
            var videoRequest = new VideoFileRequest { VideoFile = "base64" };

            request.Body = Encryption.Encryption.Encrypt(JsonConvert.SerializeObject(videoRequest), "IconFlux-CLIENT-001", "ed8fb6c1-2bcf-4163-a7ec-f28973acf538").Result;

            HttpClient httpClient = new HttpClient();
            var apiCallUrl = "";
            FaceResponse responseObject = null;

            try
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var request_string = JsonConvert.SerializeObject(request);

                using (StringContent content = new StringContent(request_string, Encoding.UTF8, "application/json"))
                {
                    var response = await httpClient.PostAsync($"", content).ConfigureAwait(true);
                    var response_string = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
                    Console.WriteLine($"Get response: {apiCallUrl}  \n{response_string} ***");
                    if (!string.IsNullOrEmpty(response_string))
                    {
                        responseObject = JsonConvert.DeserializeObject<FaceResponse>(response_string);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public class Request
    {
        public string Body { get; set; }
    }

    public class VideoFileRequest
    {
        public string VideoFile { get; set; }
    }
}
