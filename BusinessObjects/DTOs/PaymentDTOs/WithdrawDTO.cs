namespace BusinessObjects
{
    public class WithdrawDTO
    {
        public int Amount {  get; set; }

        public string BankNumber { get; set; }
        public EBankName BankName { get; set;}
    }
}
