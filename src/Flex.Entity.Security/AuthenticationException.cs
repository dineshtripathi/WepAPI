using System;

namespace Flex.Entity.Security
{
    public class AuthenticationException:ApplicationException
    {
       public AuthenticationErrorCode ErrorCode { get; protected set; }
        public ErrorResult ErrorResult { get; protected set; }
        public AuthenticationException(Exception innerException, AuthenticationErrorCode code = AuthenticationErrorCode.AuthorizationFailure) : base("Authorization Failure ,Invalid Token", innerException)
        {
            ErrorCode = code;
            
        }
        public AuthenticationException(ErrorResult errorResult, Exception innerException, AuthenticationErrorCode code = AuthenticationErrorCode.AuthorizationFailure) : base(errorResult.ReasonPhrase, innerException)
        {
            ErrorCode = code;
            ErrorResult = errorResult;
        }
        public AuthenticationException(ErrorResult errorResult, AuthenticationErrorCode code = AuthenticationErrorCode.AuthorizationFailure) : base(errorResult.ReasonPhrase)
        {
            ErrorCode = code;
            ErrorResult = errorResult;
        }


    }
}
