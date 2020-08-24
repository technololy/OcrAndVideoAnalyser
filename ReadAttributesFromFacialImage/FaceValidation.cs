using System;
using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace ReadAttributesFromFacialImage
{
    public class FaceValidation
    {
        // Add your Face subscription key to your environment variables.
        private const string subscriptionKey = "e8ef40efa4704769860e661c210a0fc5";
        // Add your Face endpoint to your environment variables.
        private const string faceEndpoint = "https://eastus.api.cognitive.microsoft.com/";

        private readonly IFaceClient faceClient = new FaceClient(
            new ApiKeyServiceClientCredentials(subscriptionKey),
            new System.Net.Http.DelegatingHandler[] { });

        // The list of detected faces.
        private IList<DetectedFace> faceList;

        // The list of descriptions for the detected faces.
        private string[] faceDescriptions;
        // The resize factor for the displayed image.
        private double resizeFactor;
        public FaceValidation()
        {
        }

        public async System.Threading.Tasks.Task<(bool IsSuccess, IList<DetectedFace> faces)> PerformFaceValidationAsync(string imageUrl)
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
                imageUrl = "https://sbnk2storage.blob.core.windows.net/k2container/OneBank/14864679/0074721460/Passport.jpg";
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
