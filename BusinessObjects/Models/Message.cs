using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Message
{
    public Guid Id { get; set; }

    public Guid SenderId { get; set; }

    public Guid? ReceiverId { get; set; }

    public Guid? CampaignChatId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime SentAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual CampaignChat? CampaignChat { get; set; }

    public virtual User? Receiver { get; set; }

    public virtual User Sender { get; set; } = null!;
}
