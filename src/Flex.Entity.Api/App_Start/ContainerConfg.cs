using System.ComponentModel.Composition;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Web.Http;
using System.Web;
using Autofac;
using Autofac.Integration.WebApi;
using Flex.Entity.Api.CustomFilters.Authorization;
using Flex.Entity.JWTSecurity;
using Flex.Entity.Repository;
using Flex.Entity.Security;
using Flex.Logging.NLog;
using Microsoft.ApplicationInsights.Extensibility.Implementation;

namespace Flex.Entity.Api
{
    /// <summary>
    /// Does all IOC Registrations
    /// </summary>
    public static class ContainerConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ILifetimeScope Register(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();
            
            RegisterTypesWithContainer(builder,config);

            /////////////////////////////////////////////////////////////////////////////////
            //Dont do any autofac component registrations below this line, all reg should go in 
            //above method.
            // i.e in the method RegisterTypesWithContainer which is defined below in this file
            //
            /////////////////////////////////////////////////////////////////////////////////

            //This is init code

            ILifetimeScope container = builder.Build();           
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);//Autofac WebAPI 2 resolver

            using (var scope = container.BeginLifetimeScope())
            {
                var repoInitializer = scope.Resolve<IRepositoryInitializer>();
                object options = null;
                config.Properties.TryGetValue("dapper", out options);
                repoInitializer.Initialize(options);
            }
            return container;
        }

        private static void RegisterTypesWithContainer(ContainerBuilder builder, HttpConfiguration config)
        {
            builder.RegisterType<NullRepositoryInitializer>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<SecurityAuthentication>().AsImplementedInterfaces().InstancePerRequest().PropertiesAutowired();
            builder.RegisterType<JsonWebTokenAuthenticationAttribute>().InstancePerRequest().PropertiesAutowired();
            //builder.RegisterType<SecurityContextProvider>().AsImplementedInterfaces().InstancePerRequest().PropertiesAutowired();
            builder.Register(c => HttpContext.Current.User).As<IPrincipal>().InstancePerRequest();
            builder.Register<ISecurityContextProvider>(c => new SecurityContextProvider(c.Resolve<IPrincipal>())).InstancePerRequest();
            builder.RegisterModule<NLogAppModule>();
            //builder.RegisterModule(new AutofacWebTypesModule());

            // OPTIONAL: Register the Autofac filter provider.
            builder.RegisterWebApiFilterProvider(config);

           
            AutofacModuleLoader.Loader(builder);

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly()).PropertiesAutowired();
          
            builder.RegisterWebApiFilterProvider(config);
           
        }
    }
}
