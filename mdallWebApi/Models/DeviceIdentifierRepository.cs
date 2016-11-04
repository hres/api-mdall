using System.Collections.Generic;

namespace MdallWebApi.Models
{
    public class DeviceIdentifierRepository : IDeviceIdentifierRepository
    {
        private List<DeviceIdentifier> _licenceDeviceIdentifiers = new List<DeviceIdentifier>();
        private DeviceIdentifier _licenceDeviceIdentifier = new DeviceIdentifier();
        DBConnection dbConnection = new DBConnection("en");

        public IEnumerable<DeviceIdentifier> GetAll(string deviceIdentifierName, int licenceId, int deviceId)
        {
            _licenceDeviceIdentifiers = dbConnection.GetAllDeviceIdentifier(deviceIdentifierName, licenceId, deviceId);
            return _licenceDeviceIdentifiers;
        }

        public DeviceIdentifier Get(int id)
        {
            _licenceDeviceIdentifier = dbConnection.GetDeviceIdentifierById(id);
            return _licenceDeviceIdentifier;
        }
    }
}