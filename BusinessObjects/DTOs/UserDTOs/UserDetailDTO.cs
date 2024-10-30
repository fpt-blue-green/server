

namespace BusinessObjects
{
    public class UserDetailDTO
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = null!;

        public string? DisplayName { get; set; }

        public string? Avatar { get; set; }

        public int Role { get; set; }

        public decimal Wallet { get; set; }

        public int Provider { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsBanned { get; set; }
    }
}
