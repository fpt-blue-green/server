namespace BusinessObjects
{
    public class BannedUserDTO
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Reason { get; set; } = null!;

        public DateTime BanDate { get; set; }

        public DateTime? UnbanDate { get; set; }

        public Guid BannedById { get; set; }

        public virtual UserDTO BannedBy { get; set; } = null!;

    }

    public class BannedUserRequestDTO
    {
        public string Reason { get; set; } = null!;

        public EBanDate BannedTime { get; set; } = EBanDate.None;
    }
}
