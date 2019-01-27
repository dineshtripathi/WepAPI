using System;
using Dapper.Extensions.Linq.Core;

namespace Flex.Entity.DapperRepository.DO
{
    public class Tag : IEntity
    {        
        public virtual int TagId { get; set; }
        public virtual int EntityId { get; set; }
        public virtual string Key { get; set; }
        public virtual string Value { get; set; }
        public virtual string UserId { get; set; }
        public virtual DateTime ValidFrom { get; set; }
        public virtual DateTime ValidTo { get; set; }
        public virtual byte[] RowVersion { get; set; }
    }
}