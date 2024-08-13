using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Brand
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public string Name { get; set; } = null!;

    public bool? IsPremium { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<Campaign> Campaigns { get; set; } = new List<Campaign>();

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User? User { get; set; }
}
