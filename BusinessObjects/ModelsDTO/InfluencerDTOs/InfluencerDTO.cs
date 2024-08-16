using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.ModelsDTO.InfluencerDTO
{
    public class InfluencerDTO
    {
        public Guid Id { get; set; }

       /* public Guid UserId { get; set; }*/

        public string FullName { get; set; } = null!;

        public string NickName { get; set; } = null!;

        public int? Gender { get; set; }

        public string? Bio { get; set; }

        public string Phone { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public decimal? RateAverage { get; set; }
        public decimal? AveragePrice { get; set; }

        public virtual ICollection<ChannelDTO> Channels { get; set; } = new List<ChannelDTO>();
        public virtual ICollection<InfluencerTagDTO> InfluencerTags { get; set; } = new List<InfluencerTagDTO>();
        public virtual ICollection<PackageDTO> Packages { get; set; } = new List<PackageDTO>();
    }
}
