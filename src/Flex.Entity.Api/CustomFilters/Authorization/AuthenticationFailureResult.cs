using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Flex.Entity.JWTSecurity
{
    public static class AuthenticationFailureResult
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reasonPhrase"></param>
        /// <param name="request"></param>
        public static IHttpActionResult GetAuthenticationFailureResult(string reasonPhrase, HttpRequestMessage request)
        {
            return TransformHelper.GetFailureResult(HttpStatusCode.Unauthorized, reasonPhrase, request);

        }
    }
}