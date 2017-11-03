using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.IO;
using System.Net.Http.Headers;
using System.Text;

namespace MdallWebApi.Controllers
{
    public class CSVController : ApiController
    {
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public HttpResponseMessage DownloadCSV(string dataType, string lang)
        {
            DBConnection dbConnection = new DBConnection(lang);
            var jsonResult = string.Empty;
            var fileNameDate = string.Format("{0}{1}{2}",
                           DateTime.Now.Year.ToString(),
                           DateTime.Now.Month.ToString().PadLeft(2, '0'),
                           DateTime.Now.Day.ToString().PadLeft(2, '0'));
            var fileName = string.Format(dataType + "_{0}.csv", fileNameDate);
            byte[] outputBuffer = null;
            string resultString = string.Empty;
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var json = string.Empty;

            switch (dataType)
            {
                case "licence":
                    var licences = dbConnection.GetAllLicence("active", "").ToList();
                    if (licences.Count > 0)
                    {
                        json = JsonConvert.SerializeObject(licences);
                    }
                    break;
                case "company":
                    var companies = dbConnection.GetAllCompany("active", "").ToList();
                    if (companies.Count > 0)
                    {   
                        json = JsonConvert.SerializeObject(companies);
                    }
                    break;

                case "device":
                    var devices = dbConnection.GetAllDevice("active", "", 0).ToList();
                    if (devices.Count > 0)
                    {
                        json = JsonConvert.SerializeObject(devices);
                    }
                    break;

                case "identifer":
                    var identifiers = dbConnection.GetAllDeviceIdentifier("active", "", 0, 0).ToList();
                    if (identifiers.Count > 0)
                    {
                        json = JsonConvert.SerializeObject(identifiers);
                    }
                    break;

                case "licenceType":
                    var types = dbConnection.GetAllLicenceType(lang).ToList();
                    if (types.Count > 0)
                    {
                        json = JsonConvert.SerializeObject(types);
                    }
                    break;

                case "sbdLocation":
                    var sbdLocations = dbConnection.GetAllSbdLocation().ToList();
                    if (sbdLocations.Count > 0)
                    {
                        json = JsonConvert.SerializeObject(sbdLocations);
                    }
                    break;


                case "archLicence":
                    var archLicence = dbConnection.GetAllLicence("archived", "").ToList();
                    if (archLicence.Count > 0)
                    {
                        json = JsonConvert.SerializeObject(archLicence);
                    }
                    break;

                case "archDevice":
                    var archDevice = dbConnection.GetAllDevice("archived", "", 0).ToList();
                    if (archDevice.Count > 0)
                    {
                        json = JsonConvert.SerializeObject(archDevice);
                    }
                    break;

                case "archIdentifier":
                    var archIdentifiers = dbConnection.GetAllDeviceIdentifier("archived", "", 0, 0).ToList();
                    if (archIdentifiers.Count > 0)
                    {
                        json = JsonConvert.SerializeObject(archIdentifiers);
                    }
                    break;

            }

            if (!string.IsNullOrWhiteSpace(json))
            {
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
                if (dt.Rows.Count > 0)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            UtilityHelper.WriteDataTable(dt, writer, true);
                            outputBuffer = stream.ToArray();
                            resultString = Encoding.UTF8.GetString(outputBuffer, 0, outputBuffer.Length);
                        }
                    }
                }
            }
            result.Content = new StringContent(resultString);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = fileName };
            return result;

        }
    }
}
