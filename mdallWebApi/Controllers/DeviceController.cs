using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MdallWebApi.Models;

namespace MdallWebApi.Controllers
{
    public class DeviceController : ApiController
    {
        static readonly IDeviceRepository databasePlaceholder = new DeviceRepository();

        public IEnumerable<Device> GetAllDevice(string state = "", string device_name = "", int licence_id = 0)
        {

            return databasePlaceholder.GetAll(state, device_name, licence_id);
        }


        public Device GetDeviceById(int id)
        {
            Device device = databasePlaceholder.Get(id);
            if (device == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return device;
        }

        public IEnumerable<ViewLicenceDevice> GetAllLicenceDeviceByLicenceId(string licenceId, string lang)
        {
            return null; //databasePlaceholder.GetAllLicenceDeviceByLicenceId(licenceId, lang);
        }
    }
}