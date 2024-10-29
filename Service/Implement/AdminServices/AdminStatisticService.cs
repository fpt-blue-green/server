using BusinessObjects;
using Repositories.Implement;
using Repositories.Interface;
using System;

namespace Service
{
    public class AdminStatisticService : IAdminStatisticService
    {
        private readonly IUserDeviceRepository _userDeviceRepository = new UserDeviceRepository();
        public async Task<Dictionary<string, int>> GetLoginCountsByTimeFrame(int year, ETimeFrame timeFrame)
        {
            DateTime startDate;
            DateTime endDate;

            // Xác định khoảng thời gian dựa trên enum
            switch (timeFrame)
            {
                case ETimeFrame.FullYear:
                    startDate = new DateTime(year, 1, 1); // Ngày đầu của năm
                    endDate = new DateTime(year + 1, 1, 1); // Ngày đầu năm sau
                    break;
                case ETimeFrame.FirstHalf:
                    startDate = new DateTime(year, 1, 1); // Ngày đầu của 6 tháng đầu
                    endDate = new DateTime(year, 7, 1); // Ngày đầu của 6 tháng sau
                    break;
                case ETimeFrame.SecondHalf:
                    startDate = new DateTime(year, 7, 1); // Ngày đầu của 6 tháng sau
                    endDate = new DateTime(year + 1, 1, 1); // Ngày đầu năm sau
                    break;
                default:
                    throw new Exception("Invalid time frame specified.");
            }

            var data = await _userDeviceRepository.GetAll();

            // Lấy dữ liệu từ cơ sở dữ liệu
            var results = data
                .Where(ud => ud.RefreshTokenTime >= startDate && ud.RefreshTokenTime < endDate)
                .GroupBy(ud => new { ud.RefreshTokenTime.Year, ud.RefreshTokenTime.Month }) // Nhóm theo năm và tháng
                .Select(g => new
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1), // Tạo DateTime cho tháng
                    LoginCount = g.Select(ud => ud.UserId).Distinct().Count() // Đếm số người dùng duy nhất
                })
                .ToList();

            // Chuyển đổi kết quả sang tháng tiếng Việt
            var loginCounts = results.ToDictionary(
                r => GetVietnameseMonthName(r.Month.Month),
                r => r.LoginCount
            );

            // Tạo danh sách tháng trong khoảng thời gian
            var monthsInTimeFrame = new List<string>();
            if (timeFrame == ETimeFrame.FullYear)
            {
                for (int i = 0; i < 12; i++)
                {
                    var monthDate = new DateTime(year, i + 1, 1);
                    monthsInTimeFrame.Add(GetVietnameseMonthName(monthDate.Month)); // Thêm tên tháng vào danh sách
                }
            }
            else if (timeFrame == ETimeFrame.FirstHalf)
            {
                for (int i = 0; i < 6; i++)
                {
                    var monthDate = new DateTime(year, i + 1, 1);
                    monthsInTimeFrame.Add(GetVietnameseMonthName(monthDate.Month)); // Thêm tên tháng vào danh sách
                }
            }
            else if (timeFrame == ETimeFrame.SecondHalf)
            {
                for (int i = 6; i < 12; i++)
                {
                    var monthDate = new DateTime(year, i + 1, 1);
                    monthsInTimeFrame.Add(GetVietnameseMonthName(monthDate.Month)); // Thêm tên tháng vào danh sách
                }
            }

            var finalCounts = monthsInTimeFrame.ToDictionary(
                month => month,
                month => loginCounts.ContainsKey(month) ? loginCounts[month] : 0
            );

            return finalCounts;
        }

        private static string GetVietnameseMonthName(int month)
        {
            var monthNames = new[]
            {
                "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6",
                "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"
            };
            return monthNames[month - 1]; // Trả về tên tháng dựa trên chỉ số
        }
    }
}
