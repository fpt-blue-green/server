using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class PaymentHistory
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public decimal? Amount { get; set; }

    public string BankInformation { get; set; }

    public int? Status { get; set; }

    public int Type { get; set; }

    public DateTime? Date { get; set; }

    public string? AdminMessage { get; set; }

    public virtual User? User { get; set; }
}
