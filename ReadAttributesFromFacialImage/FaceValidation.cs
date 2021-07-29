using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.Extensions.Configuration;

namespace ReadAttributesFromFacialImage
{

    public interface IFaceValidation
    {
        public Task<(bool IsSuccess, IList<DetectedFace> faces)> PerformFaceValidationAsync(string imageUrl);

        public Task<(bool IsSuccess, IList<SimilarFace> faces)> FindSimilarFaces(List<string> targetImageURLs, string sourceImageURLs);

        public Task<(bool IsSuccess, VerifyResult faces)> VerifySimilarFaces(List<string> targetImageURLs, string sourceImageURLs);
    }


    public class FaceValidation : IFaceValidation
    {
        // Add your Face subscription key to your environment variables.
        private static string subscriptionKey = "";
        // Add your Face endpoint to your environment variables.
        static string fURL = "";
        private string faceEndpoint = "";

        private readonly IFaceClient faceClient = null;
        private readonly IConfiguration configuration;
        //const string RECOGNITION_MODEL3 = RecognitionModel.Recognition02;
        const string RECOGNITION_MODEL3 = "recognition_04";

        // The list of detected faces.
        private IList<DetectedFace> faceList;

        // The list of descriptions for the detected faces.
        private string[] faceDescriptions;
        // The resize factor for the displayed image.
        private double resizeFactor;
        public FaceValidation()
        {
            subscriptionKey = ReadTextFromImageConsole.Program.config.GetSection("AppSettings").GetSection("subscriptionKey").Value;
            fURL = ReadTextFromImageConsole.Program.config.GetSection("AppSettings").GetSection("FaceBaseURL").Value;
            faceEndpoint = fURL;
            faceClient = new FaceClient(
            new ApiKeyServiceClientCredentials(subscriptionKey),
            new System.Net.Http.DelegatingHandler[] { });

        }

        public FaceValidation(IConfiguration configuration)
        {
            this.configuration = configuration;
            subscriptionKey = configuration.GetSection("AppSettings").GetSection("subscriptionKey").Value;
            fURL = configuration.GetSection("AppSettings").GetSection("FaceBaseURL").Value;
            faceEndpoint = fURL;
            faceClient = new FaceClient(
            new ApiKeyServiceClientCredentials(subscriptionKey),
            new System.Net.Http.DelegatingHandler[] { });

        }

