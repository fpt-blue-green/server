using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class DateTimeToLocalConverter : ValueConverter<DateTime, DateTime>
{
    private static readonly TimeSpan Offset = TimeSpan.FromHours(7);

    public DateTimeToLocalConverter() : base(
        v => v.ToUniversalTime(),  // Lưu vào DB (chuyển sang UTC)
        v => v.Add(Offset))        // Đọc từ DB (chuyển sang UTC+7)
    {
    }
}
