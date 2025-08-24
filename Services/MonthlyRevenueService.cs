using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using MonthlyRevenueApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using MonthlyRevenueApi.Infrastructure.Database; // Add this if SqlExtension is in Extensions namespace
using MonthlyRevenueApi.Utils;

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

        public async Task<(int success, int fail, List<string> errors)> BulkInsertAllAsync(List<Industry> industries, List<Company> companies, List<MonthlyRevenue> revenues)
        {
            int success = 0, fail = 0;
            var errors = new List<string>();
            if ((industries == null || industries.Count == 0) && (companies == null || companies.Count == 0) && (revenues == null || revenues.Count == 0))
                return (0, 0, errors);

            var industryTable = industries?.ToDataTable() ?? new System.Data.DataTable();
            var companyTable = companies?.ToDataTable() ?? new System.Data.DataTable();
            var revenueTable = revenues?.ToDataTable() ?? new System.Data.DataTable();

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
                        new SqlParamter("@IndustryList", industryTable, true),
                        new SqlParamter("@CompanyList", companyTable, true),
                        new SqlParamter("@MonthlyRevenueList", revenueTable, true),
                        new SqlParamter("@OutString", string.Empty, SqlParamDirectionEnum.Output)
                    }
                )
            );
            if (result.OutString == "00000000")
            {
                success = (industries?.Count ?? 0) + (companies?.Count ?? 0) + (revenues?.Count ?? 0);
            }
            else
            {
                fail = (industries?.Count ?? 0) + (companies?.Count ?? 0) + (revenues?.Count ?? 0);
                errors.Add($"批次匯入失敗: {result.OutString}");
            }
            return (success, fail, errors);
        }
    }
}
