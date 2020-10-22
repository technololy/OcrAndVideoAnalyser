using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.Extensions.Configuration;

namespace ReadAttributesFromFacialImage
{

    public interface IFaceValidation
    {
        public Task<(bool IsSuccess, IList<DetectedFace> faces)> PerformFaceValidationAsync(string imageUrl);
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
                IList<DetectedFace> faces = await faceClient.Face.DetectWithUrlAsync(imageUrl, true, true, faceAttributes);
                if (faces != null && faces.Count > 0)
                {
                    return (true, faces);
                }
                else
                {
                    return (false, null);
                }
            }
            catch (Exception ex)
            {
                return (false, null);
            }
        }


    }
}
