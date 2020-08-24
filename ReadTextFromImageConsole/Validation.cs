using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using ReadTextFromImageConsole.Models;

namespace ReadTextFromImageConsole
{
    public class Validation
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public DocumentType docType;
        public string idNumber;
        public string firstName;
        public string lastName;
        public string dateOfBirth;
        public string ExpiryDate;
        public string exception;
        public string issueDate;
        private string expiryDate;
        public bool IsExtractionOk;
        private Camudatafield camudata;

        public Validation()
        {
        }


        public void ExtractDocKeyDetails(List<string> returnedExtractedTextFromImages, Models.Camudatafield camu)
        {
            //get the type of identification and extract biodetails from the doc
            this.returnedExtractedTextFromImages = returnedExtractedTextFromImages;
            IsExtractionOk = false;
            this.camudata = camu;
            GetMoreDateFormats();

            GetDocTypeAndDetails(returnedExtractedTextFromImages);

        }

        private void GetDocTypeAndDetails(List<string> returnedExtractedTextFromImages)
        {
            try
            {
                //checking image if it contains passport or passeport

                if (returnedExtractedTextFromImages.Any(x => x.ToLower().Contains("passport")) || returnedExtractedTextFromImages.Any(x => x.ToLower().Contains("passeport")))
                {
                    //checking image if it contains passport or passeport is true

                    docType = DocumentType.InternationalPassport;
                    //now extracting the biodata for passport
                    ExtractBioDataForPassport(returnedExtractedTextFromImages);

                }
                //checking image if it contains voter or electoral or commission

                else if (returnedExtractedTextFromImages.Any(x => x.ToLower().Contains("voter")) || returnedExtractedTextFromImages.Any(x => x.ToLower().Contains("electoral")) || returnedExtractedTextFromImages.Any(x => x.ToLower().Contains("commission")))
                {
                    //checking image if it contains voter or electoral or commission is true

                    docType = DocumentType.VotersCard;
                    ExtractBioDataForVotersCard(returnedExtractedTextFromImages);

                }
                //checking image if it contains drivers or licence or license

                else if (returnedExtractedTextFromImages.Any(x => x.ToLower().Contains("drivers")) || returnedExtractedTextFromImages.Any(x => x.ToLower().Contains("licence")) || returnedExtractedTextFromImages.Any(x => x.ToLower().Contains("license")))
                {
                    //checking image if it contains drivers or licence or license is true


                    docType = DocumentType.DriversLicense;
                    ExtractBioDataForDriversLicense
                        (returnedExtractedTextFromImages);

                }
                else
                {
                    IsExtractionOk = false;
                    exception = "can't tell type of identification";
                }
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                IsExtractionOk = false;
            }
        }

        private void ExtractBioDataForDriversLicense(List<string> returnedExtractedTextFromImages)
        {
            try
            {
                GetIdNumberFromDriversLicense();
                if (!IsExtractionOk)
                    return;
                GetNamesFromDriversLicense();
                if (!IsExtractionOk)
                    UseNamesFromDb();
                GetDatesFromDriversLicense();
                if (!IsExtractionOk)
                    return;


            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                IsExtractionOk = false;

            }
        }

        private void GetNamesFromDriversLicense()
        {
            var positionIssue = returnedExtractedTextFromImages.FindIndex(x => x.ToLower().StartsWith("iss"));
            for (int i = positionIssue; i < positionIssue + 3; i++)
            {
                if (returnedExtractedTextFromImages[i].Contains(","))
                {
                    var name = returnedExtractedTextFromImages[i].Split(',');
                    lastName = name?[0];
                    var fn = name?[0].Split(' ');
                    firstName = fn?[1] ?? fn?[0];
                    break;
                }

                SetExtractionFailed("can't get name from driver license");
            }
        }

