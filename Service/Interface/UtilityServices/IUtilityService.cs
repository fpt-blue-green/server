﻿using BusinessObjects;
namespace Service
{
	public interface IUtilityService
	{
		IEnumerable<string> GetCitiesWithCountry(string keyword);

		Task<ChannelStatDTO> GetChannelProfile(int platform, string channelId);
		Task<string> GetVideoInformation(int platform, string url);
	}
}
