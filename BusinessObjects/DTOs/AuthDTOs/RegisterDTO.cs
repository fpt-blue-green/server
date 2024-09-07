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
}
