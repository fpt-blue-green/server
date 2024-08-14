using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Influencer
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public bool? IsPublish { get; set; }

    public string FullName { get; set; } = null!;

    public string NickName { get; set; } = null!;

    public int? Gender { get; set; }

    public string? Bio { get; set; }

    public string Phone { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public bool? IsDeleted { get; set; }

    public decimal? RateAverage { get; set; }

    public virtual ICollection<Channel> Channels { get; set; } = new List<Channel>();

    public virtual ICollection<Deal> Deals { get; set; } = new List<Deal>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<InfluencerJobHistory> InfluencerJobHistories { get; set; } = new List<InfluencerJobHistory>();

    public virtual ICollection<InfluencerTag> InfluencerTags { get; set; } = new List<InfluencerTag>();

    public virtual ICollection<Package> Packages { get; set; } = new List<Package>();

    public virtual User? User { get; set; }
}
