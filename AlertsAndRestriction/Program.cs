using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using ReadTextFromImageConsole;
using ReadTextFromImageConsole.Models;

namespace AlertsAndRestriction
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        static string acctOpenBaseURL = "";
        static void Main(string[] args)
        {
            acctOpenBaseURL = ReadTextFromImageConsole.Program.config.GetSection("AppSettings").GetSection("CamsURL").Value;
            GetDataFromTable();


        }

        private static void GetDataFromTable()
        {
            var cams = GetTreated();
            if (cams == null)
            {
                return;
            }
            if (cams.Count > 0)
            {
                foreach (var item in cams)
                {
                    if (item.FacialImageCheckResponse.ToLower().Contains("success") && item.IdentificationImageCheckResponse.ToLower().Contains("success"))
                    {
                        var response = LiftRestriction(item.AccountNumber).Result;

                        if (response.isSuccess && response.model.restrictionResponse.responseCode == "00")
                        {
                            string body = ComposeSuccessEmail(item);
#if DEBUG
                            //     item.EmailAddress = "loladeking@yahoo.com";
#endif
                            SendEmail(true, body, item.EmailAddress, item.FirstName + " " + item.LastName);
                            log.Info($"restriction lifted for {item.AccountNumber}");
                        }
                        else
                        {
                            log.Info($"restriction failed to be lifted for {item.AccountNumber}");

                        }

                    }
                    else
                    {

                        string body = ComposeFailedEmail(item);
#if DEBUG
                        // item.EmailAddress = "loladeking@yahoo.com";
#endif
                        SendEmail(false, body, item.EmailAddress, item.FirstName + " " + item.LastName);
                    }
                }
            }
        }

        private static string ComposeSuccessEmail(Camudatafield cam)
        {

            var file = System.IO.File.ReadAllText("AccountOpeningFailed.html");
            file = file.Replace("%accountNumber%", cam.AccountNumber);
            file = file.Replace("%accountName%", cam.AccountName);


            return file;
        }

        private static string ComposeFailedEmail(Camudatafield cam)
        {
            var reason = cam.IdentificationImageCheckResponse + " ; " + cam.FacialImageCheckResponse;
            var file = System.IO.File.ReadAllText("AccountOpeningFailed.html");
            file = file.Replace("%accountNumber%", cam.AccountNumber);
            file = file.Replace("%docType%", cam.Idtype);
            file = file.Replace("%challenge%", reason);
            file = file.Replace("%action%", "Kindly upload a new one on OneBank's document upload page");
            return file;
        }

        private static void SendEmail(bool isSuccess, string body, string receipientEmail, string recepientName)
        {
            API pI = new API(acctOpening: true);
            var model = new SendEmail
            {
                subj = isSuccess ? "Account activated successfully" : "Account activation failed ",
                body = body,
                base64Attachment = "",
                customerName = recepientName,
                destinationEmail = receipientEmail,
                fileName = "",
                fromEmail = "",
                recieverFirstName = recepientName,
                addressesToCopy = "ololade.oyebanji@IconFlux.ng"
            };
            var resp = pI.PostAny<dynamic>(model, $"{acctOpenBaseURL}/api/User/SendMailAttachment").Result;
            if (resp.isSuccess)
            {
                log.Info($"{receipientEmail} email was successfully lifted");
            }
            else
            {
                log.Info($"{receipientEmail} email wasnt sent failed to be lifted");
            }
        }


        private static List<Camudatafield> GetSuccessful()
        {
            var context = new AppDbContext();
            var successList = context.Camudatafield.Where(s => s.FacialImageCheckResponse.ToLower() == "success" && s.IdentificationImageCheckResponse.ToLower().Contains("success")).Take(100).ToList();
            log.Info($"{successList.Count} successful records found");
            return successList;
        }

        private static List<Camudatafield> GetTreated()
        {
            var context = new AppDbContext();
            var successList = context.Camudatafield.Where(s => s.FacialImageCheckResponse.ToLower() != null && s.IdentificationImageCheckResponse.ToLower() != null).Take(100).ToList();
            log.Info($"{successList.Count} records found");
            return successList;
        }

        private static List<Camudatafield> GetFailed()
        {
            var context = new AppDbContext();
            var successList = context.Camudatafield.Where(s => s.FacialImageCheckResponse.ToLower() != "success" && s.IdentificationImageCheckResponse.ToLower() != ("success")).Take(100).ToList();
            log.Info($"{successList.Count} failed records found");
            return successList;
        }


        private static Task<(bool isSuccess, string result, Restriction.RestrictionResponseResponse model)> LiftRestriction(string nuban)
        {
            Restriction.RestrictionResponseRequest restriction = new Restriction.RestrictionResponseRequest()
            {
                removeRestriction = new Restriction.RemoveRestriction
                {
                    account = nuban,
                    accountsType = "ACCOUNTS",
                    branchcode = "NG0020555",
                    restriction_code = "1"
                }
            };
            API pI = new API(acctOpening: true);
            var resp = pI.PostAny<Restriction.RestrictionResponseResponse>(restriction, $"{acctOpenBaseURL}/api/User/FioranoRemoveRestriction").Result;
            if (resp.isSuccess)
            {
                log.Info($"{nuban} restriction was successfully lifted");

            }
            else
            {
                log.Info($"{nuban} restriction failed to be lifted");


            }

            return Task.FromResult(resp);
        }


        private static List<ReadTextFromImageConsole.Models.Camudatafield> SendNotification(string email, bool isSuccess, ReadTextFromImageConsole.Models.Camudatafield camudatafield)
        {
            var context = new AppDbContext();
            var successList = context.Camudatafield.Where(s => s.FacialImageCheckResponse.ToLower() != "success" && s.IdentificationImageCheckResponse.ToLower() != ("success")).Take(100).ToList();
            log.Info($"{successList.Count} failed records found");
            return successList;
        }

    }
}
