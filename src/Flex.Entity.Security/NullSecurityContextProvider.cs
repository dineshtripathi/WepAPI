using System.Security.Principal;
using Autofac;

namespace Flex.Entity.Security
{
    public class NullSecurityContextProvider : ISecurityContextProvider
    {

        public NullSecurityContextProvider(IPrincipal principal)
        {
            Principal = principal;
        }

        public IPrincipal Principal { get; set; }
        public ILifetimeScope LifetimeScope { get; set; }

        public FlexSecurityContext GetClaims()
        {
            return new FlexSecurityContext { UserId = "unknown@oe.com" };
        }
    }

   
  
}
