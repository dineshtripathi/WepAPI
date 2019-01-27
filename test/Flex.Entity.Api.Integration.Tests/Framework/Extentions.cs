using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Entity.Api.Integration.Tests.Framework
{
    public static class Extensions
    {
        public static string GetBaseUrl(this Uri uri)
        {
            return uri.Scheme + "://" + uri.Authority +
                   ((uri.IsDefaultPort) ? "/" : ":" + uri.Port.ToString() + "/");
        }
    }
}
