using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class CampaignTag
{
    public Guid Id { get; set; }

    public Guid? CampaignId { get; set; }

    public Guid? TagId { get; set; }

    public virtual Campaign? Campaign { get; set; }

    public virtual Tag? Tag { get; set; }
}
