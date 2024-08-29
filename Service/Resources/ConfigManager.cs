namespace Service.Domain
{
    public class ConfigManager
    {
        public readonly string ProjectName = "AdFusion";
        public readonly string SeverErrorMessage = "Yêu cầu của bạn không thể hoàn thành tại thời điểm này. Vui lòng liên hệ với bộ phận hỗ trợ nếu vấn đề vẫn tiếp diễn.";
        public readonly string TokenInvalidErrorMessage = "Lỗi thông tin người dùng. Vui lòng đăng nhập lại.";
        public readonly string InvalidInfluencer = "Influencer invalid";

        public readonly string JWTKey = "JWTKey";
        public readonly string EmailConfig = "EmailConfig";
        public readonly string FirebaseStorageBucket = "bluegreen2.appspot.com";
        public readonly string YoutubeAPIKey = "YoutubeAPIKey";

        public readonly string WebBaseUrl = "http://127.0.0.1:5500";
        public readonly string WebApiBaseUrl = "https://localhost:7073/api";

        public readonly List<string> AdminEmails = new List<string> { "nguyenhoang062017@gmail.com" };
        public readonly string LogLink = "https://supabase.com/dashboard/project/uucyeumznprpthpykxwv/editor";
    }
}
