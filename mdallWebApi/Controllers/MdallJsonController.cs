using System.Collections.Generic;
using System.Linq;
using MdallWebApi.Models;
using System.Web.Http;
using System.Web.Mvc;
using System.Web;
using System;
using System.Text;
using System.ComponentModel;

namespace MdallWebApi.Controllers
{
    public class MdallJsonController : Controller
    {
        public ActionResult GetAllListForJsonByCategory(string lang, string status, string term, int categoryType)
        {
            var companyResult = new List<Company>();
            var searchResult = new List<Search>();
            var companyController = new CompanyController();           
            switch (categoryType)
            {
                case (int)category.company:
                     companyResult = companyController.GetAllCompany(status, term).ToList();
                    return Json(new { companyResult }, JsonRequestBehavior.AllowGet);
                case (int)category.licence:
                    var licenceController = new LicenceController();
                    var licenceResult = licenceController.GetAllLicence(status, term).ToList();
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
                                search.company_address = UtilityHelper.BuildAddress(company); 
                            }
                            searchResult.Add(search);
                        }
                    }
                    return Json(new { searchResult }, JsonRequestBehavior.AllowGet);
                case (int)category.device:
                    searchResult = new List<Search>();
                    var deviceResult = new List<Device>();
                    var deviceller = new DeviceController();
                    deviceResult = deviceller.GetAllDevice(status, term, 0).ToList();
                    if (deviceResult.Count > 0)
                    {
                        var licController = new LicenceController();
                        foreach (var device in deviceResult)
                        {
                            var search = new Search();
                            var company = new Company();
                            var licence = new Licence();
                            search.original_licence_no = device.original_licence_no;
                            search.device_name = device.trade_name;
                            search.device_id = device.device_id;
                            licence = licController.GetLicenceById(device.original_licence_no, status);
                            if(licence != null && licence.original_licence_no > 0)
                            {
                                search.licence_name = licence.licence_name;
                                search.licence_status = licence.licence_status;
                                search.application_id = licence.application_id;
                                company = companyController.GetCompanyById(licence.company_id, lang);  
                                if (company != null && company.company_id > 0)
                                {
                                    search.company_id = licence.company_id;
                                    search.company_name = company.company_name;
                                    search.company_address = UtilityHelper.BuildAddress(company);
                                }
                            }
                            searchResult.Add(search);
                        }
                    }
                    return Json(new { searchResult }, JsonRequestBehavior.AllowGet);
            }
            return  Json(new { companyResult }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetCompanyByIDForJson(int id, [DefaultValue(0)] int licID, string lang)
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
                data.company_id = company.company_id;
                data.company_name = company.company_name;
                data.company_address = UtilityHelper.BuildAddress(company);
                if (licID == 0)
                {
                    data.licenceList = licenceController.GetAllLicenceByCompanyId(company.company_id, "active").ToList();
                }
                else
                {
                    data.licenceList.Add(licenceController.GetLicenceById(licID, "active"));
                }

                if (data.licenceList != null && data.licenceList.Count > 0)
                {
                    //Get Licence
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
