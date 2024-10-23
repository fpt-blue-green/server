using static BusinessObjects.AuthEnumContainer;

namespace BusinessObjects
{
    public class RegisterDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public ERole Role { get; set; }
    }

    public class RegisterThirdPartyDTO
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public ERole Role { get; set; }
        public string Image {  get; set; }
        public EAccountProvider AccountProvider { get; set; } = EAccountProvider.AdFusionAccount;
    }
}
