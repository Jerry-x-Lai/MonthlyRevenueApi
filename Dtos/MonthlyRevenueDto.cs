namespace MonthlyRevenueApi.Dtos
{
    public sealed class MonthlyRevenueDto
    {
        public string? ReportDate { get; set; }
        public string? DataYearMonth { get; set; }
        public string? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string? IndustryName { get; set; }
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
