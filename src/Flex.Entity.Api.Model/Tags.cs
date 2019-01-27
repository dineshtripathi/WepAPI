using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Flex.Entity.Api.Model
{
    [DataContract]
    public class Tags
    {
        // ReSharper disable InconsistentNaming
        [DataMember]
        public IEnumerable<TagAt> tags { get; set; }
    }
}
