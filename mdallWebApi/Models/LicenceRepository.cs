using System.Collections.Generic;

namespace MdallWebApi.Models
{
    public class LicenceRepository : ILicenceRepository
    {
        // We are using the list and _fakeDatabaseID to represent what would
        // most likely be a database of some sort, with an auto-incrementing ID field:
        private List<Licence> _licenses = new List<Licence>();
        private Licence _licence = new Licence();
        private SbdLocation location = new SbdLocation();
        DBConnection dbConnection = new DBConnection("en");

        public IEnumerable<Licence> GetAll(string state = "", string licenceName = "", string lang = "")
        {
            _licenses = dbConnection.GetAllLicence(state, licenceName, lang);
            return _licenses;
        }

        public IEnumerable<Licence> GetAllLicenceByCompanyId(int company_id, string state = "")
        {
            _licenses = dbConnection.GetAllLicenceByCompanyId(company_id, state);
            return _licenses;
        }

        public Licence Get(int id, string state = "", string lang = "")

        {
            _licence = dbConnection.GetLicenceById(id, state, lang);


            return _licence;
        }

    }
}