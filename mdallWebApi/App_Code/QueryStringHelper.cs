using System.Collections.Specialized;
using System.Linq;



namespace MdallWebApi
{
    /// <summary>
    /// Summary description for Common
    /// </summary>
    public static class QueryStringHelper
    {
        #region queryString
  
        public static string GetLang(this NameValueCollection queryString)
        {
            return queryString.AllKeys.Contains("lang") ? queryString["lang"] : string.Empty;
        }       
        public static string GetStatus(this NameValueCollection queryString)
        {
            return queryString.AllKeys.Contains("status") ? queryString["status"] : string.Empty;
        }
        public static string GetSearchTerm(this NameValueCollection queryString)
        {
            return queryString.AllKeys.Contains("term") ? queryString["term"] : string.Empty;
        }
      
        public static string GetLicenceID(this NameValueCollection queryString)
        {
            return queryString.AllKeys.Contains("licenceId") ? queryString["licenceId"] : string.Empty;
        }
        public static string GetCompanyId(this NameValueCollection queryString)
        {
            return queryString.AllKeys.Contains("id") ? queryString["id"] : string.Empty;
        }
       
        #endregion

    }

}


