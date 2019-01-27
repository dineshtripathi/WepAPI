using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Flex.Entity.Api.CustomFilters.Authorization.Claims;
using Flex.Entity.JWTSecurity.JWTModel.Claims;

namespace Flex.Entity.Security
{
    public interface ISecurityContextProvider
    {

        IPrincipal Principal { get; set; }
        FlexSecurityContext GetClaims();
     }
}
