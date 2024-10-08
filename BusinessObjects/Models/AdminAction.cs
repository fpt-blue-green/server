using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class AdminAction
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public int ActionType { get; set; }

    public string ObjectType { get; set; } = null!;

    public string ActionDetails { get; set; } = null!;

    public DateTime ActionDate { get; set; }

    public virtual User User { get; set; } = null!;
}
