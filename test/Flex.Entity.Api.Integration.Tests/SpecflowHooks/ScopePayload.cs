using System;

namespace Flex.Entity.Api.Helpers
{
    public class ScopePayload
    {
        public string Root { get; set; }
        public Int32? Permission { get; set; }
    }
    public class Asset:ScopePayload   { }
    public class Service : ScopePayload { }
}