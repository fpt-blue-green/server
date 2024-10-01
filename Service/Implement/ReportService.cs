using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
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
        public ReportService(IMapper mapper)
        {
            _mapper = mapper;
        }
        public async Task CreateInfluencerReport(Guid influencerId, ReportRequestDTO reportRequestDTO, UserDTO userDTO)
        {
            _loggerService.Information("Start to report Influencer: ");
            var influReportList =  await _reportRepository.GetInfluencerReportsByInfluencerId(influencerId);
            if (influReportList != null)
            {
                if (influReportList.FirstOrDefault(x => x.ReporterId == userDTO.Id) != null) 
                {
                    throw new InvalidOperationException("Không được report 1 Influencer quá 2 lần.");
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
            _loggerService.Information("End to report Influencer: ");
        }

        public async Task DeleteInfluencerReport(Guid id)
        {
            var influencerReport = await _reportRepository.GetById(id);
            if (influencerReport == null)
            {
                throw new KeyNotFoundException();
            }

            await _reportRepository.Delete(influencerReport);
        }

        public Task<InfluencerReport> GetInfluencerReportById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<InfluencerReport>> GetInfluencerReports()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<InfluencerReport>> GetInfluencerReportsByInfluencerId(Guid influencerId)
        {
            throw new NotImplementedException();
        }
    }
}
