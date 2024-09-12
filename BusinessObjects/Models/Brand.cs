using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Brand
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Name { get; set; }

    public string Address { get; set; }

    public string Description { get; set; }

    public bool IsPremium { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual ICollection<Campaign> Campaigns { get; set; } = new List<Campaign>();

    public virtual User User { get; set; }
}
