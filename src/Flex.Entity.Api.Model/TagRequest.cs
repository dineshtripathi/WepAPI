using System;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Flex.Entity.Api.Model
{
    [DataContract]
    public class TagRequest
    {
        // ReSharper disable InconsistentNaming
        [DataMember]
        
        [MaxLength(1024)]
        public string value { get; set; }
    }
}
