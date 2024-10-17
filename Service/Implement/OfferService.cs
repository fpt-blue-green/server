using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Repositories;
using Serilog;
using static BusinessObjects.JobEnumContainer;


namespace Service
{
    public class OfferService : IOfferService
    {
        #region Config
        private static readonly IInfluencerRepository _influencerRepository = new InfluencerRepository();
        private static readonly IOfferRepository _offerRepository = new OfferRepository();
        private static readonly IJobRepository _jobRepository = new JobRepository();
        private static readonly ICampaignRepository _campaignRepository = new CampaignRepository();
        private static readonly ConfigManager _configManager = new ConfigManager();
        private static readonly EmailTemplate _emailTemplate = new EmailTemplate();
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
                throw new InvalidOperationException("Quản trị viên không thể tạo được đề nghị.");
            }
            var campagin = await _campaignRepository.GetById(offerCreateRequestDTO.Job.CampaignId);
            if (campagin == null)
            {
                throw new InvalidOperationException("Nhãn hàng không tồn tại, hãy kiểm tra lại.");
            }
            else
            {
                if (campagin.Status != (int)ECampaignStatus.Active && campagin.Status != (int)ECampaignStatus.Published)
                {
                    throw new InvalidOperationException("Nhãn hàng này chưa đi vào hoạt động, hãy bắt đầu trước.");
                }
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
            offer.Status = (int)JobEnumContainer.EOfferStatus.Offering;
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
            if (oldOffer.From != (int)userDTO.Role)
            {
                throw new AccessViolationException();
            }

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
            await SendMail(newOffer, job, EOfferStatus.Offering, true);
        }

        public async Task ApproveOffer(Guid id, UserDTO userDTO)
        {
            var offer = await _offerRepository.GetById(id);

            if (offer.From != (int)userDTO.Role)
            {
                throw new AccessViolationException();
            }
            var job = await _jobRepository.GetJobFullDetailById(offer.JobId);

            if (job == null)
            {
                throw new KeyNotFoundException();
            }

            offer.Status = (int)JobEnumContainer.EOfferStatus.WaitingPayment;
            job.Status = (int)EJobStatus.InProgress;
            offer.Job = job;

            await _offerRepository.UpdateJobAndOffer(offer);

            //Send Mail
            await SendMail(offer, job!, EOfferStatus.WaitingPayment);
        }

        public async Task RejectOffer(Guid id, UserDTO userDTO)
        {
            var offer = await _offerRepository.GetById(id);

            if (offer.From != (int)userDTO.Role)
            {
                throw new AccessViolationException();
            }
            var job = await _jobRepository.GetJobFullDetailById(offer.JobId);

            if (job == null)
            {
                throw new KeyNotFoundException();
            }

            offer.Status = (int)JobEnumContainer.EOfferStatus.Rejected;
            job.Status = (int)EJobStatus.NotCreated;
            offer.Job = job;

            await _offerRepository.UpdateJobAndOffer(offer);

            //Send Mail
            await SendMail(offer, job!, EOfferStatus.Rejected);
        }

        #region Send Mail
        public static async Task SendMail(Offer offer, Job job, EOfferStatus offerType, bool isReOffer = false)
        {
            try
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
                            body = GenerateOfferEmailBody(
                                _emailTemplate.influencerOffer,
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
                            body = GenerateOfferEmailBody(
                                _emailTemplate.brandOffer,
                                brandUser.DisplayName!,
                                influencerUser.DisplayName!,
                                offer,
                                job
                            );
                        }
                        #endregion
                        break;
                    case EOfferStatus.WaitingPayment:
                        #region Approve
                        subject = "Offer của bạn đã được chấp thuận";
                        if (offer.From == (int)AuthEnumContainer.ERole.Influencer)
                        {
                            recipientEmail = brandUser.Email;
                            body = GenerateBodyEmailToConfirm(influencerUser.DisplayName!, brandUser.DisplayName!, "Thông báo Offer đã được ", offer, job);
                        }
                        else if (offer.From == (int)AuthEnumContainer.ERole.Brand)
                        {
                            recipientEmail = influencerUser.Email;
                            body = GenerateBodyEmailToConfirm(brandUser.DisplayName!, influencerUser.DisplayName!, "Thông báo Offer đã được ", offer, job);
                        }
                        #endregion
                        break;
                    case EOfferStatus.Rejected:
                        #region Reject
                        subject = "Offer của bạn đã bị từ chối";
                        if (offer.From == (int)AuthEnumContainer.ERole.Influencer)
                        {
                            recipientEmail = brandUser.Email;
                            body = GenerateBodyEmailToConfirm(influencerUser.DisplayName!, brandUser.DisplayName!, "Thông báo Offer đã bị ", offer, job);
                        }
                        else if (offer.From == (int)AuthEnumContainer.ERole.Brand)
                        {
                            recipientEmail = influencerUser.Email;
                            body = GenerateBodyEmailToConfirm(brandUser.DisplayName!, influencerUser.DisplayName!, "Thông báo Offer đã bị ", offer, job);
                        }
                        #endregion
                        break;
                    default:
                        _loggerService.Error($"Error when send mail: {offerType}");
                        return;
                }

                _ = Task.Run(async () => await _emailService.SendEmail(new List<string> { recipientEmail }, subject, body));
            }
            catch (Exception ex)
            {
                _loggerService.Error("Lỗi khi gửi mail offer" + ex);
            }


        }
        private static string GenerateOfferEmailBody(string template, string brandName, string influencerName, Offer offer, Job job)
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

        public static string GenerateBodyEmailToConfirm(string name, string actor, string title, Offer offer, Job job)
        {
            var body = _emailTemplate.confirmOffer.Replace("{projectName}", _configManager.ProjectName)
                                                    .Replace("{Title}", title + ((EOfferStatus)offer.Status).GetEnumDescription())
                                                    .Replace("{Name}", name)
                                                    .Replace("{Actor}", actor)
                                                    .Replace("{Status}", ((EOfferStatus)offer.Status).GetEnumDescription())
                                                    .Replace("{ContentType}", ((EContentType)offer.ContentType).GetEnumDescription())
                                                    .Replace("{Price}", offer?.Price.ToString("N0") + " VND" ?? "")
                                                    .Replace("{CreatedAt}", offer?.CreatedAt.ToString())
                                                    .Replace("{Description}", offer?.Description)
                                                    .Replace("{ResponseTime}", DateTime.Now.ToString())
                                                    .Replace("{DetailsLink}", "");
            return body;
        }
        #endregion
    }
}
