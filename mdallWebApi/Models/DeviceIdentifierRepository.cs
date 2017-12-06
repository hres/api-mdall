using System.Collections.Generic;

namespace MdallWebApi.Models
{
    public class DeviceIdentifierRepository : IDeviceIdentifierRepository
    {
        private List<DeviceIdentifier> _licenceDeviceIdentifiers = new List<DeviceIdentifier>();
        private DeviceIdentifier _licenceDeviceIdentifier = new DeviceIdentifier();
        DBConnection dbConnection = new DBConnection("en");

        public IEnumerable<DeviceIdentifier> GetAll(string status, string deviceIdentifierName, int licenceId, int deviceId)
        {
            _licenceDeviceIdentifiers = dbConnection.GetAllDeviceIdentifier(status, deviceIdentifierName, licenceId, deviceId);
            return _licenceDeviceIdentifiers;
        }

        public IEnumerable<DeviceIdentifier> Get(int id)
        {
            _licenceDeviceIdentifiers = dbConnection.GetDeviceIdentifierById(id);
            return _licenceDeviceIdentifiers;
        }
    }
}