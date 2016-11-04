using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MdallWebApi.Models
{
    public class Licence
    {
        public int original_licence_no { get; set; }
        public String licence_status { get; set; }
        public int application_id { get; set; }
        public int appl_risk_class { get; set; }
        public String licence_name { get; set; }
        public DateTime? first_licence_status_dt { get; set; }
        public DateTime? last_refresh_dt { get; set; }
        public DateTime? end_date { get; set; }
        public String licence_type_cd { get; set; }
        public int company_id { get; set; }
        public string licence_type_desc { get; set; }

        public IList<Device> deviceList { get; set; }
    }


}