using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class PaymentBooking
{
    public Guid Id { get; set; }

    public Guid? JobId { get; set; }

    public decimal? Amount { get; set; }

    public int? Type { get; set; }

    public DateTime? PaymentDate { get; set; }

    public virtual Job? Job { get; set; }
}
