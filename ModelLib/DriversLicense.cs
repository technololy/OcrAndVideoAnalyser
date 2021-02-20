
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
using System;
using System.Collections.Generic;
namespace QuickType.DriversLicense
{


    public class ReadResult
    {
        public int page { get; set; }
        public int angle { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string unit { get; set; }
    }

    public class PageResult
    {
        public int page { get; set; }
        public List<object> tables { get; set; }
    }

    public class DateOfIssue
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

    public class FacialMarks
    {
        public string type { get; set; }
        public string valueString { get; set; }
        public string text { get; set; }
        public int page { get; set; }
        public List<double> boundingBox { get; set; }
        public double confidence { get; set; }
    }

    public class ClassOfLicense
    {
        public string type { get; set; }
        public string valueString { get; set; }
        public string text { get; set; }
        public int page { get; set; }
        public List<double> boundingBox { get; set; }
        public double confidence { get; set; }
    }

    public class DateOfBirth
    {
        public string type { get; set; }
        public string valueString { get; set; }
        public string text { get; set; }
        public int page { get; set; }
        public List<double> boundingBox { get; set; }
        public double confidence { get; set; }
    }

    public class FirstIssueState
    {
        public string type { get; set; }
        public string valueString { get; set; }
        public string text { get; set; }
        public int page { get; set; }
        public List<double> boundingBox { get; set; }
        public double confidence { get; set; }
    }

    public class BloofGroup
    {
        public string type { get; set; }
        public string valueString { get; set; }
        public string text { get; set; }
        public int page { get; set; }
        public List<double> boundingBox { get; set; }
        public double confidence { get; set; }
    }

    public class NextOfKin
    {
        public string type { get; set; }
        public string valueString { get; set; }
        public string text { get; set; }
        public int page { get; set; }
        public List<double> boundingBox { get; set; }
        public double confidence { get; set; }
    }

    public class Height
    {
        public string type { get; set; }
        public string valueString { get; set; }
        public string text { get; set; }
        public int page { get; set; }
        public List<double> boundingBox { get; set; }
        public double confidence { get; set; }
    }

    public class Rep
    {
        public string type { get; set; }
        public string valueString { get; set; }
        public string text { get; set; }
        public int page { get; set; }
        public List<double> boundingBox { get; set; }
        public double confidence { get; set; }
    }

    public class FullName
    {
        public string type { get; set; }
        public string valueString { get; set; }
        public string text { get; set; }
        public int page { get; set; }
        public List<double> boundingBox { get; set; }
        public double confidence { get; set; }
    }

    public class CardType
    {
        public string type { get; set; }
        public string valueString { get; set; }
        public string text { get; set; }
        public int page { get; set; }
        public List<double> boundingBox { get; set; }
        public double confidence { get; set; }
    }

    public class StateLocationOnCard
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

    public class DateOfExpiry
    {
        public string type { get; set; }
        public string valueString { get; set; }
        public string text { get; set; }
        public int page { get; set; }
        public List<double> boundingBox { get; set; }
        public double confidence { get; set; }
    }

    public class Address
    {
        public string type { get; set; }
        public string valueString { get; set; }
        public string text { get; set; }
        public int page { get; set; }
        public List<double> boundingBox { get; set; }
        public double confidence { get; set; }
    }

    public class DriversLicenseNo
    {
        public string type { get; set; }
        public string valueString { get; set; }
        public string text { get; set; }
        public int page { get; set; }
        public List<double> boundingBox { get; set; }
        public double confidence { get; set; }
    }

    public class Ren
    {
        public string type { get; set; }
        public string valueString { get; set; }
        public string text { get; set; }
        public int page { get; set; }
        public List<double> boundingBox { get; set; }
        public double confidence { get; set; }
    }

    public class Glasses
    {
        public string type { get; set; }
        public string valueString { get; set; }
        public string text { get; set; }
        public int page { get; set; }
        public List<double> boundingBox { get; set; }
        public double confidence { get; set; }
    }

    public class DateOfFirstIssue
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
        public DateOfIssue DateOfIssue { get; set; }
        public Sex Sex { get; set; }
        public FacialMarks FacialMarks { get; set; }
        public ClassOfLicense ClassOfLicense { get; set; }
        public DateOfBirth DateOfBirth { get; set; }
        public FirstIssueState FirstIssueState { get; set; }
        public BloofGroup BloofGroup { get; set; }
        public NextOfKin NextOfKin { get; set; }
        public Height Height { get; set; }
        public Rep Rep { get; set; }
        public FullName FullName { get; set; }
        public CardType CardType { get; set; }
        public StateLocationOnCard StateLocationOnCard { get; set; }
        public Type Type { get; set; }
        public DateOfExpiry DateOfExpiry { get; set; }
        public Address Address { get; set; }
        public DriversLicenseNo DriversLicenseNo { get; set; }
        public Ren Ren { get; set; }
        public Glasses Glasses { get; set; }
        public DateOfFirstIssue DateOfFirstIssue { get; set; }
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

    public class DriversLicenseRoot
    {
        public string status { get; set; }
        public DateTime createdDateTime { get; set; }
        public DateTime lastUpdatedDateTime { get; set; }
        public AnalyzeResult analyzeResult { get; set; }
    }


}