using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.ModelsDTO.InfluencerDTO
{
    public class InfluencerTagDTO
    {
        public Guid Id { get; set; }

        public Guid InfluencerId { get; set; }

        public Guid TagId { get; set; }
        public virtual Tag Tag { get; set; } = null!;
    }
}
