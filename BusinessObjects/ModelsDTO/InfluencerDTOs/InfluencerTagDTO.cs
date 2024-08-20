﻿using BusinessObjects.Models;
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
        public virtual TagDTO Tag { get; set; } = null!;
    }
}
