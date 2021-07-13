using System;
namespace IdentificationValidationLib
{
    public class AppSettings
    {
        public int MaximumUsageForOCR { get; set; }
        public string ApiDocumentBaseUrl { get; set; }
        public string AzureContentFolderName { get; set; }
        public string subscriptionKey { get; set; }
        public string AzureFacialBaseUrl { get; set; }
        public string EncryptionKey { get; set; }
        public string EncryptionIV { get; set; }
        public string ContentServerDirectory { get; set; }
        public string LivenessRootFolder { get; set; }
        public string LivenessVideoFormat { get; set; }
        public VerifyMeConfig VerifyMeConfig { get; set; }
        public string SignalrEventName { get; set; }
        public string FaceListId { get; set; }
        public string FaceListName { get; set; }
    }

    public class VerifyMeConfig
    {
        public string BASE_URL { get; set; }
        public string AUTH_TOKEN { get; set; }
    }
}
