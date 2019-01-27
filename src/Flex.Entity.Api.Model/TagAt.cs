using System;
using System.Runtime.Serialization;

namespace Flex.Entity.Api.Model
{
    [DataContract]
    public class TagAt : Tag
    {
        // ReSharper disable InconsistentNaming
        [DataMember]
        public DateTime updated_at { get; set; }
    }
}
