using System.Security.Claims;
using System.Security.Principal;
using Flex.Entity.Security;

namespace Flex.Entity.JWTSecurity
{
    /// <summary>
    /// 
    /// </summary>
    public class SecurityContextProvider : ISecurityContextProvider
    {
        public SecurityContextProvider(IPrincipal httpContext)
        {
            Principal = httpContext;
        }

        public IPrincipal Principal { get; set; }

        public FlexSecurityContext GetClaims()
        {
           
            FlexSecurityContext context = TokenParser.GetSecurityContext("FlexSecurityClaim",Principal.Identity as ClaimsIdentity);
            return context;
        }

    }
}
