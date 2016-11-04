using System.Collections.Generic;

namespace MdallWebApi.Models
{
    public class DeviceRepository : IDeviceRepository
    {
        private List<Device> _devices = new List<Device>();
        private Device _device = new Device();
        private List<ViewLicenceDevice> _viewLicenceDevices = new List<ViewLicenceDevice>();

        DBConnection dbConnection = new DBConnection("en");

        public IEnumerable<Device> GetAll(string status, string deviceName, int licenceId)
        {
            _devices = dbConnection.GetAllDevice(status, deviceName, licenceId);
            return _devices;
        }

        public Device Get(int id)
        {
            _device = dbConnection.GetDeviceById(id);
            return _device;
        }

        public IEnumerable<ViewLicenceDevice> GetAllLicenceDeviceByLicenceId(string licenceId, string lang)
        {
            _viewLicenceDevices = dbConnection.GetAllLicenceDeviceByLicenceId(licenceId, lang);
            return _viewLicenceDevices;
        }
    }
}