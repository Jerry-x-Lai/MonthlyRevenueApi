using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using MonthlyRevenueApi.Infrastructure.Database;
using MonthlyRevenueApi.Models.Base;
using MonthlyRevenueApi.Utils;
using MonthlyRevenueApi.Dtos;
using MonthlyRevenueApi.Models;
using AutoMapper;

namespace MonthlyRevenueApi.Services
{
    public class MonthlyRevenueService : IMonthlyRevenueService
    {
        private readonly SqlExtension _sqlExtension;
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly ILogger<MonthlyRevenueService> _logger;
        private readonly IMapper _mapper;
        public MonthlyRevenueService(SqlExtension sqlExtension, IDbConnectionFactory dbConnectionFactory, ILogger<MonthlyRevenueService> logger, IMapper mapper)
        {
            _sqlExtension = sqlExtension;
            _dbConnectionFactory = dbConnectionFactory;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<MonthlyRevenueQueryDto>>> GetByCompanyIdAsync(string? companyId)
        {
            var sqlProperty = new SqlPropety
            {
                DbConnectionName = "DefaultConnection",
                SpName = "GetMonthlyRevenueByCompanyId"
            };
            var result = await Task.Run(() =>
                _sqlExtension.Execute<MonthlyRevenueQueryDto>(
                    sqlProperty,
                    ErrorCodes.DefaultError,
                    () => new List<SqlParamter>
                    {
                        new SqlParamter("@CompanyId", companyId),
                        new SqlParamter("@OutString", string.Empty, SqlParamDirectionEnum.Output)
                    }
                )
            );
            if (result.OutString == ErrorCodes.Success)
                return ApiResponse<IEnumerable<MonthlyRevenueQueryDto>>.Success(result.Result);
            else
                return ApiResponse<IEnumerable<MonthlyRevenueQueryDto>>.Fail($"查詢失敗: {result.OutString}");
        }

        public async Task<ApiResponse> BulkInsertAllAsync(List<Industry> industries, List<Company> companies, List<MonthlyRevenue> revenues)
        {
            if ((industries == null || !industries.Any()) && (companies == null || !companies.Any()) && (revenues == null || !revenues.Any()))
                return ApiResponse.Fail("無匯入資料");

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
                        new SqlParamter("@OutString", string.Empty, SqlParamDirectionEnum.Output),
                        new SqlParamter("@ErrorMessage", string.Empty, SqlParamDirectionEnum.Output)
                    }
                )
            );
            if (result.OutString == ErrorCodes.Success)
            {
                return ApiResponse.Success("批次匯入成功");
            }
            else
            {
                return ApiResponse.Fail($"批次匯入失敗: {result.OutString}");
            }
        }
    }
}
