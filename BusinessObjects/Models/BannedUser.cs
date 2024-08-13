using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class BannedUser
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public string? Reason { get; set; }

    public DateTime? BanDate { get; set; }

    public DateTime? UnbanDate { get; set; }

    public Guid? BannedById { get; set; }

    public virtual User? BannedBy { get; set; }

    public virtual User? User { get; set; }
}
