using Flex.Entity.Api.CustomFilters.Authorization.Claims;

namespace Flex.Entity.JWTSecurity.JWTModel.Claims
{
    public class AppMetadata
    {
        public Claim asset { get; set; }
        public Claim service { get; set; }
    }
}
