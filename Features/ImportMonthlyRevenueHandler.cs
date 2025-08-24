using MediatR;
using MonthlyRevenueApi.Models.Base;
using MonthlyRevenueApi.Models;
using MonthlyRevenueApi.Services;

namespace MonthlyRevenueApi.Features
{
    public class ImportMonthlyRevenueHandler : IRequestHandler<ImportMonthlyRevenueCommand, ApiResponse<ImportMonthlyRevenueResult>>
    {
        private readonly IMonthlyRevenueService _service;
        public ImportMonthlyRevenueHandler(IMonthlyRevenueService service)
        {
            _service = service;
        }

        public async Task<ApiResponse<ImportMonthlyRevenueResult>> Handle(ImportMonthlyRevenueCommand request, CancellationToken cancellationToken)
        {
            var result = new ImportMonthlyRevenueResult();
            if (request.Records == null || request.Records.Count == 0)
                return ApiResponse<ImportMonthlyRevenueResult>.Fail("無匯入資料");

            // 轉換 DTO 為 DB Model
            var entities = request.Records.Select(dto => new MonthlyRevenue
            {
                ReportDate = dto.ReportDate ?? string.Empty,
                DataYearMonth = dto.DataYearMonth ?? string.Empty,
                CompanyId = dto.CompanyId ?? string.Empty,
                Revenue = dto.Revenue,
                LastMonthRevenue = dto.LastMonthRevenue,
                LastYearMonthRevenue = dto.LastYearMonthRevenue,
                MoMChange = dto.MoMChange,
                YoYChange = dto.YoYChange,
                AccRevenue = dto.AccRevenue,
                LastYearAccRevenue = dto.LastYearAccRevenue,
                AccChange = dto.AccChange,
                Memo = dto.Memo
            }).ToList();

            // 呼叫 Service 批次寫入
            var serviceResult = await _service.BulkInsertAllAsync(new List<Industry>(), new List<Company>(), entities);
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
