using System.Collections.Generic;

namespace MdallWebApi.Models
{
    public class LicenceCompanyRepository : ILicenceCompanyRepository
    {
        // We are using the list and _fakeDatabaseID to represent what would
    // most likely be a database of some sort, with an auto-incrementing ID field:
        private List<LicenceCompany> _licencecompanies = new List<LicenceCompany>();
        private LicenceCompany _licencecompany = new LicenceCompany();
        DBConnection dbConnection = new DBConnection("en");
        
        public IEnumerable<LicenceCompany> GetAll(string lang, string state = "", string licence_name = "", long licence_id = 0, string company_name = "", int company_id = 0)
        {
            _licencecompanies = dbConnection.GetLicenceCompanyByCriteria(lang, state, licence_name, licence_id, company_name, company_id);

            return _licencecompanies;
        }

        public LicenceCompany Get(string lang, long licence_id, string state = "")
        {
            _licencecompany = dbConnection.GetLicenceCompanyById(lang, licence_id, state);
            return _licencecompany;
        }

    }
}