using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Image
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Url { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ImageType { get; set; }

    public virtual User User { get; set; } = null!;
}
