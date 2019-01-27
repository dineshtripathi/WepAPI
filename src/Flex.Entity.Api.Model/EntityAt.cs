using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Flex.Entity.Api.Model
{
    [DataContract]
    public class EntityAt: Entity
    {
        [DataMember]
        public DateTime updated_at { get; set; }
    }
}
