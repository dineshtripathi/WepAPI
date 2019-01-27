using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Flex.Entity.Api.Model
{
    [DataContract]
    public class TagType
    {
        // ReSharper disable InconsistentNaming
        [DataMember]
        [Required]
        [MaxLength(64)]
        [RegularExpression(@"^[a-zA-Z0-9]{1,64}$")]
        public string key { get; set; }
    }
}
