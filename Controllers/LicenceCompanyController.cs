using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MdallWebApi.Models;
namespace MdallWebApi.Controllers
{
    public class LicenceCompanyController : ApiController
    {
        static readonly ILicenceCompanyRepository databasePlaceholder = new LicenceCompanyRepository();
        //lang is mandatory, the other parameters are optional
        public IEnumerable<LicenceCompany> GetAll(string lang, string status = "", string licence_name = "", long licence_id = 0, string company_name = "", int company_id = 0)
        {

            return databasePlaceholder.GetAll(lang, status, licence_name, licence_id, company_name, company_id);
        }
        //lang and licence_id are mandatory, status is optional
        public LicenceCompany GetLicenceCompanyById(string lang, long licence_id, string status = "")
        {
            LicenceCompany company = databasePlaceholder.Get(lang, licence_id, status);
            if (company == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return company;
        }
 
    }
}
