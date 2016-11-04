using System.Collections.Generic;

namespace MdallWebApi.Models
{
    public class SbdLocationRepository : ISbdLocationRepository
    {
        private List<SbdLocation> _sbdLocations = new List<SbdLocation>();
        private SbdLocation _sbdLocation = new SbdLocation();
        DBConnection dbConnection = new DBConnection("en");

        public IEnumerable<SbdLocation> GetAll()
        {
            _sbdLocations = dbConnection.GetAllSbdLocation();
            return _sbdLocations;
        }

        public SbdLocation Get(int id)
        {
            _sbdLocation = dbConnection.GetSbdLocationById(id);
            return _sbdLocation;
        }
    }
}