namespace BusinessObjects
{
    public class WithdrawRequestDTO
    {
        public decimal Amount {  get; set; }
        public string BankId { get; set; }
        public string AccountNo { get; set;}
    }

    public class AdminPaymentResponse
    {
        public bool IsApprove { get; set; } = true;
        public string? AdminMessage { get; set; }
    }
}
