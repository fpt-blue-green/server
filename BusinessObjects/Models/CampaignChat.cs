using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class CampaignChat
{
    public Guid Id { get; set; }

    public Guid? SenderId { get; set; }

    public string? RoomName { get; set; }

    public string? Message { get; set; }

    public DateTime SendTime { get; set; }

    public Guid CampaignId { get; set; }

    public virtual Campaign? Campaign { get; set; }

    public virtual User? Sender { get; set; }
}
