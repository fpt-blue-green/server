using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Influencer
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public bool IsPublish { get; set; }

    public string FullName { get; set; }

    public string Slug { get; set; }

    public string Summarise { get; set; }

    public string Description { get; set; }

    public int Gender { get; set; }

    public string? Phone { get; set; }

    public string Address { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public bool IsDeleted { get; set; }

    public decimal? RateAverage { get; set; }

    public decimal? AveragePrice { get; set; }

    public virtual ICollection<Channel> Channels { get; set; } = new List<Channel>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<InfluencerImage> InfluencerImages { get; set; } = new List<InfluencerImage>();

    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();

    public virtual ICollection<Package> Packages { get; set; } = new List<Package>();

    public virtual User User { get; set; }

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
