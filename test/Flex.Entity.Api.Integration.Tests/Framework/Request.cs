using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Flex.Entity.Api.Integration.Tests.Framework
{
    public class Request
    {
        public string RelativeUrl { get; set; }

        public Uri RelativeUri => new Uri(RelativeUrl,UriKind.Relative);
        public Uri BaseUrl { get; } = (Uri)FeatureContext.Current["AUT"];

        public string Accept { get; set; }
        public string Authorization { get; set; }
        public RequestMethod HttpMethod { get; set; }
    }
}
