using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MdallWebApi.Models
{
    interface ILicenceRepository
    {
        IEnumerable<Licence> GetAll(string state = "", string licenceName = "", string lang="");

        IEnumerable<Licence> GetAllLicenceByCompanyId(int company_id, string state = "");
        

        Licence Get(int id, string state = "",string lang="");
        
    }
}
