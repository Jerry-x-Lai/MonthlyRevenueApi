using System.Collections.Generic;

namespace MonthlyRevenueApi.Infrastructure.Database
{
    public class SqlPropety
    {
        public string DbConnectionName { get; set; }
        public string SpName { get; set; }
        public List<object> FlushCacheGroups { get; set; } // 可依需求調整型別
        public List<object> CacheGroups { get; set; } // 可依需求調整型別
        public string CacheKey { get; set; }
        public int CacheDuration { get; set; }
    }
}
