namespace BusinessObjects
{
    public class CampaignJobDetailBaseDTO
    {
        public string TotalJob { get; set; }
        public string TargetReaction { get; set; }
        public string TotalView { get; set; }
        public string TotalLike { get; set; }
        public string TotalComment { get; set; }
        public string TotalReaction { get; set; }
        public string TotalFee { get; set; }
    }

    public class CampaignJobDetailDTO : CampaignJobDetailBaseDTO
    {
        public string Name {  get; set; }
        public string Avatar {  get; set; }
       
    }

    public class CampaignDailyStatsDTO
    {
        public string Date { get; set; }
        public int TotalReaction { get; set; }
    }

}
