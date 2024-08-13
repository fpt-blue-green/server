﻿using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IInfluencerRepository
    {
        Task<IEnumerable<Influencer>> GetAlls();
        Task<Influencer> GetById(Guid id);
        Task Create(Influencer influencer);
        Task Update(Influencer influencer);
        Task Delete(Guid id);
    }
}
