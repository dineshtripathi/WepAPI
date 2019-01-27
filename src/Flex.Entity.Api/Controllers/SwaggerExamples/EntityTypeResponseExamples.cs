using System.Collections.Generic;
using Flex.Entity.Api.CustomFilters.Swagger;
using Flex.Entity.Api.Model;

namespace Flex.Entity.Api.Controllers.SwaggerExamples
{
    /// <summary>
    /// </summary>
    public class EntityTypeResponseExamples : IProvideExample
    {
        /// <summary>
        /// </summary>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public object GetExample(string mediaType = "application/json")
        {
            return new List<EntityType>
            {
                new EntityType
                {
                    allow_in_asset_hierarchy = true,
                    allow_in_service_hierarchy = true,
                    allow_same_type_descendant = true,
                    name = "Load",
                    prefix = "L"
                }
                ,
                new EntityType
                {
                    allow_in_asset_hierarchy = true,
                    allow_in_service_hierarchy = false,
                    allow_same_type_descendant = false,
                    name = "Site",
                    prefix = "S"
                }
            };
        }
    }

    /// <summary>
    /// </summary>
    public class EntityTypeResponseExamplesSingle : IProvideExample
    {
        /// <summary>
        /// </summary>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public object GetExample(string mediaType = "application/json")
        {
            return
                new EntityType
                {
                    allow_in_asset_hierarchy = true,
                    allow_in_service_hierarchy = true,
                    allow_same_type_descendant = true,
                    name = "Load",
                    prefix = "L"
                };
        }
    }
}