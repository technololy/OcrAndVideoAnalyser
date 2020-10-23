using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace ReadTextFromImageConsole
{




    //private readonly IServiceProvider serviceProvider;

    //public Program(IApplicationEnvironment env, IServiceManifest serviceManifest)
    //{
    //    var services = new ServiceCollection();
    //    ConfigureServices(services);
    //    serviceProvider = services.BuildServiceProvider();
    //}

    //private void ConfigureServices(IServiceCollection services)
    //{
    //}



    public class Program
    {
        static HttpClient client = new HttpClient();
        static HttpClient apiClient = new HttpClient();
        public static Repository.CamuDataTableRepository repository = new Repository.CamuDataTableRepository();
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // Add your Computer Vision subscription key and endpoint to your environment variables.
        //static string subscriptionKey = Environment.GetEnvironmentVariable("COMPUTER_VISION_SUBSCRIPTION_KEY");
        static string subscriptionKey = "e8ef40efa4704769860e661c210a0fc5";

        // An endpoint should have a format like "https://westus.api.cognitive.microsoft.com"
        //static string endpoint = Environment.GetEnvironmentVariable("COMPUTER_VISION_ENDPOINT");
        static string endpoint = "";

        // the Batch Read method endpoint
        static string uriBase = "";

        // Add a local image with text here (png or jpg is OK)
        //static string imageFilePath = @"damilola_oyebanji_international_passport.jpg";
        //static string imageFilePath = @"passport_renewal_Page_1.pdf";
        //static string imageFilePath = @"Identification.jpg";
        static string imageFilePath = @"IMG_20200719_021151.jpg";
        public static IConfigurationRoot config;

        static void Main(string[] args)
        {
            // Load configuration
            var builder = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            config = builder.Build();
            endpoint = config.GetSection("AppSettings").GetSection("AzureFacialBaseUrl").Value;
            uriBase = endpoint + config.GetSection("AppSettings").GetSection("AzureFacialFaceEndPoint").Value;
            //var token = config.GetSection("AppSettings").GetSection("AppruveToken").Value;

            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            log.Info("Start job");
            var images = GetImages();
            if (images == null || !images.Any())
            {
                return;
            }
            foreach (var item in images)
            {
                // Call the REST API method.
                Console.WriteLine("\nExtracting text...\n");
                //ValidateDoc(null).Wait();
                ReadText(item.UrlmeansOfId, item).Wait();

                Console.WriteLine("\nPress Enter to exit...");

            }

        }

        private static List<Models.Camudatafield> GetImages()
        {
            var context = new AppDbContext();
            var imagesList = context.Camudatafield.Where(s => s.IsIdentificationImageChecked == null).Take(100).ToList();
            return imagesList;
        }


        /// Gets the text from the specified image file by using
        /// the Computer Vision REST API.
        /// </summary>
        /// <param name="imageFilePath">The image file with text.</param>
        static async Task<(bool isSuccess, string message)> ReadText(string imageFilePath, Models.Camudatafield camudatafield = null)
        {
            try
            {
                //HttpClient client = new HttpClient();
                bool isSuccess = false;
                string responseMessage = "";
                // Request headers.
                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", subscriptionKey);

                string url = uriBase;

                HttpResponseMessage response;

                // Two REST API methods are required to extract text.
                // One method to submit the image for processing, the other method
                // to retrieve the text found in the image.

                // operationLocation stores the URI of the second REST API method,
                // returned by the first REST API method.
                string operationLocation;

                // Reads the contents of the specified local image
                // into a byte array.
                byte[] byteData = GetImageURLAsByteArray(imageFilePath);

                if (byteData?.Length <= 0)
                {
                    UpdateResponseOnTable(isSuccess = false, responseMessage = "Invalid Image. Byte length is zero", camudatafield);
                    return (isSuccess, responseMessage);
                }

                // Adds the byte array as an octet stream to the request body.
                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    // This example uses the "application/octet-stream" content type.
                    // The other content types you can use are "application/json"
                    // and "multipart/form-data".
                    content.Headers.ContentType =
                        new MediaTypeHeaderValue("application/octet-stream");

                    // The first REST API method, Batch Read, starts
                    // the async process to analyze the written text in the image.
                    response = await client.PostAsync(url, content);
                }

                // The response header for the Batch Read method contains the URI
                // of the second method, Read Operation Result, which
                // returns the results of the process in the response body.
                // The Batch Read operation does not return anything in the response body.
                if (response.IsSuccessStatusCode)
                    operationLocation =
                        response.Headers.GetValues("Operation-Location").FirstOrDefault();
                else
                {
                    // Display the JSON error data.
                    string errorString = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("\n\nResponse:\n{0}\n",
                        JToken.Parse(errorString).ToString());
                    return (isSuccess = false, errorString);
                }

                // If the first REST API method completes successfully, the second 
                // REST API method retrieves the text written in the image.
                //
                // Note: The response may not be immediately available. Text
                // recognition is an asynchronous operation that can take a variable
                // amount of time depending on the length of the text.
                // You may need to wait or retry this operation.
                //
                // This example checks once per second for ten seconds.
                string contentString;
                int i = 0;
                do
                {
                    System.Threading.Thread.Sleep(1000);
                    response = await client.GetAsync(operationLocation);
                    contentString = await response.Content.ReadAsStringAsync();
                    ++i;
                }
                while (i < 60 && contentString.IndexOf("\"status\":\"succeeded\"") == -1);

                if (i == 60 && contentString.IndexOf("\"status\":\"succeeded\"") == -1)
                {
                    Console.WriteLine("\nTimeout error.\n");
                    return (false, responseMessage = "Timeout error");
                }

                // Display the JSON response.
                Console.WriteLine("\nResponse:\n\n{0}\n",
                    JToken.Parse(contentString).ToString());
                //extract the whole text into a list of strings
                //from this list, know if its passport, drivers licsnse or voters card
                var returnedExtractedTextFromImages = ExtractWordIntoLists(contentString);


                if (returnedExtractedTextFromImages == null || !returnedExtractedTextFromImages.Any())
                {
                    //can not extract text from image. exit
                    UpdateResponseOnTable(true, responseMessage = "can not see/extract text from image", camudatafield);
                    return (isSuccess = false, responseMessage);

                }
                //go to extract the biodata from the images 
                Validation v = new Validation();
                v.ExtractDocKeyDetails
                    (returnedExtractedTextFromImages, camudatafield);
                if (!v.IsExtractionOk)
                {
                    //something made impossible extracting the preferred biodata from the list of text sent for analysis
                    log.Info($"error. something made impossible extracting the preferred biodata from the list of text sent for analysis. {v.exception} for customer {camudatafield.EmailAddress} and account {camudatafield.AccountNumber} and row id {camudatafield.Id} ");
                    UpdateResponseOnTable(true, $"{v.exception}", camudatafield, contentString);
                    return (v.IsExtractionOk, v.exception);
                }
                else
                {
                    UpdateRecordsWithExtractedBioData(v, camudatafield, contentString);
                }

                //send to appruve API to validate the details of the identification document

                var validationResult = ValidateDoc(v, camudatafield);
                return (validationResult, "successful");

            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.InnerException);
                return (false, e.InnerException.ToString());

            }
        }

        private static void UpdateRecordsWithExtractedBioData(Validation v, Models.Camudatafield camudatafield, string extractedText = "")
        {
            camudatafield.Idno = v.idNumber;
            camudatafield.Idtype = Enum.GetName(typeof(Validation.DocumentType), v.docType);
            camudatafield.Issuedate = v.issueDate;
            camudatafield.Iexpirydate = v.ExpiryDate;
            camudatafield.IsIdentificationImageChecked = true;
            camudatafield.DateIdentificationImageIsChecked = DateTime.Now;
            camudatafield.IdentificationImageExtractedTextFromImage = extractedText;

            repository.UpdateCamuData(camudatafield);


        }

        private static void UpdateResponseOnTable(bool IsChecked, string description, Models.Camudatafield camudatafield, string extractedText = "")
        {
            camudatafield.IsIdentificationImageChecked = IsChecked;
            camudatafield.IdentificationImageCheckResponse = description;
            camudatafield.DateIdentificationImageIsChecked = DateTime.Now;
            camudatafield.IdentificationImageExtractedTextFromImage = extractedText;
            repository.UpdateCamuData(camudatafield);

        }

        private static bool ValidateDoc(Validation v, Models.Camudatafield camudatafield)
        {
            string url = GetURLByDocType(v.docType);
            var model = new { id = v.idNumber, first_name = v.firstName, last_name = v.lastName, date_of_birth = v.dateOfBirth };
#if DEBUG
            //if (v.docType == Validation.DocumentType.DriversLicense)
            //{
            //    model = new { id = "ABC00578AA2", first_name = "Henry", last_name = "Nwandicne", date_of_birth = "1976-04-15" };

            //}
            //else if (v.docType == Validation.DocumentType.VotersCard)
            //{
            //    model = new { id = "90F5B0407E2960502637", first_name = "Nwabia", last_name = "Chidozie", date_of_birth = "1998-01-10" };

            //}
            //else if (v.docType == Validation.DocumentType.InternationalPassport)
            //{
            //    model = new { id = "A50013320", first_name = "Sunday", last_name = "Obafemi", date_of_birth = "1975-04-25" };

            //}
            //else if (v.docType == Validation.DocumentType.nationalId)
            //{
            //    model = new { id = "AKW06968AA2", first_name = "Michael", last_name = "Olugbenga", date_of_birth = "1982-05-20" };

            //}
#endif


            API aPI = new API();
            var result = aPI.Post<AppruveResponseModelSuccess, AppruveResponseModelFailure>(model, url).Result;
            if (result.isSuccess)
            {
                //got success from appruve
                UpdateCamuDataOfAppruveResponse(result, camudatafield);
                Console.WriteLine(result.returnedStringContent);
            }
            else
            {
                //failure from appruve
                Console.WriteLine(result.failedObj.message);
                UpdateCamuDataOfAppruveResponse(result, camudatafield);

                Console.WriteLine(result.returnedStringContent);

            }
            return result.isSuccess;

        }

        private static void UpdateCamuDataOfAppruveResponse((bool isSuccess, string returnedStringContent, AppruveResponseModelSuccess SuccessObj, AppruveResponseModelFailure failedObj) result, Models.Camudatafield camudatafield)
        {
            if (result.isSuccess)
            {
                camudatafield.IdentificationImageCheckResponse = "successful";
                camudatafield.IdentificationImageCheckResponseJson = result.returnedStringContent;
                repository.UpdateCamuData(camudatafield);
            }
            else
            {
                camudatafield.IdentificationImageCheckResponse = "failed:" + result.failedObj.message;
                camudatafield.IdentificationImageCheckResponseJson = result.returnedStringContent;
                repository.UpdateCamuData(camudatafield);
            }
        }

        private static string GetURLByDocType(Validation.DocumentType docType)
        {
            string categoryEndPoint = "";
            switch (docType)
            {
                case Validation.DocumentType.DriversLicense:
                    categoryEndPoint = AppruveCurl.driver_license;
                    break;
                case Validation.DocumentType.InternationalPassport:
                    categoryEndPoint = AppruveCurl.driver_license;
                    break;
                case Validation.DocumentType.VotersCard:
                    categoryEndPoint = AppruveCurl.driver_license;
                    break;
                case Validation.DocumentType.nationalId:
                    categoryEndPoint = AppruveCurl.national_id;
                    break;
                default:
                    break;
            }
            string url = $"{config.GetSection("AppSettings").GetSection("AppruveBaseURL").Value}/{categoryEndPoint}";
            return url;
        }

        public class AppruveCurl
        {
            public const string passport = "passport";
            public const string voter = "voter";
            public const string driver_license = "driver_license";
            public const string national_id = "national_id";
        }

        private async static Task ValidateDoc(List<string> returnedExtractedTextFromImages)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response;
                string accessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJqdGkiOiI5YTZkZDc3NS0zODc4LTQ1M2UtYmM4Ny1lZjU4MTA3YjRhYjEiLCJhdWQiOiI3ZmRmNTVmNC1hNTRhLTQ1ZWItOWRjZS00ZThlM2IwMWRmNmEiLCJzdWIiOiJjZjRiYWMxNi0zOTA3LTQ4MDAtYTY5Mi04YTQzZDhjMzkwYjAiLCJuYmYiOjAsInNjb3BlcyI6WyJ2ZXJpZmljYXRpb25fdmlldyIsInZlcmlmaWNhdGlvbl9saXN0IiwidmVyaWZpY2F0aW9uX2RvY3VtZW50IiwidmVyaWZpY2F0aW9uX2lkZW50aXR5Il0sImV4cCI6MzE3Mjk1NTMyMCwiaWF0IjoxNTk1MTE4NTIwfQ.5qGsVsB35RJPl3UsgCvdfWOlBqRP28YEMjrO43Up3tM";

                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer ", accessToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);



                string baseUrl = "https://api.appruve.co";
                string endPoint = baseUrl + "/v1/verifications/ng/passport";
                var model = new { id = "A50013320", first_name = "Sunday", last_name = "Obafemi", date_of_birth = "1975-04-25" };
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                response = client.PostAsync(endPoint, content).Result;
                var result = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {

                }
                else
                {

                }

                await Task.FromResult("");

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

        }

        private static List<string> ExtractWordIntoLists(string contentString)
        {
            try
            {
                //extracting string objects into a list
                log.Info("extracting string objects into a list");
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ExtracteTextModel>(contentString);
                var extract = result.analyzeResult.readResults.FirstOrDefault().lines.Select(x => x.text).ToList();
                return extract;
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// Returns the contents of the specified file as a byte array.
        /// </summary>
        /// <param name="imageFilePath">The image file to read.</param>
        /// <returns>The byte array of the image data.</returns>
        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            // Open a read-only file stream for the specified file.
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                // Read the file's contents into a byte array.
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }

        public static byte[] GetImageURLAsByteArray(string imageFilePath)
        {
            var response = apiClient.GetAsync(imageFilePath).Result;
            var bytes = response.Content.ReadAsByteArrayAsync().Result;
            return bytes;
        }
    }
}
