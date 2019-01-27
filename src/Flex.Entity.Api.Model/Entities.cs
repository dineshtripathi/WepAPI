using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Flex.Entity.Api.Model
{
    [DataContract]
    public class Entities 
    {
        [DataMember]
        public IEnumerable<EntityDetail> entities { get; set; }
        [DataMember]
        public int count { get; set; }
    }
}