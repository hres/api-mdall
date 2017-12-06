using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MdallWebApi.Models;

namespace MdallWebApi.Controllers
{
    public class LicenceTypeController : ApiController
    {
        static readonly ILicenceTypeRepository databasePlaceholder = new LicenceTypeRepository();


        public IEnumerable<LicenceType> GetAllLicenceType(string lang = "en")
        {

            return databasePlaceholder.GetAll(lang);
        }


        public LicenceType GetLicenceTypeByCode(string code, string lang = "en")
        {
            LicenceType type = databasePlaceholder.Get(code, lang);
            if (type == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return type;
        }
    }
}