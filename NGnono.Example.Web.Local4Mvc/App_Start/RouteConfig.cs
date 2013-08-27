using System.Web.Mvc;
using System.Web.Routing;

namespace NGnono.Example.Web.Local4Mvc
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
"LogOn", // Route name
    "Account/{action}", // URL with parameters
    new { controller = "Account", action = "LogOn" } // Parameter defaults
    );

            routes.MapRoute(
               "Localization", // 路由名称
               "{lang}/{controller}/{action}/{id}", // 带有参数的 URL
               new { controller = "Home", action = "Index", id = UrlParameter.Optional }//参数默认值
               );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}