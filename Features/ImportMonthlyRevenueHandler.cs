using MediatR;
using MonthlyRevenueApi.Dtos;
using MonthlyRevenueApi.Models;
using MonthlyRevenueApi.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace MonthlyRevenueApi.Features
{
    public class ImportMonthlyRevenueHandler : IRequestHandler<ImportMonthlyRevenueCommand, ImportMonthlyRevenueResult>
    {
        private readonly IMonthlyRevenueService _service;
        public ImportMonthlyRevenueHandler(IMonthlyRevenueService service)
        {
            _service = service;
        }

        public async Task<ImportMonthlyRevenueResult> Handle(ImportMonthlyRevenueCommand request, CancellationToken cancellationToken)
        {
            var result = new ImportMonthlyRevenueResult();
            if (request.Records == null || request.Records.Count == 0)
                return result;

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
            var (success, fail, errors) = await _service.BulkInsertAsync(entities);
            result.SuccessCount = success;
            result.FailCount = fail;
            result.Errors = errors;
            return result;
        }
    }
}
