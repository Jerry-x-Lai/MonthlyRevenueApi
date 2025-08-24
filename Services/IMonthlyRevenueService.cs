using MonthlyRevenueApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MonthlyRevenueApi.Services
{
    public interface IMonthlyRevenueService
    {
    Task<IEnumerable<MonthlyRevenue>> GetByCompanyIdAsync(string? companyId);
    Task<(int success, int fail, List<string> errors)> BulkInsertAllAsync(List<MonthlyRevenueApi.Models.Industry> industries, List<MonthlyRevenueApi.Models.Company> companies, List<MonthlyRevenueApi.Models.MonthlyRevenue> revenues);
    }
}
