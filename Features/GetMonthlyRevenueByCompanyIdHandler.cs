
using MediatR;
using MonthlyRevenueApi.Models.Base;
using MonthlyRevenueApi.Models;
using MonthlyRevenueApi.Services;
using MonthlyRevenueApi.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MonthlyRevenueApi.Features
{
    public class GetMonthlyRevenueByCompanyIdHandler : IRequestHandler<GetMonthlyRevenueByCompanyIdQuery, ApiResponse<IEnumerable<MonthlyRevenueQueryDto>>>
    {
        private readonly IMonthlyRevenueService _service;
        public GetMonthlyRevenueByCompanyIdHandler(IMonthlyRevenueService service)
        {
            _service = service;
        }

        public async Task<ApiResponse<IEnumerable<MonthlyRevenueQueryDto>>> Handle(GetMonthlyRevenueByCompanyIdQuery request, CancellationToken cancellationToken)
        {
            return await _service.GetByCompanyIdAsync(request.CompanyId);
        }
    }
}
