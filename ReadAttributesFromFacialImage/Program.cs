using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using ReadTextFromImageConsole;
using ReadTextFromImageConsole.Models;

namespace ReadAttributesFromFacialImage
{
    class Program
    {
        static HttpClient client = new HttpClient();
        static HttpClient apiClient = new HttpClient();
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        static string url = "https://prod-00.westeurope.logic.azure.com:443/workflows/7a61090e0d2f4081b2ab5e523e55d008/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=UzmEkqqC9NcFuJlFcTr4a432cvSoytfydDqhhhYTiPU&returnFaceAttributes=emotion,fear,disgust";

        static void Main(string[] args)
        {
            var facialImages = GetImages();
            if (facialImages == null || !facialImages.Any())
            {
                return;
            }
            foreach (var item in facialImages)
            {
                // Call the REST API method.
                Console.WriteLine("\nExtracting text...\n");
                //ValidateDoc(null).Wait();
                if (string.IsNullOrEmpty(item.UrlphotoId))
                {
                    continue;
                }
                ReadFacialAttributes(item.UrlphotoId, item).Wait();

                Console.WriteLine("\nPress Enter to exit...");

            }



        }

        private static List<Camudatafield> GetImages()
        {
            var context = new AppDbContext();
            var imagesList = context.Camudatafield.Where(s => s.IsFacialImageChecked == null).Take(100).ToList();
            return imagesList;
        }



        /// Gets the text from the specified image file by using
        /// the Computer Vision REST API.
        /// </summary>
        /// <param name="imageFilePath">The image file with text.</param>
        static async Task ReadFacialAttributes(string imageFilePath, Camudatafield camudatafield = null)
        {
            try
            {
                //HttpClient client = new HttpClient();




                // ValidateFaceUsingPowerAutomate(imageFilePath, camudatafield);

                var resp = ValidateFaceUsingAzureSDK(imageFilePath).Result;
                if (resp.isSuccess)
                {
                    UpdateResponseOnTable(true, "successful", camudatafield, Newtonsoft.Json.JsonConvert.SerializeObject(resp.faces));
                }
                else
                {
                    UpdateResponseOnTable(false, "failed", camudatafield, "");

                }



            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
            }
        }

        private async static Task<(bool isSuccess, IList<DetectedFace> faces)> ValidateFaceUsingAzureSDK(string imageFilePath)
        {
            FaceValidation f = new FaceValidation();
            return f.PerformFaceValidationAsync(imageFilePath).Result;
        }

        private static void ValidateFaceUsingPowerAutomate(string imageFilePath, Camudatafield camudatafield)
        {
            // Two REST API methods are required to extract text.
            // One method to submit the image for processing, the other method
            // to retrieve the text found in the image.

            FacialValidationRequest facial = new FacialValidationRequest()
            {
                image = imageFilePath,
                imageType = "facial_doc",
                userName = $"{camudatafield.FirstName}_{camudatafield.LastName}"
            };
            API aPI = new API(false);
            var apiCall = aPI.PostAny<FacialImageResponse>(facial, url).Result;
            FacialImageResponse facialImageResponse;

            if (apiCall.isSuccess)
            {
                facialImageResponse = apiCall.SuccessObj;

            }
            else
            {
                // Display the JSON error data.


                log.Error(apiCall.returnedStringContent);
                return;
            }

            if (facialImageResponse == null || !facialImageResponse.MyArray.Any())
            {
                UpdateResponseOnTable(true, "failed. no image seen", camudatafield, "");
                return;
            }
            else
            {
                UpdateResponseOnTable(true, "successful", camudatafield, apiCall.returnedStringContent);
                return;
            }
        }

        private static void UpdateResponseOnTable(bool IsChecked, string description, Camudatafield camudatafield, string extractedText = "")
        {
            camudatafield.IsFacialImageChecked = IsChecked;
            camudatafield.FacialImageCheckResponse = description;
            camudatafield.DateFacialImageIsChecked = DateTime.Now;
            camudatafield.FacialImageExtractedTextFromImage = extractedText;
            ReadTextFromImageConsole.Program.repository.UpdateCamuData(camudatafield);

        }
    }
}
