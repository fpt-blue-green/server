using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Campaign
{
    public Guid Id { get; set; }

    public Guid BrandId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public decimal? Budget { get; set; }

    public short? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<AdsCampaign> AdsCampaigns { get; set; } = new List<AdsCampaign>();

    public virtual Brand Brand { get; set; } = null!;

    public virtual ICollection<CampaignTag> CampaignTags { get; set; } = new List<CampaignTag>();

    public virtual ICollection<InfluencerJobHistory> InfluencerJobHistories { get; set; } = new List<InfluencerJobHistory>();

    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
}
