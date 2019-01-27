using System;

namespace Flex.Entity.DapperRepository.DO
{
    public class Entity 
    {        
        public virtual int EntityId { get; set; }
        public virtual int EntityTypeId { get; set; }
        public virtual string TypePrefix { get; set; }
        public virtual string Type { get; set; }
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }
        public virtual string AssetParentCode { get; set; }
        public virtual string ServiceParentCode { get; set; }
        public virtual DateTime ValidFrom { get; set; }
        public virtual DateTime ValidTo { get; set; }
    }
}
