using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Flex.Entity.Api.Model
{
    [DataContract]
    public class Entity: EntityPatchRequest
    {
        [DataMember]
        [MaxLength(10)]
        [RegularExpression(@"^[a-zA-Z]{0,2}\d{0,8}$")]
        public string code { get; set; }
        [DataMember]
        public string type { get; set; }
    }
}
