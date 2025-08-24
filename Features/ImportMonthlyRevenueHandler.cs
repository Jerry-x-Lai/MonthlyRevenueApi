using MediatR;
using MonthlyRevenueApi.Models.Base;
using MonthlyRevenueApi.Models;
using MonthlyRevenueApi.Services;
using AutoMapper;
using MonthlyRevenueApi.Dtos;

namespace MonthlyRevenueApi.Features
{
    public class ImportMonthlyRevenueHandler : IRequestHandler<ImportMonthlyRevenueCommand, ApiResponse<ImportMonthlyRevenueResult>>
    {
        private readonly IMonthlyRevenueService _service;
        private readonly IMapper _mapper;
        public ImportMonthlyRevenueHandler(IMonthlyRevenueService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public async Task<ApiResponse<ImportMonthlyRevenueResult>> Handle(ImportMonthlyRevenueCommand request, CancellationToken cancellationToken)
        {
            var result = new ImportMonthlyRevenueResult();
            if (request.Records == null || request.Records.Count == 0)
                return ApiResponse<ImportMonthlyRevenueResult>.Fail("無匯入資料");

            // 轉換 DTO 為 DB Model，並取得唯一 Industry/Company 清單
            var entities = _mapper.Map<List<MonthlyRevenue>>(request.Records);
            
            var companies = request.Records
                .Where(x => !string.IsNullOrWhiteSpace(x.CompanyId) && !string.IsNullOrWhiteSpace(x.CompanyName))
                .Select(x => _mapper.Map<Company>(x))
                .GroupBy(x => x.CompanyId)
                .Select(g => g.First())
                .ToList();

            var industries = request.Records
                .Where(x => !string.IsNullOrWhiteSpace(x.IndustryName))
                .Select(x => _mapper.Map<Industry>(x))
                .GroupBy(x => x.IndustryName)
                .Select(g => g.First())
                .ToList();

            // 呼叫 Service 批次寫入
            var serviceResult = await _service.BulkInsertAllAsync(industries, companies, entities);
            if (serviceResult.IsSuccess)
            {
                result.SuccessCount = entities.Count;
                return ApiResponse<ImportMonthlyRevenueResult>.Success(result, "批次匯入成功");
            }
            else
            {
                result.FailCount = entities.Count;
                result.Errors = new List<string> { serviceResult.Msg };
                return ApiResponse<ImportMonthlyRevenueResult>.Fail(serviceResult.Msg, result);
            }
        }
    }
}
