using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Payment
{
    public Guid Id { get; set; }

    public Guid? BrandId { get; set; }

    public decimal? Amount { get; set; }

    public int? Status { get; set; }

    public DateTime? PaymentDate { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual Brand? Brand { get; set; }
}
