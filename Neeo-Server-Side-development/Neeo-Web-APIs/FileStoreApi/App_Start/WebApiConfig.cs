using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace FileStoreApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Web API routes
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{uID}/{gID}/{timeStamp}/{requiredDimension}",
            //    defaults: new { uID = RouteParameter.Optional,
            //                    timeStamp = RouteParameter.Optional,
            //                    requiredDimension = RouteParameter.Optional,
            //                    gID = RouteParameter.Optional

                
            //    }
            //);
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "v1/{controller}"
            );
        }
    }
}
