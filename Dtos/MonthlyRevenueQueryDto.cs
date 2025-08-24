namespace MonthlyRevenueApi.Dtos
{
    public class MonthlyRevenueQueryDto
    {
        public string CompanyId { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string IndustryName { get; set; } = string.Empty;
        public string DataYearMonth { get; set; } = string.Empty;
        public string ReportDate { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public decimal? LastMonthRevenue { get; set; }
        public decimal? LastYearMonthRevenue { get; set; }
        public decimal? MoMChange { get; set; }
        public decimal? YoYChange { get; set; }
        public decimal? AccRevenue { get; set; }
        public decimal? LastYearAccRevenue { get; set; }
        public decimal? AccChange { get; set; }
        public string? Memo { get; set; }
    }
}
