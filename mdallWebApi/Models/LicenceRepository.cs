using System.Collections.Generic;

namespace MdallWebApi.Models
{
    public class LicenceRepository : ILicenceRepository
    {
        // We are using the list and _fakeDatabaseID to represent what would
        // most likely be a database of some sort, with an auto-incrementing ID field:
        private List<Licence> _licenses = new List<Licence>();
        private Licence _licence = new Licence();
        DBConnection dbConnection = new DBConnection("en");

        public IEnumerable<Licence> GetAll(string status = "", string licenceName = "")
        {
            _licenses = dbConnection.GetAllLicence(status, licenceName);
            return _licenses;
        }

        public IEnumerable<Licence> GetAllLicenceByCompanyId(int company_id, string status = "")
        {
            _licenses = dbConnection.GetAllLicenceByCompanyId(company_id, status);
            return _licenses;
        }

        public Licence Get(int id, string status = "")
        {
            _licence = dbConnection.GetLicenceById(id, status);

            return _licence;
        }
        
    }
}