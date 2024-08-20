using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.ModelsDTO.InfluencerDTO
{
    public class PackageDTO
    {
        public Guid Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public decimal? Price { get; set; }

        public bool? IsDisplay { get; set; }

        public int? Quantity { get; set; }

        public short? Status { get; set; }
    }
}
