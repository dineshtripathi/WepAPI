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
    public class EntitiesRepresentation : SimpleListRepresentation<EntityRepresentation>
    {
        private readonly DateTime? _at;
        /// <summary>
        /// total number of pages for current top value
        /// </summary>
        public int page_count { get; set; }
        /// <summary>
        /// current page number base don current top value
        /// </summary>
        public int page_number { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="at"></param>
        /// <param name="pageCount"></param>
        /// <param name="pageNumber"></param>
        public EntitiesRepresentation(IList<EntityRepresentation> list, DateTime? at,int pageCount, int pageNumber) : base(list)
        {
            _at = at;
            page_count = pageCount;
            page_number = pageNumber;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void CreateHypermedia()
        {
            var selfLink = HatoasLinkTemplates.entities.GetEntities.CreateLink(new {at = _at?.ToString("o")});
            Href = selfLink.Href;
            Rel = selfLink.Rel;
        }

    }
}