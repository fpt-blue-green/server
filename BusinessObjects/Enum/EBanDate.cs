using System.ComponentModel;

namespace BusinessObjects
{
    public enum EBanDate
    {
        [Description("Hủy Bỏ Lệnh Cấm.")]
        None,

        [Description("Bị ban 1 tuần")]
        OneWeek,

        [Description("Bị ban 2 tuần")]
        TwoWeeks,

        [Description("Bị ban 1 tháng")]
        OneMonth,

        [Description("Bị ban 3 tháng")]
        ThreeMonths,

        [Description("Bị ban 6 tháng")]
        SixMonths,

        [Description("Bị ban 1 năm")]
        OneYear,

        [Description("Bị ban 2 năm")]
        TwoYear,

        [Description("Bị ban vô thời hạn")]
        Indefinitely
    }

    public static class EBanDateExtensions
    {
        public static TimeSpan ToTimeSpan(this EBanDate banDate)
        {
            return banDate switch
            {
                EBanDate.OneWeek => TimeSpan.FromDays(7),
                EBanDate.TwoWeeks => TimeSpan.FromDays(14),
                EBanDate.OneMonth => TimeSpan.FromDays(30),
                EBanDate.ThreeMonths => TimeSpan.FromDays(90),
                EBanDate.SixMonths => TimeSpan.FromDays(180),
                EBanDate.OneYear => TimeSpan.FromDays(365),
                EBanDate.TwoYear => TimeSpan.FromDays(730),
                EBanDate.Indefinitely => TimeSpan.MaxValue, // Không có giới hạn
                _ => throw new Exception()
            };
        }
    }
}
