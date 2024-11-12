using Microsoft.AspNetCore.Mvc;

namespace BusinessObjects
{
    public class CollectionLinkRequest
    {
        public string orderInfo { get; set; }
        public string partnerCode { get; set; }
        public string redirectUrl { get; set; }
        public string ipnUrl { get; set; }
        public long amount { get; set; }
        public string orderId { get; set; }
        public string requestId { get; set; }
        public string extraData { get; set; }
        public string partnerName { get; set; }
        public string storeId { get; set; }
        public string requestType { get; set; }
        public string orderGroupId { get; set; }
        public bool autoCapture { get; set; }
        public string lang { get; set; }
        public string signature { get; set; }
    }

    public class DepositRequestDTO
    {
        public long amount { get; set; }
        public string redirectUrl { get; set; }
    }

    public class CallbackDTO
    {
        public string partnerCode { get; set; } = string.Empty;
        public string orderId { get; set; } = string.Empty;
        public string requestId { get; set; } = string.Empty;
        public long amount { get; set; }
        public string orderInfo { get; set; } = string.Empty;
        public string partnerUserId { get; set; } = string.Empty;
        public string orderType { get; set; } = "momo_wallet"; // giá trị mặc định
        public long transId { get; set; }
        public int resultCode { get; set; }
        public string message { get; set; } = string.Empty;
        public string payType { get; set; } = string.Empty;
        public long responseTime { get; set; }
        public string extraData { get; set; } = string.Empty;
        public string signature { get; set; } = string.Empty;
    }


    public class UpdatePremiumRequestDTO
    {
        public string redirectUrl { get; set; }
    }

    public class ExtraDataDTO
    {
        public Guid BrandId { get; set; }
    }
}
