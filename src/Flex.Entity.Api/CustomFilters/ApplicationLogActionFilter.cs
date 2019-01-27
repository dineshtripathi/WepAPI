using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Autofac.Integration.WebApi;

namespace Flex.Entity.Api.CustomFilters
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationLogActionFilter : IAutofacActionFilter
    {
        /// <summary>
        /// 
        /// </summary>
        // [ApplicationInsightsLogger("ApplicationInsights")]
        //public ILogger Logger { get; set; }

        public Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Occurs before the action method is invoked.
        /// </summary>
        /// <param name="actionContext">The context for the action.</param><param name="cancellationToken">A cancellation token for signaling task ending.</param>
        public Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}