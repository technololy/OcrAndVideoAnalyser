using System;
using System.Collections.Generic;
using System.Text;

namespace IdentificationValidationLib.Models
{
    public class NextOfKin
    {
        public object firstname { get; set; }
        public object lastname { get; set; }
        public object middlename { get; set; }
        public object address1 { get; set; }
        public string address2 { get; set; }
        public object lga { get; set; }
        public object state { get; set; }
        public object town { get; set; }
    }

    public class Residence
    {
        public string address1 { get; set; }
        public object address2 { get; set; }
        public object town { get; set; }
        public string lga { get; set; }
        public string state { get; set; }
        public object status { get; set; }
    }

    public class NINData
    {
        public string nin { get; set; }
        public string title { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string middlename { get; set; }
        public string phone { get; set; }
        public string birthdate { get; set; }
        public string nationality { get; set; }
        public string gender { get; set; }
        public object profession { get; set; }
        public string stateOfOrigin { get; set; }
        public string lgaOfOrigin { get; set; }
        public object placeOfOrigin { get; set; }
        public string photo { get; set; }
        public object maritalStatus { get; set; }
        public string height { get; set; }
        public object email { get; set; }
        public object employmentStatus { get; set; }
        public object birthState { get; set; }
        public string birthCountry { get; set; }
        public NextOfKin nextOfKin { get; set; }
        public object nspokenlang { get; set; }
        public object ospokenlang { get; set; }
        public object parentLastname { get; set; }
        public object religion { get; set; }
        public Residence residence { get; set; }
        public string signature { get; set; }
    }

    public class NINDataResponse
    {
        public string status { get; set; }
        public NINData data { get; set; }
    }

    public class NINResponse
    {
        public string responseCode { get; set; }
        public NINDataResponse dataResponse { get; set; }
    }


}
