using System.Runtime.Serialization;

namespace Flex.Entity.Api.Model
{
    [DataContract]
    public class EntityDetail : EntityAt
    {
        [DataMember]
        public string asset_children { get; set; }
        [DataMember]
        public string service_children { get; set; }
        [DataMember]
        public string tags { get; set; }
    }
}