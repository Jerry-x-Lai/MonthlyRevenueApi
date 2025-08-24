using MonthlyRevenueApi.Models;
using MonthlyRevenueApi.Models.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MonthlyRevenueApi.Services
{
    public interface IMonthlyRevenueService
    {
        Task<ApiResponse<IEnumerable<MonthlyRevenue>>> GetByCompanyIdAsync(string? companyId);
        Task<ApiResponse<object>> BulkInsertAllAsync(List<MonthlyRevenueApi.Models.Industry> industries, List<MonthlyRevenueApi.Models.Company> companies, List<MonthlyRevenueApi.Models.MonthlyRevenue> revenues);
    }
}
