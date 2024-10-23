namespace BusinessObjects
{
    using System.ComponentModel;

    public enum EReportReason
    {
        [Description("Lý do khác.")]
        Other = 0,

        [Description("Nội dung không phù hợp.")]
        InappropriateContent = 1,

        [Description("Ngôn từ thù địch.")]
        HateSpeech = 2,

        [Description("Gian lận.")]
        Fraud = 3,

        [Description("Nội dung giả mạo.")]
        FalseInformation = 4,

        [Description("Không hoàn thành công việc.")]
        UncompletedWork = 5,

        [Description("Vi phạm điều khoản dịch vụ.")]
        ViolationOfTerms = 6,
    }


    public enum EReportStatus
    {
        Pending = 0,
        Rejected = 1,
        Approved = 2,
    }
}
