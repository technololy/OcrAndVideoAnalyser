using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AcctOpeningImageValidationAPI.Repository.Abstraction
{
    public interface IFaceRepository
    {
        Tuple<bool, string> ExtractFrameFromVideo(string directory, string fileName);
        //void ExtractFrameFromVideo (string directory, string fileName, Action<Tuple<bool, string>> callback);
        Task<Tuple<bool, bool, bool, bool, string, string>> RunHeadGestureOnImageFrame(string filePath, string userIdentification);
        Task RunHeadGestureOnImageFrame(string filePath, string userIdentification, Action<Tuple<bool, bool, bool, bool, string, string>> action);
        bool SaveImageToDisk(string base64String, string path);
        Tuple<bool, bool> AnalyzeFaceLandMark(FaceLandmarks faceLandMark);
        Task<EyeBlinkResult> RunEyeBlinkAlgorithm(string filePath, string userIdentification);
        Task RunEyeBlinkAlgorithm(string filePath, string userIdentification, Action<EyeBlinkResult> action);
        Task CreateFaceList(string faceId);
        Task<PersistedFace> AddFaceToFaceList(Stream stream, string name);
        Task<PersistedFace> VerifyFaceToFaceList(Guid persistedFaceId);
    }
}
