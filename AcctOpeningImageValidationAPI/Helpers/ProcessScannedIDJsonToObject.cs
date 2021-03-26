using System;
using System.Linq;
using System.Text.Json;
using ModelLib;
using static IdentificationValidationLib.Validation;


namespace AcctOpeningImageValidationAPI.Helpers
{
    public class ProcessScannedIDJsonToObject
    {


        public ProcessScannedIDJsonToObject()
        {
        }


        internal static Models.ScannedIDCardDetails ProcessJsonToObject(Passport.Root documentRoot, string json)
        {
            Models.ScannedIDCardDetails scannedIDCardDetails = new Models.ScannedIDCardDetails();
            string firstName = ""; string middleName = ""; string lastName = ""; string idNumber = ""; string documentType = ""; DateTime dateOfBirth = new DateTime();

            if (documentRoot.analyzeResult.documentResults.FirstOrDefault().docType.ToLower().Contains("driverslicense"))
            {
                var driversLicense = JsonSerializer.Deserialize<QuickType.DriversLicense.DriversLicenseRoot>(json);
                //docType = DocumentType.DriversLicense;
                documentType = "Drivers license";
                string[] splitNames = HelperLib.Function.SplitDriversLicenseFullName(driversLicense.analyzeResult.documentResults.FirstOrDefault().fields.FullName.text);
                firstName = splitNames[0];
                middleName = splitNames[1];
                lastName = splitNames[2];
                var details = driversLicense.analyzeResult.documentResults.FirstOrDefault().fields;


                idNumber = driversLicense.analyzeResult.documentResults.FirstOrDefault().fields.DriversLicenseNo.text;
                dateOfBirth = Convert.ToDateTime(driversLicense.analyzeResult.documentResults.FirstOrDefault().fields.DateOfBirth.text);


                scannedIDCardDetails = new Models.ScannedIDCardDetails()
                {
                    Address = details.Address?.text,
                    IDType = details.CardType?.text,
                    DateOfBirth = dateOfBirth.ToLongDateString(),
                    BloodGroup = details.BloofGroup?.text,
                    FullName = details.FullName?.text,
                    Gender = details.Sex?.ToString(),
                    IDNumber = idNumber,
                    IssueDate = details.DateOfIssue?.text,
                    FirstName = firstName,
                    MiddleName = middleName,
                    LastName = lastName,
                    FirstIssueState = details.FirstIssueState?.text ?? "",
                    ExpiryDate = details.DateOfExpiry?.text,
                    FormerIDNumber = details?.FullName?.text,
                    NextOfKin = details?.NextOfKin?.text,
                    Height = details?.Height?.text,
                    IDClass = details?.ClassOfLicense?.text,
                    DocumentType = documentType


                };


            }
            else if (documentRoot.analyzeResult.documentResults.FirstOrDefault().docType.ToLower().Contains("voterscard"))
            {
                var votersCard = JsonSerializer.Deserialize<QuickType.VotersCard.Root>(json);
                // docType = DocumentType.VotersCard;
                documentType = "Voters card";

                string[] splitNames = HelperLib.Function.SplitDriversLicenseFullName(votersCard.analyzeResult.documentResults.FirstOrDefault().fields.FullName.text);
                firstName = splitNames[0];
                middleName = splitNames[1];
                lastName = splitNames[2];
                dateOfBirth = Convert.ToDateTime(votersCard.analyzeResult.documentResults.FirstOrDefault().fields.DateOfBirth.text);

                var details = votersCard.analyzeResult.documentResults.FirstOrDefault().fields;

                scannedIDCardDetails = new Models.ScannedIDCardDetails()
                {
                    Address = details.Address?.text,
                    IDType = details.CardType?.text,
                    DateOfBirth = dateOfBirth.ToLongDateString(),
                    Delim = details.delim?.text,
                    FullName = details.FullName?.text,
                    Gender = details.Sex?.ToString(),
                    IDNumber = details.VoterCardNo?.text,
                    Occupation = details.Occupation?.text,
                    FirstName = firstName,
                    MiddleName = middleName,
                    LastName = lastName,
                    DocumentType = documentType

                };




            }
            else if (documentRoot.analyzeResult.documentResults.FirstOrDefault().docType.ToLower().Contains("nationalidnumber"))
            {
                var nationalId = JsonSerializer.Deserialize<QuickType.NationalID.Root>(json);
                // docType = DocumentType.nationalId;
                documentType = "National ID";
                firstName = nationalId.analyzeResult.documentResults.FirstOrDefault().fields.Firstname?.text;
                lastName = nationalId.analyzeResult.documentResults.FirstOrDefault().fields.Surname?.text;
                middleName = nationalId.analyzeResult.documentResults.FirstOrDefault().fields.MiddleName?.text;
                dateOfBirth = Convert.ToDateTime(nationalId.analyzeResult.documentResults.FirstOrDefault().fields.DateOfBirth.text);

                var details = nationalId.analyzeResult.documentResults.FirstOrDefault().fields;

                scannedIDCardDetails = new Models.ScannedIDCardDetails()
                {

                    IDType = details.CardType?.text,
                    DateOfBirth = dateOfBirth.ToLongDateString(),

                    IDNumber = details.NumberOnCard?.text,
                    IssueDate = details.IssueDate?.text,
                    FirstName = firstName,
                    MiddleName = middleName,
                    LastName = lastName,
                    FullName = $"{firstName} {middleName} {lastName}",
                    ExpiryDate = details.DateOfExpiry?.text,
                    DocumentType = documentType


                };

            }
            else if (documentRoot.analyzeResult.documentResults.FirstOrDefault().docType.ToLower().Contains("internationalpassport"))
            {
                var internationalPassport = JsonSerializer.Deserialize<QuickType.Passport.Root>(json);

                // docType = DocumentType.InternationalPassport;
                documentType = "International Passport";

                string[] splitNames = HelperLib.Function.SplitInternationalPassportGivenName(internationalPassport.analyzeResult.documentResults.FirstOrDefault().fields.GivenNames.text);
                firstName = splitNames[0];
                middleName = splitNames[1];
                lastName = internationalPassport?.analyzeResult?.documentResults?.FirstOrDefault().fields.Surname.text;
                dateOfBirth = Convert.ToDateTime(internationalPassport?.analyzeResult?.documentResults?.FirstOrDefault().fields?.DateOfBirth?.text);
                idNumber = internationalPassport?.analyzeResult?.documentResults?.FirstOrDefault().fields?.PassPortNumber?.text;



                var details = internationalPassport.analyzeResult?.documentResults?.FirstOrDefault().fields;
                scannedIDCardDetails = new Models.ScannedIDCardDetails()
                {

                    IDType = details?.CardType?.text,
                    DateOfBirth = dateOfBirth.ToLongDateString(),

                    IDNumber = idNumber,
                    IssueDate = details?.DateOfIssue?.text,
                    FirstName = firstName,
                    MiddleName = middleName,
                    LastName = lastName,

                    ExpiryDate = details?.DateOfExpiry?.text,
                    FormerIDNumber = details?.FormerPassportNo?.text,
                    FullName = details?.Surname?.text + " " + details?.GivenNames?.text,
                    Address = details?.Authority?.text,
                    IssuingAuthority = details?.Authority?.text,
                    Gender = details?.Sex?.text,




                };

            }

            return scannedIDCardDetails;

        }
    }
}
