﻿using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Repositories;

namespace Service
{
    public class OfferService : IOfferService
    {
        private static readonly IInfluencerRepository _influencerRepository = new InfluencerRepository();
        private static readonly IOfferRepository _offerRepository = new OfferRepository();
        private static readonly IJobRepository _jobRepository = new JobRepository();
        private static readonly IUserRepository _userRepository = new UserRepository();
        private static readonly ConfigManager _configManager = new ConfigManager();
        private static readonly EmailTemplate _emailTempalte = new EmailTemplate();
        private static readonly IEmailService _emailService = new EmailService(); 
        private static readonly IMapper _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        }).CreateMapper();
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
            jobnew.Link = null;
            await _jobRepository.Create(jobnew);

            //Create offer
            var offer = _mapper.Map<Offer>(offerCreateRequestDTO.Offer);
            offer.Status =(int) JobEnumContainer.EOfferStatus.Offering;
            offer.JobId = jobnew.Id;
            offer.From = (int)userDTO.Role;
            await _offerRepository.Create(offer);

            //Send Mail
            await SendMail(offer, jobnew);
        }

        public static async Task SendMail(Offer offer, Job job,bool isReOffer = false)
        {
            var brandUser = await _userRepository.GetUserByCampaignId(job.CampaignId);
            var influencerUser = await _userRepository.GetUserByInfluencerId(job.InfluencerId);

            string subject = "";
            string body = "";
            string recipientEmail = "";
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
                    brandUser.DisplayName,
                    influencerUser.DisplayName,
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
                    brandUser.DisplayName,
                    influencerUser.DisplayName,
                    offer,
                    job
                );
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
                .Replace("{JobLink}",  "")
                .Replace("{JobPayment}", offer?.Price.ToString("N0") ?? "");
        }

        public async Task ReOffer(Guid id, UserDTO userDTO, ReOfferDTO reOfferDTO)
        {
            //set status old offer to reject
            var oldOffer = await _offerRepository.GetById(id);
            oldOffer.Status = (int)JobEnumContainer.EOfferStatus.Rejected;
            await _offerRepository.Update(oldOffer);

            //create new offer (Re-offer)
            var job = await _jobRepository.GetJobOfferById(reOfferDTO.JobId);
            var newOffer = new Offer
            {
                Description = reOfferDTO.Description,
                Price = reOfferDTO.Price,
                Quantity = reOfferDTO.Quantity,
                ContentType = oldOffer.ContentType,
                CreatedAt = oldOffer.CreatedAt,
                Duration = oldOffer.Duration,
                From = (int)userDTO.Role,
                Platform = oldOffer.Platform,
                Status = (int)JobEnumContainer.EOfferStatus.Offering,
                JobId = reOfferDTO.JobId,
            };
            await _offerRepository.Create(newOffer);
            //Send Mail
            await SendMail(newOffer, job,true);
        }
    }
}