using Microsoft.AspNetCore.Mvc;
using MediatR;
using MonthlyRevenueApi.Models.Base;
using MonthlyRevenueApi.Models;
using MonthlyRevenueApi.Utils;
using MonthlyRevenueApi.Features;

namespace MonthlyRevenueApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MonthlyRevenueController : ControllerBase
    {
        private readonly IMediator _mediator;
        public MonthlyRevenueController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// 查詢全部或依公司代號查詢
        /// </summary>
        /// <param name="companyId">公司代號，可選</param>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MonthlyRevenue>>> Get([FromQuery] string? companyId)
        {
            var result = await _mediator.Send(new GetMonthlyRevenueByCompanyIdQuery(companyId));
            return Ok(result);
        }

        /// <summary>
        /// 匯入上市公司每月營收 CSV
        /// </summary>
        /// <param name="file">CSV 檔案</param>
        [HttpPost("import")]
        public async Task<IActionResult> Import([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Ok(ApiResponse<object>.Fail("請上傳 CSV 檔案。"));

            using var stream = file.OpenReadStream();
            var records = CsvUtils.ParseMonthlyRevenue(stream).ToList();
            if (records.Count == 0)
                return Ok(ApiResponse<object>.Fail("CSV 無有效資料。"));

            // 呼叫 Service 進行批次寫入
            var result = await _mediator.Send(new ImportMonthlyRevenueCommand(records));
            return Ok(result);
        }
    }
}
