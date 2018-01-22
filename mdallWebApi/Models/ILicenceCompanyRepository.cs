using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MdallWebApi.Models
{
    interface ILicenceCompanyRepository
    {
        IEnumerable<LicenceCompany> GetAll(string lang, string state = "", string licence_name = "", long licence_id = 0, string company_name = "", int company_id = 0);
        LicenceCompany Get(string lang, long licence_id, string state = "");       
    }
}
