using AutoMapper;
using BusinessObjects.DTOs.UserDTOs;
using BusinessObjects.Enum;
using BusinessObjects.Models;
using Newtonsoft.Json.Linq;
using Repositories.Implement;
using Repositories.Interface;
using Service.Implement.UtilityServices;
using Service.Interface;
using Service.Interface.UtilityServices;

namespace Service.Implement
{
    public class ChannelService : IChannelService
    {
        private static IChannelRepository _channelRepository = new ChannelRepository();
        private static IUserRepository _userRepository = new UserRepository();
        private static IUtilityService _utilityRepository = new UtilityService();
        private readonly IMapper _mapper;
        public ChannelService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task CreateInfluencerChannel(UserDTO user, Dictionary<int, string> channels)
        {
            // Lấy ID của influencer từ repository
            Guid influencerId = _userRepository.GetUserById(user.Id).Result.Influencers.FirstOrDefault()!.Id;

            foreach (var item in channels)
            {
                // Lấy đối tượng channel từ repository nếu có
                var existingChannel = await _channelRepository.GetChannel(influencerId, (EPlatform)item.Key);

                // Khởi tạo đối tượng channel mới
                Channel channelNew = new Channel
                {
                    Id = Guid.NewGuid(),
                    InfluencerId = influencerId
                };

                switch ((EPlatform)item.Key)
                {
                    case EPlatform.Tiktok:
                        // Lấy dữ liệu kênh từ phương thức GetChannelTitokData
                        var tiktokChannel = await GetChannelTitokData(channelNew, item.Key, item.Value);
                        channelNew = tiktokChannel;
                        break;
                    default:
                        throw new Exception("CreateInfluencerChannel: Invalid input!");
                }

                if (!existingChannel.Any())
                {
                    // Nếu không tồn tại kênh, tạo kênh mới
                    await _channelRepository.CreateChannel(channelNew);
                }
                else
                {
                    // Nếu kênh đã tồn tại, cập nhật kênh
                    await _channelRepository.UpdateChannel(channelNew);
                }
            }
        }

        public async Task<Channel> GetChannelTitokData(Channel channel, int ePlatform, string id)
        {
            // Gọi phương thức để lấy dữ liệu hồ sơ kênh từ repository
            var data = await _utilityRepository.GetChannelProfile(ePlatform, id);

            // Phân tích chuỗi JSON thành JObject
            JObject jsonObject = JObject.Parse(data);

            // Trích xuất đối tượng "stats" từ JSON
            var stats = jsonObject["stats"];

            // Cập nhật thuộc tính của đối tượng channel
            channel.UserName = id;
            channel.FollowersCount = (int)stats["followerCount"]!;
            channel.LikesCount = (int)stats["heartCount"]!;
            channel.PostsCount = (int)stats["videoCount"]!;
            channel.Type = ePlatform;

            return channel;
        }


    }
}
