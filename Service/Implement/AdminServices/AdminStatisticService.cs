using BusinessObjects;
using BusinessObjects.Models;
using Repositories;
using Repositories.Implement;
using Repositories.Interface;
using static BusinessObjects.AuthEnumContainer;
using static BusinessObjects.JobEnumContainer;

namespace Service
{
    public class AdminStatisticService : IAdminStatisticService
    {
        private readonly IUserDeviceRepository _userDeviceRepository = new UserDeviceRepository();
        private readonly IUserRepository _userRepository = new UserRepository();
        private readonly IPaymentRepository _paymentRepository = new PaymentRepository();
        private readonly ICampaignRepository _campaignRepository = new CampaignRepository();
        private readonly IJobRepository _jobRepository = new JobRepository();

        #region GetLoginCountsByTimeFrame
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

        public async Task<List<int>> GetAvailableYearInActiveUser()
        {
            var data = await _userDeviceRepository.GetAll();
            var year = data.Select(u => u.RefreshTokenTime.Year).Distinct().ToList();
            return year;
        }
        #endregion

        private static string GetVietnameseMonthName(int month)
        {
            var monthNames = new[]
            {
                "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6",
                "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"
            };
            return monthNames[month - 1]; // Trả về tên tháng dựa trên chỉ số
        }

        #region GetMonthlyMetricsTrend
        public async Task<List<MonthlyMetricsTrendDTO>> GetMonthlyMetricsTrend()
        {
            return new List<MonthlyMetricsTrendDTO>
            {
               await GetRevenuetMonthlyMetricsTrend(),
               await GetNewUserMonthlyMetricsTrend(),
               await GetActiveUserMonthlyMetricsTrend(),
               await GetActiveCampaignMonthlyMetricsTrend()
            };
            
        }
        protected async Task<MonthlyMetricsTrendDTO> GetRevenuetMonthlyMetricsTrend()
        {
            var revenueData = await _paymentRepository.GetAllProfitPayment();

            // Lấy dữ liệu cho tháng hiện tại
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var currentRevenueData = revenueData.Where(r => r.CreatedAt.Month == currentMonth && r.CreatedAt.Year == currentYear);
            var totalRevenueNow = currentRevenueData
                .Where(r => r.NetAmount != null) // Kiểm tra xem NetAmount không null
                .Sum(r => r.NetAmount!.Value); // Tính tổng giá trị NetAmount

            // Lấy dữ liệu cho tháng trước
            var previousMonth = currentMonth == 1 ? 12 : currentMonth - 1;
            var previousYear = currentMonth == 1 ? currentYear - 1 : currentYear;

            var previousRevenueData = revenueData.Where(r => r.CreatedAt.Month == previousMonth && r.CreatedAt.Year == previousYear);
            var totalRevenueLastMonth = previousRevenueData
                .Where(r => r.NetAmount != null) // Kiểm tra xem NetAmount không null
                .Sum(r => r.NetAmount!.Value); // Tính tổng giá trị NetAmount

            // Tính chênh lệch và phần trăm thay đổi
            var revenueDifference = totalRevenueNow - totalRevenueLastMonth;
            var percentageChange = totalRevenueLastMonth != 0 ? (revenueDifference / totalRevenueLastMonth) * 100 : 0;

            string comment = string.Empty;
            // Đưa ra nhận xét
            if (revenueDifference > 0)
            {
                comment = $"+{(percentageChange == 0 ? "#" : Math.Abs(percentageChange).ToString("F2"))}% so với tháng trước";
            }
            else if (revenueDifference < 0)
            {
                comment = $"-{(percentageChange == 0 ? "#" : Math.Abs(percentageChange).ToString("F2"))}% so với tháng trước";
            }
            else
            {
                comment = $"= Không có sự thay đổi so với tháng trước";
            }

            return new MonthlyMetricsTrendDTO
            {
                Comment = comment,
                Data = totalRevenueNow.ToString("N2"),
                Type = "Revenue"
            };
        }
        protected async Task<MonthlyMetricsTrendDTO> GetNewUserMonthlyMetricsTrend()
        {
            var userData = await _userRepository.GetUsers();

            // Lấy dữ liệu cho tháng hiện tại
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var currentMonthUserCount = userData.Count(u => u.CreatedAt.Month == currentMonth && u.CreatedAt.Year == currentYear);

            // Lấy dữ liệu cho tháng trước
            var previousMonth = currentMonth == 1 ? 12 : currentMonth - 1;
            var previousYear = currentMonth == 1 ? currentYear - 1 : currentYear;

            var previousMonthUserCount = userData.Count(u => u.CreatedAt.Month == previousMonth && u.CreatedAt.Year == previousYear);

            // Tính chênh lệch và phần trăm thay đổi
            var userDifference = currentMonthUserCount - previousMonthUserCount;
            var percentageChange = previousMonthUserCount != 0 ? (userDifference / (double)previousMonthUserCount) * 100 : 0;

            // Đưa ra nhận xét
            string comment;
            if (userDifference > 0)
            {
                comment = $"+{(percentageChange == 0 ? "#" : Math.Abs(percentageChange).ToString("F2"))}% so với tháng trước";
            }
            else if (userDifference < 0)
            {
                comment = $"-{(percentageChange == 0 ? "#" : Math.Abs(percentageChange).ToString("F2"))}% so với tháng trước";
            }
            else
            {
                comment = "= Không có sự thay đổi.";
            }

            return new MonthlyMetricsTrendDTO
            {
                Comment = comment,
                Data = currentMonthUserCount.ToString("N0"),
                Type = "NewUsers"
            };
        }
        protected async Task<MonthlyMetricsTrendDTO> GetActiveUserMonthlyMetricsTrend()
        {
            var userData = await _userDeviceRepository.GetAll();

            // Lấy dữ liệu cho tháng hiện tại
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var currentMonthUserCount = userData.Count(u => u.RefreshTokenTime.Month == currentMonth &&
                                                            u.RefreshTokenTime.Year == currentYear);

            // Lấy dữ liệu cho tháng trước
            var previousMonth = currentMonth == 1 ? 12 : currentMonth - 1;
            var previousYear = currentMonth == 1 ? currentYear - 1 : currentYear;

            var previousMonthUserCount = userData.Count(u => u.RefreshTokenTime.Month == previousMonth &&
                                                             u.RefreshTokenTime.Year == previousYear);

            // Tính chênh lệch và phần trăm thay đổi
            var userDifference = currentMonthUserCount - previousMonthUserCount;
            var percentageChange = previousMonthUserCount != 0 ? (userDifference / (double)previousMonthUserCount) * 100 : 0;

            // Đưa ra nhận xét
            string comment;
            if (userDifference > 0)
            {
                comment = $"+{(percentageChange == 0 ? "#" : Math.Abs(percentageChange).ToString("F2"))}% so với tháng trước";
            }
            else if (userDifference < 0)
            {
                comment = $"-{(percentageChange == 0 ? "#" : Math.Abs(percentageChange).ToString("F2"))}% so với tháng trước";
            }
            else
            {
                comment = "= Không có sự thay đổi";
            }

            return new MonthlyMetricsTrendDTO
            {
                Comment = comment,
                Data = currentMonthUserCount.ToString("N0"),
                Type = "ActiveUsers"
            };
        }
        protected async Task<MonthlyMetricsTrendDTO> GetActiveCampaignMonthlyMetricsTrend()
        {
            var campaignData = await _campaignRepository.GetAlls();

            // Lấy dữ liệu cho tháng hiện tại
            var currentDate = DateTime.Now;
            var currentMonth = currentDate.Month;
            var currentYear = currentDate.Year;

            // Lọc các chiến dịch có EndDate trong tháng hiện tại
            var currentMonthCampaignCount = campaignData.Count(c =>
                c.EndDate.Month == currentMonth &&
                c.EndDate.Year == currentYear &&
                c.EndDate <= currentDate);

            // Tính toán cho tháng trước
            var previousMonth = currentMonth == 1 ? 12 : currentMonth - 1;
            var previousYear = currentMonth == 1 ? currentYear - 1 : currentYear;

            // Lọc các chiến dịch có EndDate trong tháng trước
            var previousMonthCampaignCount = campaignData.Count(c =>
                c.EndDate.Month == previousMonth &&
                c.EndDate.Year == previousYear);

            // Tính chênh lệch và phần trăm thay đổi
            var campaignDifference = currentMonthCampaignCount - previousMonthCampaignCount;
            var percentageChange = previousMonthCampaignCount != 0
                ? (campaignDifference / (double)previousMonthCampaignCount) * 100
                : 0;

            // Đưa ra nhận xét
            string comment;
            if (campaignDifference > 0)
            {
                comment = $"+{(percentageChange == 0 ? "#" : Math.Abs(percentageChange).ToString("F2"))}% so với tháng trước";
            }
            else if (campaignDifference < 0)
            {
                comment = $"-{(percentageChange == 0 ? "#" : Math.Abs(percentageChange).ToString("F2"))}% so với tháng trước";
            }
            else
            {
                comment = "= Không có sự thay đổi";
            }

            return new MonthlyMetricsTrendDTO
            {
                Comment = comment,
                Data = currentMonthCampaignCount.ToString("N0"),
                Type = "ActiveCampaigns"
            };
        }
        #endregion
        
