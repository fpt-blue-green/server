

using System.ComponentModel;

namespace BusinessObjects
{
    public enum EContentType
    {
        [Description("TikTok Post")]
        TikTokPost = EPlatform.Tiktok * 10,

        [Description("TikTok Story")]
        TikTokStory,

        [Description("TikTok Live")]
        TikTokLive,

        [Description("Instagram Feed Post")]
        InstagramFeedPost = EPlatform.Instagram * 10,

        [Description("Instagram Reel")]
        InstagramReel,

        [Description("Instagram Story")]
        InstagramStory,

        [Description("Instagram Live")]
        InstagramLive,

        [Description("YouTube Video")]
        YouTubeVideo = EPlatform.Youtube * 10,

        [Description("YouTube Short")]
        YouTubeShort,

        [Description("YouTube Live")]
        YouTubeLive
    }
}
