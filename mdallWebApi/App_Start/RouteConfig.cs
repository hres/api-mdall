﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MdallWebApi
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapPageRoute("HtmlRoute", "detailsURL", "~/medical-device-detail.html");

            routes.MapRoute(
                name: "Root",
                url: "{action}",
                defaults: new { controller = "Home", action = "IndexEN" });
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "IndexEN", id = UrlParameter.Optional });
        }
    }
}