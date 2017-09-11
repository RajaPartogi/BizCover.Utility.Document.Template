using System.Reflection;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using BizCover.Common.Infrastructure.Services;
using NLog;

namespace BizCover.Utility.Document.Template
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Services.Replace(typeof(IExceptionLogger), new NLogExceptionLogger(LogManager.GetLogger(Assembly.GetExecutingAssembly().GetName().Name)));

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
