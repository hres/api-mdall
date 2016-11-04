<%@ WebHandler Language="C#" Class="mdall.MdallHandler" %>

using System;
using System.Web;
using MdallWebApi;

namespace mdall
{
    public class MdallHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            try
            {
                var jsonResult = string.Empty;
                var lang = string.IsNullOrEmpty(context.Request.QueryString.GetLang().Trim()) ? "en" : context.Request.QueryString.GetLang().Trim();
                if (lang == "en")
                {
                    UtilityHelper.SetDefaultCulture("en");
                }
                else
                {
                    UtilityHelper.SetDefaultCulture("fr");
                }

                //Get All the QueryStrings
                var term  = context.Request.QueryString.GetSearchTerm().ToLower().Trim();
                var status  = context.Request.QueryString.GetStatus().ToLower().Trim();
                var companyId = string.IsNullOrWhiteSpace(context.Request.QueryString.GetCompanyId().Trim())? 0: Convert.ToInt32(context.Request.QueryString.GetCompanyId().Trim());

                if ( companyId > 0)
                {
                    jsonResult = UtilityHelper.GetCompanyByIDForJson(lang, companyId);
                    if( string.IsNullOrWhiteSpace(jsonResult))
                    {
                        context.Response.Write("{\"company_id\":\"\"}");
                    }
                    else
                    {
                        jsonResult = jsonResult.Replace("licenceList", "data");
                        context.Response.Write(jsonResult);
                    }
                }
                else
                {
                    jsonResult = UtilityHelper.GetCompanyListForJson(lang, status, term);
                    if( string.IsNullOrWhiteSpace(jsonResult))
                    {
                        context.Response.Write("{\"data\":[]}");
                    }
                    else
                    {
                        jsonResult = "{\"data\":" + jsonResult + "}";
                        context.Response.Write(jsonResult);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.LogException(ex, "mdallController.ashx");
                context.Response.Write("{\"data\":[]}");
            }
        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}