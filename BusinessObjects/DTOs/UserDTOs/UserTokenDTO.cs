using static BusinessObjects.Enum.AuthEnumContainer;

namespace BusinessObjects.DTOs.UserDTOs
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Image { get; set; }
        public ERole Role { get; set; }
        public string? AccessToken { get; set; }
        public string? refreshToken { get; set; }
    }

    public class UserDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Image { get; set; }
        public ERole Role { get; set; }
    }
}
