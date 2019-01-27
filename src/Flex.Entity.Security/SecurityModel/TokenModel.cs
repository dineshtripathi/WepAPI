using System.Collections.Generic;
using Flex.Entity.Api.CustomFilters.Authorization.Claims;

namespace Flex.Entity.JWTSecurity.JWTModel.Claims
{
    public class TokenModel
    {
        public string email { get; set; }
        public bool email_verified { get; set; }
        public string user_id { get; set; }
        public string picture { get; set; }
        public string nickname { get; set; }
        public string updated_at { get; set; }
        public string created_at { get; set; }
        public string name { get; set; }
        public UserMetadata user_metadata { get; set; }
        public AppMetadata app_metadata { get; set; }
        public Identity Identitiy { get; set; }
        public string last_ip { get; set; }
        public string last_login { get; set; }
        public int logins_count { get; set; }
        public List<object> blocked_for { get; set; }
        public List<object> guardian_enrollments { get; set; }
        public string global_client_id { get; set; }
    }
}