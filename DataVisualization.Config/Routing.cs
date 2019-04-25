using System.Web.Http;
using System.Web.Routing;

namespace DataVisualization.Config
{
    public static class Routing
    {
        // Precondition 
        public static void RegisterRoutes()
        {

            RouteTable.Routes.MapHttpRoute(
                name: "ChartsRange",
                routeTemplate: "api/{controller}/{action}/{outputName}",
                defaults: new
                {
                    outputName = RouteParameter.Optional
                });

            RouteTable.Routes.MapHttpRoute(
                name: "ChartsApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new
                {
                    id = RouteParameter.Optional
                }
            );
        }
    }
}
