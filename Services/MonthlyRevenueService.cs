using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using MonthlyRevenueApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using MonthlyRevenueApi.Infrastructure.Database;
using MonthlyRevenueApi.Models.Base;
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

        public async Task<ApiResponse<IEnumerable<MonthlyRevenue>>> GetByCompanyIdAsync(string? companyId)
        {
            var sqlProperty = new SqlPropety
            {
                DbConnectionName = "DefaultConnection",
                SpName = "GetMonthlyRevenueByCompanyId"
            };
            var result = await Task.Run(() =>
                _sqlExtension.Execute<MonthlyRevenue>(
                    sqlProperty,
                    ErrorCodes.DefaultError,
                    () => new List<SqlParamter>
                    {
                        new SqlParamter( "@CompanyId", companyId ),
                        new SqlParamter( "@OutString", SqlParamDirectionEnum.Output)
                    }
                )
            );
            if (result.OutString == ErrorCodes.Success)
                return ApiResponse<IEnumerable<MonthlyRevenue>>.Success(result.Result);
            else
                return ApiResponse<IEnumerable<MonthlyRevenue>>.Fail($"查詢失敗: {result.OutString}");
        }

        public async Task<ApiResponse<object>> BulkInsertAllAsync(List<Industry> industries, List<Company> companies, List<MonthlyRevenue> revenues)
        {
            if ((industries == null || industries.Count == 0) && (companies == null || companies.Count == 0) && (revenues == null || revenues.Count == 0))
                return ApiResponse<object>.Fail("無匯入資料");

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
                    ErrorCodes.DefaultError,
                    () => new List<SqlParamter>
                    {
                        new SqlParamter("@IndustryList", industryTable, true),
                        new SqlParamter("@CompanyList", companyTable, true),
                        new SqlParamter("@MonthlyRevenueList", revenueTable, true),
                        new SqlParamter("@OutString", string.Empty, SqlParamDirectionEnum.Output)
                    }
                )
            );
            if (result.OutString == ErrorCodes.Success)
            {
                var count = (industries?.Count ?? 0) + (companies?.Count ?? 0) + (revenues?.Count ?? 0);
                return ApiResponse<object>.Success(new { SuccessCount = count }, "批次匯入成功");
            }
            else
            {
                var count = (industries?.Count ?? 0) + (companies?.Count ?? 0) + (revenues?.Count ?? 0);
                return ApiResponse<object>.Fail($"批次匯入失敗: {result.OutString}", new { FailCount = count });
            }
        }
    }
}
