using System;
using System.Collections.Generic;
using System.Linq;
using IdentificationValidationLib.Models;

namespace IdentificationValidationLib
{
    public class HelperServices
    {
        public HelperServices()
        {
        }

        public static List<string> ExtractWordIntoLists(string contentString)
        {
            try
            {
                //extracting string objects into a list
                //log.Info("extracting string objects into a list");
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ExtracteTextModel>(contentString);
                var extract = result.analyzeResult.readResults.FirstOrDefault().lines.Select(x => x.text).ToList();
                return extract;
            }
            catch (Exception)
            {
                return null;
            }
        }



        public static bool Compare(string contentString)
        {
            try
            {
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool Compare(Validation v, Camudatafield camudatafield)
        {
            if (v.idNumber.ToLower() == camudatafield.Idno.ToLower())
            {
                return true;

            }
            else
            {
                return false;

            }
        }



    }
}
