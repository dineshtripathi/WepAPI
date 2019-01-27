using Flex.Entity.Api;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(SessionAuthenticationConfig), "PreAppStart")]

namespace Flex.Entity.Api
{
    /// <summary>
    /// 
    /// </summary>
    public static class SessionAuthenticationConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public static void PreAppStart()
        {
            DynamicModuleUtility.RegisterModule(typeof(System.IdentityModel.Services.SessionAuthenticationModule));
        }
    }
}