using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace Flex.Entity.Security
{
    public  interface ISecurityAuthentication
    {

        Task<IPrincipal> AuthenticateTokenAsync( HttpRequestMessage request, string authorizationHeaderValue, string symmetricKey,string audience,string issuer,string domain,CancellationToken cancellationToken);
    }
   
}
