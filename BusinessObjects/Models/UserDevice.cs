using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class UserDevice
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string? RefreshToken { get; set; }

    public string? UserAgent { get; set; }

    public DateTime? LoginTime { get; set; }

    public bool? IsActive { get; set; }

    public virtual User User { get; set; } = null!;
}
