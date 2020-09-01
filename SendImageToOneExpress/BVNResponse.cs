using System;
using Newtonsoft.Json;

namespace SendImageToOneExpress
{
    public class BVNResponse
    {
        [JsonProperty("responseCode")]
        public string ResponseCode { get; set; }

        public string ResponseDesc { get; set; }

        [JsonProperty("bvn")]
        public string Bvn { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("middleName")]
        public string MiddleName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("dateOfBirth")]
        public string DateOfBirth { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("registrationDate")]
        public string RegistrationDate { get; set; }

        [JsonProperty("enrollmentBank")]
        public string EnrollmentBank { get; set; }

        [JsonProperty("enrollmentBranch")]
        public string EnrollmentBranch { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("phoneNumber2")]
        public string PhoneNumber2 { get; set; }

        [JsonProperty("levelOfAccount")]
        public string LevelOfAccount { get; set; }

        [JsonProperty("lgaOfOrigin")]
        public string LgaOfOrigin { get; set; }

        [JsonProperty("lgaOfResidence")]
        public string LgaOfResidence { get; set; }

        [JsonProperty("maritalStatus")]
        public string MaritalStatus { get; set; }

        [JsonProperty("nin")]
        public string Nin { get; set; }

        [JsonProperty("nameOnCard")]
        public string NameOnCard { get; set; }

        [JsonProperty("nationality")]
        public string Nationality { get; set; }

        [JsonProperty("residentialAddress")]
        public string ResidentialAddress { get; set; }

        [JsonProperty("stateOfOrigin")]
        public string StateOfOrigin { get; set; }

        [JsonProperty("stateOfResidence")]
        public string StateOfResidence { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("watchListed")]
        public string WatchListed { get; set; }

        [JsonProperty("base64Image")]
        public string Base64Image { get; set; }
    }

    public class BvnRequest
    {
        public string bvn { get; set; }
        public DateTime DateOfBirth { get; set; }
    }


    public class CamuAzureImageRequest
    {
        public string appId { get; set; }
        public string folderName { get; set; }
        public string fileName { get; set; }
        public string base64String { get; set; }
    }

    public class CamuAzureResponse
    {
        public string url { get; set; }
        public string responseCode { get; set; }
        public string responseDescription { get; set; }
    }

    public class DataFields
    {
        public string CustomerID { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string MandateURL { get; set; }
        public string Source { get; set; }
    }

    public class OneExpressSubmitImage
    {
        public DataFields dataFields { get; set; }
    }

}
