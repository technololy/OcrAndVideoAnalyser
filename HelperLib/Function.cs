using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RestSharp;

namespace HelperLib
{
    public class Function
    {
        static HttpClient client;
        public Function()
        {
            client = new HttpClient();

        }

        public async static Task<(bool isSuccess, string message)> GetBlobStorageBearerToken()
        {
            var client = new RestClient("https://login.microsoftonline.com/78d4d009-02e3-4eef-91cb-59d1c8cc861e/oauth2/token");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Cookie", "x-ms-gateway-slice=estsfd; stsservicecookie=estsfd; fpc=AozFCcN2rl9NkSynO5IQpoMwaIP2AQAAACuyp9cOAAAA");
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_Id", "b0b0ebc7-b067-4c6b-91d8-20918996e520");
            request.AddParameter("client_secret", "t.KOR817vr.rkE8Y1_6u4Tq.a_hZl3C10N");
            request.AddParameter("resource", "https://sbnk2storage.blob.core.windows.net");
            IRestResponse response = await client.ExecuteAsync(request);
            Console.WriteLine(response.Content);
            ModelLib.BearerTokenSBBankBlobDTO getTokenResponseDto = new ModelLib.BearerTokenSBBankBlobDTO();
            if (response != null && !string.IsNullOrEmpty(response.Content))
            {
                getTokenResponseDto = JsonSerializer.Deserialize<ModelLib.BearerTokenSBBankBlobDTO>(response.Content);
            }

            if (!string.IsNullOrEmpty(getTokenResponseDto.access_token))
            {
                return (true, getTokenResponseDto.access_token);
            }
            else
            {
                return (false, "");

            }
        }

        public async static Task<(bool isSuccess, byte[] message)> GetImageStream(string accessToken, string imageURL)
        {
            var client = new RestClient(imageURL);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {accessToken}");
            request.AddHeader("x-ms-version", "2019-12-12");
            IRestResponse response = await client.ExecuteAsync(request);
            Console.WriteLine(response.Content);
            if (response != null && !string.IsNullOrEmpty(response.Content))
            {
                var base64String = Convert.ToBase64String(response.RawBytes);

                return (true, response.RawBytes);
            }
            else
            {
                return (false, null);

            }
        }


        public static byte[] GetImageURLAsByteArray(string imageFilePath)
        {


            //static string imageFilePath = @"damilola_oyebanji_international_passport.jpg";
            //            string imageFilePath2 = "voters_card_sterling_customer.png";
            //            imageFilePath2 = @"lolades_drivers_license.jpg";
            //            //static string imageFilePath = @"Identification.jpg";
            //#if DEBUG
            //            var img2 = System.IO.File.ReadAllBytesAsync(imageFilePath2);
            //            var img = System.IO.File.ReadAllBytes(imageFilePath2);
            //            return img;
            //#endif

            var response = client.GetAsync(imageFilePath).Result;
            var bytes = response.Content.ReadAsByteArrayAsync().Result;
            return bytes;
        }

        public static string[] SplitDriversLicenseFullName(string text)
        {
            string lastName = ""; string firstName = ""; string middleName = "";
            var keys = new string[] { " ", "," };
            var splitted = text.Split(keys, StringSplitOptions.RemoveEmptyEntries);
            if (splitted.Length == 3)
            {
                lastName = splitted[0];
                firstName = splitted[1];
                middleName = splitted[2];
            }
            else if (splitted.Length == 2)
            {
                lastName = splitted[0];
                firstName = splitted[1];

            }
            else
            {

            }
            return new string[] { firstName, middleName, lastName };


        }

        public static string[] SplitInternationalPassportGivenName(string text)
        {
            string firstName = ""; string middleName = "";
            var keys = new string[] { " ", "," };
            var splitted = text.Split(keys, StringSplitOptions.RemoveEmptyEntries);
            if (splitted.Length == 2)
            {

                firstName = splitted[0];
                middleName = splitted[1];
            }
            else if (splitted.Length == 1)
            {

                firstName = splitted[0];

            }
            else
            {

            }
            return new string[] { firstName, middleName };
        }
    }
}
