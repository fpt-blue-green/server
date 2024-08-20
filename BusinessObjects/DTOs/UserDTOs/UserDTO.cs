using BusinessObjects.Models;

namespace BusinessObjects.DTOs.UserDTOs
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int Role { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsBanned { get; set; }
        public string? DisplayName { get; set; }
        public UserDTO() { }

        public UserDTO(User user)
        {
            Id = user.Id;
            Password = user.Password;
            Email = user.Email;
            Role = user.Role;
            CreatedAt = user.CreatedAt;
            ModifiedAt = user.ModifiedAt;
            IsDeleted = user.IsDeleted;
            IsBanned = user.IsBanned;
            DisplayName = user.DisplayName;
        }
    }
}
