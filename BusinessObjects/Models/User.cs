using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int Role { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<AdminAction> AdminActions { get; set; } = new List<AdminAction>();

    public virtual ICollection<BannedUser> BannedUserBannedBies { get; set; } = new List<BannedUser>();

    public virtual ICollection<BannedUser> BannedUserUsers { get; set; } = new List<BannedUser>();

    public virtual ICollection<Brand> Brands { get; set; } = new List<Brand>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Influencer> Influencers { get; set; } = new List<Influencer>();
}
