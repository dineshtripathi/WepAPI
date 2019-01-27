namespace Flex.Entity.JWTSecurity.JWTModel.Claims
{
    public class Identity
    {
        public string user_id { get; set; }
        public string provider { get; set; }
        public string connection { get; set; }
        public bool isSocial { get; set; }
    }
}