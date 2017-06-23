using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MdallWebApi.Models
{
    public class Device
    {
        public int original_licence_no { get; set; }
        public int device_id { get; set; }
        public DateTime? device_first_issue_dt { get; set; }
        public DateTime? end_date { get; set; }
        public String trade_name { get; set; }        

        //public IList <DeviceIdentifier> deviceIdentifierList { get; set; }
    }

    
}