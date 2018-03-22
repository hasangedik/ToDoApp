using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Newtonsoft.Json;
using ToDoApp.WebApi.Helper;
using ToDoApp.WebApi.Utility;

namespace ToDoApp.WebApi
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ConfigureFormatter(GlobalConfiguration.Configuration);
            ContractMapping.MappingRegistration();
        }

        private void ConfigureFormatter(HttpConfiguration configuration)
        {
            var formatters = configuration.Formatters;
            formatters.Remove(formatters.XmlFormatter);

            var jsonFormatter = configuration.Formatters.JsonFormatter;
            jsonFormatter.SerializerSettings = new JsonSerializerSettings()
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };
        }
    }
}
