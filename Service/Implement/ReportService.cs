﻿using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Repositories;
using Repositories.Implement;
using Repositories.Interface;
using Serilog;
using Service.Helper;
using System.Transactions;

namespace Service
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository = new ReportRepository();
        private readonly IUserRepository _userRepository = new UserRepository();
        private static ILogger _loggerService = new LoggerService().GetDbLogger();
        private static ConfigManager _configManager = new ConfigManager();
        private static EmailTemplate _emailTemplate = new EmailTemplate();
        private static IEmailService _emailService = new EmailService();
        private static IBannedUserService _bannedUserService = new BannedUserService();
        private static AdminActionNotificationHelper adminActionNotificationHelper = new AdminActionNotificationHelper();
        private static readonly IMapper _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        }).CreateMapper();

        public async Task CreateInfluencerReport(Guid influencerId, ReportRequestDTO reportRequestDTO, UserDTO userDTO)
        {
            var influReportList = await _reportRepository.GetInfluencerReportsByInfluencerId(influencerId);
            if (influReportList != null)
            {
                if (influReportList.FirstOrDefault(x => x.ReporterId == userDTO.Id) != null)
                {
                    throw new InvalidOperationException("Không được báo cáo 1 nhà sáng tạo nội dung quá 2 lần.");
                }
            }

            var curInfluencer = await _userRepository.GetUserById(userDTO.Id);
            if (curInfluencer != null && curInfluencer.Influencer?.Id == influencerId)
            {
                throw new InvalidOperationException("Không được báo cáo chính mình.");
            }

            var influencerReport = new InfluencerReport()
            {
                ReporterId = userDTO.Id,
                InfluencerId = influencerId,
                Reason = (int)reportRequestDTO.Reason,
                Description = reportRequestDTO.Description,
            };
            await _reportRepository.Create(influencerReport);
            await SendEmailToAdmin(influencerReport.Id);
        }

        public async Task DeleteInfluencerReport(Guid id, UserDTO userDTO)
        {
            var influencerReport = await _reportRepository.GetById(id);
            if (influencerReport == null)
            {
                throw new KeyNotFoundException();
            }
            if (userDTO.Role != AuthEnumContainer.ERole.Admin && userDTO.Id != influencerReport.ReporterId)
            {
                throw new AccessViolationException();
            }

            await _reportRepository.Delete(influencerReport);
        }

        public async Task<InfluencerReport> GetReportById(Guid id)
        {
            return await _reportRepository.GetById(id);
        }

        public async Task<IEnumerable<ReportDTO>> GetReports()
        {
            var reports = await _reportRepository.GetAll();

            var sortedReports = reports
                         .OrderBy(r => r.InfluencerId)
                         .ThenByDescending(r => r.CreatedAt)
                         .ToList();
            return _mapper.Map<IEnumerable<ReportDTO>>(sortedReports);
        }

        public async Task RejectReport(Guid id)
        {
            var sameTypeReports = await _reportRepository.GetReportWithSameType(id);

            if (sameTypeReports != null && sameTypeReports.Any())
            {
                foreach (var sameReport in sameTypeReports)
                {
                    sameReport.ReportStatus = (int)EReportStatus.Rejected;
                }

                await _reportRepository.UpdateReports(sameTypeReports);
            }
        }

        public async Task ApproveReport(Guid id, UserDTO user, BannedUserRequestDTO userRequestDTO)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var sameTypeReports = await _reportRepository.GetReportWithSameType(id);

                    if (sameTypeReports != null && sameTypeReports.Any())
                    {
                        foreach (var sameReport in sameTypeReports)
                        {
                            sameReport.ReportStatus = (int)EReportStatus.Approved;
                        }

                        await _reportRepository.UpdateReports(sameTypeReports);
                    }
                    var result = await _bannedUserService.BanUser(sameTypeReports!.FirstOrDefault()?.Influencer?.User!, userRequestDTO, user);
                    await adminActionNotificationHelper.CreateNotification<BannedUser>(user, null,result, null, null, true);

                    scope.Complete();
                }
                catch
                {
                    throw;
                }
            }
        }

        public Task<IEnumerable<ReportDTO>> GetReportsByInfluencerId(Guid influencerId)
        {
            throw new NotImplementedException();
        }

        public async Task SendEmailToAdmin(Guid id)
        {
            try
            {
                var influencerReport = await GetReportById(id);
                if (influencerReport == null)
                {
                    return;
                }

                var body = _emailTemplate.reportTemplate.Replace("{projectName}", _configManager.ProjectName)
                                                        .Replace("{Reason}", Enum.GetName(typeof(BusinessObjects.EReportReason), influencerReport.Reason!))
                                                        .Replace("{InfluencerName}", influencerReport.Influencer.FullName)
                                                        .Replace("{Reporter}", influencerReport.Reporter.DisplayName)
                                                        .Replace("{CreatedAt}", influencerReport.CreatedAt.ToString("dd/MM/yyyy"))
                                                        .Replace("{Description}", influencerReport.Description);
                await _emailService.SendEmail(_configManager.AdminReportHandler, "Đơn báo cáo Influencer", body);
            }
            catch (Exception ex)
            {
                _loggerService.Error("Has error when send mail in Report : " + ex.ToString());
            }
        }
    }
}