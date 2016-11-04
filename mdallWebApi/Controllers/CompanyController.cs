using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MdallWebApi.Models;
namespace MdallWebApi.Controllers
{
    public class CompanyController : ApiController
    {
        static readonly ICompanyRepository databasePlaceholder = new CompanyRepository();

        public IEnumerable<Company> GetAllCompany(string status = "", string company_name = "")
        {

            return databasePlaceholder.GetAll(status, company_name);
        }


        public Company GetCompanyById(int id, string lang)
        {
            Company company = databasePlaceholder.Get(id, lang);
            if (company == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return company;
        }
 
    }
}
