using System.Collections.Generic;
using System.Linq;
using MdallWebApi.Models;
using System.Web.Http;
using System.Web.Mvc;
using System.Web;
using System;
using System.Text;

namespace MdallWebApi.Controllers
{
    public class MdallJsonController : Controller
    {
        public ActionResult GetAllListForJsonByCategory(string lang, string status, string term, int categoryType)
        {
            var companyResult = new List<Company>();
            var companyController = new CompanyController();
            switch (categoryType)
            {
                case (int)category.company:
                     companyResult = companyController.GetAllCompany(status, term).ToList();
                    return Json(new { companyResult }, JsonRequestBehavior.AllowGet);
                case (int)category.licence:
                    var searchResult = new List<Search>();
                    var licenceResult = new List<Licence>();
                    var licenceController = new LicenceController();
                    licenceResult = licenceController.GetAllLicence(status, term).ToList();
                    if (licenceResult.Count > 0)
                    {
                        foreach (var licence in licenceResult)
                        {
                            var search = new Search();
                            var company = new Company();
                            var address = new StringBuilder();
                            company = companyController.GetCompanyById(licence.company_id, lang);
                            search.original_licence_no = licence.original_licence_no;
                            search.licence_status = licence.licence_status;
                            search.application_id = licence.application_id;
                            search.company_id = licence.company_id;
                            search.licence_name = licence.licence_name;
                            if (company != null && company.company_id > 0)
                            {
                                search.company_name = company.company_name;
                                address.AppendFormat(company.addr_line_1).Append(" ");
                                address.Append(company.addr_line_2).Append(" ");
                                address.Append(company.addr_line_3).AppendLine();
                                address.Append(company.city).Append(",");
                                address.Append(company.region_cd).Append(",");
                                address.Append(company.country_cd).Append(","); 
                                address.Append(company.postal_code);
                                search.company_address = address.ToString();
                            }
                            searchResult.Add(search);
                        }
                    }
                    return Json(new { searchResult }, JsonRequestBehavior.AllowGet);                   
            }
            return  Json(new { companyResult }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetCompanyByIDForJson(int id,string lang)
        {
            var companyController = new CompanyController();
            var licenceController = new LicenceController();
            var deviceController = new DeviceController();
            var data = new CompanyDetail();
            data.licenceList = new List<Licence>();
            var licenceDetail = new LicenceDetail();

            var company = new Company();
            var deviceList = new List<Device>();

           company = companyController.GetCompanyById(id, lang);
            if (company != null && company.company_id > 0)
            {
                var address = new StringBuilder();
                data.company_id = company.company_id;
                data.company_name = company.company_name;
                address.AppendFormat(company.addr_line_1).Append(" ");
                address.Append(company.addr_line_2).Append(" ");
                address.Append(company.addr_line_3).Append(",");
                address.Append(company.city).Append(",");
                address.Append(company.region_cd).Append(",");
                address.Append(company.country_cd).Append(",");
                address.Append(company.postal_code);
                data.company_address = address.ToString();

                data.licenceList = licenceController.GetAllLicenceByCompanyId(company.company_id, "active").ToList();
                if (data.licenceList != null && data.licenceList.Count > 0)
                {
                    //Get Device
                    foreach (var licence in data.licenceList)
                    {
                        licence.deviceList = new List<Device>();
                        //Get Device
                        deviceList = deviceController.GetAllDevice("", "", licence.original_licence_no).ToList();
                        if (deviceList != null && deviceList.Count > 0)
                        {
                            licence.deviceList = deviceList;
                        }
                    }
                }
                
            }

            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }
    }
}
