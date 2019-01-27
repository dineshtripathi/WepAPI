using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Flex.Entity.Api.Controllers.SwaggerExamples;
using Flex.Entity.Api.CustomFilters.Swagger;
using Flex.Entity.Api.Model;
using Flex.Entity.Repository;
using Flex.Logging.Container;
using Flex.Tag.Api.Controllers.SwaggerExamples;
using Swashbuckle.Swagger.Annotations;

namespace Flex.Entity.Api.Controllers
{
    /// <summary>
    /// </summary>
    /// 
    [RoutePrefix(@"entities/{entityCode:regex(^[a-zA-Z]{1,2}\d{1,8}$)}/tags")]
    //[RoutePrefix("entities/{entityCode}/tags")]
    [SwaggerResponseExample(HttpStatusCode.NotFound, typeof(ApiResult), typeof(ApiResultEntityTypeResponseExamples404))]
    public class TagsController : ExtendedApiController
    {
        private readonly ITagRepository _tagRepository;

        /// <summary>
        /// ApplicationInsights
        /// </summary>
        [ApplicationInsightsLogger("ApplicationInsights")]
        public ILogger Logger { get; set; }
        /// <summary>
        /// </summary>
        /// <param name="tagRepository"></param>
        public TagsController(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }


        /// <summary>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="entityCode"></param>
        /// <param name="at"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        // GET: api/Tags
        [SwaggerOperation("Get all Tags Values")]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(Tags), typeof(TagsResponseExamples))]
        [SwaggerResponse(HttpStatusCode.NotFound, "entity code do not exist.")]
        [SwaggerResponse(HttpStatusCode.OK, "returns an array of all tag value.", typeof(IEnumerable<Model.TagAt>))]
        [Authorize()]
        [HttpGet]
        [Route("", Name = "GetAllTagsByEntityCode")]
        //[Route(@"{entityCode:regex(^[a-zA-Z]{1,2}\d{1,8}$)}", Name = "GetTagValueByEntityCode")]
        [ResponseType(typeof(IEnumerable<TagRequest>))]
        public async Task<IHttpActionResult> GetAllAsync(CancellationToken cancellationToken, string entityCode, DateTime? at=null)
        {
            var result = await _tagRepository.GetAsync(entityCode, at, cancellationToken).ConfigureAwait(false);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(new Tags { tags = result });
        }

        /// <summary>
        /// </summary>
        /// <param name="entityCode"></param>
        /// <param name="key"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="at"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        // GET: api/Tags/L
        [SwaggerOperation("Get Tag value by key")]
        [SwaggerResponseExample(HttpStatusCode.OK,typeof(Model.TagAt), typeof(TagResponseExamplesSingle))]
        [SwaggerResponse(HttpStatusCode.NotFound, "tag key = {key} not found.", typeof(ApiResult))]
        [SwaggerResponse(HttpStatusCode.OK, "returns the tag with specified key value.", typeof(Model.TagAt))]
        [Authorize]
        [HttpGet]
        [Route("{key}", Name = "GetTagByEntityCodeAndKey")]
        [ResponseType(typeof(Model.TagAt))]
        public async Task<IHttpActionResult> GetSingleAsync(CancellationToken cancellationToken,string entityCode, string key, DateTime? at=null)
        {
            var result = await _tagRepository.GetAsync(entityCode, key, at, cancellationToken).ConfigureAwait(false);
            if (result == null)
            {
                return NotFoundApiResult($"tag key = {key} not found.");
            }
            return Ok(result);
        }

        // POST: api/Tags
        /// <summary>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="key"></param>
        /// <param name="tag"></param>
        /// <param name="entityCode"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>s
        [SwaggerOperation("Put New Tag")]
        [SwaggerRequestExample(typeof(TagRequest), typeof(TagRequestExample))]
        [SwaggerResponseExample(HttpStatusCode.Created, typeof(Model.TagAt), typeof(TagResponseExamplesSingle))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(Model.TagAt), typeof(ApiResultTagResponseExamples200))]
        [SwaggerResponseExample(HttpStatusCode.BadRequest,typeof(ApiResult),typeof(ApiResultTagResponseExamples400))]
        [SwaggerResponseExample(HttpStatusCode.NotFound, typeof(ApiResult), typeof(ApiResultEntityTypeResponseExamples404))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "bad request", typeof(ApiResult))]
        [SwaggerResponse(HttpStatusCode.NotFound, "tag with entityCode = {entityCode} and key = {key} not found.", typeof(ApiResult))]
        [SwaggerResponse(HttpStatusCode.Created, "created tag", typeof(Model.TagAt))]
        [SwaggerResponse(HttpStatusCode.OK, "tag  created", typeof(ApiResult))]
        [SwaggerResponseRemoveDefaults]
        [Authorize]
        [HttpPut]
        [Route("{key}", Name = "CreateTagValueByEntityCodeWithKey")]
        [ResponseType(typeof(Model.TagAt))]
        public async Task<IHttpActionResult> CreateAsync(CancellationToken cancellationToken, [FromUri] string entityCode, [FromUri] string key, [FromBody] TagRequest tag)
        {
            var result = new ApiResult { success = false, error = string.Empty };
            if (!ModelState.IsValid)
            {
                return BadRequestApiResult(ModelState);
            }
            try
            {
                var existingType = await _tagRepository.GetAsync(entityCode, key, null, cancellationToken).ConfigureAwait(false);
                if (existingType == null)
                {
                    await _tagRepository.CreateAsync(entityCode, key, tag, cancellationToken).ConfigureAwait(false);
                    var uri = Url.Link("CreateTagValueByEntityCodeWithKey", new { entityCode, key });
                    var responseTag = await _tagRepository.GetAsync(entityCode, key, null, cancellationToken).ConfigureAwait(false);
                    return Created(new Uri(uri), responseTag);
                }

                var updatedResult  = await _tagRepository.UpdateAsync(entityCode, key, tag, cancellationToken).ConfigureAwait(false);
                if (updatedResult == 1)
                {
                    result.success = true;
                    result.error = string.Empty;
                    return Ok(result);
                }
            }
            catch (DbException exception)
            {
                if (exception.Reason == DbException.ErrorReason.ConstraintViolation)
                {
                    ModelState.AddModelError("duplicate", exception.Message);
                    return BadRequestApiResult(ModelState);
                }
                throw;
            }
            return NotFoundApiResult($"tag with entityCode = {entityCode} and key = {key} not found.");
        }


        // DELETE: entities/tags/LO
        /// <summary>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="entityCode"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [SwaggerOperation("Delete Tag ")]
        [SwaggerResponseExample(HttpStatusCode.NotFound, typeof(ApiResult), typeof(ApiResultEntityTypeResponseExamples404))]
        [SwaggerResponse(HttpStatusCode.NotFound, "tag key = {key} not found", typeof(ApiResult))]
        [SwaggerResponse(HttpStatusCode.OK, "tag  deleted", typeof(ApiResult))]
        [Authorize]
        [HttpDelete]
        [Route("{key}")]
        public async Task<IHttpActionResult> DeleteAsync(CancellationToken cancellationToken, string entityCode, string key)
        {
            var result = new ApiResult {success = false, error = string.Empty};
            try
            {
               int delResult =  await _tagRepository.DeleteAsync(entityCode, key, cancellationToken).ConfigureAwait(false);
                if (delResult == 1)
                {
                    result.success = true;
                    result.error = string.Empty;
                    return Ok(result);
                }
                return NotFoundApiResult($"tag key = {key} not found.");
            }
            catch (DbException exception)
            {
                if (exception.Reason == DbException.ErrorReason.ConstraintViolation)
                {
                    ModelState.AddModelError($"Constraint", $"There are currently entities in the system of this  so it cannot be deleted. Please delete the entities and try again. {exception.Message}");
                    return BadRequestApiResult(ModelState);
                }
                throw;
            }
        }
    }

   
}