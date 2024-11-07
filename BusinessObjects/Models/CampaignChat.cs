using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class CampaignChat
{
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }

    public string RoomName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }


    public virtual Campaign Campaign { get; set; } = null!;

    public virtual ICollection<ChatMember> ChatMembers { get; set; } = new List<ChatMember>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
