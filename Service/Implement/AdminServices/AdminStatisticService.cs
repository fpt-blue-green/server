using BusinessObjects;
using BusinessObjects.DTOs;
using BusinessObjects.Models;
using Repositories;
using Repositories.Implement;
using Repositories.Interface;
using System.Linq;
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

            var data = await _userDeviceRepository.GetAllIgnoreFilter();

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

        public List<int> GetAvailableYearInSystem()
        {
            var currentYear = DateTime.Now.Year;

            // Tạo danh sách các năm từ 2023 đến năm hiện tại
            var years = Enumerable.Range(2023, currentYear - 2023 + 1).ToList();

            return years;
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
            var revenueData = await _paymentRepository.GetAllProfitPaymentIgnoreFilter();

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
            var userData = await _userRepository.GetUsersIgnoreFilter();

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
            var userData = await _userDeviceRepository.GetAllIgnoreFilter();

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
            var campaignData = await _campaignRepository.GetAllsIgnoreFilter();

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

        #region PIE CHART

        public async Task<List<CommomPieChartDTO>> GetRoleData()
        {
            var data = await _userRepository.GetUsersIgnoreFilter();

            var result = new List<CommomPieChartDTO>
            {
                new CommomPieChartDTO
                {
                    Label = "Nhà sáng tạo nội dung",
                    Value = data.Count(u => u.Role == (int)ERole.Influencer)
                },
                new CommomPieChartDTO
                {
                    Label = "Nhãn hàng",
                    Value = data.Count(user => user.Role == (int)ERole.Brand && user.Brand != null && user.Brand.IsPremium == false)
                },
                new CommomPieChartDTO
                {
                    Label = "Nhãn hàng trả phí",
                    Value = data.Count(user => user.Role == (int)ERole.Brand && user.Brand != null && user.Brand.IsPremium == true)
                },
                new CommomPieChartDTO
                {
                    Label = "Quản trị viên",
                    Value = data.Count(u => u.Role == (int)ERole.Admin)
                }
            };

            return result;
        }


        public async Task<List<CommomPieChartDTO>> GetJobStatusData()
        {
            var data = await _jobRepository.GetAllJobIgnoreFilter();

            var result = new List<CommomPieChartDTO>
            {
                new CommomPieChartDTO
                {
                    Label = (EJobStatus.Pending).GetEnumDescription(),
                    Value = data.Count(u => u.Status == (int)EJobStatus.Pending)
                },
                new CommomPieChartDTO
                {
                    Label = (EJobStatus.Approved).GetEnumDescription(),
                    Value = data.Count(u => u.Status == (int)EJobStatus.Approved)
                },
                new CommomPieChartDTO
                {
                    Label = (EJobStatus.InProgress).GetEnumDescription(),
                    Value = data.Count(u => u.Status == (int)EJobStatus.InProgress)
                },
                new CommomPieChartDTO
                {
                    Label = (EJobStatus.Completed).GetEnumDescription(),
                    Value = data.Count(u => u.Status == (int)EJobStatus.Completed)
                },
                new CommomPieChartDTO
                {
                    Label = (EJobStatus.Failed).GetEnumDescription(),
                    Value = data.Count(u => u.Status == (int)EJobStatus.Failed)
                },
                new CommomPieChartDTO
                {
                    Label = (EJobStatus.NotCreated).GetEnumDescription(),
                    Value = data.Count(u => u.Status == (int)EJobStatus.NotCreated)
                }
            };

            return result;
        }

        #endregion

        #region TOP 5
        public async Task<List<TopFiveStatisticDTO>> GetTopFiveInfluencerUser()
        {
            var result = await _userRepository.GetInfluencerUsersWithPaymentHistory();
            return result.Select(u => new TopFiveStatisticDTO
            {
                Amount = u.Wallet + u.PaymentHistories.Where(p => p.Type == (int)EPaymentType.WithDraw && p.Status == (int)EPaymentStatus.Done).Sum(u => u.Amount),
                DisplayName = u.DisplayName ?? "Unknow",
                Email = u.Email,
                Avatar = u.Avatar!
            }).OrderByDescending(u => u.Amount).Take(5).ToList();
        }
        public async Task<List<TopFiveStatisticDTO>> GetTopFiveBrandUser()
        {
            var result = await _userRepository.GetBrandUsersWithPaymentHistory();
            return result.Select(u => new TopFiveStatisticDTO
            {
                Amount = u.Wallet + u.PaymentHistories
                                            .Where(p => (p.Type == (int)EPaymentType.WithDraw || p.Type == (int)EPaymentType.BuyPremium)
                                                        && p.Status == (int)EPaymentStatus.Done).Sum(u => u.Amount),
                DisplayName = u.DisplayName ?? "Unknow",
                Email = u.Email,
                Avatar = u.Avatar!
            }).OrderByDescending(u => u.Amount).Take(5).ToList();
        }
        #endregion
    }
}
