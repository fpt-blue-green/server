using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Repositories;

namespace Service.Implement
{
    public class OfferService : IOfferService
    {
        private static IInfluencerRepository _influencerRepository = new InfluencerRepository();
        private static IOfferRepository _offerRepository = new OfferRepository();
        private static IJobRepository _jobRepository = new JobRepository();
        private static readonly IMapper _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        }).CreateMapper();


        public async Task CreateOffer(UserDTO userDTO, OfferCreateRequestDTO offerCreateRequestDTO)
        {
            //Create Job First
            if (userDTO.Role == AuthEnumContainer.ERole.Admin)
            {
                throw new InvalidOperationException("Admin không thể tạo được Offer.");
            }

            if(userDTO.Role == AuthEnumContainer.ERole.Influencer)
            {
                var influencer = await _influencerRepository.GetByUserId(userDTO.Id);
                if(influencer == null)
                {
                    throw new KeyNotFoundException();
                }
                offerCreateRequestDTO.Job.InfluencerId = influencer.Id;
            }

            if(offerCreateRequestDTO.Job.InfluencerId == null)
            {
                throw new Exception("CreateOffer: InfluencerID bị null");
            }

            var jobnew = _mapper.Map<Job>(offerCreateRequestDTO.Job);
            jobnew.Link = null;
            await _jobRepository.Create(jobnew);

            //Create offer
            var offer = _mapper.Map<Offer>(offerCreateRequestDTO.Offer);
            offer.JobId = jobnew.Id;
            offer.From = (int)userDTO.Role;
            await _offerRepository.Create(offer);

            //Send Mail
            await SendMail(offer);
        }

        public async Task SendMail(Offer offer)
        {

        }
    }
}
