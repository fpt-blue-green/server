namespace BusinessObjects.Models;

public partial class JobDetails
{
    public Guid Id { get; set; }

    public Guid JobId { get; set; }

    public int ViewCount { get; set; }

    public int LikesCount { get; set; }

    public int CommentCount { get; set; }

    public string? Link { get; set; }

    public DateTime UpdateDate { get; set; }

    public virtual Job Job { get; set; } = null!;
}
