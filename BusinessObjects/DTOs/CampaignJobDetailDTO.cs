namespace BusinessObjects
{
    public class CampaignJobDetailBaseDTO
    {
        public int TotalJob { get; set; }
        public int TargetReaction { get; set; }
        public int TotalView { get; set; }
        public int TotalLike { get; set; }
        public int TotalComment { get; set; }
        public long TotalReaction { get; set; }
        public decimal TotalFee { get; set; }
    }

    public class CampaignJobDetailDTO : CampaignJobDetailBaseDTO
    {
        public string Name { get; set; }
        public string Avatar { get; set; }

    }

    public class CampaignDailyStatsDTO
    {
        public string Date { get; set; }
        public long TotalReaction { get; set; }
    }

}
