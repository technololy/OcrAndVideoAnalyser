using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace IdentificationValidationLib
{
    public class OCR
    {
        HttpClient apiClient;

        public OCR()
        {
            apiClient = new HttpClient();
        }





        public byte[] GetImageURLAsByteArray(string imageFilePath)
        {
            var response = apiClient.GetAsync(imageFilePath).Result;
            var bytes = response.Content.ReadAsByteArrayAsync().Result;
            return bytes;
        }
    }
}
