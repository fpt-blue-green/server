using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using OfficeOpenXml;
using Repositories;
using Serilog;
using Service.Helper;

namespace Service
{
    public class BannedUserService : IBannedUserService
    {
        private readonly IUserRepository _userRepository = new UserRepository();
        private readonly IBannedUserRepository _bannedUserRepository = new BannedUserRepository();
        private static AdminActionNotificationHelper adminActionNotificationHelper = new AdminActionNotificationHelper();
        private static ConfigManager _configManager = new ConfigManager();
        private static EmailTemplate _emailTemplate = new EmailTemplate();
        private static ILogger _loggerService = new LoggerService().GetDbLogger();
        private static IEmailService _emailService = new EmailService();
        private static readonly IMapper _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        }).CreateMapper();

        public async Task<IEnumerable<BannedUserDTO>> GetBannedUsers()
        {
            var bannedUsers = (await _bannedUserRepository.GetBannedUsers()).Take(100).ToList();
            return _mapper.Map<IEnumerable<BannedUserDTO>>(bannedUsers);
        }

        public async Task<BannedUser> BanUserBaseOnReport(User user, BannedUserRequestDTO userRequestDTO, UserDTO userDTO)
        {
            if(userRequestDTO.Reason.Length > 350)
            {
                throw new InvalidOperationException("Nội dung nguyên nhân phải giới hạn trong 350 ký tự.");
            }

            if (userRequestDTO.BannedTime == EBanDate.None)
            {
                throw new InvalidOperationException("Thời gian bị cấm không hợp lệ");
            }
            var bannedUserData = user.Influencer?.User.BannedUserUsers.Where(b => b.IsActive).FirstOrDefault();
            if (bannedUserData != null)
            {
                bannedUserData.BanDate = DateTime.Now;
                bannedUserData.Reason = userRequestDTO.Reason;
                bannedUserData.UnbanDate = userRequestDTO.BannedTime == EBanDate.Indefinitely ? DateTime.MaxValue : DateTime.Now.Add(userRequestDTO.BannedTime.ToTimeSpan());

                await _bannedUserRepository.UpdateBannedUserData(bannedUserData);
                return bannedUserData;
            }
            else
            {
                var banUser = new BannedUser
                {
                    UserId = user.Id,
                    Reason = userRequestDTO.Reason,
                    BanDate = DateTime.Now,
                    BannedById = userDTO.Id,
                    UnbanDate = userRequestDTO.BannedTime == EBanDate.Indefinitely ? DateTime.MaxValue : DateTime.Now.Add(userRequestDTO.BannedTime.ToTimeSpan()),
                };
                await _bannedUserRepository.CreateBannedUserData(banUser);

                user.IsBanned = true;
                await _userRepository.UpdateUser(user);
                return banUser;
            }
        }

        public async Task BanUser(Guid userId, BannedUserRequestDTO userRequestDTO, UserDTO userDTO)
        {
            if (userRequestDTO.Reason.Length > 350)
            {
                throw new InvalidOperationException("Nội dung nguyên nhân phải giới hạn trong 350 ký tự.");
            }
            if (userRequestDTO.BannedTime == EBanDate.None)
            {
                throw new InvalidOperationException("Thời gian bị cấm không hợp lệ");
            }

            var user = await _userRepository.GetUserById(userId) ?? throw new KeyNotFoundException();
            var result = new BannedUser();
            
            var bannedUserData = user.Influencer?.User.BannedUserUsers.Where(b => b.IsActive).FirstOrDefault();
            if (bannedUserData != null)
            {
                bannedUserData.BanDate = DateTime.Now;
                bannedUserData.Reason = userRequestDTO.Reason;
                bannedUserData.UnbanDate = userRequestDTO.BannedTime == EBanDate.Indefinitely ? DateTime.MaxValue : DateTime.Now.Add(userRequestDTO.BannedTime.ToTimeSpan());

                await _bannedUserRepository.UpdateBannedUserData(bannedUserData);
                result = bannedUserData;
            }
            else
            {
                var banUser = new BannedUser
                {
                    UserId = user.Id,
                    Reason = userRequestDTO.Reason,
                    BanDate = DateTime.Now,
                    BannedById = userDTO.Id,
                    UnbanDate = userRequestDTO.BannedTime == EBanDate.Indefinitely ? DateTime.MaxValue : DateTime.Now.Add(userRequestDTO.BannedTime.ToTimeSpan())
                };
                await _bannedUserRepository.CreateBannedUserData(banUser);

                user.IsBanned = true;
                await _userRepository.UpdateUser(user);
                result = banUser;
            }

            await adminActionNotificationHelper.CreateNotification<BannedUser>(userDTO, null, result, null, null, true);

            await SendBannedMailToUser(user.Email, result.Reason, false);
        }

        public async Task UnBanUser(Guid userId, BannedUserRequestDTO userRequestDTO, UserDTO userDTO)
        {
            var user = await _userRepository.GetUserById(userId);
            var banUserData = user.BannedUserUsers.Where(b => b.IsActive).FirstOrDefault() ?? throw new KeyNotFoundException();

            user.IsBanned = false;
            await _userRepository.UpdateUser(user);

            banUserData!.IsActive = false;
            await _bannedUserRepository.UpdateBannedUserData(banUserData);

            banUserData.Reason = userRequestDTO.Reason;
            await adminActionNotificationHelper.CreateNotification<BannedUser>(userDTO, null, banUserData, null, null, true);

            await SendBannedMailToUser(user.Email, banUserData.Reason, true);
        }

        public async Task SendBannedMailToUser(string email, string description, bool isUnban, string? time = null)
        {
            try
            {
                var body = string.Empty;
                var title = string.Empty;
                if (isUnban)
                {
                    body = _emailTemplate.reportResultTemplate.Replace("{projectName}", _configManager.ProjectName)
                                                      .Replace("{BanDate}", time)
                                                      .Replace("{UnbanDate}", DateTime.Now.ToString())
                                                      .Replace("{Description}", description);
                    title = "Thông báo tài khoản đã được mở khoá";
                }
                else
                {
                    body = _emailTemplate.reportResultTemplate.Replace("{projectName}", _configManager.ProjectName)
                                                       .Replace("{UnbanDate}", time)
                                                       .Replace("{CurrenDate}", DateTime.Now.ToString())
                                                       .Replace("{Description}", description);
                    title = "Thông Báo Tài Khoản Đã Bị Cấm";
                }

                _ = Task.Run(async () => await _emailService.SendEmail(new List<string> { email }, title, body));
            }
            catch (Exception ex)
            {
                _loggerService.Error("Has error when send mail in SendBannedMailToUser : " + ex.ToString());
            }
        }

        public async Task<(byte[] fileContent, string fileName)> GetDataFile()
        {
            var bannedUsers = await _bannedUserRepository.GetBannedUsers();
            var file = CreateExcel(bannedUsers);
            return (file, $"BannedUserData_{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}.csv");
        }

        public static byte[] CreateExcel(IEnumerable<BannedUser> bannedUsers)
        {

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("BannedUserData");

                worksheet.Cells[1, 1].Value = "User Email";
                worksheet.Cells[1, 2].Value = "Banned By";
                worksheet.Cells[1, 3].Value = "Is Active";
                worksheet.Cells[1, 4].Value = "Ban Date (UTC+7)";
                worksheet.Cells[1, 5].Value = "Unban Date (UTC+7)";
                worksheet.Cells[1, 6].Value = "Reason";

                int row = 2;

                foreach (var bannedUser in bannedUsers)
                {
                    worksheet.Cells[row, 1].Value = bannedUser.User.Email + $" ({bannedUser.User.DisplayName})";
                    worksheet.Cells[row, 2].Value = bannedUser.BannedBy.DisplayName;
                    worksheet.Cells[row, 3].Value = bannedUser.IsActive;
                    worksheet.Cells[row, 4].Value = bannedUser.BanDate.ToString("yyyy-MM-dd HH:mm:ss");
                    worksheet.Cells[row, 5].Value = bannedUser.UnbanDate == null
                                                     || bannedUser.UnbanDate.Value.ToString("yyyy-MM-dd HH:mm:ss") == DateTime.MaxValue.ToString("yyyy-MM-dd HH:mm:ss")
                                                            ? "#INF#" 
                                                            : bannedUser.UnbanDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                    worksheet.Cells[row, 6].Value = bannedUser.Reason;
                    row++;
                }

                worksheet.Cells[2, 6, row - 1, 6].Style.WrapText = true;
                worksheet.Cells.AutoFitColumns();
                worksheet.Column(6).Width = 100;
                return package.GetAsByteArray();
            }
        }
    }
}
