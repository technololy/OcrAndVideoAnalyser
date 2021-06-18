using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AcctOpeningImageValidationAPI.Repository.Abstraction
{
    public interface IFaceRepository
    {
        Tuple<bool, string> ExtractFrameFromVideo(string directory, string fiileName);
        Task<Tuple<bool, bool, bool, bool, string, string>> RunHeadGestureOnImageFrame(string filePath, string userIdentification);
        bool SaveImageToDisk(string base64String, string path);
        Tuple<bool, bool> AnalyzeFaceLandMark(FaceLandmarks faceLandMark);
        Task<EyeBlinkResult> RunEyeBlinkAlgorithm(string filePath, string userIdentification);
    }
}
