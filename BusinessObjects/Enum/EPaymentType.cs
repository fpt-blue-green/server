namespace BusinessObjects
{
    public enum EPaymentType
    {
        BrandPayment = 0,         // Thanh toán từ brand
        InfluencerPayment = 1,    // Thanh toán cho influencer
        Refund = 3,               // Tiền hoàn trả cho Brand
        WithDraw = 4,             // Rút tiền
        Deposit = 5,              // Nạp tiền
        BuyPremium = 6,
    }

    public enum EPaymentStatus
    {
        Pending = 0,   // Đang chờ xử lý
        Rejected = 1,  // Bị từ chối
        Done = 2,      // Đã phê duyệt
        Error = 3,
    }

    public enum EPayType 
    {
        qr = 0,
        webApp = 1,
        credit = 2,
        napas = 3,
    }
}
