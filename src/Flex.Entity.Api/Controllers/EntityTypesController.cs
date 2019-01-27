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
using Swashbuckle.Swagger.Annotations;
using Flex.Entity.Security;

namespace Flex.Entity.Api.Controllers
{
    /// <summary>
    /// </summary>
    /// 
    [RoutePrefix("entities/types")]
    [SwaggerResponseExample(HttpStatusCode.NotFound, typeof(ApiResult), typeof(ApiResultEntityTypeResponseExamples404))]
    public class EntityTypesController : ExtendedApiController
    {
        private readonly IEntityTypeRepository _entityTypeRepository;
       // public Lazy<ISecurityContextProvider> SecurityContextProvider { get; set; }
        /// <summary>
        /// Logger Injected by the framework
        /// </summary>
        [ApplicationInsightsLogger("ApplicationInsights")]
        public ILogger Logger { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="entityTypeRepository"></param>
        public EntityTypesController(IEntityTypeRepository entityTypeRepository)
        {
            _entityTypeRepository = entityTypeRepository;
        }


        /// <summary>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        // GET: api/EntityTypes
        [SwaggerOperation("Get Entity Types")]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(IEnumerable<EntityType>), typeof(EntityTypeResponseExamples))]
        [SwaggerResponse(HttpStatusCode.NotFound, "entity types do not exist.")]
        [SwaggerResponse(HttpStatusCode.OK, "returns an array of all entity types.", typeof(IEnumerable<Model.EntityType>))]
        [Authorize()]
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(IEnumerable<Model.EntityType>))]
        public async Task<IHttpActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
          //  var x2 = SecurityContextProvider.Value.Principal;
            
            //var x4 = SecurityContextProvider.Value.LifetimeScope;

            //var x3 = SecurityContextProvider.Value.GetClaims();
            //var x = Logger.LoggerName;
            var result = await _entityTypeRepository.GetAsync(cancellationToken).ConfigureAwait(false);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }



        /// <summary>
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        // GET: api/EntityTypes/L
        [SwaggerOperation("Get Entity Type by prefix")]
        [SwaggerResponseExample(HttpStatusCode.OK,typeof(EntityType), typeof(EntityTypeResponseExamplesSingle))]
        [SwaggerRequestExample(typeof(string), typeof(StringRequestExample))]
        [SwaggerResponse(HttpStatusCode.NotFound, "entity types does not exist.", typeof(ApiResult))]
        [SwaggerResponse(HttpStatusCode.OK, "returns the entity type with specified prefix.", typeof(Model.EntityType))]
        [Authorize]
        [HttpGet]
        [Route(@"{prefix:regex(^[a-zA-Z]{1,2}$)}", Name = "GetEntityTypeByPrefix")]
        //[Route("{prefix}", Name = "GetEntityTypeByPrefix")]
        [ResponseType(typeof(Model.EntityType))]
        public async Task<IHttpActionResult> GetSingleAsync(CancellationToken cancellationToken,string prefix)
        {
            var result = await _entityTypeRepository.GetAsync(prefix, cancellationToken).ConfigureAwait(false);
            if (result == null)
            {
                return NotFoundApiResult($"entity type with prefix = {prefix} not found.");
            }
            return Ok(result);
        }

        // POST: api/EntityTypes
        /// <summary>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>s
        [SwaggerOperation("Create New Entity Type")]
        [SwaggerResponseExample(HttpStatusCode.Created, typeof(EntityType), typeof(EntityTypeResponseExamplesSingle))]
        [SwaggerResponseExample(HttpStatusCode.BadRequest,typeof(ApiResult),typeof(ApiResultEntityTypeResponseExamples400))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "this entity type already exists", typeof(Model.ApiResult))]
        [SwaggerResponse(HttpStatusCode.Created, "created entity", typeof(Model.EntityType))]
        [SwaggerResponseRemoveDefaults]
        [Authorize]
        [HttpPost]
        [Route("")]
        [ResponseType(typeof(Model.EntityType))]
        public async Task<IHttpActionResult> CreateAsync(CancellationToken cancellationToken, [FromBody] Model.EntityType entityType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestApiResult(ModelState);
            }
            try
            {
                var existingType = await _entityTypeRepository.GetAsync(entityType.prefix, cancellationToken).ConfigureAwait(false);
                if (existingType == null)
                {
                    await _entityTypeRepository.CreateAsync(entityType, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    ModelState.AddModelError("prefix", $"prefix = {entityType.prefix} already exisits");
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
            var uri = Url.Link("GetEntityTypeByPrefix", new {entityType.prefix});

            return Created(new Uri(uri), entityType);
        }


        // DELETE: api/EntityTypes/Delete/LO
        /// <summary>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [SwaggerOperation("Delete Entity Type")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof(ApiResult), typeof(ApiResultEntityTypeResponseExamples400))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ApiResult), typeof(ApiResultEntityTypeResponseExamples200))]
        [SwaggerResponse( HttpStatusCode.BadRequest, "There are currently entities tags values in the system of this type so it cannot be deleted.Please delete the entities tags values and try again.", typeof(Model.ApiResult))]
        [SwaggerResponse(HttpStatusCode.NotFound, "could not locate the requested entity type", typeof(Model.ApiResult))]
        [SwaggerResponse(HttpStatusCode.OK, "entity type deleted", typeof(Model.ApiResult))]
        [Authorize]
        [HttpDelete]
        [Route(@"{prefix:regex(^[a-zA-Z]{1,2}$)}", Name = "DeleteEntityTypeByPrefix")]
        //[Route("{prefix}")]
        public async Task<IHttpActionResult> DeleteAsync(CancellationToken cancellationToken, string prefix)
        {
            var result = new ApiResult {success = false, error = string.Empty};
            try
            {
               int delResult =  await _entityTypeRepository.DeleteAsync(prefix, cancellationToken).ConfigureAwait(false);
                if (delResult == 1)
                {
                    result.success = true;
                    result.error = string.Empty;
                    return Ok(result);
                }
                return NotFoundApiResult($"enitytype with entity_type_prefix = {prefix} not found.");
            }
            catch (DbException exception)
            {
                if (exception.Reason == DbException.ErrorReason.ConstraintViolation)
                {
                    ModelState.AddModelError($"Constraint", $"There are currently entities tags values in the system of this type so it cannot be deleted. Please delete the entities tags values and try again.{exception.Message}");
                    return BadRequestApiResult(ModelState);
                }
                throw;
            }
        }
    }
}