        private void GetDatesFromDriversLicense()
        {

            try
            {
                int whereIStopped = 0;
                var positionIssue = returnedExtractedTextFromImages.FindIndex(x => x.ToLower().StartsWith("iss"));
                var positionExp = returnedExtractedTextFromImages.FindIndex(x => x.ToLower().StartsWith("exp"));
                var positionDOB = returnedExtractedTextFromImages.FindIndex(x => x.ToLower().StartsWith("d o b"));
                for (int index = 0; index < returnedExtractedTextFromImages.Count - 1; index++)
                {
                    var wordarray = returnedExtractedTextFromImages[index].Trim().Split(' ');
                    var last = wordarray.LastOrDefault().ToString();
                    if (last.Contains('-') && CheckIfDate(last, out DateTime iss))
                    {
                        whereIStopped = index;
                        issueDate = iss.ToString("yyyy-MM-dd");
                        break;
                    }
                }

                for (int index = whereIStopped + 1; index < returnedExtractedTextFromImages.Count - 1; index++)
                {
                    var wordarray = returnedExtractedTextFromImages[index].Trim().Split(' ');
                    var last = wordarray.LastOrDefault().ToString();
                    if (last.Contains('-') && CheckIfDate(last, out DateTime dob))
                    {
                        whereIStopped = index;
                        dateOfBirth = dob.ToString("yyyy-MM-dd");
                        break;
                    }
                }

                for (int index = whereIStopped + 1; index < returnedExtractedTextFromImages.Count - 1; index++)
                {
                    var wordarray = returnedExtractedTextFromImages[index].Trim().Split(' ');
                    var last = wordarray.LastOrDefault().ToString();
                    if (last.Contains('-') && CheckIfDate(last, out DateTime exp))
                    {
                        whereIStopped = index;
                        expiryDate = exp.ToString("yyyy-MM-dd");
                        break;
                    }
                }
                if (string.IsNullOrEmpty(dateOfBirth))
                {

                    UseDOBFromDb();
                }



                //if (positionIssue > 0)
                //{
                //    issueDate = returnedExtractedTextFromImages[positionIssue].Split(' ')?[1];
                //    DateTime dateTime;
                //    if (DateTime.TryParseExact(issueDate, dateFormats, new CultureInfo("en-US"),
                //                  DateTimeStyles.None, out dateTime))
                //    {
                //        issueDate = dateTime.ToString("yyyy-MM-dd");
                //    }
                //}
                //if (positionExp > 0)
                //{
                //    expiryDate = returnedExtractedTextFromImages[positionExp].Split(' ')?[1];
                //    DateTime dateTime;
                //    if (DateTime.TryParseExact(expiryDate, dateFormats, new CultureInfo("en-US"),
                //                  DateTimeStyles.None, out dateTime))
                //    {
                //        expiryDate = dateTime.ToString("yyyy-MM-dd");
                //    }
                //}
                //if (positionDOB > 0)
                //{
                //    dateOfBirth = returnedExtractedTextFromImages[positionDOB].Split(' ')?[1];
                //    DateTime dateTime;
                //    if (DateTime.TryParseExact(dateOfBirth, dateFormats, new CultureInfo("en-US"),
                //                  DateTimeStyles.None, out dateTime))
                //    {
                //        dateOfBirth = dateTime.ToString("yyyy-mm-dd");
                //    }
                //}
            }
            catch (Exception ex)
            {
                log.Error(ex);
                UseDOBFromDb();
            }
        }

        private bool CheckIfDate(string last, out DateTime iss)
        {
            var check = DateTime.TryParseExact(last, dateFormats, new CultureInfo("en-US"),
                                 DateTimeStyles.None, out iss);
            return check;
        }

