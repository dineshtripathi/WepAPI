using Dapper.Extensions.Linq.Core;

namespace Flex.Entity.DapperRepository.DO
{
    public class EntityType : IEntity
    {        
        public virtual int EntityTypeId { get; set; }
        public virtual string Prefix { get; set; }
        public virtual string Name { get; set; }
        public virtual bool IsAllowedAsAssetNode { get; set; }
        public virtual bool IsAllowedAsServiceNode { get; set; }
        public virtual bool IsAllowedSameDescendantNode { get; set; }
    }
}
