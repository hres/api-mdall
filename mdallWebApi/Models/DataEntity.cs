using System;
using System.Collections.Generic;



namespace MdallWebApi.Models
{
    public enum category
    {
        company = 1,
        licence = 2,
        device = 3
    }

    public class Search
    {
        public int original_licence_no { get; set; }
        public String licence_status { get; set; }
        public int application_id { get; set; }
        public String licence_name { get; set; }
        public int company_id { get; set; }
        public string company_name { get; set; }
        public string company_address { get; set; }
    }


    public class CompanyDetail
    {
        public int company_id { get; set; }
        public string company_name { get; set; }
        public string company_address { get; set; }
        public IList<Licence> licenceList { get; set; }
    }

    public class LicenceDetail
    {
        public Licence licence { get; set; }
        public IList<Device> deviceList { get; set; }
    }

    public class DeviceDeteail
    {
        public Device device { get; set; }
        public IList<DeviceIdentifier> deviceIdentifierList { get; set; }
    }
}