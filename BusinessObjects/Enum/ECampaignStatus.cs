namespace BusinessObjects
{
    public enum ECampaignStatus
    {
        Draft = 0,        // Campaign đang trong quá trình tuyển thành viên, chưa bắt đầu thực hiện.
        Published = 1,    // Campaign đã có offer được thanh toán thành công
        Active = 2,       // Campaign đang diễn ra, đã bắt đầu nhưng chưa kết thúc. Phải do chủ Campaign bấm nút bắt đầu
        Completed = 3,    // Campaign đã kết thúc thành công. Phải do chủ Campaign bấm nút kết thúc hoặc các mục tiêu đều đã hoàn thành.
        Expired = 4       // Campaign đã hết hạn theo thời gian đặt ra mà chưa hoàn thành.
    }
}
