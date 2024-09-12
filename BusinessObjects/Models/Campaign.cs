﻿using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Campaign
{
    public Guid Id { get; set; }

    public Guid BrandId { get; set; }

    public string Name { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public decimal? Budget { get; set; }

    public short? Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Brand Brand { get; set; }

    public virtual ICollection<CampaignImage> CampaignImages { get; set; } = new List<CampaignImage>();

    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();

    public virtual ICollection<Requirement> Requirements { get; set; } = new List<Requirement>();

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
