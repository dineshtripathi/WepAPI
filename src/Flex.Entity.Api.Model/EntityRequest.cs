using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Flex.Entity.Api.Model
{
    [DataContract]
    public class EntityRequest: EntityPatchRequest
    {

        [DataMember]
        [MaxLength(2)]
        [RegularExpression(@"^[a-zA-Z]{1,2}$")]
        public string typePrefix { get; set; }


    }
}
