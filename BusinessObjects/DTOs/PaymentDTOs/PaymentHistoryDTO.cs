namespace BusinessObjects
{
    public class PaymentHistoryDTO
    {
        public Guid Id { get; set; }

        public Guid? UserId { get; set; }

        public decimal? Amount { get; set; }

        public string BankInformation { get; set; }

        public EPaymentStatus? Status { get; set; }

        public EPaymentType Type { get; set; }

        public DateTime? Date { get; set; }

        public string? AdminMessage { get; set; }

        public UserDTO User { get; set; }
    }
}
