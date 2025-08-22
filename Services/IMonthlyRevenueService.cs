using MonthlyRevenueApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MonthlyRevenueApi.Services
{
    public interface IMonthlyRevenueService
    {
    Task<IEnumerable<MonthlyRevenue>> GetByCompanyIdAsync(string? companyId);
    Task<(int success, int fail, List<string> errors)> BulkInsertAsync(List<MonthlyRevenueApi.Models.MonthlyRevenue> entities);
    }
}
