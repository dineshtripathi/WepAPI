using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Flex.Entity.Api.Controllers.SwaggerExamples;
using Flex.Entity.Api.CustomFilters.Swagger;
using Flex.Entity.Api.HateoasRepresentations;
using Flex.Entity.Api.Helpers;
using Flex.Entity.Api.Model;
using Flex.Entity.Repository;
using Flex.Logging.Container;
using Swashbuckle.Swagger.Annotations;
using WebGrease.Css.Extensions;

namespace Flex.Entity.Api.Controllers
{
    /// <summary>
    /// </summary>
    [RoutePrefix("entities")]
    [SwaggerResponseExample(HttpStatusCode.NotFound, typeof (ApiResult), typeof (ApiResultEntityResponseExamples404))]
    public class EntityController : ExtendedApiController
    {
        private readonly IEntityRepository _entityRepository;

        /// <summary>
        /// </summary>
        /// <param name="entityRepository"></param>
        public EntityController(IEntityRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        /// <summary>
        ///     Logger Injected by the framework
        /// </summary>
        [ApplicationInsightsLogger("ApplicationInsights")]
        public ILogger Logger { get; set; }


        // https://<domain>/entities[?parent_id][&name][&limit][&offset][&service_descendant][&asset_descendant][&service_child][&asset_child][&has_tag][&matches_tag]
        /// <summary>
        ///     Provides all entities, possibly filtered by the query string. See "Parameters" for all the filter parameters.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="top">Maximum number of results to return.( default top=1000)</param>
        /// <param name="skip">Pagination offset (default skip = 0)</param>
        /// <param name="name">Partial match on the entity's name.</param>
        /// <param name="service_descendant">Descends from this service entity code.</param>
        /// <param name="asset_descendant">Descends from this asset entity code.</param>
        /// <param name="service_child">Is immediate child of this service entity code.</param>
        /// <param name="asset_child">Is immediate child of this asset entity code.</param>
        /// <param name="has_tag">Has non-null value for this tag.</param>
        /// <param name="matches_tag">Has given key:value for the tag.</param>
        /// <param name="at">travels back in time as at datetime and then applies any of the above filters. Default is now</param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [SwaggerOperation("Get all entities matching the provided filter")]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof (Entities),typeof (EntitiesResponseExamples))]
        [SwaggerResponse(HttpStatusCode.NotFound, "no entities match the filter.", typeof (ApiResult))]
        [SwaggerResponse(HttpStatusCode.OK, "returns a collection of entity objects that match the specified filter.",typeof (Entities))]
        [Authorize]
        [HttpGet]
        [Route("", Name = "GetEntityCollection")]
        [ResponseType(typeof (EntitiesRepresentation))]
        public async Task<IHttpActionResult> GetCollectionAsync(CancellationToken cancellationToken
            , int top = 1000
            , int skip = 0
            , string name = null
            , string service_descendant = null
            , string asset_descendant = null
            , string service_child = null
            , string asset_child = null
            , string has_tag = null
            , string matches_tag = null
            , DateTime? at = null)
        {
            try
            {
                //avoid divide by zero 
                top = (top == 0) ? 1000 : top;
                Model.Tag matchesTag = null;
                if (matches_tag != null)
                {
                    matches_tag = matches_tag.Trim();
                    int index = matches_tag.IndexOf(':');
                    matchesTag = new Model.Tag()
                    {
                        key = matches_tag.Substring(0, (index < 0) ? matches_tag.Length : index).Trim().ToLowerInvariant()
                        ,
                        value = matches_tag.Substring((index < 0) ? matches_tag.Length : index + 1).Trim().ToLowerInvariant()
                    };
                    matchesTag.value = (matchesTag.value == "null") ? null : matchesTag.value;

                }

                Entities retVal = await _entityRepository.GetAsync(cancellationToken, top, skip, name, service_descendant,
                            asset_descendant, service_child, asset_child, has_tag, matchesTag).ConfigureAwait(false);
                var entityDetails = retVal.entities as IList<EntityDetail> ?? retVal.entities?.ToList();
                if (retVal.count == 0 || entityDetails == null || !entityDetails.Any())
                {
                    return NotFoundApiResult("no entities match the filter.");
                }

                entityDetails.ForEach(ed =>
                {
                    ed.asset_children = new Uri(Url.Link("GetEntityChildrenByCode", new {ed.code, hierarchy = "asset"})).LocalPath;
                    ed.service_children = new Uri(Url.Link("GetEntityChildrenByCode", new { ed.code, hierarchy = "service" })).LocalPath;
                    ed.tags = new Uri(Url.Link("GetAllTagsByEntityCode", new { entityCode = ed.code })).LocalPath;
                });

                retVal.entities = entityDetails;                

                if(Request.DoesClientAcceptHalContent())
                {
                    var entitiesList = entityDetails.Select(e => new EntityRepresentation(at)
                    {
                        code = e.code
                        ,
                        name = e.name
                        ,
                        asset_parent = e.asset_parent
                        ,
                        service_parent = e.service_parent
                        ,
                        type = e.type
                        ,
                        updated_at = e.updated_at
                    }).ToList();

                    int nextSkip = skip + top;
                    int previousSkip = (skip - top < 0) ? 0 : skip - top;
                    int pageCount = (int)Math.Ceiling(retVal.count/(float)top);
                    int pageNumber= (int)Math.Floor(skip / (float)top );
                    var retCollection = new EntitiesRepresentation(entitiesList, at, pageCount,pageNumber)
                    {
                        Links =
                        {
                            HatoasLinkTemplates.entities.NextPageEntities.CreateLink(new {top = top, skip = nextSkip, at})
                            ,
                            HatoasLinkTemplates.entities.PreviousPageEntities.CreateLink(
                                new {top = top, skip = previousSkip, at})
                        }
                    };
                    return Ok(retCollection);
                }
                return Ok(retVal);

            }
            catch (DbException exception)
            {
                switch (exception.Reason)
                {
                    case DbException.ErrorReason.ConstraintViolation:
                        ModelState.AddModelError("Constraint", exception.Message);
                        return BadRequestApiResult(ModelState);
                    case DbException.ErrorReason.SecurityViolation:
                        return UnauthorizedApiResult(exception.Message);
                }
                throw;
            }
            catch (ArgumentException)
            {
                return NotFoundApiResult("no entities match the filter.");
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="code"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="hierarchy">Either "asset" or "service"</param>
        /// <param name="at"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [SwaggerOperation("Get Entities children in relevant hierarchy by code")]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(IEnumerable<EntityAt>), typeof(EntityAtResponseExamples))]
        [SwaggerResponse(HttpStatusCode.NotFound, "entity does not exist.", typeof(ApiResult))]
        [SwaggerResponse(HttpStatusCode.OK, "returns the children of entity with specified code, in either the asset or service hierarchy", typeof(IEnumerable<EntityAt>))]
        [Authorize]
        [HttpGet]
        [Route(@"{code:regex(^[a-zA-Z]{1,2}\d{1,8}$)}/children/{hierarchy:regex(^(asset|service)$)}", Name = "GetEntityChildrenByCode")]
        [ResponseType(typeof(IEnumerable<EntityAt>))]
        public async Task<IHttpActionResult> GetChildrenAsync(CancellationToken cancellationToken, string code, string hierarchy,
            DateTime? at = null)
        {
            try
            {
                var result = await _entityRepository.GetAsync(code, cancellationToken, hierarchy, at).ConfigureAwait(false);
                var entityAts = result as IList<EntityAt> ?? result.ToList();
                if (result == null || !entityAts.Any())
                {
                    return NotFoundApiResult($"entity children for code = {code} in hierarchy {hierarchy} not found.");
                }
                if (Request.DoesClientAcceptHalContent())
                {
                    var entitiesList = entityAts.Select(e => new EntityRepresentation(at)
                    {
                        code = e.code
                        ,
                        name = e.name
                        ,
                        asset_parent = e.asset_parent
                        ,
                        service_parent = e.service_parent
                        ,
                        type = e.type
                        ,
                        updated_at = e.updated_at
                    }).ToList();

                    return Ok( new EntityChildrenRepresentation(entitiesList,code,hierarchy,at));
                }
                return Ok(entityAts);
            }
            catch (DbException exception)
            {
                switch (exception.Reason)
                {
                    case DbException.ErrorReason.ConstraintViolation:
                        ModelState.AddModelError("Constraint", exception.Message);
                        return BadRequestApiResult(ModelState);
                    case DbException.ErrorReason.SecurityViolation:
                        return UnauthorizedApiResult(exception.Message);
                }
                throw;
            }
            catch (ArgumentException)
            {
                return NotFoundApiResult($"entity children for code = {code} in hierarchy {hierarchy} not found.");
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="code"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="at"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [SwaggerOperation("Get Entity by code")]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof (EntityAt), typeof (EntityAtResponseExamplesSingle))]
        [SwaggerResponse(HttpStatusCode.NotFound, "entity does not exist.", typeof (ApiResult))]
        [SwaggerResponse(HttpStatusCode.OK, "returns the entity with specified code.", typeof (EntityAt))]
        [Authorize]
        [HttpGet]
        [Route(@"{code:regex(^[a-zA-Z]{1,2}\d{1,8}$)}", Name = "GetEntityByCode")]
        [ResponseType(typeof (EntityAt))]
        public async Task<IHttpActionResult> GetSingleAsync(CancellationToken cancellationToken, string code,
            DateTime? at = null)
        {
            try
            {
                var result = await _entityRepository.GetAsync(code, cancellationToken, at).ConfigureAwait(false);
                if (result == null)
                {
                    return NotFoundApiResult($"entity with code = {code} not found.");
                }
                if (Request.DoesClientAcceptHalContent())
                {
                   var repResult =  new EntityRepresentation(at)
                    {
                        code = result.code
                        ,
                        name = result.name
                        ,
                        asset_parent = result.asset_parent
                        ,
                        service_parent = result.service_parent
                        ,
                        type = result.type
                        ,
                        updated_at = result.updated_at
                    };
                    return Ok(repResult);
                }
                return Ok(result);

            }
            catch (DbException exception)
            {
                switch (exception.Reason)
                {
                    case DbException.ErrorReason.ConstraintViolation:
                        ModelState.AddModelError("Constraint", exception.Message);
                        return BadRequestApiResult(ModelState);
                    case DbException.ErrorReason.SecurityViolation:
                        return UnauthorizedApiResult(exception.Message);
                }
                throw;
            }
            catch (ArgumentException)
            {
                return NotFoundApiResult($"entity with code = {code} not found.");
            }

        }


        // POST: Entity
        /// <summary>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="entityRequest"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        /// s
        [SwaggerOperation("Create New Entity")]
        [SwaggerRequestExample(typeof (EntityRequest), typeof (EntityRequestExampleSingle))]
        [SwaggerResponseExample(HttpStatusCode.Created, typeof (Model.Entity), typeof (EntityResponseExamplesSingle))]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof (ApiResult),
            typeof (ApiResultEntityResponseExamples400))]
        [SwaggerResponse(HttpStatusCode.BadRequest,
            "Entity with name ({entity}) and AssetParentCode (({code})), ServiceParentCode ({code}) cannot be created.",
            typeof (ApiResult))]
        [SwaggerResponse(HttpStatusCode.Created, "created entity", typeof (Model.Entity))]
        [SwaggerResponseRemoveDefaults]
        [Authorize]
        [HttpPost]
        [Route("")]
        [ResponseType(typeof (Model.Entity))]
        public async Task<IHttpActionResult> CreateAsync(CancellationToken cancellationToken,
            [FromBody] EntityRequest entityRequest)
        {
            Model.Entity entity;
            if (!ModelState.IsValid)
            {
                return BadRequestApiResult(ModelState);
            }
            try
            {
                entity = await _entityRepository.CreateAsync(entityRequest, cancellationToken).ConfigureAwait(false);
            }
            catch (DbException exception)
            {
                switch (exception.Reason)
                {
                    case DbException.ErrorReason.ConstraintViolation:
                        ModelState.AddModelError("Constraint", exception.Message);
                        return BadRequestApiResult(ModelState);
                    case DbException.ErrorReason.SecurityViolation:
                        return UnauthorizedApiResult(exception.Message);
                }
                throw;
            }
            var uri = Url.Link("GetEntityByCode", new {entity.code});

            return Created(new Uri(uri), entity);
        }


        // PATCH: Entity
        /// <summary>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="code">The entity code that needs to be updated</param>
        /// <param name="entityRequestDelta">the properties of the entity that need to be updated</param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        /// s
        [SwaggerOperation("Update Entity")]
        [SwaggerRequestExample(typeof (EntityPatchRequest), typeof (EntityPatchRequestExampleSingle))]
        //[SwaggerRequestExample(typeof(Delta<EntityPatchRequest>), typeof(EntityPatchRequestExampleSingle))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof (ApiResult), typeof (ApiResultEntityResponseExamples200))]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof (ApiResult),
            typeof (ApiResultEntityResponseExamples400))]
        [SwaggerResponse(HttpStatusCode.OK, "created entity", typeof (ApiResult))]
        [SwaggerResponse(HttpStatusCode.NotFound, "entity does not exist.", typeof (ApiResult))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Entity with code ({code}) cannot be updated.", typeof (ApiResult))]
        [SwaggerResponseRemoveDefaults]
        [Authorize]
        [HttpPatch]
        [Route(@"{code:regex(^[a-zA-Z]{1,2}\d{1,8}$)}", Name = "UpdateEntityByCode")]
        [ResponseType(typeof (ApiResult))]
        public async Task<IHttpActionResult> UpdateAsync(CancellationToken cancellationToken, string code,
            [FromBody] Delta<EntityPatchRequest> entityRequestDelta)
        {
            var result = new ApiResult {success = false, error = string.Empty};

            if (!ModelState.IsValid)
            {
                return BadRequestApiResult(ModelState);
            }
            try
            {
                var oldEntity = await _entityRepository.GetAsync(code, cancellationToken).ConfigureAwait(false);
                if (oldEntity != null)
                {
                    entityRequestDelta.Patch(oldEntity);
                    EntityPatchRequest entityRequest = new EntityPatchRequest();
                    var propNames = entityRequestDelta.GetChangedPropertyNames();
                    var changedProperties = propNames as string[] ?? propNames.ToArray();
                    entityRequest.name = changedProperties.Contains("name") ? entityRequestDelta.GetEntity().name : null;
                    entityRequest.service_parent = changedProperties.Contains("service_parent")
                        ? entityRequestDelta.GetEntity().service_parent ?? string.Empty
                        : null;
                    entityRequest.asset_parent = changedProperties.Contains("asset_parent")
                        ? entityRequestDelta.GetEntity().asset_parent ?? string.Empty
                        : null;

                    var entity =
                        await
                            _entityRepository.UpdateAsync(code, entityRequest, cancellationToken).ConfigureAwait(false);
                    if (entity?.code == code)
                    {
                        result.success = true;
                        result.error = string.Empty;
                        return Ok(result);
                    }
                }
                return NotFoundApiResult($"entity with code = {code} not found.");
            }
            catch (DbException exception)
            {
                switch (exception.Reason)
                {
                    case DbException.ErrorReason.ConstraintViolation:
                        ModelState.AddModelError("Constraint", exception.Message + (exception.InnerException?.Message ?? string.Empty));
                        return BadRequestApiResult(ModelState);
                    case DbException.ErrorReason.SecurityViolation:
                        return UnauthorizedApiResult(exception.Message);
                }
                throw;
            }
        }

        // DELETE: api/EntityTypes/Delete/LO
        /// <summary>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="code"></param>
        /// <param name="delete_all_descendants"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [SwaggerOperation("Delete Entity")]
        [SwaggerResponseExample(HttpStatusCode.BadRequest, typeof (ApiResult),
            typeof (ApiResultEntityResponseExamples400))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof (ApiResult), typeof (ApiResultEntityResponseExamples200))]
        [SwaggerResponse(HttpStatusCode.BadRequest,
            "There are currently tags in the system for this entity so it cannot be deleted. Please delete the tags and try again.",
            typeof (ApiResult))]
        [SwaggerResponse(HttpStatusCode.NotFound, "could not locate the requested entity", typeof (ApiResult))]
        [SwaggerResponse(HttpStatusCode.OK, "entity deleted", typeof (ApiResult))]
        [Authorize]
        [HttpDelete]
        [Route(@"{code:regex(^[a-zA-Z]{1,2}\d{1,8}$)}", Name = "DeleteEntityByCode")]
        // ReSharper disable once InconsistentNaming
        public async Task<IHttpActionResult> DeleteAsync(CancellationToken cancellationToken, string code,
            bool delete_all_descendants = false)
        {
            var result = new ApiResult {success = false, error = string.Empty};
            try
            {
                var delResult =
                    await
                        _entityRepository.DeleteAsync(code, delete_all_descendants, cancellationToken)
                            .ConfigureAwait(false);
                if (delResult == 1)
                {
                    result.success = true;
                    result.error = string.Empty;
                    return Ok(result);
                }
                return NotFoundApiResult($"enitytype with entity_type_prefix = {code} not found.");
            }
            catch (DbException exception)
            {
                switch (exception.Reason)
                {
                    case DbException.ErrorReason.ConstraintViolation:
                        ModelState.AddModelError("Constraint", exception.Message);
                        return BadRequestApiResult(ModelState);
                    case DbException.ErrorReason.SecurityViolation:
                        return UnauthorizedApiResult(exception.Message);
                }
                throw;
            }
        }
    }
}