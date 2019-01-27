using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Flex.Entity.Api.Model;

namespace Flex.Entity.JWTSecurity
{
    public static class TransformHelper
    {
        /// <summary>
        /// Converts an HttpError to ApiResult
        /// </summary>
        /// <param name="statusCodeToCheck"></param>
        /// <param name="originalResponseMessage"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> TransformHttpErrorToApiResult(HttpStatusCode statusCodeToCheck, HttpResponseMessage originalResponseMessage, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = originalResponseMessage;
            if (response.StatusCode == statusCodeToCheck)
            {
                //HttpError error = response.Content
                //convert the response into an ApiResult
                string errorMessage = null;
                HttpError error = null;
                try
                {
                    error = await response.Content.ReadAsAsync<HttpError>(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    //eat up exceptions
                }

                if (error != null)
                {
                    errorMessage = string.Join(",",
                        error.Select(p => p.Value).Select(o => o as string).Where(s => !string.IsNullOrWhiteSpace(s)));
                }
                response =
                    await
                        GetFailureResult(statusCodeToCheck, errorMessage ?? response.ReasonPhrase,
                            response.RequestMessage).ExecuteAsync(cancellationToken).ConfigureAwait(false);

            }
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="reasonPhrase"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static IHttpActionResult GetFailureResult(HttpStatusCode statusCode, string reasonPhrase, HttpRequestMessage request)
        {
            return new NegotiatedContentResult<ApiResult>(statusCode,
                new ApiResult() { error = reasonPhrase ?? string.Empty, success = false }, new DefaultContentNegotiator(),
                request,
                new List<MediaTypeFormatter>()
                {
                    new JsonMediaTypeFormatter(),
                    new XmlMediaTypeFormatter(),
                    new FormUrlEncodedMediaTypeFormatter(),
                    new BsonMediaTypeFormatter()
                });

        }


    }
}