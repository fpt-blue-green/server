using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BusinessObjects.Helper
{
    public static class DateTimeConverter
    {
        public static void ConfigureDateTimeConversion(ModelBuilder modelBuilder)
        {
            // Lặp qua tất cả các loại thực thể được định nghĩa trong DbContext
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Lặp qua tất cả các thuộc tính của thực thể
                foreach (var property in entityType.GetProperties())
                {
                    // Kiểm tra xem thuộc tính có phải là DateTime không
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                            v => DateTime.SpecifyKind(v.ToUniversalTime(), DateTimeKind.Utc), // Chuyển đổi khi lưu
                            v => ConvertToLocalTime(v) // Chuyển đổi khi đọc
                        ));
                    }
                }
            }
        }

        // Phương thức chuyển đổi từ UTC sang UTC+7
        private static DateTime ConvertToLocalTime(DateTime utcDateTime)
        {
            // Tạo một đối tượng TimeZoneInfo cho UTC+7
            //TimeZoneInfo utcPlus7 = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // Múi giờ UTC+7
            TimeZoneInfo utcPlus7 = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"); // Múi giờ UTC+8 Neu test o Cty

            // Chuyển đổi từ UTC sang UTC+7
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, utcPlus7);
        }

    }
}
