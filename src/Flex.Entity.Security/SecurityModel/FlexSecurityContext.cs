namespace Flex.Entity.Security
{
    public class FlexSecurityContext
    {
        public string UserName { get; set; }
        public string EmailId { get; set; }
        public string UserId { get; set; }
        public string NickName { get; set; }
        public int AssetPermission { get; set; }
        public string AssetRoot { get; set; }
        public int ServicePermission { get; set; }
        public string ServiceRoot { get; set; }
    }
}