        public async Task<(bool IsSuccess, IList<DetectedFace> faces)> PerformFaceValidationAsync(string imageUrl)
        {

            try
            {

                IList<FaceAttributeType> faceAttributes =
               new FaceAttributeType[]
               {
                    FaceAttributeType.Gender, FaceAttributeType.Age,
                    FaceAttributeType.Smile, FaceAttributeType.Emotion,
                    FaceAttributeType.Glasses, FaceAttributeType.Hair
               };


                faceClient.Endpoint = faceEndpoint;
                //imageUrl = "https://sbnk2storage.blob.core.windows.net/k2container/OneBank/14864679/0074721460/Passport.jpg";
                IList<DetectedFace> faces = await faceClient.Face.DetectWithUrlAsync(imageUrl, true, true, faceAttributes, recognitionModel : "recognition_04");
                if (faces != null && faces.Count > 0)
                {
                    Console.WriteLine($"performing facial validation result is {Newtonsoft.Json.JsonConvert.SerializeObject(faces)}");
                    return (true, faces);
                }
                else
                {
                    return (false, null);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return (false, null);
            }
        }

        /*
 * FIND SIMILAR
 * This example will take an image and find a similar one to it in another image.
 */
        public async Task<(bool IsSuccess, IList<SimilarFace> faces)> FindSimilarFaces(List<string> targetImageURLs, string sourceImageURLs)
        {
            try
            {
                Console.WriteLine("========FIND SIMILAR========");
                Console.WriteLine();
                faceClient.Endpoint = faceEndpoint;

                IList<Guid?> targetFaceIds = new List<Guid?>();
                foreach (var targetImageFileName in targetImageURLs)
                {
                    // Detect faces from target image url.
                    var faces = await DetectFaceRecognize(faceClient, $"{targetImageFileName}", RECOGNITION_MODEL3);
                    // Add detected faceId to list of GUIDs.
                    targetFaceIds.Add(faces[0].FaceId.Value);
                }

                // Detect faces from source image url.
                IList<DetectedFace> detectedFaces = await DetectFaceRecognize(faceClient, $"{sourceImageURLs}", RECOGNITION_MODEL3);
                Console.WriteLine();

                //(bool isSuccess, IList<DetectedFace> detectedFaces) = await PerformFaceValidationAsync(sourceImageURLs);
                //Console.WriteLine();
                //if (!isSuccess)
                //{
                //    return (false, null);

                //}
                Console.WriteLine("detected source face is " + Newtonsoft.Json.JsonConvert.SerializeObject(detectedFaces));

                // Find a similar face(s) in the list of IDs. Comapring only the first in list for testing purposes.
                // IList<SimilarFace> similarResults = await faceClient.Face.FindSimilarAsync(detectedFaces[0].FaceId.Value, null, null, targetFaceIds, mode: FindSimilarMatchMode.MatchFace);
                var sourceID = detectedFaces[0].FaceId.Value;
                Console.WriteLine("sleep findsimilar faces method for 10s");
                Console.WriteLine();

                System.Threading.Thread.Sleep(10000);

                IList<SimilarFace> similarResults = await faceClient.Face.FindSimilarAsync(sourceID, null, null, targetFaceIds, mode: FindSimilarMatchMode.MatchFace);

                Console.WriteLine(" similar result is  " + Newtonsoft.Json.JsonConvert.SerializeObject(similarResults));
                similarResults.FirstOrDefault().Validate();




                return (true, similarResults);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return (false, null);

            }
        }

        /*
* FIND SIMILAR
* This example will take an image and find a similar one to it in another image.
*/
        public async Task<(bool IsSuccess, VerifyResult faces)> VerifySimilarFaces(List<string> targetImageURLs, string sourceImageURLs)
        {
            try
            {
                Console.WriteLine("========VERIFY SIMILAR========");
                Console.WriteLine();
                faceClient.Endpoint = faceEndpoint;

                IList<Guid?> targetFaceIds = new List<Guid?>();
                //foreach (var targetImageFileName in targetImageURLs)
                //{
                // Detect faces from target image url.
                var targetFaces = await DetectFaceRecognize(faceClient, $"{targetImageURLs.FirstOrDefault()}", RECOGNITION_MODEL3);

                Console.WriteLine("target detected image is " + Newtonsoft.Json.JsonConvert.SerializeObject(targetFaces));

                // Add detected faceId to list of GUIDs.
                //    targetFaceIds.Add(faces[0].FaceId.Value);
                //}

                // Detect faces from source image url.
                IList<DetectedFace> sourceDetectedFaces = await DetectFaceRecognize(faceClient, $"{sourceImageURLs}", RECOGNITION_MODEL3);
                //(bool isSuccess, IList<DetectedFace> sourceDetectedFaces) = await PerformFaceValidationAsync(sourceImageURLs);
                //Console.WriteLine();
                //if (!isSuccess)
                //{
                //    return (false, null);

                //}

                Console.WriteLine("source detected image is " + Newtonsoft.Json.JsonConvert.SerializeObject(sourceDetectedFaces));


                // Find a similar face(s) in the list of IDs. Comapring only the first in list for testing purposes.
                // IList<SimilarFace> similarResults = await faceClient.Face.FindSimilarAsync(detectedFaces[0].FaceId.Value, null, null, targetFaceIds, mode: FindSimilarMatchMode.MatchFace);
                System.Threading.Thread.Sleep(10000);
                VerifyResult verifyResults = await faceClient.Face.VerifyFaceToFaceAsync(sourceDetectedFaces[0].FaceId.Value, targetFaces[0].FaceId.Value);
                Console.WriteLine(" verieied result is " + Newtonsoft.Json.JsonConvert.SerializeObject(verifyResults));


                return (true, verifyResults);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return (false, null);

            }
        }

        /*
* FIND SIMILAR
* This example will take an image and find a similar one to it in another image.
*/
        public async Task<(bool IsSuccess, IList<SimilarFace> faces)> FindSimilarFaces(System.IO.Stream targetImageURLs, System.IO.Stream sourceImageURLs)
        {
            try
            {
                Console.WriteLine("========FIND SIMILAR========");
                Console.WriteLine();
                faceClient.Endpoint = faceEndpoint;

                IList<Guid?> targetFaceIds = new List<Guid?>();

                // Detect faces from target image url.
                var faces = await DetectFaceRecognize(faceClient, targetImageURLs, RECOGNITION_MODEL3);
                // Add detected faceId to list of GUIDs.
                targetFaceIds.Add(faces[0].FaceId.Value);


                // Detect faces from source image url.
                IList<DetectedFace> detectedFaces = await DetectFaceRecognize(faceClient, $"{sourceImageURLs}", RECOGNITION_MODEL3);
                Console.WriteLine();

                // Find a similar face(s) in the list of IDs. Comapring only the first in list for testing purposes.
                IList<SimilarFace> similarResults = await faceClient.Face.FindSimilarAsync(detectedFaces[0].FaceId.Value, null, null, targetFaceIds, mode: FindSimilarMatchMode.MatchFace);

                return (true, similarResults);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return (false, null);

            }
        }








        private async Task<List<DetectedFace>> DetectFaceRecognize(IFaceClient faceClient, string url, string recognition_model)
        {
            // Detect faces from image URL. Since only recognizing, use the recognition model 1.
            // We use detection model 2 because we are not retrieving attributes.
            IList<DetectedFace> detectedFaces = await faceClient.Face.DetectWithUrlAsync(url, recognitionModel: recognition_model, detectionModel: DetectionModel.Detection02);
            Console.WriteLine($"{detectedFaces.Count} face(s) detected from image");
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(detectedFaces));
            return detectedFaces.ToList();
        }


        private async Task<List<DetectedFace>> DetectFaceRecognize(IFaceClient faceClient, System.IO.Stream url, string recognition_model)
        {
            // Detect faces from image URL. Since only recognizing, use the recognition model 1.
            // We use detection model 2 because we are not retrieving attributes.
            IList<DetectedFace> detectedFaces = await faceClient.Face.DetectWithStreamAsync(url, recognitionModel: recognition_model, detectionModel: DetectionModel.Detection02);
            Console.WriteLine($"{detectedFaces.Count} face(s) detected from image");
            return detectedFaces.ToList();
        }
    }
}
