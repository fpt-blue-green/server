﻿using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Job
{
    public Guid Id { get; set; }

    public Guid InfluencerId { get; set; }

    public Guid CampaignId { get; set; }

    public int Status { get; set; }

    public DateTime? CompleteAt { get; set; }

    public virtual Campaign Campaign { get; set; } = null!;

    public virtual Influencer Influencer { get; set; } = null!;

    public virtual ICollection<JobDetails> JobDetails { get; set; } = new List<JobDetails>();

    public virtual ICollection<Offer> Offers { get; set; } = new List<Offer>();

    public virtual ICollection<PaymentBooking> PaymentBookings { get; set; } = new List<PaymentBooking>();
}
