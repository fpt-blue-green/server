using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.ModelsDTO.UserDTOs
{
    public class ImagesDTO
    {
        public Guid Id { get; set; }

        public string Url { get; set; } = null!;

        public string? Description { get; set; }

        public int? ImageType { get; set; }

    }
}
