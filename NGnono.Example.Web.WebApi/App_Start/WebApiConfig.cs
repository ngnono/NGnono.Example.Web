﻿using System.Web.Http;
using System.Web.Http.Dispatcher;
using NGnono.Example.Web.WebApi.Support.Filters;

namespace NGnono.Example.Web.WebApi.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();

            // To disable tracing in your application, please comment out or remove the following line of code
            // For more information, refer to: http://www.asp.net/web-api
            //config.EnableSystemDiagnosticsTracing();


            //config.Services.Replace(typeof(IHttpControllerSelector), new VersionControllerSelector(config));
            config.MessageHandlers.Add(new UserProfileMessageHandler());

            config.EnsureInitialized();
        }
    }
}
