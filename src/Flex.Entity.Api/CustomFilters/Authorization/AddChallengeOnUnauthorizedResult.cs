using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Flex.Entity.JWTSecurity
{
    /// </summary>
    public class AddChallengeOnUnauthorizedResult : IHttpActionResult
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="challenge"></param>
        /// <param name="innerResult"></param>
        public AddChallengeOnUnauthorizedResult(AuthenticationHeaderValue challenge, IHttpActionResult innerResult)
        {
            Challenge = challenge;
            InnerResult = innerResult;
        }

        /// <summary>
        /// 
        /// </summary>
        public AuthenticationHeaderValue Challenge { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IHttpActionResult InnerResult { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await InnerResult.ExecuteAsync(cancellationToken).ConfigureAwait(false);
            response = await TransformHelper.TransformHttpErrorToApiResult(HttpStatusCode.Unauthorized, response, cancellationToken).ConfigureAwait(false);
            // Only add one challenge per authentication scheme, if sending back an unauthorized response
            if (response.StatusCode == HttpStatusCode.Unauthorized && response.Headers.WwwAuthenticate.All(h => h.Scheme != Challenge.Scheme))
            {
                response.Headers.WwwAuthenticate.Add(Challenge);
            }

            return response;
        }

    }
}