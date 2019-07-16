using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SyncApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.MessageHandlers.Insert(0, new DecompressionHandler()); // first runs last
            config.MessageHandlers.Insert(1, new CompressionHandler());
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
