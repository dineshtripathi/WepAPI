using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using Flex.Entity.Api.Model;

namespace Flex.Entity.Api.CustomFilters
{
    /// <summary>
    /// 
    /// </summary>
    public class UnhandledExceptionFilter: ExceptionFilterAttribute
    {
        #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public override  Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            try
            {
                actionExecutedContext.Response = 
                    (new NegotiatedContentResult<ApiResult>(HttpStatusCode.InternalServerError,
                        new ApiResult() { error = actionExecutedContext.Exception?.Message ?? string.Empty, success = false }, new DefaultContentNegotiator(), actionExecutedContext.Request,
                        new List<MediaTypeFormatter>()
                        {
                        new JsonMediaTypeFormatter(),
                        new XmlMediaTypeFormatter(),
                        new FormUrlEncodedMediaTypeFormatter(),
                        new BsonMediaTypeFormatter()
                        })).ExecuteAsync(cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();

            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
            return Task.CompletedTask;
        }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    }
}