using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using Flex.Entity.Api.App_Start;
using Flex.Entity.Api.CustomFilters;
using Flex.Entity.Api.CustomFilters.Authorization;
using Flex.Entity.JWTSecurity;

namespace Flex.Entity.Api.App_Start
{
    /// <summary>
    /// 
    /// </summary>
    public class JsonWebTokenConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            config.EnableCors();
            var audience = WebConfigurationManager.AppSettings["auth0:ClientId"];
            var symmetricKey = WebConfigurationManager.AppSettings["auth0:ClientSecret"];
            var issuer = WebConfigurationManager.AppSettings["auth0:Issuer"];
            if (string.IsNullOrWhiteSpace(issuer))
                issuer = null;
            config.Filters.Add(new JsonWebTokenAuthenticationAttribute(new SecurityAuthentication()) {Audience = audience, SymmetricKey = symmetricKey, Issuer = issuer});
        }
    }
}
