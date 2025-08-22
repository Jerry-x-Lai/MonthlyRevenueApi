using MediatR;
using MonthlyRevenueApi.Dtos;
using System.Collections.Generic;

namespace MonthlyRevenueApi.Features
{
    public class ImportMonthlyRevenueCommand : IRequest<ImportMonthlyRevenueResult>
    {
        public List<MonthlyRevenueDto> Records { get; }
        public ImportMonthlyRevenueCommand(List<MonthlyRevenueDto> records)
        {
            Records = records;
        }
    }

    public class ImportMonthlyRevenueResult
    {
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
