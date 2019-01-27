using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Auth0.Core;
using Flex.Entity.Api.CustomFilters.Authorization.Claims;
using Flex.Entity.Api.Helpers;
using Flex.Entity.JWTSecurity;
using Flex.Entity.JWTSecurity.JWTModel.Claims;
using Flex.Entity.Security;
using Newtonsoft.Json;

namespace Flex.Entity.Api.CustomFilters.Authorization
{
    /// <summary>
    /// 
    /// </summary>
    public class FlexClaimsPrincipal : ClaimsPrincipal
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        public FlexClaimsPrincipal(string userName)
        {
            GenericIdentity userGenericIdentity = new GenericIdentity(userName, "Custom Authentication");
            ClaimsIdentity ci = new ClaimsIdentity(userGenericIdentity);
            AddIdentity(ci);
        }

        private static ClaimsPrincipal CreateClaimsPrincipal()
        {
            string userName = Thread.CurrentPrincipal.Identity.Name;
            FlexClaimsPrincipal cp = new FlexClaimsPrincipal(userName);
            Thread.CurrentPrincipal = cp;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = cp;
            }
            return cp;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum Root
    {
        /// <summary>
        /// 
        /// </summary>
        Oe = 0,

        /// <summary>
        /// 
        /// </summary>
        Jsb = 1,

        /// <summary>
        /// 
        /// </summary>
        Entities = 2,

        /// <summary>
        /// 
        /// </summary>
        Decendants = 3,
    }

    //public class AuthorizeClaimsAttribute : AuthorizeAttribute
    //{

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public ISecurityContextProvider SecurityContextProvider;
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="assetPermission"></param>
    //    /// <param name="servicePermission"></param>
    //    public AuthorizeClaimsAttribute(PermissionMask assetPermission, PermissionMask servicePermission)
    //    {
    //        AssetPermission = assetPermission;
    //        ServicePermission = servicePermission;
    //    }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public readonly List<string> ClaimTypes = new List<string>();

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public Root ValidRoot { get; set; }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public PermissionMask? AssetPermission { get; set; }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public PermissionMask? ServicePermission { get; set; }

    //    /// <summary>Processes requests that fail authorization.</summary>
    //    /// <param name="actionContext">The context.</param>
    //    protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
    //    {
    //        actionContext.Response = new HttpResponseMessage()
    //        {
    //            StatusCode = HttpStatusCode.Forbidden,
    //            Content =
    //                new StringContent(
    //                    "Sorry,you do not have enough permission to peferom this operation.Contact OE Admin")
    //        };
    //        base.HandleUnauthorizedRequest(actionContext);
    //    }
        
    //    /// <summary>Indicates whether the specified control is authorized.</summary>
    //    /// <returns>true if the control is authorized; otherwise, false.</returns>
    //    /// <param name="actionContext">The context.</param>
    //    protected override bool IsAuthorized(HttpActionContext actionContext)
    //    {
    //        ClaimsPrincipal claimsPrincipal =
    //            new FlexClaimsPrincipal(actionContext.RequestContext.Principal.Identity.Name);

    //        if (!claimsPrincipal.Identity.IsAuthenticated)
    //        {
    //            return false;
    //        }

    //        // Will do some additional test and coding to get all the claims or will remove it 
    //        /*
    //        var hasAllClaims =
    //            ClaimTypes.All(
    //                type =>
    //                    claimsPrincipal.HasClaim(
    //                        claim => claim.Type.Equals(type, StringComparison.InvariantCultureIgnoreCase)));

    //        */
    //        var httpContext = HttpContext.Current.User;
    //        if (httpContext.Identity is ClaimsIdentity)
    //        {
    //            FlexSecurityContext repositoryClaims = SecurityContextProvider.GetSecurityContext();
    //            if (repositoryClaims != null)
    //            {
                
    //                {
    //                    bool AssetAuthorized = false;
    //                    bool ServiceAuthorized = false;
    //                    if (repositoryClaims.TokenRepository != null)
    //                    {
    //                        if (ServicePermission != null)
    //                        {
    //                            if (repositoryClaims.TokenRepository.service != null)
    //                            {
    //                                var serviceMask =
    //                                    BitMaskChecker.PermissionMasksV2<PermissionMask>(
    //                                        (PermissionMask)
    //                                        Enum.ToObject(typeof(PermissionMask),
    //                                            (repositoryClaims.TokenRepository.service.permission)));


