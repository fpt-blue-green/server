

namespace BusinessObjects
{
    public class PaymentResponseDTO
    {
        public int TotalCount { get; set; }
        public IEnumerable<PaymentHistoryDTO> PaymentHistories { get; set; }
    }
}
