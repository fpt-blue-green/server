using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Repositories;

namespace Service
{
    public class FavoriteService : IFavoriteService
    {
        private static readonly IFavoriteRepository _favoriteRepository = new FavoriteRepository();
        private static readonly IBrandRepository _brandRepository = new BrandRepository();
        private static readonly IMapper _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        }).CreateMapper();

        public async Task CreateFavorite(Guid id, UserDTO user)
        {
            var brandFavorites = await _brandRepository.GetBrandWithFavoriteByUserId(user.Id);
            if (brandFavorites == null)
            {
                throw new InvalidOperationException("Brand không tồn tại.");
            }
            if(brandFavorites.Favorites.Count == 10 && !brandFavorites.IsPremium)
            {
                throw new InvalidOperationException("Bạn chỉ có thể thêm tối đa 10 Influencer vào danh sách yêu thích. Vui lòng nâng cấp lên Premium để có thể thêm nhiều hơn.");
            }

            var favorites = brandFavorites?.Favorites?.Any(x => x.InfluencerId == id) ?? false;
            if (favorites)
            {
                throw new InvalidOperationException("Influencer đã được thêm vào danh sách yêu thích.");
            }
            var favorite = new Favorite()
            {
                BrandId = brandFavorites!.Id,
                InfluencerId = id,
            };

            await _favoriteRepository.CreateFavorite(favorite);
        }

        public async Task DeleteFavoriteById(Guid favoriteId)
        {
            await _favoriteRepository.DeleteFavorite(favoriteId);
        }
        public async Task DeleteFavoriteByInfluencerId(Guid influencerId, UserDTO user)
        {
            var favorites = await _favoriteRepository.GetAllFavoriteByUserId(user.Id) ?? throw new KeyNotFoundException();
            await _favoriteRepository.DeleteFavoriteByInfluencerId(favorites.FirstOrDefault(f => f.InfluencerId == influencerId)!);
        }

        public async Task<IEnumerable<InfluencerDTO>> GetAllFavorites(UserDTO user)
        {
            var favorites = await _favoriteRepository.GetAllFavoriteByUserId(user.Id);
            
            return _mapper.Map<IEnumerable<InfluencerDTO>>(favorites.Select(f => f.Influencer));
        }
    }
}
