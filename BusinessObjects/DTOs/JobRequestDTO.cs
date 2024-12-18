﻿using static BusinessObjects.JobEnumContainer;

namespace BusinessObjects
{
    public class JobDTO
    {
        public Guid Id { get; set; }

        public Guid? InfluencerId { get; set; }

        public Guid CampaignId { get; set; }

        public EJobStatus Status { get; set; }

        public virtual OfferDTO Offer { get; set; } = null!;

        public virtual CampaignDTO Campaign { get; set; } = null!;

        public virtual InfluencerDTO Influencer { get; set; } = null!;
    }

    public class JobRequestDTO
    {
        public Guid? InfluencerId { get; set; }

        public Guid CampaignId { get; set; }

        public EJobStatus Status { get; set; }

    }

    public class JobLinkDTO
    {
        public List<string> Link { get; set; }
    }

    public class JobStatistical
    {
        public EJobStatus JobStatus { get; set; }  // Enum EJobStatus
        public int Count { get; set; }
    }

    public class JobInfluencerDTO
    {
        public Guid Id { get; set; }

        public Guid? InfluencerId { get; set; }

        public Guid CampaignId { get; set; }

        public EJobStatus Status { get; set; }
        public virtual OfferDTO Offer { get; set; } = null!;

    }

    public class JobLinkResponseDTO
    {
        public string Link { get; set; }
        public bool  IsApprove { get; set; }
    }
}
