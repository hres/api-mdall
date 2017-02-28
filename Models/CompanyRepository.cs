using System.Collections.Generic;

namespace MdallWebApi.Models
{
    public class CompanyRepository : ICompanyRepository
    {
        // We are using the list and _fakeDatabaseID to represent what would
        // most likely be a database of some sort, with an auto-incrementing ID field:
        private List<Company> companies = new List<Company>();
        private Company company = new Company();

        DBConnection dbConnection = new DBConnection("en");


        public IEnumerable<Company> GetAll(string status = "", string company_name = "")
        {
            companies = dbConnection.GetAllCompany(status, company_name);

            return companies;
        }


        public Company Get( int id, string lang, string status = "")
        {
            DBConnection dbConnection = new DBConnection(lang);
            company = dbConnection.GetCompanyById(status, id);
            //company.licenceList = new List<Licence>();

            //if (company != null && company.company_id > 0)
            //{
            //    var licenceList = dbConnection.GetAllLicenceByCompanyId(company.company_id, "active");
            //    if (licenceList != null && licenceList.Count > 0)
            //    {
            //       company.licenceList = licenceList;

            //        foreach (var licence in company.licenceList)
            //        {
            //            licence.deviceList = new List<Device>();
            //            //Get Device
            //            var deviceList = dbConnection.GetAllDevice("", "", licence.original_licence_no);
            //            if (deviceList != null && deviceList.Count > 0)
            //            {
            //                licence.deviceList = deviceList;
            //            }
            //        }
            //    }
            //}
            return company;
        }
    }
}