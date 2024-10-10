namespace BusinessObjects
{
    public enum ECampaignStatus
    {
        Draft = 0,        // Campaign đang trong quá trình chuẩn bị.
        Published = 1,    // Campaign đang trong quá trình tuyển thành viên, chưa bắt đầu thực hiện
        Active = 2,       // Campaign đang diễn ra, đã bắt đầu nhưng chưa kết thúc. Phải do chủ Campaign bấm nút bắt đầu và có ít nhất 1 Job Inprogress.
        Completed = 3,    // Campaign đã kết thúc thành công. Phải do chủ Campaign bấm nút kết thúc hoặc các mục tiêu đều đã hoàn thành.
        Expired = 4       // Campaign đã hết hạn theo thời gian đặt ra mà chưa hoàn thành.
    }
}
