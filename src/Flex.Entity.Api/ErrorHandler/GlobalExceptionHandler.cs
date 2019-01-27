using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;
using Flex.Entity.Api.Model;
using Microsoft.ApplicationInsights;

namespace Flex.Entity.Api.ErrorHandler
{

    class OopsExceptionHandler : IExceptionHandler
    {
        private readonly IExceptionHandler _innerHandler;

        public OopsExceptionHandler(IExceptionHandler innerHandler)
        {
            if (innerHandler == null)
                throw new ArgumentNullException(nameof(innerHandler));

            _innerHandler = innerHandler;
        }

        public IExceptionHandler InnerHandler => _innerHandler;

        public Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            return _innerHandler.HandleAsync(context, cancellationToken);
        }

        public void Handle(ExceptionHandlerContext context)
        {
            // Create your own custom result here...
            // In dev, you might want to null out the result
            // to display the YSOD.
            // context.Result = null;
            context.Result = new InternalServerErrorResult(context.Request);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class GlobalExceptionHandler : ExceptionHandler
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override void Handle(ExceptionHandlerContext context)
        {
            if (context?.ExceptionContext != null && context.RequestContext != null && context.Exception != null)
            {
                //If customError is Off, then AI HTTPModule will report the exception
                if (context.RequestContext.IncludeErrorDetail)
                {
                    var ai = new TelemetryClient();
                    ai.TrackException(context.Exception);
                }
            }
            
            if (context != null)
            {
                context.Result = new NegotiatedContentResult<ApiResult>(HttpStatusCode.InternalServerError,
                        new ApiResult() {error = context.Exception?.Message ?? string.Empty, success = false},
                        new DefaultContentNegotiator(), context.Request,
                        new List<MediaTypeFormatter>()
                        {
                            new JsonMediaTypeFormatter(),
                            new XmlMediaTypeFormatter(),
                            new FormUrlEncodedMediaTypeFormatter(),
                            new BsonMediaTypeFormatter()
                        });
            }
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        //private class TextPlainErrorResult : IHttpActionResult
        //    {
        //        public HttpRequestMessage Request { get; set; }

        //        public string Content { get; set; }

        //        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        //        {
        //            HttpResponseMessage response =
        //                             new HttpResponseMessage(HttpStatusCode.InternalServerError);
        //            response.Content = new StringContent(Content);
        //            response.RequestMessage = Request;
        //            return Task.FromResult(response);
        //        }
        //    }
        }
}