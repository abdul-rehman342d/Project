using System.Web.Http;
using System.Web.Routing;
using PowerfulPal.Neeo.DashboardAPI.Filter;
using PowerfulPal.Neeo.DashboardAPI.Session;

namespace PowerfulPal.Neeo.DashboardAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            

            config.EnableCors();
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute("DefaultApi", "{controller}/{action}/{id}", new { id = RouteParameter.Optional }
                );
            RouteTable.Routes.MapHttpRoute(
            name: "SessionApi",
            routeTemplate: "api/v2/{controller}").RouteHandler = new SessionEnabledControllerRouteHandler();
            //config.Routes.MapHttpRoute("DefaultApi", "{controller}"
            //   );
        }
    }
}