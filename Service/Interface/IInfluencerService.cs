<<<<<<< HEAD
﻿using BusinessObjects.DTOs.InfluencerDTO;
using BusinessObjects.Models;
=======
﻿using BusinessObjects.Models;
using BusinessObjects.ModelsDTO;
using BusinessObjects.ModelsDTO.InfluencerDTO;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
>>>>>>> 22856a3 (update logic uploadImage & Api Influencer)

namespace Service.Interface
{
    public interface IInfluencerService
    {
        Task<List<InfluencerDTO>> GetAllInfluencers();
        Task<List<InfluencerDTO>> GetAllInfluencers(InfluencerFilterDTO filter);
        Task<InfluencerDTO> GetInfluencerById(Guid id);
        Task<List<InfluencerDTO>> GetTopInfluencer();
        Task<List<InfluencerDTO>> GetTopInstagramInfluencer();
        Task<List<InfluencerDTO>> GetTopTiktokInfluencer();
        Task<List<InfluencerDTO>> GetTopYoutubeInfluencer();
        Task<ApiResponse<Influencer>> CreateInfluencer(Influencer influencer);
        Task UpdateInfluencer(Influencer influencer);
        Task DeleteInfluencer(Guid id);
    }
}