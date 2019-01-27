using WebApi.Hal;

namespace Flex.Entity.Api.HateoasRepresentations
{
    /// <summary>
    /// HAL Link Templates 
    /// </summary>
    public static class HatoasLinkTemplates
    {
        /// <summary>
        /// defines the links for entities
        /// </summary>
        public static class entities
        {
            /// <summary>
            /// /all entities
            /// </summary>
            public static Link GetEntities => new Link("entities", "~/entities{?at}");
            //public static Link SearchEntities => new Link("search", "~/entities{?top,skip,name,service_descendant,asset_descendant,service_child,asset_child,has_tag,matches_tag,at}");

            /// <summary>
            /// /entities?top={top}&amp;skip={skip}
            /// </summary>
            public static Link NextPageEntities => new Link("next", "~/entities{?top,skip,at}");

            /// <summary>
            /// /entities?top={top}&amp;skip={skip}
            /// </summary>
            public static Link PreviousPageEntities => new Link("previous", "~/entities{?top,skip,at}");

        }

        /// <summary>
        /// 
        /// </summary>
        public static class entitiyChildren
        {
            /// <summary>
            /// ~/entities/{code}/children/asset"
            /// </summary>
            public static Link AssetChildren => new Link("asset_children", "~/entities/{code}/children/asset{?at}");
            /// <summary>
            /// /entities/{code}/children/service
            /// </summary>
            public static Link ServiceChildren => new Link("service_children", "~/entities/{code}/children/service{?at}");

        }

        public static class entity
        {
            /// <summary>
            /// /entities/{code}
            /// </summary>
            public static Link Entity => new Link("entities", "~/entities/{code}{?at}");
            public static Link AssetParentEntity => new Link("asset_parent", "~/entities/{asset_parent}{?at}");
            public static Link ServicePatentEntity => new Link("service_parent", "~/entities/{service_parent}{?at}");
            /// <summary>
            /// /entities/{code}/tags
            /// </summary>
            public static Link Tags => new Link("tags", "~/entities/{code}/tags{?at}");
            /// <summary>
            /// /entities/{code}/children/asset
            /// </summary>
            public static Link AssetChildren => new Link("asset_children", "~/entities/{code}/children/asset{?at}");
            /// <summary>
            /// /entities/{code}/children/service
            /// </summary>
            public static Link ServiceChildren => new Link("service_children", "~/entities/{code}/children/service{?at}");

        }
    }
}