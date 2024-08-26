using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Job
{
    public Guid Id { get; set; }

    public Guid InfluencerId { get; set; }

    public Guid CampaignId { get; set; }

    public int Status { get; set; }

    public string Link { get; set; } = null!;

    public int ViewCount { get; set; }

    public int LikesCount { get; set; }

    public virtual Campaign Campaign { get; set; } = null!;

    public virtual Influencer Influencer { get; set; } = null!;

    public virtual ICollection<Offer> Offers { get; set; } = new List<Offer>();

    public virtual ICollection<PaymentBooking> PaymentBookings { get; set; } = new List<PaymentBooking>();
}
