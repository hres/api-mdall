using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using MdallWebApi.Models;
using MdallWebApi.Controllers;

namespace MdallWebApi
{
    public static class UtilityHelper
    {
        public static void SetDefaultCulture(string lang)
        {
            if (lang == "en")
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-CA");
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en-CA");
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("fr-FR");
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("fr-FR");
            }
        }

        //public static string GetCompanyListForJson(string lang, string status, string term)
        //{
        //    var companyController = new CompanyController();
        //    var companyList = new List<Company>();
        //    var jsonResult = string.Empty;

        //    try
        //    {
        //        companyList = companyController.GetAllCompany(status, term).ToList();
        //        if(companyList!= null && companyList.Count > 0)
        //        {
        //            jsonResult = JsonHelper.JsonSerializer<List<Company>>(companyList);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var errorMessages = string.Format("UtilityHelper - GetCompanyList()- Error Message:{0}", ex.Message);
        //        ExceptionHelper.LogException(ex, errorMessages);
        //    }
        //    finally
        //    {

        //    }
        //    return jsonResult;
        //}


        //public static string GetCompanyByIDForJson(string lang, int id)
        //{
        //    var companyController = new CompanyController();
        //    var company = new Company();
        //    var jsonResult = string.Empty;

        //    try
        //    {
        //        company = companyController.GetCompanyById(id, lang);
        //        if (company != null && company.company_id > 0)
        //        {
        //            jsonResult = JsonHelper.JsonSerializer<Company>(company);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var errorMessages = string.Format("UtilityHelper - GetCompanyByIDForJson()- Error Message:{0}", ex.Message);
        //        ExceptionHelper.LogException(ex, errorMessages);
        //    }
        //    finally
        //    {

        //    }
        //    return jsonResult;
        //}


        public static List<T> ToNonNullList<T>(this IEnumerable<T> obj)
        {
            return obj == null ? new List<T>() : obj.ToList();
        }

    }
}