using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.Filters;
using Autofac;
using Autofac.Integration.WebApi;
using Flex.Entity.Api;
using Flex.Entity.Api.App_Start;
using Flex.Entity.Api.Controllers;
using Flex.Entity.JWTSecurity.Configuration.Tests;
using Moq;

namespace Flex.Entity.JWTSecurity.Configuration.Tests
{
       public class ArticlesReversedFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var objectContent = actionExecutedContext.Response.Content as ObjectContent;
            if (objectContent != null)
            {
                //List<Article> _articles = objectContent.Value as List<Article>;
                //if (_articles != null && _articles.Count > 0)
                //{
                //    _articles.Reverse();
                //}
            }
        }
    }
    public class CustomAssembliesResolver : DefaultAssembliesResolver
    {
        public override ICollection<Assembly> GetAssemblies()
        {
            var baseAssemblies = base.GetAssemblies().ToList();
            var assemblies = new List<Assembly>(baseAssemblies) { typeof(EntityTypesController).Assembly };
            baseAssemblies.AddRange(assemblies);

            return baseAssemblies.Distinct().ToList();
        }
    }
    public class EndRequestHandler : DelegatingHandler
    {
        async protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri.AbsoluteUri.Contains("test"))
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("Unit testing message handlers!")
                };

                var tsc = new TaskCompletionSource<HttpResponseMessage>();
                tsc.SetResult(response);
                return await tsc.Task;
            }
            else
            {
                return await base.SendAsync(request, cancellationToken);
            }
        }
    }
    public class HeaderAppenderHandler : DelegatingHandler
    {
        async protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            response.Headers.Add("X-WebAPI-Header", "Web API Unit testing in chsakell's blog.");
            return response;
        }
    }

    public class Startup
    {
        public void Configuration()
        {
            GlobalConfiguration.Configure(confignew =>
            {

                HandlerConfig.Register(confignew);
                WebApiFilterConfig.Register(confignew);
                JsonWebTokenConfig.Register(confignew);
                WebApiConfig.Register(confignew               );
            });
            var config = new HttpConfiguration();
            config.MessageHandlers.Add(new HeaderAppenderHandler());
            config.MessageHandlers.Add(new EndRequestHandler());
            config.Filters.Add(new ArticlesReversedFilter());
            config.Services.Replace(typeof(IAssembliesResolver), new CustomAssembliesResolver());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new {id = RouteParameter.Optional}
            );
            config.MapHttpAttributeRoutes();

            // Autofac configuration
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(typeof(EntityTypesController).Assembly);



            ILifetimeScope container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            //  appBuilder.UseWebApi(config);
        }
    }
}         