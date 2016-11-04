using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MdallWebApi.Models
{
    interface ILicenceTypeRepository
    {
        IEnumerable<LicenceType> GetAll(string lang = "");
        
        LicenceType Get(String code, string lang = "");
    }
}
