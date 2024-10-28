using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class CampaignMeetingRoom
{
    public Guid Id { get; set; }

    public string RoomName { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime StartAt { get; set; }

    public DateTime? EndAt { get; set; }

    public string RoomLink { get; set; } = null!;

    public string? Participants { get; set; }

    public string? Description { get; set; }

    public bool IsFirstTime { get; set; }

    public DateTime? CreatedAt { get; set; }

    public Guid CampaignId { get; set; }

    public virtual Campaign Campaign { get; set; } = null!;
}
