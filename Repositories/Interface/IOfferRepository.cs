﻿using BusinessObjects.Models;

namespace Repositories
{
    public interface IOfferRepository
    {
        Task Create(Offer offer);
        Task<Offer> GetById (Guid id);
        Task Update(Offer offer);
        Task UpdateJobAndOffer(Offer offer);
        Task<IEnumerable<Offer>> GetByJobId(Guid jobId);
        Task<IEnumerable<Offer>> GetOfferByCampaignAndInfluencerId(Guid campaignId, Guid influencerId);
    }
}
