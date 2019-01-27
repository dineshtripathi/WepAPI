namespace Flex.Entity.Security
{
    public enum AuthenticationErrorCode
    {
        InvalidToken= 0,
        InvalidAppMetadata = 1,
        InvalidUserMetadata = 2,
        TokenExpired = 3,
        ChallengeFailed = 4,
        AnnonymousUser = 5,
        AuthorizationFailure = 6,
        SignatureVerificationFailed=7,
        InvalidKey = 255
    }
}