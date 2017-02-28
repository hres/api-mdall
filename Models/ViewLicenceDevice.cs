using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MdallWebApi.Models
{
    public class ViewLicenceDevice
    {
        public int original_licence_no { get; set; }
        public int device_id { get; set; }
        public DateTime? first_licence_dt { get; set; }
        public DateTime? end_date { get; set; }
        public String trade_name { get; set; }
        public String device_identifier { get; set; }
        public DateTime? device_identifier_first_date { get; set; }
    }
}