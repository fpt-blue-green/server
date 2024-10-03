namespace BusinessObjects
{

    public static class JobEnumContainer
    {
        public enum EJobStatus
        {
            Pending = 0,          // Offer đang được xem xét
            InProgress = 1,       // Job đang được thực hiện
            Completed = 2,        // Job đã hoàn thành
            Failed = 3,           // Job thất bại
            NotCreated = 4,       // Job không thành công do không có offer nào được đồng ý
        }


        public enum EOfferStatus
        {
            Offering = 0,          // Offer đã được gửi và đang chờ xử lý
            Rejected = 1,          // Offer đã bị từ chối và không có re-offer
            WaitingPayment = 2,    // Offer đã được chấp nhận và đang đợi Brand thanh toán
            Cancelled = 3,         // Offer đã được chấp nhận nhưng Brand từ chối thanh toán
            Expired = 4,           // Offer đã hết hạn khi không có sự thỏa thuận giữa hai bên (> 10 ngay)
            Done = 5,              // Offer đã được Brand thanh toán thành công
        }
    }
}
