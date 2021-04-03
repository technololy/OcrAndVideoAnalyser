using System;
using System.ComponentModel.DataAnnotations;

namespace ImageValidationBlazorServer.Models
{

    public class ValidateInputModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Please enter a valid email")]
        public string Email { get; set; }


        public string Base64Encoded { get; set; }
        public string Base64EncodedTarget { get; set; }

        [Required(ErrorMessage = "Please enter a valid username")]

        public string UserName { get; set; }
    }
    public class ScannedIDCardDetails
    {
        public int Id { get; set; }
        public string DocumentType { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string IDType { get; set; }
        public string IDNumber { get; set; }
        public string IssueDate { get; set; }
        public string DateOfBirth { get; set; }
        public string ExpiryDate { get; set; }
        public string FormerIDNumber { get; set; }
        public string IDClass { get; set; }
        public string BVN { get; set; }
        public string Email { get; set; }
        public string BloodGroup { get; set; }
        public string Height { get; set; }
        public string IssuingAuthority { get; set; }
        public string Address { get; set; }
        public string Occupation { get; set; }
        public string NextOfKin { get; set; }
        public string FirstIssueState { get; set; }
        public string Delim { get; set; }
        public string Gender { get; set; }
        public DateTime? DateInserted { get; set; }
    }
}
