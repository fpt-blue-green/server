namespace BusinessObjects
{
    public class PaymentHistoryDTO
    {
        public Guid Id { get; set; }

        public Guid? UserId { get; set; }

        public decimal? Amount { get; set; }

        public int? Status { get; set; }

        public int? Type { get; set; }

        public DateTime? Date { get; set; }

        public string? Description { get; set; }

        public UserDTO User { get; set; }
    }
}
