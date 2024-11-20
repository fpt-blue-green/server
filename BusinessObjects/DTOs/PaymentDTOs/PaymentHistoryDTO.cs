namespace BusinessObjects
{
    public class PaymentHistoryDTO
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public decimal Amount { get; set; }

        public decimal? NetAmount { get; set; }

        public string BankInformation { get; set; }

        public int? Status { get; set; }

        public int Type { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ResponseAt { get; set; }

        public string? AdminMessage { get; set; }

        public UserDTO User { get; set; }
    }
}
