using System.ComponentModel.DataAnnotations.Schema;
using Pgvector;

namespace BusinessObjects.Models;

public partial class Influencer
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public bool IsPublish { get; set; }

    public string FullName { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string Summarise { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int Gender { get; set; }

    public string? Phone { get; set; }

    public string Address { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public decimal? RateAverage { get; set; }

    public decimal? AveragePrice { get; set; }

    [Column(TypeName = "vector(1536)")]
    public Vector? Embedding { get; set; }

    public virtual ICollection<Channel> Channels { get; set; } = new List<Channel>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<InfluencerImage> InfluencerImages { get; set; } = new List<InfluencerImage>();

    public virtual ICollection<InfluencerReport> InfluencerReports { get; set; } = new List<InfluencerReport>();

    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();

    public virtual ICollection<Package> Packages { get; set; } = new List<Package>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
