using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Pgvector;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjects.Models;

public partial class Embedding
{
    public Guid Id { get; set; }

    public Guid? InfluencerId { get; set; }

    public Guid? CampaignId { get; set; }

    [Column(TypeName = "vector(1536)")]
    public Vector? EmbeddingValue { get; set; }

    public virtual Campaign? Campaign { get; set; }

    public virtual Influencer? Influencer { get; set; }
}
