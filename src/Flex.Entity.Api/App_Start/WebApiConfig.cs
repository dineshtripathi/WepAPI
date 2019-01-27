using System.Web.Http;
using WebApi.Hal;

namespace Flex.Entity.Api
{
    /// <summary>
    /// Webapi configuration
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.EnableCors();
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute("DefaultApi",
                    "{controller}/{id}",
                    defaults: new { id = RouteParameter.Optional }
                    );

            config.Formatters.Add(new JsonHalMediaTypeFormatter());
            config.Formatters.Add(new XmlHalMediaTypeFormatter());

            //config.Routes.MapHttpRoute("entitytypes",
            //    "entities/types/{prefix}",
            //    defaults: new {controller = "EntityTypes", prefix = RouteParameter.Optional }//, 
            //    //constraints: new {prefix = @"\[a-zA-Z]{1,2}"}
            //    );

            //config.Routes.MapHttpRoute("entitytypesdelete",
            //    "entities/types/{entity_type_prefix}",
            //    defaults: new { controller = "EntityTypes",method = "HttpDelete" }//, 
            //    //constraints: new {prefix = @"\[a-zA-Z]{1,2}"}
            //    );


        }
    }
}