using System;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using Autofac;
using Flex.Entity.JWTSecurity;
using Flex.Entity.Security;

namespace Flex.Entity.Api.CustomFilters.Authorization
{
    /// <summary>
    /// </summary>
    public class JsonWebTokenAuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        /// <summary>
        /// 
        /// </summary>
#pragma warning disable 649
        private IAuthenticationFilter _authenticationFilterImplementation;
#pragma warning restore 649

        /// <summary>
        /// 
        /// </summary>
        public IComponentContext ComponentContext;

        /// <summary>
        /// </summary>
        public IContainer Container;

        /// <summary>
        /// </summary>
        public JsonWebTokenAuthenticationAttribute(ISecurityAuthentication authentication)
        {
            Authentication = authentication;
            Realm = "Open Energi";
        }

        /// <summary>
        /// </summary>
        public ISecurityAuthentication Authentication { get; set; }

        /// <summary>
        /// </summary>
        public ILifetimeScope LifetimeScope { get; set; }

        /// <summary>
        /// </summary>
        public string SymmetricKey { get; set; }

        /// <summary>
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// </summary>
        public string Realm { get; set; }

        /// <summary>
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// </summary>
        public virtual bool AllowMultiple
        {
            get { return false; }
        }

        bool IFilter.AllowMultiple
        {
            get { return _authenticationFilterImplementation.AllowMultiple; }
        }

        async Task IAuthenticationFilter.AuthenticateAsync(HttpAuthenticationContext context,
            CancellationToken cancellationToken)
        {
            
            var requestScope = context.Request.GetDependencyScope();

            // Resolve the service you want to use.
            var lifetimeScope = requestScope.GetService(typeof(ILifetimeScope)) as ILifetimeScope;

            IPrincipal principal;

            var request = context.Request;


            var authorization = request.Headers.Authorization;

            if (authorization == null)
                return;

            if (!string.Equals(authorization.Scheme, "bearer", StringComparison.OrdinalIgnoreCase))
                return;

            if (string.IsNullOrEmpty(authorization.Parameter))
            {
                // Authentication was attempted but failed. Set ErrorResult to indicate an error.
                context.ErrorResult = AuthenticationFailureResult.GetAuthenticationFailureResult("Missing Token",
                    request);
                return; 
            }
            try
            {
                principal = await Authentication.AuthenticateTokenAsync(context.Request,
                    context.Request.Headers.Authorization.Parameter, SymmetricKey, Audience, Issuer, Domain,
                    cancellationToken).ConfigureAwait(false);
                if (principal == null)
                    context.ErrorResult =
                        AuthenticationFailureResult.GetAuthenticationFailureResult("Invalid username or password",
                            request);
                else
                    context.Principal = principal;
                if (lifetimeScope != null)
                {
                    using (var scope = lifetimeScope.BeginLifetimeScope())
                    {
                        var securityProvider = scope.Resolve<ISecurityContextProvider>();
                        securityProvider.Principal = context.Principal;
                    }

                }
                else
                {
                    context.ErrorResult = AuthenticationFailureResult.GetAuthenticationFailureResult(
                    "System Failure: server not respodning . please try after sometime or contact administrator.", request);
                }
              
            }
            catch (AuthenticationException authenticationException)
            {
                context.ErrorResult = AuthenticationFailureResult.GetAuthenticationFailureResult(
                    $"JWT Token Error: {authenticationException.ErrorCode}", authenticationException.ErrorResult.Request);
            }
            catch (Exception ex)
            {
                context.ErrorResult = AuthenticationFailureResult.GetAuthenticationFailureResult(ex.Message, request);
            }
            //return  _authenticationFilterImplementation.AuthenticateAsync(context, cancellationToken);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            Challenge(context);
            return Task.FromResult(0);
        }


        private void Challenge(HttpAuthenticationChallengeContext context)
        {
            string parameter;

            if (string.IsNullOrEmpty(Realm))
                parameter = null;
            else
                parameter = "realm=\"" + Realm + "\"";


            context.ChallengeWith("Bearer", parameter);
        }
    }
}