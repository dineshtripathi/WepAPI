using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Flex.Entity.Api.Model
{
    [DataContract]
    public class Tag
    {
        // ReSharper disable InconsistentNaming
        
        [DataMember]
        [MaxLength(64)]
        [RegularExpression(@"^([a-z]|\d|_)+$")]
        public string key { get; set; }

        [DataMember]
        [Required]
        [MaxLength(1024)]

        public string value { get; set; }

     
    }
}