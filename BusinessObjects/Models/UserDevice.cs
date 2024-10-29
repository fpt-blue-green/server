using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class UserDevice
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string? RefreshToken { get; set; }

    public string? BrowserName { get; set; }

    public DateTime LastLoginTime { get; set; }

    public DateTime RefreshTokenTime { get; set; }

    public string? DeviceOperatingSystem { get; set; }

    public string? DeviceType { get; set; }

    public virtual User User { get; set; } = null!;
}
