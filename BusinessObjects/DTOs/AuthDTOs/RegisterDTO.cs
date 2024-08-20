using static BusinessObjects.Enum.AuthEnumContainer;

namespace BusinessObjects.DTOs.AuthDTO
{
    public class RegisterDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public ERole Role { get; set; }
    }
}
