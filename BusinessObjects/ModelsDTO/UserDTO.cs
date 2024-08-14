using BusinessObjects.Models;
using System.Text.Json.Serialization;

namespace BusinessObjects.ModelsDTO
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

        public virtual ICollection<Influencer> Influencers { get; set; } = new List<Influencer>();

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
            Influencers = user.Influencers;
        }
    }
}
