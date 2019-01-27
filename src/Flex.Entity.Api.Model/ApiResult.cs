using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Entity.Api.Model
{
    [DataContract]
    public class ApiResult
    {
        // ReSharper disable InconsistentNaming

        [DataMember]
        public bool success { get; set; }
        [DataMember]
        public string error { get; set; }
    }
}
