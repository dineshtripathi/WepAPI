using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Flex.Entity.Api.Model
{
    [DataContract]
    public class EntityType
    {
        // ReSharper disable InconsistentNaming
        [DataMember]
        [Required]
        [MaxLength(2)]
        [RegularExpression(@"^[a-zA-Z]{1,2}$")]
        public string prefix { get; set; }
        [DataMember]
        [Required]
        [MaxLength(50)]
        public string name { get; set; }
        [DataMember]
        public bool allow_in_asset_hierarchy { get; set; }
        [DataMember]
        public bool allow_in_service_hierarchy { get; set; }
        [DataMember]
        public bool allow_same_type_descendant { get; set; }

    }
}
