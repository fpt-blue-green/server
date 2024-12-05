using static BusinessObjects.AuthEnumContainer;

namespace BusinessObjects
{
    public class LoginDTO
    {
        public string Email { get; set; }
        public string? Password { get; set; }
        public EAccountProvider? Provider { get; set; }
    }
}
