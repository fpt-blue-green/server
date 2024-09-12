

namespace BusinessObjects
{
	public enum EContentType
	{
		TikTokPost = EPlatform.Tiktok * 10,
		TikTokStory,
		TikTokLive,
		InstagramFeedPost = EPlatform.Instagram * 10,
		InstagramReel,
		InstagramStory,
		InstagramLive,
		YouTubeVideo = EPlatform.Youtube *10,
		YouTubeShort,
		YouTubeLive,
	}
}
