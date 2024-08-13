using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class AdsCampaign
{
    public Guid Id { get; set; }

    public Guid? CampaignId { get; set; }

    public string? Content { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public virtual Campaign? Campaign { get; set; }
}
