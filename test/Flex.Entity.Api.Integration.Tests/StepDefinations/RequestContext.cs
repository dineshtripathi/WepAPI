using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flex.Entity.Api.Integration.Tests.Framework;

namespace Flex.Entity.Api.Integration.Tests.StepDefinations
{
    public class RequestContext
    {
        public Request RequestTable { get; set; }
        public string CacheDataForAsserts { get; set; }
        public string QueryStringParams { get; set; }
        public string Code { get; set; }
        public object Body { get; set; }
    }
}
