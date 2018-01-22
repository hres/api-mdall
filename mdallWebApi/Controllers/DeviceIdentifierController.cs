using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MdallWebApi.Models;
namespace MdallWebApi.Controllers
{
    public class DeviceIdentifierController : ApiController
    {
        static readonly IDeviceIdentifierRepository databasePlaceholder = new DeviceIdentifierRepository();

        public IEnumerable<DeviceIdentifier> GetAllDeviceIdentifier(string state="", string device_identifier = "", int licence_id = 0, int device_id = 0)
        {
            return databasePlaceholder.GetAll(state, device_identifier, licence_id, device_id);
        }
     
        public IEnumerable<DeviceIdentifier> GetDeviceIdentifierByID(int id)
        {
            return databasePlaceholder.Get(id);
        }
    }
}