using System;
using System.Collections.Generic;

namespace ModelLib
{
    public class Passport
    {
        public Passport()
        {
        }



        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        public class SelectionMark
        {
            public List<double> boundingBox { get; set; }
            public double confidence { get; set; }
            public string state { get; set; }
        }

        public class ReadResult
        {
            public int page { get; set; }
            public double angle { get; set; }
            public double width { get; set; }
            public double height { get; set; }
            public string unit { get; set; }
            public List<SelectionMark> selectionMarks { get; set; }
        }

        public class PageResult
        {
            public int page { get; set; }
            public List<object> tables { get; set; }
        }

        public class DoB
        {
            public string type { get; set; }
            public string valueDate { get; set; }
            public string text { get; set; }
            public int page { get; set; }
            public List<double> boundingBox { get; set; }
            public double? confidence { get; set; }
        }

        public class DoE
        {
            public string type { get; set; }
            public string valueDate { get; set; }
            public string text { get; set; }
            public int page { get; set; }
            public List<double> boundingBox { get; set; }
            public double confidence { get; set; }
        }

        public class Surname
        {
            public string type { get; set; }
            public string valueString { get; set; }
            public string text { get; set; }
            public int page { get; set; }
            public List<double> boundingBox { get; set; }
            public double confidence { get; set; }
        }

        public class Nationality
        {
            public string type { get; set; }
            public string valueString { get; set; }
            public string text { get; set; }
            public int page { get; set; }
            public List<double> boundingBox { get; set; }
            public double confidence { get; set; }
        }

        public class PassportNumber
        {
            public string type { get; set; }
            public string valueString { get; set; }
            public string text { get; set; }
            public int page { get; set; }
            public List<double> boundingBox { get; set; }
            public double confidence { get; set; }
        }

        public class OtherNames
        {
            public string type { get; set; }
            public string valueString { get; set; }
            public string text { get; set; }
            public int page { get; set; }
            public List<double> boundingBox { get; set; }
            public double confidence { get; set; }
        }

        public class Type
        {
            public string type { get; set; }
            public string valueString { get; set; }
            public string text { get; set; }
            public int page { get; set; }
            public List<double> boundingBox { get; set; }
            public double confidence { get; set; }
        }

        public class DoI
        {
            public string type { get; set; }
            public string valueDate { get; set; }
            public string text { get; set; }
            public int page { get; set; }
            public List<double> boundingBox { get; set; }
            public double confidence { get; set; }
        }

        public class Authority
        {
            public string type { get; set; }
            public string valueString { get; set; }
            public string text { get; set; }
            public int page { get; set; }
            public List<double> boundingBox { get; set; }
            public double confidence { get; set; }
        }

        public class Sex
        {
            public string type { get; set; }
            public string valueString { get; set; }
            public string text { get; set; }
            public int page { get; set; }
            public List<double> boundingBox { get; set; }
            public double confidence { get; set; }
        }

        public class CountryCode
        {
            public string type { get; set; }
            public string valueString { get; set; }
            public string text { get; set; }
            public int page { get; set; }
            public List<double> boundingBox { get; set; }
            public double confidence { get; set; }
        }

        public class PlaceOfBirth
        {
            public string type { get; set; }
            public string valueString { get; set; }
            public string text { get; set; }
            public int page { get; set; }
            public List<double> boundingBox { get; set; }
            public double confidence { get; set; }
        }

        public class Fields
        {
            public DoB DoB { get; set; }
            public DoE DoE { get; set; }
            public Surname Surname { get; set; }
            public Nationality Nationality { get; set; }
            public PassportNumber PassportNumber { get; set; }
            public OtherNames OtherNames { get; set; }
            public Type Type { get; set; }
            public DoI DoI { get; set; }
            public Authority Authority { get; set; }
            public Sex Sex { get; set; }
            public CountryCode CountryCode { get; set; }
            public PlaceOfBirth PlaceOfBirth { get; set; }
        }

        public class DocumentResult
        {
            public string docType { get; set; }
            public string modelId { get; set; }
            public List<int> pageRange { get; set; }
            public Fields fields { get; set; }
            public double docTypeConfidence { get; set; }
        }

        public class AnalyzeResult
        {
            public string version { get; set; }
            public List<ReadResult> readResults { get; set; }
            public List<PageResult> pageResults { get; set; }
            public List<DocumentResult> documentResults { get; set; }
            public List<object> errors { get; set; }
        }

        public class Root
        {
            public string status { get; set; }
            public DateTime createdDateTime { get; set; }
            public DateTime lastUpdatedDateTime { get; set; }
            public AnalyzeResult analyzeResult { get; set; }
        }


    }
}




