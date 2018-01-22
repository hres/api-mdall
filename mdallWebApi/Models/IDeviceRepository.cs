using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MdallWebApi.Models
{
    interface IDeviceRepository
    {
        IEnumerable<Device> GetAll(string state, string deviceName, int licenceId);
        Device Get(int id);
        //IEnumerable<ViewLicenceDevice> GetAllLicenceDeviceByLicenceId(string licenceId, string lang);
    }
}
