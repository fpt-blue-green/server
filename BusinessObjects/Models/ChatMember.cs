using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class ChatMember
{
    public Guid CampaignChatId { get; set; }

    public Guid UserId { get; set; }

    public DateTime? JoinAt { get; set; }

    public virtual CampaignChat CampaignChat { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