        private void GetIdNumberFromDriversLicense()
        {
            IsExtractionOk = true;
            try
            {
                var position = returnedExtractedTextFromImages.FindIndex(x => x.ToLower().Contains("l/no"));
                log.Info($"drivers license: position containing l/no is {position}");
                var position2 = returnedExtractedTextFromImages.FindIndex(x => x.ToLower().StartsWith("l") && x.ToLower().Contains("no "));
                log.Info($"drivers license: position starting with L and contains no is {position2}");

                var position3 = returnedExtractedTextFromImages.FindIndex(x => x.ToLower().Contains("driver"));
                log.Info($"drivers license: position containing driver is {position3}");

                var position4 = returnedExtractedTextFromImages.FindIndex(x => x.ToLower().Contains("class"));
                log.Info($"drivers license: position containing class is {position4}");

                if (position > 0)
                {
                    idNumber = returnedExtractedTextFromImages[position].Split(' ')?[1];
                }
                else if (position2 > 0)
                {
                    idNumber = returnedExtractedTextFromImages[position2].Split(' ')?[1];

                }
                else if (position3 > 0 && position4 > 0)
                {
                    for (int index = position3; index < position4; index++)
                    {
                        var word = returnedExtractedTextFromImages[index];
                        var split = word.Split(' ');
                        if (split.LastOrDefault().ToString().Length == 12)
                        {
                            idNumber = split.LastOrDefault().ToString();
                            break;
                        }
                    }
                }
                else
                {
                    IsExtractionOk = false;
                    exception = "cant get id from drivers license";
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                SetExtractionFailed("cant get id from drivers license");
            }
        }

        private void ExtractBioDataForVotersCard(List<string> returnedExtractedTextFromImages)
        {
            try
            {
                var positionOfDateofBirthText = returnedExtractedTextFromImages.FindIndex(x => x.ToLower().Equals("date of birth"));//this number is key to finding others
                GetNameFromVotersCard(positionOfDateofBirthText);
                if (!IsExtractionOk)
                {
                    UseNamesFromDb();
                }
                GetIdNumberFromVotersCard();
                if (!IsExtractionOk)
                    return;
                GetDateOfBirthFromVotersCard(positionOfDateofBirthText);

                //else
                //{
                //    //extractiong date not successful
                //    IsExtractionOk = false;
                //}

            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                IsExtractionOk = false;

            }
        }

        private void GetDateOfBirthFromVotersCard(int positionofDateOfBirth)
        {
            for (int i = positionofDateOfBirth; i < returnedExtractedTextFromImages.Count; i++)
            {
                DateTime dateTime;
                if (DateTime.TryParseExact(returnedExtractedTextFromImages[i], dateFormats, new CultureInfo("en-US"),
                              DateTimeStyles.None, out dateTime))
                {
                    dateOfBirth = dateTime.ToString("yyyy-MM-dd");
                    IsExtractionOk = true;
                    break;
                }
                IsExtractionOk = false;
                exception = "cant get date of birth from voters card";
            }
        }

        private void GetIdNumberFromVotersCard()
        {
            var id = this.returnedExtractedTextFromImages.Where(x => x.ToLower().Contains("vin:")).FirstOrDefault();
            idNumber = id?.Split(':')?[1];
            IsExtractionOk = string.IsNullOrEmpty(idNumber) ? false : true;

        }

        private void GetNameFromVotersCard(int positionOfDateofBirthText)
        {
            for (int i = positionOfDateofBirthText - 3; i < positionOfDateofBirthText; i++)
            {
                if (returnedExtractedTextFromImages[i].Contains(','))//means its the name field
                {
                    var sp = returnedExtractedTextFromImages[i].Split(',');
                    lastName = sp?[0];
                    firstName = sp?[1].Split(' ')?[1];
                    IsExtractionOk = true;
                    break;

                }
                IsExtractionOk = false;
                exception = "can't get name from voters card";
            }
        }

        private void ExtractBioDataForPassport(List<string> returnedExtractedTextFromImages)
        {
            try
            {
                GetIdFromPassport();
                if (!IsExtractionOk)
                    return;
                GetNameFromPassport();
                if (!IsExtractionOk)
                    return;
                GetDateofBirthFromPassport();
                if (!IsExtractionOk)
                    return;
                GetIssueAndExpiryDateFromPassport();


            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                IsExtractionOk = false;

            }


        }

        private void SetExtractionFailed(string msg)
        {
            exception = msg;
            IsExtractionOk = false;
            log.Info(msg);
        }

        private void GetIssueAndExpiryDateFromPassport()
        {
            issueDate = ReturnDate("issue");
            expiryDate = ReturnDate("expiry");
        }


        List<string> monthsSummarized = new List<string> { "jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec" };
        private void GetDateofBirthFromPassport()
        {
            try
            {
                var searchKey = "date of birth";
                var dobCalculated = ReturnDate(searchKey);
                if (!string.IsNullOrEmpty(dobCalculated))
                {
                    dateOfBirth = dobCalculated;
                    UseDOBFromDb();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                UseDOBFromDb();
            }
        }

        private string ReturnDate(string searchKey)
        {
            //start from where the search key is found. to reduce the loop
            var position1 = returnedExtractedTextFromImages.FindIndex(x => x.ToLower().Contains(searchKey));
            //loop from this position till the end
            for (int i = position1; i < returnedExtractedTextFromImages.Count; i++)
            {
                //if it contains any of the summarized months in monthsSummarized
                if (monthsSummarized.Any(d => returnedExtractedTextFromImages[i].ToLower().Contains(d)) || returnedExtractedTextFromImages[i].Contains("/"))
                {
                    int year = 0;
                    int day = 0;
                    string mth = "";
                    var word = returnedExtractedTextFromImages[i];

                    var possibleDate = word.Split('/');
                    //get the first array. this array has month and year i.e jul 09
                    var mthAndyear = possibleDate?[1];//get year
                    //split it into individual string, by space
                    var mthAndyearArray = mthAndyear.Split(' ');
                    bool isDay = false;
                    bool isYear = false;
                    //loop through the split to pick out the number. the number represents the year
                    for (int nd = 0; nd < mthAndyearArray.Length; nd++)
                    {
                        var key = mthAndyearArray[nd];
                        //check if its a valid number
                        isYear = int.TryParse(key, out year);
                        if (isYear)
                        {
                            //if yes, stop processing
                            break;
                        }

                    }
                    if (!isYear)
                    {
                        //if its not, continue the loop
                        continue;
                    }
                    //get day by doing similar thing done above here
                    var dayAndMth = possibleDate?[0];
                    var dayAndMthArray = dayAndMth.Split(' ');
                    for (int nd = 0; nd < dayAndMthArray.Length; nd++)
                    {
                        var key = dayAndMthArray[nd];
                        isDay = int.TryParse(key, out day);
                        if (isDay)
                        {
                            //get the actual month
                            mth = dayAndMthArray[1];
                            break;
                        }

                    }
                    if (!isDay)
                    {
                        continue;
                    }
                    var dob = $"{day}/{mth}/{year}";
                    DateTime dateTime;
                    if (DateTime.TryParseExact(dob, dateFormats, new CultureInfo("en-US"),
                                  DateTimeStyles.None, out dateTime))
                    {
                        IsExtractionOk = true;
                        return dateTime.ToString("yyyy-MM-dd");
                        ;
                    }

                }

                //extractiong date not successful
                SetExtractionFailed("extractiong date of birth from passport not successful");

            }
            return "";
        }

        private void UseDOBFromDb()
        {
            IsExtractionOk = true;
            dateOfBirth = camudata.Dob;
            log.Info($"use dob from db instead as exception was gotten trying to read doc type {camudata.Idtype} of user {camudata.AccountName} {camudata.AccountNumber} {camudata.EmailAddress} ");
        }

        private void GetNameFromPassport()
        {
            try
            {
                var position1 = returnedExtractedTextFromImages.FindIndex(x => x.ToLower().Contains("surname"));
                var position2 = returnedExtractedTextFromImages.FindIndex(x => x.ToLower().Contains("given name"));
                if ((position2 - position1) == 2)//then pick whats between
                {
                    lastName = returnedExtractedTextFromImages[position1 + 1];
                    IsExtractionOk = true;

                }
                else
                {
                    UseNamesFromDb();
                    //SetExtractionFailed("cant get last name");
                    return;
                }
                var position3 = returnedExtractedTextFromImages.FindIndex(x => x.ToLower().Contains("nationality"));
                if ((position3 - position2) == 2)//then pick whats between
                {
                    if (returnedExtractedTextFromImages[position2 + 1].Contains(" "))
                    {
                        firstName = returnedExtractedTextFromImages[position2 + 1].Split(' ')[0];
                        IsExtractionOk = true;

                    }
                    else
                    {
                        firstName = returnedExtractedTextFromImages[position2 + 1];
                        IsExtractionOk = true;

                    }
                }
                else
                {
                    // SetExtractionFailed("cant get last name");
                    UseNamesFromDb();

                    return;
                }
            }
            catch (Exception ex)
            {
                //use names from the db

                log.Error(ex);
                UseNamesFromDb();

            }


        }

        private void UseNamesFromDb()
        {
            IsExtractionOk = true;
            firstName = camudata.FirstName;
            lastName = camudata.LastName;
            log.Info($"use names from db instead as exception was gotten trying to read doc type {camudata.Idtype} of user {camudata.AccountName} {camudata.AccountNumber} {camudata.EmailAddress} ");


        }

        private void GetIdFromPassport()
        {
            var position1 = returnedExtractedTextFromImages.FindIndex(x => x.ToLower().Contains("nga"));
            var position2 = returnedExtractedTextFromImages.FindIndex(x => x.ToLower().Contains("passeport"));
            //passport is between these two
            for (int i = 0; i < returnedExtractedTextFromImages.Count - 1; i++)
            {
                var _word = returnedExtractedTextFromImages[i];
                if (_word.StartsWith("A") && _word.Length == 9)//means its passport. should i add the length?
                {
                    idNumber = returnedExtractedTextFromImages[i];
                    IsExtractionOk = true;
                    break;
                }
                IsExtractionOk = false;
                exception = "cant get id for intl passport";
            }
        }

        public enum DocumentType
        {
            InternationalPassport,
            DriversLicense,
            VotersCard,
            nationalId
        }
        public string[] dateFormats;
        List<string> dataFormatWork = new List<string>();
        List<string> dateFormatsListType = new List<string> {"M/d/yyyy", "d/M/yyyy",
            "M/d/yy", "d/M/yy",
                   "MM/dd/yyyy", "dd/MM/yyyy",

                   "MM/dd/yy", "dd/MM/yy","d/MMM/yy","dd/MMM/yy"};

        public void GetMoreDateFormats()
        {
            try
            {
                foreach (var item in dateFormatsListType)
                {
                    var more = item.Replace("/", "-");
                    dataFormatWork.Add(more);
                }
                foreach (var item in dateFormatsListType)
                {
                    var more = item.Replace("/", " ");
                    dataFormatWork.Add(more);
                }
                dataFormatWork.AddRange(dateFormatsListType);
                dateFormats = dataFormatWork.ToArray();
            }
            catch (Exception ex)
            {

            }
        }
        private List<string> returnedExtractedTextFromImages;
    }
}
