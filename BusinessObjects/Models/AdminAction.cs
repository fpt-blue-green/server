﻿using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class AdminAction
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public int? ActionType { get; set; }

    public string? ActionDetails { get; set; }

    public DateTime? ActionDate { get; set; }

    public virtual User User { get; set; } = null!;
}
