using System.Collections.Generic;
using System.Linq;
using MdallWebApi.Models;
using System.Web.Http;
using System.Web.Mvc;
using System.Web;
using System;
using System.Text;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Data;
using System.IO;

namespace MdallWebApi.Controllers
{
    public class MdallJsonController : Controller
    {

        public ActionResult GetAllListForJsonByCategory(string lang, string status, string term, int categoryType)
        {
            var companyResult = new List<Company>();
            var searchResult = new List<Search>();
            var companyController = new CompanyController();
            var numberTerm = 0;

            if (!string.IsNullOrWhiteSpace(term))
            {
                numberTerm = UtilityHelper.GetNumberTerm(term);
            }
            switch (categoryType)
            {
                case (int)category.company:
                    if (numberTerm > 0)
                    {
                        //companyResult.Add(companyController.GetCompanyById(numberTerm, lang,status));
                        var company = new Company();
                        company = companyController.GetCompanyById(numberTerm, status);
                        if (company.company_id != 0)
                        {
                            companyResult.Add(companyController.GetCompanyById(numberTerm, status));
                        }
                    }
                    else
                    {
                        companyResult = companyController.GetAllCompany(status, term).ToList();
                    }

                    return Json(new { companyResult }, JsonRequestBehavior.AllowGet);

                case (int)category.licence:
                    var licenceController = new LicenceController();
                    var licenceResult = new List<Licence>();
                    //var locationController = new SbdLocationController();                 
                    if (numberTerm > 0)
                    {
                        licenceResult.Add(licenceController.GetLicenceById(numberTerm, status));
                    }
                    else
                    {
                        if (status == "active")
                        {
                            licenceResult = licenceController.GetAllLicence(status, term).ToList();
                        }

                    }
                    if (licenceResult.Count > 0 && licenceResult[0].original_licence_no != 0)
                    {
                        foreach (var licence in licenceResult)
                        {
                            var search = new Search();
                            var company = new Company();
                            var address = new StringBuilder();
                            company = companyController.GetCompanyById(licence.company_id, status);

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
                    var deviceController = new DeviceController();
                    deviceResult = deviceController.GetAllDevice(status, term, 0).ToList();
                    if (deviceResult.Count > 0)
                    {
                        var controller = new LicenceController();
                        foreach (var device in deviceResult)
                        {
                            var search = new Search();
                            var company = new Company();
                            var licence = new Licence();
                            search.original_licence_no = device.original_licence_no;
                            search.device_name = device.trade_name;
                            search.device_id = device.device_id;
                            licence = controller.GetLicenceById(device.original_licence_no, status);
                            if (licence != null && licence.original_licence_no > 0)
                            {
                                search.licence_name = licence.licence_name;
                                search.licence_status = licence.licence_status;
                                search.application_id = licence.application_id;
                                company = companyController.GetCompanyById(licence.company_id, status);
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
                case (int)category.deviceIdentifier:
                    searchResult = new List<Search>();
                    var identifierResult = new List<DeviceIdentifier>();
                    var identifierController = new DeviceIdentifierController();
                    var devController = new DeviceController();
                    var licController = new LicenceController();

                    identifierResult = identifierController.GetAllDeviceIdentifier(status, term, 0, 0).ToList();
                    if (identifierResult.Count > 0)
                    {
                        foreach (var identifier in identifierResult)
                        {
                            var search = new Search();
                            var company = new Company();
                            var licence = new Licence();
                            var device = new Device();
                            search.original_licence_no = identifier.original_licence_no;
                            search.device_id = identifier.device_id;
                            search.device_identifier = identifier.device_identifier;
                            device = devController.GetDeviceById(identifier.device_id);
                            if (device != null && device.original_licence_no > 0)
                            {
                                search.device_name = device.trade_name;
                                //if (status == "active")
                                //{
                                licence = licController.GetLicenceById(device.original_licence_no, status);
                                //}
                                //else
                                //{
                                //    licence = licController.GetArchivedLicenceById(device.original_licence_no);
                                //}

                                if (licence != null && licence.original_licence_no > 0)
                                {
                                    search.licence_name = licence.licence_name;
                                    search.licence_status = licence.licence_status;
                                    search.application_id = licence.application_id;
                                    company = companyController.GetCompanyById(licence.company_id, status);
                                    if (company != null && company.company_id > 0)
                                    {
                                        search.company_id = licence.company_id;
                                        search.company_name = company.company_name;
                                        search.company_address = UtilityHelper.BuildAddress(company);
                                    }
                                }
                            }
                            searchResult.Add(search);
                        }
                    }
                    return Json(new { searchResult }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { companyResult }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetCompanyByIDForJson(int id, [DefaultValue(0)] int licID, [DefaultValue(0)] int devID, string identifier, string lang, string status)
        {
            var companyController = new CompanyController();
            var licenceController = new LicenceController();
            var deviceController = new DeviceController();
            var identifierController = new DeviceIdentifierController();
            var licencestatus = status;

            var data = new CompanyDetail();
            data.licenceList = new List<LicenceDetail>();

            //1. Get Company
            var company = new Company();
            company = companyController.GetCompanyById(id, status);
            if (company != null && company.company_id > 0)
            {
                data.company_id = company.company_id;
                data.company_name = company.company_name;
                data.company_address = UtilityHelper.BuildAddress(company);

                var licenceList = new List<Licence>();
                if (devID > 0)
                {
                    //2.Get Device.
                    var device = deviceController.GetDeviceById(devID);
                    var deviceDetail = new DeviceDetail();
                    //var archivedDeviceDetails = new DeviceDetail();
                    deviceDetail.device = device;
                    deviceDetail.deviceIdentifierList = new List<DeviceIdentifier>();
                    var identifierList = new List<DeviceIdentifier>();

                    //3. Get DeviceIdentifier
                    if (licID > 0)
                    {
                        identifierList = identifierController.GetAllDeviceIdentifier(licencestatus, "", licID, device.device_id).ToList();
                    }
                    else
                    {
                        identifierList = identifierController.GetAllDeviceIdentifier(licencestatus, "", 0, device.device_id).ToList();
                    }

                    if (identifierList != null && identifierList.Count > 0)
                    {
                        if (string.IsNullOrWhiteSpace(identifier.Trim()))
                        {
                            deviceDetail.deviceIdentifierList = identifierList;
                        }
                        else
                        {
                            deviceDetail.deviceIdentifierList = identifierList.Where(s => s.device_identifier == identifier).ToList();
                        }
                    }

                    //4. Get Licence

                    var licenceDetail = new LicenceDetail();
                    var archivedLicenceDetail = new LicenceDetail();
                    if (licencestatus == "active")
                    {

                        if (licID > 0)
                        {
                            licenceDetail.licence = licenceController.GetLicenceById(licID, "active");
                        }
                        else
                        {
                            licenceDetail.licence = licenceController.GetLicenceById(device.original_licence_no, "active");
                        }
                        licenceDetail.deviceList = new List<DeviceDetail>();
                        licenceDetail.deviceList.Add(deviceDetail);
                        data.licenceList.Add(licenceDetail);
                    }
                    else
                    {
                        if (licID > 0)
                        {
                            archivedLicenceDetail.licence = licenceController.GetLicenceById(licID, "archived");
                        }
                        else
                        {
                            archivedLicenceDetail.licence = licenceController.GetLicenceById(device.original_licence_no, "archived");
                        }

                        archivedLicenceDetail.deviceList = new List<DeviceDetail>();
                        archivedLicenceDetail.deviceList.Add(deviceDetail);

                        //5. Add all the list to Company(data).

                        data.licenceList.Add(archivedLicenceDetail);
                    }
                }
                else
                {
                    if (licencestatus == "active")
                    {
                        if (licID == 0)
                        {
                            licenceList = licenceController.GetAllLicenceByCompanyId(company.company_id, "active").ToList();
                        }
                        else
                        {
                            licenceList.Add(licenceController.GetLicenceById(licID, "active"));
                        }

                    }

                    else
                    {
                        if (licID == 0)
                        {
                            licenceList = licenceController.GetAllLicenceByCompanyId(company.company_id, "archived").ToList();
                        }
                        else
                        {
                            licenceList.Add(licenceController.GetLicenceById(licID, "archived"));
                        }
                    }
                    if (licenceList != null && licenceList.Count > 0)
                    {
                        //2. Get Licence
                        foreach (var licence in licenceList)
                        {
                            var licenceDetail = new LicenceDetail();
                            licenceDetail.licence = licence;
                            licenceDetail.deviceList = new List<DeviceDetail>();


                            //3. Get Device
                            //var deviceList = deviceController.GetAllDevice("", "", licence.original_licence_no).ToList();
                            var deviceList = new List<Device>();
                            deviceList = deviceController.GetAllDevice(licencestatus, "", licence.original_licence_no).ToList();

                            if (deviceList != null && deviceList.Count > 0)
                            {
                                foreach (var device in deviceList)
                                {
                                    var deviceDetail = new DeviceDetail();
                                    deviceDetail.device = device;
                                    deviceDetail.deviceIdentifierList = new List<DeviceIdentifier>();
                                    var identifierList = new List<DeviceIdentifier>();
                                    //4. Get DeviceIdentifier.
                                    //if (licID > 0)
                                    //{
                                    identifierList = identifierController.GetAllDeviceIdentifier(licencestatus, "", licID, device.device_id).ToList();
                                    //}
                                    //else
                                    //{
                                    //    identifierList = identifierController.GetAllDeviceIdentifier(licencestatus, "", 0, device.device_id).ToList();
                                    //}

                                    if (identifierList != null && identifierList.Count > 0)
                                    {
                                        deviceDetail.deviceIdentifierList = identifierList;
                                    }
                                    licenceDetail.deviceList.Add(deviceDetail);
                                }
                            }
                            //5. Add all the list to Company(data)
                            data.licenceList.Add(licenceDetail);
                        }
                    }
                }
            }

            var jsonResult = Json(new { data }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }



        public ActionResult DownloadCSV(string dataType)
        {

            DBConnection dbConnection = new DBConnection("en");

            var jsonResult = string.Empty;
            var fileNameDate = string.Format("{0}{1}{2}",
                           DateTime.Now.Year.ToString(),
                           DateTime.Now.Month.ToString().PadLeft(2, '0'),
                           DateTime.Now.Day.ToString().PadLeft(2, '0'));

            var fileName = string.Empty;
            fileName = string.Format(dataType + "_{0}.csv", fileNameDate);

            byte[] outputBuffer = null;

            switch (dataType)
            {

                case ("licence"):
                    var licenses = dbConnection.GetAllLicence("active", string.Empty).ToList();

                    if (licenses.Count > 0)
                    {
                        var json = JsonConvert.SerializeObject(licenses);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);

                            if (dt.Rows.Count > 0)
                            {

                                using (MemoryStream tempStream = new MemoryStream())
                                {
                                    using (StreamWriter writer = new StreamWriter(tempStream))
                                    {
                                        UtilityHelper.WriteDataTable(dt, writer, true);
                                    }

                                    outputBuffer = tempStream.ToArray();
                                }
                            }
                        }
                    }

                    return File(outputBuffer, "text/csv", fileName);


                case ("company"):
                    var companies = dbConnection.GetAllCompany("active", string.Empty).ToList();

                    if (companies.Count > 0)
                    {
                        var json = JsonConvert.SerializeObject(companies);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);

                            if (dt.Rows.Count > 0)
                            {

                                using (MemoryStream tempStream = new MemoryStream())
                                {
                                    using (StreamWriter writer = new StreamWriter(tempStream))
                                    {
                                        UtilityHelper.WriteDataTable(dt, writer, true);
                                    }

                                    outputBuffer = tempStream.ToArray();
                                }
                            }
                        }
                    }

                    return File(outputBuffer, "text/csv", fileName);

            case ("device"):
                    var devices = dbConnection.GetAllDevice("active", string.Empty, 0).ToList();

                if (devices.Count > 0)
                {
                    var json = JsonConvert.SerializeObject(devices);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);

                        if (dt.Rows.Count > 0)
                        {

                            using (MemoryStream tempStream = new MemoryStream())
                            {
                                using (StreamWriter writer = new StreamWriter(tempStream))
                                {
                                    UtilityHelper.WriteDataTable(dt, writer, true);
                                }

                                outputBuffer = tempStream.ToArray();
                            }
                        }
                    }
                }

                return File(outputBuffer, "text/csv", fileName);

                case ("identifier"):
                    var identifiers = dbConnection.GetAllDeviceIdentifier("active", string.Empty, 0, 0).ToList();

                    if (identifiers.Count > 0)
                    {
                        var json = JsonConvert.SerializeObject(identifiers);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);

                            if (dt.Rows.Count > 0)
                            {

                                using (MemoryStream tempStream = new MemoryStream())
                                {
                                    using (StreamWriter writer = new StreamWriter(tempStream))
                                    {
                                        UtilityHelper.WriteDataTable(dt, writer, true);
                                    }

                                    outputBuffer = tempStream.ToArray();
                                }
                            }
                        }
                    }

                    return File(outputBuffer, "text/csv", fileName);


                case ("licenceType"):
                    var licenceTypes = dbConnection.GetAllLicenceType("en").ToList();

                    if (licenceTypes.Count > 0)
                    {
                        var json = JsonConvert.SerializeObject(licenceTypes);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);

                            if (dt.Rows.Count > 0)
                            {

                                using (MemoryStream tempStream = new MemoryStream())
                                {
                                    using (StreamWriter writer = new StreamWriter(tempStream))
                                    {
                                        UtilityHelper.WriteDataTable(dt, writer, true);
                                    }

                                    outputBuffer = tempStream.ToArray();
                                }
                            }
                        }
                    }

                    return File(outputBuffer, "text/csv", fileName);

                case ("sbdLocation"):
                    var sbdLocations = dbConnection.GetAllSbdLocation().ToList();

                    if (sbdLocations.Count > 0)
                    {
                        var json = JsonConvert.SerializeObject(sbdLocations);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);

                            if (dt.Rows.Count > 0)
                            {

                                using (MemoryStream tempStream = new MemoryStream())
                                {
                                    using (StreamWriter writer = new StreamWriter(tempStream))
                                    {
                                        UtilityHelper.WriteDataTable(dt, writer, true);
                                    }

                                    outputBuffer = tempStream.ToArray();
                                }
                            }
                        }
                    }

                    return File(outputBuffer, "text/csv", fileName);

            }

            return File(outputBuffer, "text/csv", fileName);

        }

    }
}
