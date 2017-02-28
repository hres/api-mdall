using System.Collections.Generic;

namespace MdallWebApi.Models
{
    public class LicenceTypeRepository : ILicenceTypeRepository
    {
        private List<LicenceType> _licenceTypes = new List<LicenceType>();
        private LicenceType _licenceType = new LicenceType();
        DBConnection dbConnection = new DBConnection("en");
        
        public IEnumerable<LicenceType> GetAll(string lang = "")
        {
            _licenceTypes = dbConnection.GetAllLicenceType(lang);
            return _licenceTypes;
        }

        public LicenceType Get(string code, string lang = "")
        {
            DBConnection dbConnection = new DBConnection(lang);
            _licenceType = dbConnection.GetLicenceTypeByCode(code);
            return _licenceType;
        }
    }
}