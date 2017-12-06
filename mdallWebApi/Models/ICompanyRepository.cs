using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MdallWebApi.Models
{
    interface ICompanyRepository
    {
        IEnumerable<Company> GetAll(string status = "", string company_name = "");
        Company Get( int id, string status = "");
    }
}
