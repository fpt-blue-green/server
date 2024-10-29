using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string? Password { get; set; }

    public string? DisplayName { get; set; }

    public string? Avatar { get; set; }

    public int Role { get; set; }

    public decimal Wallet { get; set; }

    public int Provider { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsBanned { get; set; }

    public virtual ICollection<AdminAction> AdminActions { get; set; } = new List<AdminAction>();

    public virtual ICollection<BannedUser> BannedUserBannedBies { get; set; } = new List<BannedUser>();

    public virtual ICollection<BannedUser> BannedUserUsers { get; set; } = new List<BannedUser>();

    public virtual Brand? Brand { get; set; }

    public virtual ICollection<CampaignChat> CampaignChats { get; set; } = new List<CampaignChat>();

    public virtual ICollection<ChatRoom> ChatRoomReceivers { get; set; } = new List<ChatRoom>();

    public virtual ICollection<ChatRoom> ChatRoomSenders { get; set; } = new List<ChatRoom>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual Influencer? Influencer { get; set; }

    public virtual ICollection<InfluencerReport> InfluencerReports { get; set; } = new List<InfluencerReport>();

    public virtual ICollection<PaymentHistory> PaymentHistories { get; set; } = new List<PaymentHistory>();

    public virtual ICollection<UserDevice> UserDevices { get; set; } = new List<UserDevice>();
}
