using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using MonthlyRevenueApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MonthlyRevenueApi.Infrastructure.Database; // Add this if SqlExtension is in Extensions namespace

namespace MonthlyRevenueApi.Services
{
    public class MonthlyRevenueService : IMonthlyRevenueService
    {
        private readonly SqlExtension _sqlExtension;
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly ILogger<MonthlyRevenueService> _logger;
        public MonthlyRevenueService(SqlExtension sqlExtension, IDbConnectionFactory dbConnectionFactory, ILogger<MonthlyRevenueService> logger)
        {
            _sqlExtension = sqlExtension;
            _dbConnectionFactory = dbConnectionFactory;
            _logger = logger;
        }

        public async Task<IEnumerable<MonthlyRevenue>> GetByCompanyIdAsync(string? companyId)
        {
            var sqlProperty = new SqlPropety
            {
                DbConnectionName = "DefaultConnection",
                SpName = "GetMonthlyRevenueByCompanyId"
            };
            var result = await Task.Run(() =>
                _sqlExtension.Execute<MonthlyRevenue>(
                    sqlProperty,
                    "99999999",
                    () => new List<SqlParamter>
                    {
                        new SqlParamter( "@CompanyId", companyId ),
                        new SqlParamter( "@OutString", SqlParamDirectionEnum.Output)
                    }
                )
            );
            return result.Result;
        }

        public async Task<(int success, int fail, List<string> errors)> BulkInsertAsync(List<MonthlyRevenue> entities)
        {
            int success = 0, fail = 0;
            var errors = new List<string>();
            if (entities == null || entities.Count == 0) return (0, 0, errors);

            // 轉 DataTable
            var table = new System.Data.DataTable();
            table.Columns.Add("ReportDate", typeof(string));
            table.Columns.Add("DataYearMonth", typeof(string));
            table.Columns.Add("CompanyId", typeof(string));
            table.Columns.Add("Revenue", typeof(decimal));
            table.Columns.Add("LastMonthRevenue", typeof(decimal));
            table.Columns.Add("LastYearMonthRevenue", typeof(decimal));
            table.Columns.Add("MoMChange", typeof(decimal));
            table.Columns.Add("YoYChange", typeof(decimal));
            table.Columns.Add("AccRevenue", typeof(decimal));
            table.Columns.Add("LastYearAccRevenue", typeof(decimal));
            table.Columns.Add("AccChange", typeof(decimal));
            table.Columns.Add("Memo", typeof(string));
            foreach (var e in entities)
            {
                table.Rows.Add(
                    e.ReportDate,
                    e.DataYearMonth,
                    e.CompanyId,
                    e.Revenue,
                    (object?)e.LastMonthRevenue ?? DBNull.Value,
                    (object?)e.LastYearMonthRevenue ?? DBNull.Value,
                    (object?)e.MoMChange ?? DBNull.Value,
                    (object?)e.YoYChange ?? DBNull.Value,
                    (object?)e.AccRevenue ?? DBNull.Value,
                    (object?)e.LastYearAccRevenue ?? DBNull.Value,
                    (object?)e.AccChange ?? DBNull.Value,
                    (object?)e.Memo ?? DBNull.Value
                );
            }

            // 使用新版 SqlExtension 執行 SP 並取得 outString
            var sqlProperty = new SqlPropety
            {
                DbConnectionName = "DefaultConnection",
                SpName = "ImportMonthlyRevenueBatch"
            };
            var result = await Task.Run(() =>
                _sqlExtension.Execute(
                    sqlProperty,
                    "99999999",
                    () => new List<SqlParamter>
                    {
                        new SqlParamter("@MonthlyRevenueList", table, true),
                        new SqlParamter("@OutString", string.Empty, SqlParamDirectionEnum.Output)
                    }
                )
            );
            if (result.OutString == "00000000")
            {
                success = entities.Count;
            }
            else
            {
                fail = entities.Count;
                errors.Add($"批次匯入失敗: {result.OutString}");
            }
            return (success, fail, errors);
        }
    }
}
