namespace BusinessObjects
{
    public enum EReportReason
    {
        Other = 0,                // Lý do khác: Lý do không nằm trong các mục đã liệt kê.
        InappropriateContent = 1, // Nội dung không phù hợp: Nội dung gây khó chịu hoặc không phù hợp với cộng đồng.
        HateSpeech = 2,           // Ngôn từ thù địch: Sử dụng ngôn từ phân biệt hoặc thù địch.
        Fraud = 3,                // Gian lận: Hành động lừa đảo hoặc gian lận với người dùng.
        FalseInformation = 4,     // Nội dung giả mạo: Phát tán thông tin sai lệch.
        UncompletedWork = 5,      // Không hoàn thành công việc: Không thực hiện nhiệm vụ đã nhận.
        ViolationOfTerms = 6,     // Vi phạm điều khoản dịch vụ: Không tuân thủ quy định của nền tảng.
    }
}
