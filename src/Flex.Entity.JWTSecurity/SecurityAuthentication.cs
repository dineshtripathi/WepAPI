using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Flex.Entity.Api.CustomFilters.Authorization.Claims;
using Flex.Entity.JWTSecurity.JWTModel.Claims;
using Flex.Entity.Security;
using JWT;
using Newtonsoft.Json;

namespace Flex.Entity.JWTSecurity
{
    public class SecurityAuthentication : ISecurityAuthentication
    {

        public async Task<IPrincipal> AuthenticateTokenAsync(HttpRequestMessage request,
            string authorizationHeaderValue, string symmetricKey, string audience, string issuer,
            string domain, CancellationToken cancellationToken)
        {
            IPrincipal principal;
            ErrorResult errorResult = null;
            var token = authorizationHeaderValue;
            var secret = symmetricKey.Replace('-', '+').Replace('_', '/');
            try
            {
                principal = JsonWebToken.ValidateToken(
             token,
             secret,
             audience: audience,
             checkExpiration: true,
             issuer: issuer);

                if (principal != null)
                {

                    if (principal.Identity is ClaimsIdentity)
                    {
                        var claims = GetClaimsValue(principal.Identity, request, out errorResult);
                        if (claims == null)
                        {
                            Exception exception = new Exception("Invalid Token Format");
                            throw new AuthenticationException(exception, AuthenticationErrorCode.InvalidToken);
                        }
                    }
                }
                return await Task.FromResult<IPrincipal>(principal);
            }
            catch (JWT.SignatureVerificationException ex)
            {
                throw new AuthenticationException(new ErrorResult()
                {
                    ReasonPhrase = "Invalid Signature : Token does not valid signature information",
                    Request = request
                },ex, AuthenticationErrorCode.SignatureVerificationFailed);
              //  context.ErrorResult = AuthenticationFailureResult.GetAuthenticationFailureResult(ex.Message, request);
            }
            catch (JsonWebToken.TokenValidationException ex)
            {
                throw new AuthenticationException(new ErrorResult()
                {
                    ReasonPhrase = "Invalid token : Token does not valid Jwt Format",
                    Request = request
                },ex, AuthenticationErrorCode.InvalidToken);
             //  context.ErrorResult = AuthenticationFailureResult.GetAuthenticationFailureResult(ex.Message, request);
            }
        }
        private static ClaimsIdentity GetClaimsValue(IIdentity identity, HttpRequestMessage request,out ErrorResult result)
        {
            result = null;
            var claims = identity as ClaimsIdentity;
            if (claims != null)
            {
                var rootObject = TokenParser.GetClaimsRootValue(claims);
                if (rootObject != null)
                {
                    AppMetadata appMetadata = TokenParser.GetAppMetadata(claims);
                    if (appMetadata != null)
                    {
                        FlexSecurityContext repositoryClaims = TokenParser.GetSecurityContext(claims);
                        if (repositoryClaims == null)
                        {
                            throw new AuthenticationException(new ErrorResult()
                            {
                                ReasonPhrase = "Invalid token : Token does not contains Flex Metadata information.",
                                Request = request
                            }, AuthenticationErrorCode.InvalidToken);
                        }
                        else
                        {
                            System.Security.Claims.Claim flexsecurityClaim = new System.Security.Claims.Claim("FlexSecurityClaim", JsonConvert.SerializeObject(repositoryClaims));
                            claims.AddClaim(flexsecurityClaim);

                        }

                    }
                    else
                    {
                        throw new AuthenticationException(new ErrorResult()
                        {
                            ReasonPhrase = "Invalid token : Token does not contains App Metadata information.",
                            Request = request
                        }, AuthenticationErrorCode.InvalidToken);
                    }
                }
                else
                {
                    throw new AuthenticationException(new ErrorResult()
                    {
                        ReasonPhrase = "Invalid token : Token does not contains claims Information.",
                        Request = request
                    }, AuthenticationErrorCode.InvalidAppMetadata);
                  
                }
            }
            return claims;
        }

       
    }

   
}