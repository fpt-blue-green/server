namespace BusinessObjects
{
    public enum EPaymentType
    {
        BrandPayment = 0,         // Thanh toán từ brand
        InfluencerPayment = 1,    // Thanh toán cho influencer
        Refund = 2,               // Tiền hoàn trả cho Brand
        WithDraw = 3,             // Rút tiền
        Deposit = 4,              // Nạp tiền
        BuyPremium = 5,
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
