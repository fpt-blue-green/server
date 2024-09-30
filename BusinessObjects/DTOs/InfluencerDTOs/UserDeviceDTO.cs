namespace BusinessObjects
{
    public class UserDeviceDTO
    {
        public string? DeviceOperatingSystem { get; set; }
        public string? BrowserName { get; set; }
        public string? DeviceType { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public DateTime? RefreshTokenTime { get; set; }
    }
}
