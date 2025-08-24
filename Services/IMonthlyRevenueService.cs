using MonthlyRevenueApi.Models;
using MonthlyRevenueApi.Models.Base;
using MonthlyRevenueApi.Dtos;

namespace MonthlyRevenueApi.Services
{
    public interface IMonthlyRevenueService
    {
        Task<ApiResponse<IEnumerable<MonthlyRevenueQueryDto>>> GetByCompanyIdAsync(string? companyId);
        Task<ApiResponse> BulkInsertAllAsync(List<Industry> industries, List<Company> companies, List<MonthlyRevenue> revenues);
    }
}
