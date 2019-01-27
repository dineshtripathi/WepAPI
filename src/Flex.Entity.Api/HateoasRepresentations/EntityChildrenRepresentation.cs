using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApi.Hal;

namespace Flex.Entity.Api.HateoasRepresentations
{
    /// <summary>
    /// Collection of Entities
    /// </summary>
    public class EntityChildrenRepresentation : SimpleListRepresentation<EntityRepresentation>
    {
        private readonly string _code;
        private readonly string _hierarchy;
        private readonly DateTime? _at;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="code"></param>
        /// <param name="hierarchy"></param>
        /// <param name="at"></param>
        public EntityChildrenRepresentation(IList<EntityRepresentation> list, string code, string hierarchy, DateTime? at) :base(list)
        {
            _code = code;
            _hierarchy = hierarchy;
            _at = at;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void CreateHypermedia()
        {
            Link selfLink;
            if (_hierarchy == "asset")
                selfLink = HatoasLinkTemplates.entitiyChildren.AssetChildren.CreateLink( new { code = _code, at = _at?.ToString("o") });
            else
                selfLink = HatoasLinkTemplates.entitiyChildren.ServiceChildren.CreateLink(new { code = _code, at = _at?.ToString("o") });
            Href = selfLink.Href;
            Rel = selfLink.Rel;
        }

    }
}