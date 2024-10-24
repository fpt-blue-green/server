using System.ComponentModel;

namespace BusinessObjects
{
    public static class JobEnumContainer
    {
        public enum EJobStatus
        {
            [Description("Đang chờ xử lý")]
            Pending = 0,          // Job đang trong quá trình khởi tạo

            [Description("Đang triển khai")]
            InProgress = 1,       // Job đang được thực hiện

            [Description("Đã hoàn thành")]
            Completed = 2,        // Job đã hoàn thành

            [Description("Bị thất bại")]
            Failed = 3,           // Job thất bại

            [Description("Bị hủy bỏ")]
            NotCreated = 4,       // Job không thành công do không có offer nào được đồng ý
        }

        public enum EOfferStatus
        {
            [Description("Đang chờ xử lý")] // Offering
            Offering = 0,          // Offer đã được gửi và đang chờ xử lý

            [Description("Từ chối")] // Rejected
            Rejected = 1,          // Offer đã bị từ chối và không có re-offer

            [Description("Chấp thuận")] // WaitingPayment
            WaitingPayment = 2,    // Offer đã được chấp nhận và đang đợi Brand thanh toán

            [Description("Đã hủy")] // Cancelled
            Cancelled = 3,         // Offer đã được chấp nhận nhưng Brand từ chối thanh toán

            [Description("Đã hết hạn")] // Expired
            Expired = 4,           // Offer đã hết hạn khi không có sự thỏa thuận giữa hai bên (> 10 ngày)

            [Description("Đã thanh toán")] // Done
            Done = 5               // Offer đã được Brand thanh toán thành công
        }
    }
}
