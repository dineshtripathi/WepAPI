using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.ModelBinding;
using Flex.Entity.Api.Controllers.SwaggerExamples;
using Flex.Entity.Api.CustomFilters.Swagger;
using Flex.Entity.Api.Model;
using Swashbuckle.Swagger.Annotations;

namespace Flex.Entity.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [SwaggerResponseExample(HttpStatusCode.InternalServerError, typeof(ApiResult), typeof(ApiResultResponseExamples500))]
    [SwaggerResponseExample(HttpStatusCode.Unauthorized, typeof(ApiResult), typeof(ApiResultResponseExamples401))]
    [SwaggerResponseExample(HttpStatusCode.MethodNotAllowed, typeof(ApiResult), typeof(ApiResultResponseExamples405))]
    [SwaggerResponse(HttpStatusCode.Unauthorized, "Unauthorized to access this resource.")]
    [SwaggerResponse(HttpStatusCode.InternalServerError, "flex-entity-api server error", typeof(Model.ApiResult))]
    public class ExtendedApiController: ApiController
    {
        /// <summary>
        /// Allows you to return a 500 with an ApiResult Payload
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        protected IHttpActionResult ServerErrorApiResult(Exception exception)
        {
            return Content(HttpStatusCode.InternalServerError,
                new ApiResult { success = false, error = exception?.Message ?? string.Empty });
        }

        /// <summary>
        /// Allows you to return a 400 with an ApiResult Payload
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected IHttpActionResult BadRequestApiResult(ModelStateDictionary state)
        {
            var allErrors = string.Join(",",
                state.Values.SelectMany(v => v.Errors)
                    .Select(
                        e =>
                            !string.IsNullOrWhiteSpace(e.ErrorMessage)
                                ? e.ErrorMessage
                                : e.Exception?.Message ?? string.Empty));
            var result = new ApiResult { success = false, error = allErrors };
            return Content(HttpStatusCode.BadRequest, result);
        }

        /// <summary>
        /// Allows you to return a 404 with an ApiResult Payload
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        protected IHttpActionResult NotFoundApiResult(string errorMessage)
        {
            var result = new ApiResult { success = false, error = errorMessage };
            return Content(HttpStatusCode.NotFound, result);
        }

        /// <summary>
        /// Send an unauthorized apiresult back to caller
        /// </summary>
        /// <param name="allErrors"></param>
        /// <returns></returns>
        protected IHttpActionResult UnauthorizedApiResult(string allErrors)
        {
            var result = new ApiResult { success = false, error = allErrors };
            return Content(HttpStatusCode.Unauthorized, result);
        }


    }
}