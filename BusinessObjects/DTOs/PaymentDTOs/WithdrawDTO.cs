namespace BusinessObjects
{
    public class WithdrawRequestDTO
    {
        public decimal Amount {  get; set; }
        public string BankNumber { get; set; }
        public EBankName BankName { get; set;}
    }

    public class AdminPaymentResponse
    {
        public bool IsApprove { get; set; } = true;
        public string? AdminMessage { get; set; }
    }
}
