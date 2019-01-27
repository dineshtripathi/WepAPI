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
using Flex.TagType.Api.Controllers.SwaggerExamples;
using Swashbuckle.Swagger.Annotations;

namespace Flex.Entity.Api.Controllers
{
    /// <summary>
    /// </summary>
    /// 
    [RoutePrefix("entities/tags")]
    [SwaggerResponseExample(HttpStatusCode.NotFound, typeof(ApiResult), typeof(ApiResultTagTypeResponseExamples404))]
    public class TagTypesController : ExtendedApiController
    {
        private readonly ITagTypeRepository _tagTypeRepository;

        /// <summary>
        /// ApplicationInsights
        /// </summary>
        [ApplicationInsightsLogger("ApplicationInsights")]
        public ILogger Logger { get; set; }
        /// <summary>
        /// </summary>
        /// <param name="tagTypeRepository"></param>
        public TagTypesController(ITagTypeRepository tagTypeRepository)
        {
            _tagTypeRepository = tagTypeRepository;
        }


        /// <summary>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        // GET: api/TagTypes
        [SwaggerOperation("Get Tag Types")]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(IEnumerable<Model.TagType>), typeof(TagTypeResponseExamples))]
        [SwaggerResponse(HttpStatusCode.NotFound, "tag types do not exist.")]
        [SwaggerResponse(HttpStatusCode.OK, "returns an array of all tag types.", typeof(IEnumerable<Model.TagType>))]
        [Authorize]
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(IEnumerable<Model.TagType>))]
        public async Task<IHttpActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var result = await _tagTypeRepository.GetAsync(cancellationToken).ConfigureAwait(false);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        // GET: api/TagTypes/L
        [SwaggerOperation("Get Tag Type by prefix")]
        [SwaggerResponseExample(HttpStatusCode.OK,typeof(Model.TagType), typeof(TagTypeResponseExamplesSingle))]
        [SwaggerRequestExample(typeof(string), typeof(StringRequestExample))]
        [SwaggerResponse(HttpStatusCode.NotFound, "tag type with prefix = {key} not found.", typeof(ApiResult))]
        [SwaggerResponse(HttpStatusCode.OK, "returns the tag type with specified prefix.", typeof(Model.TagType))]
        [Authorize]
        [HttpGet]
        [Route("{key}", Name = "GetTagTypeByKey")]
        [ResponseType(typeof(Model.TagType))]
        public async Task<IHttpActionResult> GetSingleAsync(CancellationToken cancellationToken,string key)
        {
            var result = await _tagTypeRepository.GetAsync(key, cancellationToken).ConfigureAwait(false);
            if (result == null)
            {
                return NotFoundApiResult($"tag type with prefix = {key} not found.");
            }
            return Ok(result);
        }

        // POST: api/TagTypes
        /// <summary>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="tagType"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>s
        [SwaggerOperation("Create New Tag Type")]
        [SwaggerResponseExample(HttpStatusCode.Created, typeof(Model.TagType), typeof(TagTypeResponseExamplesSingle))]
        [SwaggerResponseExample(HttpStatusCode.BadRequest,typeof(ApiResult),typeof(ApiResultTagTypeResponseExamples400))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "this tag type already exists", typeof(ApiResult))]
        [SwaggerResponse(HttpStatusCode.Created, "created tag", typeof(Model.TagType))]
        [SwaggerResponseRemoveDefaults]
        [Authorize]
        [HttpPost]
        [Route("")]
        [ResponseType(typeof(Model.TagType))]
        public async Task<IHttpActionResult> CreateAsync(CancellationToken cancellationToken, [FromBody] Model.TagType tagType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestApiResult(ModelState);
            }
            try
            {
                var existingType = await _tagTypeRepository.GetAsync(tagType.key, cancellationToken).ConfigureAwait(false);
                if (existingType == null)
                {
                    await _tagTypeRepository.CreateAsync(tagType, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    ModelState.AddModelError("key", $"key = {tagType.key} already exisits");
                    return BadRequestApiResult(ModelState);
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
            var uri = Url.Link("GetTagTypeByKey", new {tagType.key});

            return Created(new Uri(uri), tagType);
        }


        // DELETE: entities/tags/LO
        /// <summary>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [SwaggerOperation("Delete Tag Type")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ApiResult), typeof(ApiResultTagTypeResponseExamples400))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ApiResult), typeof(ApiResultTagTypeResponseExamples200))]
        [SwaggerResponse( HttpStatusCode.BadRequest, "There are currently entities in the system of this type so it cannot be deleted. Please delete the entities and try again.", typeof(ApiResult))]
        [SwaggerResponse(HttpStatusCode.NotFound, "tag type with key = {key} not found.", typeof(ApiResult))]
        [SwaggerResponse(HttpStatusCode.OK, "tag type deleted", typeof(ApiResult))]
        [Authorize]
        [HttpDelete]
        [Route("{key}")]
        public async Task<IHttpActionResult> DeleteAsync(CancellationToken cancellationToken, string key)
        {
            var result = new ApiResult {success = false, error = string.Empty};
            try
            {
               int delResult =  await _tagTypeRepository.DeleteAsync(key, cancellationToken).ConfigureAwait(false);
                if (delResult == 1)
                {
                    result.success = true;
                    result.error = string.Empty;
                    return Ok(result);
                }
                return NotFoundApiResult($"tag type with key = {key} not found.");
            }
            catch (DbException exception)
            {
                if (exception.Reason == DbException.ErrorReason.ConstraintViolation)
                {
                    ModelState.AddModelError($"Constraint", "There are currently entities in the system of this type so it cannot be deleted. Please delete the entities and try again.,{exception.Message}");
                    return BadRequestApiResult(ModelState);
                }
                throw;
            }
        }
    }
}