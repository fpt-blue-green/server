using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Newtonsoft.Json;
using Repositories;
using Repositories.Implement;
using Repositories.Interface;
using Serilog;

namespace Service
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository = new ReportRepository();
        private readonly IUserRepository _userRepository = new UserRepository();
        private static ILogger _loggerService = new LoggerService().GetDbLogger();
        private readonly IMapper _mapper;
        private static ConfigManager _configManager = new ConfigManager();
        private static EmailTemplate _emailTemplate = new EmailTemplate();
        private static IEmailService _emailService = new EmailService();

        public ReportService(IMapper mapper)
        {
            _mapper = mapper;
        }
        public async Task CreateInfluencerReport(Guid influencerId, ReportRequestDTO reportRequestDTO, UserDTO userDTO)
        {
            _loggerService.Information("Start to report Influencer: " + influencerId+ " " + JsonConvert.SerializeObject(reportRequestDTO));
            var influReportList =  await _reportRepository.GetInfluencerReportsByInfluencerId(influencerId);
            if (influReportList != null)
            {
                if (influReportList.FirstOrDefault(x => x.ReporterId == userDTO.Id) != null) 
                {
                    throw new InvalidOperationException("Bạn chỉ được phép báo cáo Influencer 1 lần.");
                }
            }

            var curInfluencer = await _userRepository.GetUserById(userDTO.Id);
            if (curInfluencer != null && curInfluencer.Influencer?.Id == influencerId)
            {
                throw new InvalidOperationException("Không được report chính mình.");
            }

            var influencerReport = new InfluencerReport()
            {
                ReporterId = userDTO.Id,
                InfluencerId = influencerId,
                Reason = (int)reportRequestDTO.Reason,
                Description = reportRequestDTO.Description,
            };
            await _reportRepository.Create(influencerReport);
            await SendEmailToReport(influencerReport.Id);
            _loggerService.Information("End to report Influencer");
        }

        public async Task DeleteInfluencerReport(Guid id, UserDTO userDTO)
        {
            _loggerService.Information($"Start to delete InfluencerReport: Id {id}");
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
            _loggerService.Information("End to delete InfluencerReport.");
        }

        public async Task<InfluencerReport> GetInfluencerReportById(Guid id)
        {
            return await _reportRepository.GetById(id);
        }

        public Task<IEnumerable<InfluencerReport>> GetInfluencerReports()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<InfluencerReport>> GetInfluencerReportsByInfluencerId(Guid influencerId)
        {
            throw new NotImplementedException();
        }

        public async Task SendEmailToReport(Guid id)
        {
            try
            {
                var influencerReport = await GetInfluencerReportById(id);
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
            }catch(Exception ex)
            {
                _loggerService.Error("Has error when send mail in ReportService." + ex);
            }
            
        }
    }
}
