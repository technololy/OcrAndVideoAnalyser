using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using Microsoft.Extensions.Configuration;
using ReadTextFromImageConsole;
using SendImageToOneExpress.Models;

namespace SendImageToOneExpress
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        static int count;
        static string acctOpenBaseURL = "";
        static API accountOpeningApi = new API(acctOpening: true);
        static MandateMgtReqContext context = new MandateMgtReqContext();
        public static IConfigurationRoot config;

        static void Main(string[] args)
        {
            try
            {


                // Load configuration
                var builder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                config = builder.Build();
                acctOpenBaseURL = config.GetSection("AppSettings").GetSection("CamsURL").Value;

                WriteToConsole("statrting job....");
                List<Models.TblRecords> all = GetAllFromDatabase();
                if (all?.Count > 0)
                {
                    count = 0;
                    foreach (var item in all)
                    {
                        count++;
                        WriteToConsole($"getting bvn image for record {count} and account {item.Nuban}");
                        string img = GetImageFromBVN(item.Bvn);
                        if (!string.IsNullOrEmpty(img))
                        {
                            WriteToConsole($"getting azure url for image for record {count} and account {item.Nuban}");

                            string url = SendImageToAzureAndGetURL(img, item.Bvn);
                            if (!string.IsNullOrEmpty(url))
                            {
                                WriteToConsole($"sending to one express azure url {url} for image for record {count} and account {item.Nuban}");
                                SendToOneExpress
                        (url, item.AccountName, item.Nuban.ToString(), item);
                            }
                            else
                            {
                                //exit
                                log.Info("nothing from sendimagetoazureurl");
                            }
                        }
                        else
                        {
                            //exit
                            log.Info("nothing from getimagefrom bvn");
                        }
                    }
                }
                else
                {
                    //exit
                    log.Info("nothing");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private static void SendToOneExpress(string url, string accName, string accNum, TblRecords pictureMgt)
        {
            string urlEnd = config.GetSection("AppSettings").GetSection("OneExpressAPI").Value;
            ;
            API aPI = new API();
            OneExpressSubmitImage oneExpress = new OneExpressSubmitImage()
            {
                dataFields = new DataFields()
                {
                    AccountName = accName,
                    AccountNumber = accNum,
                    CustomerID = "",
                    MandateURL = url,
                    Source = "MandatePictureJob"
                }
            };
            var response = aPI.PostAny<dynamic>(oneExpress, urlEnd).Result;
            SaveToDBAsDone(pictureMgt, url);


        }

        private static void SaveToDBAsDone(TblRecords pictureMgt, string url)
        {
            TblRecordsDone mgt = new TblRecordsDone()
            {
                AccountName = pictureMgt.AccountName,
                Bvn = pictureMgt.Bvn,
                DateOpen = pictureMgt.DateOpen,
                Email = pictureMgt.Email,
                Nuban = pictureMgt.Nuban,
                PhoneNumber = pictureMgt.PhoneNumber,
                Restriction = pictureMgt.Restriction,
                RestrictionCode = pictureMgt.RestrictionCode,
                WorkingBalance = pictureMgt.WorkingBalance,
                UrlOfImageUploaded = url
            };
            using (var context2 = new MandateMgtReqContext())
            {
                context2.TblRecordsDone.Add(mgt);
                context2.SaveChanges();
            }

        }

        private static string SendImageToAzureAndGetURL(string img, string bvn)
        {
            // API aPI = new API(true);
            CamuAzureImageRequest model = new CamuAzureImageRequest()
            {

                appId = "4",
                folderName = $"MandatePictureJob/ValidateImage/{bvn}",
                fileName = $"{DateTime.Now.Ticks}_{bvn}.jpg",
                base64String = img

            };
            var response = accountOpeningApi.PostAny<CamuAzureResponse>(model, $"{acctOpenBaseURL}/api/User/UploadImageToAzureWithResponseCode").Result;
            if (response.isSuccess)
            {
                return response.SuccessObj?.responseCode == "00" ? response.SuccessObj?.url : "";
            }
            else
            {
                return "";
            }
        }

        private static string GetImageFromBVN(string bvn)
        {
            BvnRequest request = new BvnRequest()
            {
                bvn = bvn
            };
            //API accountOpeningApi = new API(acctOpening: true);
            var resp = accountOpeningApi.PostAny<BVNResponse>(request, $"{acctOpenBaseURL}/api/User/VerifyBVNForMobile").Result;
            if (resp.isSuccess)
            {

                return resp.SuccessObj?.Base64Image;
            }
            else
            {
                return "";
            }

        }

        private static List<TblRecords> GetAllFromDatabase()
        {


            try
            {
                var query =
                (from c in context.TblRecords
                 where !(from o in context.TblRecordsDone
                         select o.Nuban)
                        .Contains(c.Nuban)
                 select c).ToList();
                return query;
            }
            catch (Exception ex)
            {

            }


            return new List<TblRecords>();
        }

        static void WriteToConsole(string msg)
        {
            Console.WriteLine(msg);
            Console.WriteLine("");
            log.Info(msg);

        }
    }
}
