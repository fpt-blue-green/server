using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Campaign
{
    public Guid Id { get; set; }

    public Guid BrandId { get; set; }

    public string Name { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal? Budget { get; set; }

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual ICollection<CampaignChat> CampaignChats { get; set; } = new List<CampaignChat>();

    public virtual ICollection<CampaignContent> CampaignContents { get; set; } = new List<CampaignContent>();

    public virtual ICollection<CampaignImage> CampaignImages { get; set; } = new List<CampaignImage>();

    public virtual ICollection<CampaignMeetingRoom> CampaignMeetingRooms { get; set; } = new List<CampaignMeetingRoom>();

    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