    //                                if (serviceMask.Any(sm => ServicePermission.Value.HasFlag(sm) || sm == PermissionMask.FullAccess))
    //                                {
    //                                    ServiceAuthorized = true;
    //                                }
    //                            }
    //                        }
    //                        else ServiceAuthorized = true;
    //                        if (AssetPermission != null)
    //                        {
    //                            if (repositoryClaims.TokenRepository.asset != null)
    //                            {
    //                                var assetMask =
    //                                    BitMaskChecker.PermissionMasksV2<PermissionMask>(
    //                                        (PermissionMask)
    //                                        Enum.ToObject(typeof(PermissionMask),
    //                                            value: (repositoryClaims.TokenRepository.asset.permission)));


    //                                if (assetMask.Any(sm => AssetPermission.Value.HasFlag(sm) || sm == PermissionMask.FullAccess))
    //                                {
    //                                    AssetAuthorized = true;
    //                                }
    //                            }
    //                        }
    //                        else AssetAuthorized = true;
    //                    }
    //                    if (ServiceAuthorized == true && AssetAuthorized == true)
    //                        return base.IsAuthorized(actionContext);
    //                    return false;
    //                }
    //            }

    //            else
    //            {

    //                return false;
    //            }
    //        }

    //        return false;
    //    }

    //    //private bool IsPermissionMatched(PermissionMask maskValue, string claimsPermissionsString)
    //    //{
    //    //    bool flag = false;
    //    // //   foreach (var claimsPermission in claimsPermissionsString)
    //    //    {
    //    //        flag = Flag(maskValue, Convert.ToInt32(claimsPermissionsString, 16), flag);
    //    //    }
    //    //    return flag;
    //    //}

    //    //private bool IsPermissionMatched(PermissionMask maskValue, int claimsPermissionsDecimal)
    //    //{
    //    //    bool flag = false;
    //    // //   foreach (var claimsPermission in claimsPermissionsDecimal)
    //    //    {
    //    //        flag = Flag(maskValue, claimsPermissionsDecimal, flag);
    //    //    }
    //    //    return flag;
    //    //}

    //    ////Needs to be removed after doing some testing
    //    //private static bool Flag(PermissionMask maskValue, int claimsPermission, bool flag)
    //    //{
    //    //    if (maskValue.HasFlag(PermissionMask.Execute)
    //    //        &&
    //    //        ((maskValue & PermissionMask.Execute) ==
    //    //         (PermissionMask) Enum.ToObject(typeof(PermissionMask), (claimsPermission))))
    //    //    {
    //    //        flag = true;
    //    //    }
    //    //    if (maskValue.HasFlag(PermissionMask.Read)
    //    //        &&
    //    //        ((maskValue & PermissionMask.Read) ==
    //    //         (PermissionMask) Enum.ToObject(typeof(PermissionMask), (claimsPermission))))
    //    //    {
    //    //        flag = true;
    //    //    }
    //    //    if (maskValue.HasFlag(PermissionMask.Create)
    //    //        &&
    //    //        ((maskValue & PermissionMask.Create) ==
    //    //         (PermissionMask) Enum.ToObject(typeof(PermissionMask), (claimsPermission))))
    //    //    {
    //    //        flag = true;
    //    //    }
    //    //    if (maskValue.HasFlag(PermissionMask.Update)
    //    //        &&
    //    //        ((maskValue & PermissionMask.Update) ==
    //    //         (PermissionMask) Enum.ToObject(typeof(PermissionMask), (claimsPermission))))
    //    //    {
    //    //        flag = true;
    //    //    }
    //    //    if (!maskValue.HasFlag(PermissionMask.DenyAll) && ( PermissionMask.FullAccess == (PermissionMask) Enum.ToObject(typeof(PermissionMask), (claimsPermission))))
    //    //    {
    //    //        flag = true;
    //    //    }

    //    //    if (maskValue.HasFlag(PermissionMask.DenyAll)
    //    //        &&
    //    //        ((maskValue & PermissionMask.DenyAll) ==
    //    //         (PermissionMask) Enum.ToObject(typeof(PermissionMask), (claimsPermission))))
    //    //    {
    //    //        flag = true;
    //    //    }
    //    //    return flag;
    //    //}

     
    //}
}