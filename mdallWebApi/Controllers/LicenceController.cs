using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MdallWebApi.Models;

namespace MdallWebApi.Controllers
{
    public class LicenceController : ApiController
    {
        static readonly ILicenceRepository databasePlaceholder = new LicenceRepository();
        
        public IEnumerable<Licence> GetAllLicence(string state = "", string licence_name = "")
        {
            return databasePlaceholder.GetAll(state, licence_name);
        }        
        public Licence GetLicenceById(int id, string state = "")
        {
            Licence licence = databasePlaceholder.Get(id, state);
            if (licence == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return licence;
        }
      

        public IEnumerable<Licence> GetAllLicenceByCompanyId(int company_id, string state = "")
        {

            return databasePlaceholder.GetAllLicenceByCompanyId(company_id, state);
        }
        
    }
}