using MediatR;
using MonthlyRevenueApi.Models;
using MonthlyRevenueApi.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MonthlyRevenueApi.Features
{
    public class GetMonthlyRevenueByCompanyIdHandler : IRequestHandler<GetMonthlyRevenueByCompanyIdQuery, IEnumerable<MonthlyRevenue>>
    {
        private readonly IMonthlyRevenueService _service;
        public GetMonthlyRevenueByCompanyIdHandler(IMonthlyRevenueService service)
        {
            _service = service;
        }

        public async Task<IEnumerable<MonthlyRevenue>> Handle(GetMonthlyRevenueByCompanyIdQuery request, CancellationToken cancellationToken)
        {
            return await _service.GetByCompanyIdAsync(request.CompanyId);
        }
    }
}
