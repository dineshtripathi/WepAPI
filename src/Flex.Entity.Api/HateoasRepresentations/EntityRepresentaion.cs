using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Flex.Entity.Api.Model;
using WebApi.Hal;

namespace Flex.Entity.Api.HateoasRepresentations
{
    /// <summary>
    /// HATEOAS Representation for entity objects
    /// </summary>
    public sealed class EntityRepresentation : Representation
    {
        /// <summary>
        /// 
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string service_parent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string asset_parent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime updated_at { get; set; }

        private readonly DateTime? _at;
        /// <summary>
        /// Constructor
        /// </summary>
        public EntityRepresentation(DateTime? at = null)
        {
            _at = at;
        }

        /// <summary>
        /// Create the HATEOAS Links
        /// </summary>
        protected override void CreateHypermedia()
        {
            var selfLink = HatoasLinkTemplates.entity.Entity.CreateLink(new { code , at = _at?.ToString("o") });
            Href = selfLink.Href;
            Rel = selfLink.Rel;

            Links.Add(HatoasLinkTemplates.entity.Tags.CreateLink(new { code, at = _at?.ToString("o") }));

            if (asset_parent != null)
                Links.Add(HatoasLinkTemplates.entity.AssetParentEntity.CreateLink(new { asset_parent, at = _at?.ToString("o") }));
            if (service_parent != null)
                Links.Add(HatoasLinkTemplates.entity.ServicePatentEntity.CreateLink(new { service_parent, at = _at?.ToString("o") }));

            Links.Add(HatoasLinkTemplates.entity.AssetChildren.CreateLink(new { code, at = _at?.ToString("o") }));
            Links.Add(HatoasLinkTemplates.entity.ServiceChildren.CreateLink(new { code, at = _at?.ToString("o") }));

        }
    }
}