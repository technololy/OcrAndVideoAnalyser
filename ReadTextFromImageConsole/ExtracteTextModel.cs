using System;
using System.Collections.Generic;

namespace ReadTextFromImageConsole
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class FacialValidationRequest
    {
        public string userName { get; set; }
        public string image { get; set; }
        public string imageType { get; set; }

    }
    public class ExtracteTextModel
    {
        public string status { get; set; }
        public DateTime createdDateTime { get; set; }
        public DateTime lastUpdatedDateTime { get; set; }
        public AnalyzeResult analyzeResult { get; set; }

    }

    public class Word
    {
        public List<int> boundingBox { get; set; }
        public string text { get; set; }
        public double confidence { get; set; }

    }

    public class Line
    {
        public List<int> boundingBox { get; set; }
        public string text { get; set; }
        public List<Word> words { get; set; }

    }

    public class ReadResult
    {
        public int page { get; set; }
        public double angle { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string unit { get; set; }
        public List<Line> lines { get; set; }

    }

    public class AnalyzeResult
    {
        public string version { get; set; }
        public List<ReadResult> readResults { get; set; }

    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class AppruveResponseModelSuccess
    {
        public string id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string middle_name { get; set; }
        public string date_of_birth { get; set; }
        public string gender { get; set; }
        public string issue_date { get; set; }
        public string expiry_date { get; set; }
        public bool is_first_name_match { get; set; }
        public bool is_last_name_match { get; set; }
        public bool is_middle_name_match { get; set; }
        public bool is_date_of_birth_match { get; set; }
        public bool is_gender_match { get; set; }
        public bool is_expiry_date_match { get; set; }
        public bool is_issue_date_match { get; set; }

    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class AppruveResponseModelFailure
    {
        public string message { get; set; }
        public int code { get; set; }

    }

}
