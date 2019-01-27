using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Flex.Entity.Api.Helpers;

namespace Flex.Entity.Api.CustomFilters
{
    /// <summary>
    /// 
    /// </summary>
    public class ResponseTransformHandler:DelegatingHandler
    {
        /// <summary>
        /// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.
        /// </summary>
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>. The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="request">The HTTP request message to send to the server.</param><param name="cancellationToken">A cancellation token to cancel operation.</param><exception cref="T:System.ArgumentNullException">The <paramref name="request"/> was null.</exception>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            response = await TransformHelper.TransformHttpErrorToApiResult(HttpStatusCode.MethodNotAllowed, response,cancellationToken).ConfigureAwait(false);
            response = await TransformHelper.TransformHttpErrorToApiResult(HttpStatusCode.InternalServerError, response, cancellationToken).ConfigureAwait(false);
            return response;

        }
    }
}