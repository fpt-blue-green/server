using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Repositories;
using static BusinessObjects.JobEnumContainer;
using Serilog;


namespace Service
{
    public class OfferService : IOfferService
    {
        #region Config
        private static readonly IInfluencerRepository _influencerRepository = new InfluencerRepository();
        private static readonly IOfferRepository _offerRepository = new OfferRepository();
        private static readonly IJobRepository _jobRepository = new JobRepository();
        private static readonly IUserRepository _userRepository = new UserRepository();
        private static readonly ConfigManager _configManager = new ConfigManager();
        private static readonly EmailTemplate _emailTempalte = new EmailTemplate();
        private static readonly IEmailService _emailService = new EmailService();
        private static ILogger _loggerService = new LoggerService().GetDbLogger();
        private static readonly IMapper _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        }).CreateMapper();
        #endregion

        public async Task CreateOffer(UserDTO userDTO, OfferCreateRequestDTO offerCreateRequestDTO)
        {

            if (userDTO.Role == AuthEnumContainer.ERole.Admin)
            {
                throw new InvalidOperationException("Admin không thể tạo được Offer.");
            }
            if (userDTO.Role == AuthEnumContainer.ERole.Influencer)
            {
                var influencer = await _influencerRepository.GetByUserId(userDTO.Id) ?? throw new KeyNotFoundException();
                offerCreateRequestDTO.Job.InfluencerId = influencer.Id;
            }
            //Create Job First
            var jobnew = _mapper.Map<Job>(offerCreateRequestDTO.Job);
            await _jobRepository.Create(jobnew);

            //Create offer
            var offer = _mapper.Map<Offer>(offerCreateRequestDTO.Offer);
            offer.Status =(int) JobEnumContainer.EOfferStatus.Offering;
            offer.JobId = jobnew.Id;
            offer.From = (int)userDTO.Role;
            await _offerRepository.Create(offer);

            var job = await _jobRepository.GetJobFullDetailById(jobnew.Id);

            //Send Mail
            await SendMail(offer, job, EOfferStatus.Offering);
        }

        public async Task ReOffer(Guid id, UserDTO userDTO, ReOfferDTO reOfferDTO)
        {
            //set status old offer to reject
            var oldOffer = await _offerRepository.GetById(id);
            oldOffer.Status = (int)JobEnumContainer.EOfferStatus.Rejected;
            await _offerRepository.Update(oldOffer);

            //create new offer (Re-offer)
            var job = await _jobRepository.GetJobFullDetailById(oldOffer.JobId);
            var newOffer = new Offer
            {
                Description = reOfferDTO.Description,
                Price = reOfferDTO.Price,
                Quantity = reOfferDTO.Quantity,
                ContentType = oldOffer.ContentType,
                Duration = reOfferDTO.Duration,
                From = (int)userDTO.Role,
                Platform = oldOffer.Platform,
                Status = (int)JobEnumContainer.EOfferStatus.Offering,
                JobId = oldOffer.JobId,
            };
            await _offerRepository.Create(newOffer);
            //Send Mail
            await SendMail(newOffer, job,EOfferStatus.Offering,true);
        }

        #region Send Mail
        public static async Task SendMail(Offer offer, Job job, EOfferStatus offerType, bool isReOffer = false)
        {
            var brandUser = job.Campaign.Brand.User;
            var influencerUser = job.Influencer.User;

            string subject = "";
            string body = "";
            string recipientEmail = "";

            switch (offerType)
            {
                case EOfferStatus.Offering:
                    #region Offering Or Reoffer
                    if (offer.From == (int)AuthEnumContainer.ERole.Influencer)
                    {
                        subject = "Influencer đã tạo một Offer mới";
                        if (isReOffer)
                        {
                            subject = "Re-Offer: Influencer đã tạo một Offer mới";
                        }
                        recipientEmail = brandUser.Email;
                        body = GenerateEmailBody(
                            _emailTempalte.influencerOffer,
                            brandUser.DisplayName!,
                            influencerUser.DisplayName!,
                            offer,
                            job
                        );
                    }
                    else if (offer.From == (int)AuthEnumContainer.ERole.Brand)
                    {
                        subject = "Brand đã tạo một Offer mới";
                        if (isReOffer)
                        {
                            subject = "Re-Offer: Brand đã tạo một Offer mới";
                        }
                        recipientEmail = influencerUser.Email;
                        body = GenerateEmailBody(
                            _emailTempalte.brandOffer,
                            brandUser.DisplayName!,
                            influencerUser.DisplayName!,
                            offer,
                            job
                        );
                    }
                    #endregion
                    break;
                case EOfferStatus.WaitingPayment:
                    break;
                case EOfferStatus.Rejected: 
                    break;
                default:
                    _loggerService.Error($"Error when send mail: {offerType}");
                    return;
            }


            await _emailService.SendEmail(new List<string> { recipientEmail }, subject, body);
        }
        private static string GenerateEmailBody(string template, string brandName, string influencerName, Offer offer, Job job)
        {
            return template
                .Replace("{projectName}", _configManager.ProjectName)
                .Replace("{BrandName}", brandName)
                .Replace("{InfluencerName}", influencerName)
                .Replace("{Platform}", ((EPlatform)offer.Platform).GetEnumDescription())
                .Replace("{ContentType}", ((EContentType)offer.ContentType).GetEnumDescription())
                .Replace("{Quantity}", offer.Quantity.ToString() ?? "")
                .Replace("{CreatedAt}", offer.CreatedAt.ToString())
                .Replace("{JobDescription}", offer?.Description ?? "")
                .Replace("{JobLink}", "")
                .Replace("{JobPayment}", offer?.Price.ToString("N0") ?? "");
        }
        #endregion
    }
}
