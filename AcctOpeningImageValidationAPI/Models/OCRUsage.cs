using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcctOpeningImageValidationAPI.Models
{
    public class OCRUsage : BaseEntity
    {
        public int Count { get; set; }

        public string EmailAddress { get; set; }
        public List<ImagesScanned> ImageScanned { get; set; }
        public FacialValidation facialValidation { get; set; }
        public ScannedIDCardDetails scannedIDCardDetails { get; set; }
        public SimilarFace SimilarFaces { get; set; }
        public static OCRUsage Empty => new OCRUsage();
    }

    public class ImagesScanned : BaseEntity
    {
        public string ImageURL { get; set; }
        [ForeignKey("OcrUsageId")]

        public OCRUsage oCRUsage { get; set; }

        public int OcrUsageId { get; set; }
    }
}
