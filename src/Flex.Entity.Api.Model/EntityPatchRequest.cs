using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Flex.Entity.Api.Model
{
    [DataContract]
    public class EntityPatchRequest
    {

        [DataMember]
        [MaxLength(255)]
        [RegularExpression(@"^(\w|\d|\s)+$")]
        public string name { get; set; }


        [DataMember]
        [MaxLength(10)]
        [RegularExpression(@"^[a-zA-Z]{0,2}\d{0,8}$")]
        public string service_parent { get; set; }

        [DataMember]
        [MaxLength(10)]
        [RegularExpression(@"^[a-zA-Z]{0,2}\d{0,8}$")]
        public string asset_parent { get; set; }

    }
}
