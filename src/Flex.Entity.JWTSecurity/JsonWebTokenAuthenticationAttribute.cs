using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Flex.Entity.Api.CustomFilters.Authorization.Claims;
using Flex.Entity.JWTSecurity.JWTModel.Claims;
using Flex.Entity.Security;

namespace Flex.Entity.JWTSecurity
{
    //public class JsonWebTokenAuthenticationAttribute : Attribute, IAuthenticationFilter
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string SymmetricKey { get; set; }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string Audience { get; set; }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string Issuer { get; set; }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string Realm { get; set; }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public JsonWebTokenAuthenticationAttribute()
    //    {
    //        Realm = "Open Energi";

    //    }

    //    /// <summary>
    //    /// Authenticates the request.
    //    /// </summary>
    //    /// <returns>
    //    /// A Task that will perform authentication.
    //    /// </returns>
    //    /// <param name="context">The authentication context.</param><param name="cancellationToken">The token to monitor for cancellation requests.</param>
    //    public  Task AuthenticateAsync( HttpAuthenticationContext context, CancellationToken cancellationToken )
    //    {
    //        HttpRequestMessage request = context.Request;


    //        AuthenticationHeaderValue authorization = request.Headers.Authorization;

    //        if ( authorization == null )
    //        {
    //            // No authentication was attempted (for this authentication method).
    //            // Do not set either Principal (which would indicate success) or ErrorResult (indicating an error).
    //            return Task.CompletedTask;
    //        }

    //        if (!string.Equals(authorization.Scheme, "bearer", StringComparison.OrdinalIgnoreCase))
    //        {
    //            // No authentication was attempted (for this authentication method).
    //            // Do not set either Principal (which would indicate success) or ErrorResult (indicating an error).
    //            return Task.CompletedTask;
    //        }

    //        if ( String.IsNullOrEmpty(authorization.Parameter) )
    //        {
    //            // Authentication was attempted but failed. Set ErrorResult to indicate an error.
    //            context.ErrorResult = AuthenticationFailureResult.GetAuthenticationFailureResult("Missing Token", request);
    //            return Task.CompletedTask;
    //        }

    //        try
    //        {
    //            var token = authorization.Parameter;
    //            var secret = this.SymmetricKey.Replace('-', '+').Replace('_', '/');
    //            context.Principal = JsonWebToken.ValidateToken(
    //                token,
    //                secret,
    //                audience: this.Audience,
    //                checkExpiration: true,
    //                issuer: this.Issuer);

    //            if (context.Principal!=null)
    //            {

    //                if (context.Principal.Identity is ClaimsIdentity)
    //                {
    //                    var claims = GetClaimsValue(context, request);
    //                    if (claims == null)
    //                    {
    //                        context.ErrorResult = context.ErrorResult;
    //                        return Task.CompletedTask;
    //                    };
    //                }
    //            }
    //            //parse context and create the claims metadata info here rather than doing in AuthroizeFilter


    //            //if (HttpContext.Current != null)
    //            //{
    //            //    Thread.CurrentPrincipal = context.Principal;
    //            //    HttpContext.Current.User = context.Principal;
    //            //}
    //        }
    //        catch (JWT.SignatureVerificationException ex)
    //        {
    //            context.ErrorResult = AuthenticationFailureResult.GetAuthenticationFailureResult(ex.Message, request);
    //        }
    //        catch (JsonWebToken.TokenValidationException ex)
    //        {
    //            context.ErrorResult = AuthenticationFailureResult.GetAuthenticationFailureResult(ex.Message, request);
    //        }
    //        return Task.CompletedTask;
    //    }

    //    private static ClaimsIdentity GetClaimsValue(HttpAuthenticationContext context, HttpRequestMessage request)
    //    {
    //        var claims = context.Principal.Identity as ClaimsIdentity;
    //        if (claims != null)
    //        {
    //            var rootObject = TokenParser.GetClaimsRootValue(claims);
    //            if (rootObject != null)
    //            {
    //                AppMetadata appMetadata = TokenParser.GetAppMetadata(claims);
    //                if (appMetadata != null)
    //                {
    //                    //JwtClaims.SetAppMetaData(claims,appMetadata);
    //                    FlexSecurityContext repositoryClaims =TokenParser.GetSecurityContext(claims);
    //                    if (repositoryClaims != null)
    //                    {
    //                       // JwtClaims.SetRepoistoryMetaData(claims,repositoryClaims);
    //                        UserMetadata userMetadata = TokenParser.GetUserMetadata(claims);
    //                     //   if (userMetadata != null)
    //                        //    JwtClaims.SetUserMetaData(claims,userMetadata);
    //                    }
    //                    else
    //                    {
    //                        context.ErrorResult =
    //                            AuthenticationFailureResult.GetAuthenticationFailureResult(
    //                                "Invalid token : Token does not contains App Metadata and User information", request);
    //                        return null;
    //                    }
    //                }
    //                else
    //                {
    //                    context.ErrorResult =
    //                        AuthenticationFailureResult.GetAuthenticationFailureResult(
    //                            "Invalid token : Token does not contains App Metadata information", request);
    //                    return null;
    //                }
    //            }
    //            else
    //            {
    //                context.ErrorResult =
    //                    AuthenticationFailureResult.GetAuthenticationFailureResult(
    //                        "Invalid token : Token does not contains claims information", request);
    //                return null;
    //            }
    //        }
    //        return claims;
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>

    //    public Task ChallengeAsync( HttpAuthenticationChallengeContext context, CancellationToken cancellationToken )
    //    {
    //        Challenge(context);
    //        return Task.FromResult(0);
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>

    //    public virtual bool AllowMultiple
    //    {
    //        get { return false; }
    //    }


    //    private void Challenge( HttpAuthenticationChallengeContext context )
    //    {
    //        string parameter;
            
    //        if ( String.IsNullOrEmpty(Realm) )
    //        {
    //            parameter = null;
    //        }
    //        else
    //        {
    //            // A correct implementation should verify that Realm does not contain a quote character unless properly
    //            // escaped (precededed by a backslash that is not itself escaped).
    //            parameter = "realm=\"" + Realm + "\"";
    //        }


    //        context.ChallengeWith("Bearer", parameter);
    //    }
    //}
}