using System.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Flex.Entity.Api.App_Start;
using Microsoft.ApplicationInsights.Extensibility;

namespace Flex.Entity.Api
{
    /// <summary>
    /// WebAPI Application startup
    /// </summary>
    public class WebApiApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// 
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(config =>
            {
                //needed to initialize dapper to support sql types, without having to reference SQLtypes in api project
                if (!config.Properties.ContainsKey("dapper"))
                {
                    config.Properties.GetOrAdd("dapper", o => Server.MapPath("~/bin"));
                }
                HandlerConfig.Register(config);
                WebApiFilterConfig.Register(config);
                JsonWebTokenConfig.Register(config);
                WebApiConfig.Register(config);
                ContainerConfig.Register(config);
            });
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ReadApplicationInsightsKey();
        }

        private void ReadApplicationInsightsKey()
        {
            var instrumentationKey = ConfigurationManager.AppSettings["ApplicationInsightsInstrumentationKey"];

            if (string.IsNullOrWhiteSpace(instrumentationKey))
            {
                throw new ConfigurationErrorsException("Missing app setting 'ApplicationInsightsInstrumentationKey' used for Application Insights");
            }

            TelemetryConfiguration.Active.InstrumentationKey = instrumentationKey;

        }
    }

}
