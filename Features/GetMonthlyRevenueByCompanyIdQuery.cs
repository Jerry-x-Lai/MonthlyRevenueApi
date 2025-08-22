using MediatR;
using MonthlyRevenueApi.Models;
using System.Collections.Generic;

namespace MonthlyRevenueApi.Features
{
    public class GetMonthlyRevenueByCompanyIdQuery : IRequest<IEnumerable<MonthlyRevenue>>
    {
        public string? CompanyId { get; set; }
        public GetMonthlyRevenueByCompanyIdQuery(string? companyId)
        {
            CompanyId = companyId;
        }
    }
}
