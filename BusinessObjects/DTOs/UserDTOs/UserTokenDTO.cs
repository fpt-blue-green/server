using static BusinessObjects.AuthEnumContainer;

namespace BusinessObjects
{
    public class UserTokenDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Image { get; set; }
        public ERole Role { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }

    public class UserDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Image { get; set; }
        public ERole Role { get; set; }
    }

    public class UserDetailDTO
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = null!;

        public string? DisplayName { get; set; }

        public string? Avatar { get; set; }

        public int Role { get; set; }

        public int Wallet { get; set; }

        public int Provider { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsBanned { get; set; }
    }
}
