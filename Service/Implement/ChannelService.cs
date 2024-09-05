using AutoMapper;
using BusinessObjects.DTOs.InfluencerDTO;
using BusinessObjects.DTOs.UserDTOs;
using BusinessObjects.Enum;
using BusinessObjects.Models;
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

        public async Task CreateInfluencerChannel(UserDTO user, List<ChannelPlatFormUserNameDTO> channels)
        {
            // Lấy ID của influencer từ repository
            Guid influencerId = _userRepository.GetUserById(user.Id).Result.Influencers.FirstOrDefault()!.Id;

            foreach (var item in channels)
            {
                // Lấy đối tượng channel từ repository nếu có
                var existingChannel = await _channelRepository.GetChannel(influencerId, item.Platform);

                if (!existingChannel.Any())
                {
                    // Nếu không tồn tại kênh, tạo kênh mới
                    Channel channelNew = new Channel
                    {
                        Id = Guid.NewGuid(),
                        InfluencerId = influencerId
                    };
                    var channel = await GetChannelStatData(channelNew, item.Platform, item.UserName!);
                    channelNew = channel;
                    await _channelRepository.CreateChannel(channelNew);
                }
                else
                {
                    // Nếu kênh đã tồn tại, cập nhật kênh
                    var channel = await GetChannelStatData(existingChannel.FirstOrDefault()!, item.Platform, item.UserName!);
                    var channelNew = channel;
                    await _channelRepository.UpdateChannel(channelNew);
                }
            }
        }

        public async Task UpdateInfluencerChannel(Channel channel)
        {
            var channelNew = await GetChannelStatData(channel, (EPlatform)channel.Type, channel.UserName!);
            await _channelRepository.UpdateChannel(channelNew);
        }

        public async Task<List<ChannelPlatFormUserNameDTO>> GetChannelPlatFormUserNames(UserDTO user)
        {
            var result = new List<ChannelPlatFormUserNameDTO>();
            var influencer = _userRepository.GetUserById(user.Id).Result.Influencers.FirstOrDefault()!;

            if (influencer != null)
            {
                var existingChannels = await _channelRepository.GetChannels(influencer.Id);
                result = existingChannels.Select(channel => new ChannelPlatFormUserNameDTO
                {
                    Platform = (EPlatform)channel.Type,
                    UserName = channel.UserName
                }).ToList();
            }
            return result;
        }

        public async Task<Channel> GetChannelStatData(Channel channel, EPlatform ePlatform, string id)
        {
            // Gọi phương thức để lấy dữ liệu hồ sơ kênh từ repository
            var data = await _utilityRepository.GetChannelProfile((int)ePlatform, id);

            // Cập nhật thuộc tính của đối tượng channel
            channel.UserName = id;
            channel.Type = (int)ePlatform;
            _mapper.Map(data, channel);

            return channel;
        }


    }
}
