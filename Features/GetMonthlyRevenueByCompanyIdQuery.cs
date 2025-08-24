using MediatR;
using MonthlyRevenueApi.Models;
using System.Collections.Generic;

namespace MonthlyRevenueApi.Features
{
    using MonthlyRevenueApi.Utils;

    public class GetMonthlyRevenueByCompanyIdQuery : MediatR.IRequest<MonthlyRevenueApi.Models.Base.ApiResponse<IEnumerable<MonthlyRevenueApi.Dtos.MonthlyRevenueQueryDto>>> 
    {
        public string? CompanyId { get; set; }
        public GetMonthlyRevenueByCompanyIdQuery(string? companyId)
        {
            CompanyId = companyId;
        }
    }
}
