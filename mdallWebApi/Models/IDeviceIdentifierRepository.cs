using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MdallWebApi.Models
{
    interface IDeviceIdentifierRepository
    {
        IEnumerable<DeviceIdentifier> GetAll(string status,string deviceIdentifierName, int licenceId, int deviceId);
        DeviceIdentifier Get(int id);
    }
}