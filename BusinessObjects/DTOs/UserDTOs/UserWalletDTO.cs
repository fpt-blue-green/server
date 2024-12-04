namespace BusinessObjects
{
    public class UserWalletDTO
    {
        public decimal CurrentAmount { get; set; }
        public decimal? SpendAmount { get; set; }
        public DateTime? PremiumTimeExpired { get; set; }
	}

    public class UserPaymentDTO
    {
        public DateTime? Created { get; set; }
        public decimal Amount { get; set;}
        public EPaymentStatus Status { get; set; }
        public EPaymentType Type { get; set; }
    }
}