        public async Task<Dictionary<string, int>> GetRoleData()
        {
            var data = await _userRepository.GetUsers();
            return new Dictionary<string, int>
            {
                {"Nhà sáng tạo nội dung", data.Where(u => u.Role == (int)ERole.Influencer).Count() },
                {"Nhãn hàng", data.Where( user => user.Role == (int)ERole.Brand && user.Brand != null && user.Brand.IsPremium == false).ToList().Count },
                {"Nhãn hàng trả phí", data.Where(user => user.Role == (int)ERole.Brand && user.Brand != null && user.Brand.IsPremium == true).ToList().Count },
                {"Quản trị viên", data.Where(u => u.Role == (int)ERole.Admin).Count() },
            };
        }

        public async Task<Dictionary<string, int>> GetJobStatusData()
        {
            var data = await _jobRepository.GetAllJob();
            return new Dictionary<string, int>
            {
                {(EJobStatus.Pending).GetEnumDescription(), data.Where(u => u.Status == (int)EJobStatus.Pending).Count() },
                {(EJobStatus.Approved).GetEnumDescription(), data.Where(u => u.Status == (int)EJobStatus.Approved).Count() },
                {(EJobStatus.InProgress).GetEnumDescription(), data.Where(u => u.Status == (int)EJobStatus.InProgress).Count() },
                {(EJobStatus.Completed).GetEnumDescription(), data.Where(u => u.Status == (int)EJobStatus.Completed).Count() },
                {(EJobStatus.Failed).GetEnumDescription(), data.Where(u => u.Status == (int)EJobStatus.Failed).Count() },
                {(EJobStatus.NotCreated).GetEnumDescription(), data.Where(u => u.Status == (int)EJobStatus.NotCreated).Count() },
            };
        }


    }
}
