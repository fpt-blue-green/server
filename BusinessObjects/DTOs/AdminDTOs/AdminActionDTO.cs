using BusinessObjects.Models;

namespace BusinessObjects
{
    public class AdminActionDTO
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public EAdminActionType ActionType { get; set; }

        public string ActionDetails { get; set; } = null!;

        public DateTime ActionDate { get; set; }

        public string ObjectType { get; set; } = null!;

        public virtual UserDTO User { get; set; } = null!;
    }
}
