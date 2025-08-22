using Microsoft.AspNetCore.Mvc;
using MediatR;
using MonthlyRevenueApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            var result = await _mediator.Send(new MonthlyRevenueApi.Features.GetMonthlyRevenueByCompanyIdQuery(companyId));
            return Ok(result);
        }

        /// <summary>
        /// 匯入上市公司每月營收 CSV
        /// </summary>
        /// <param name="file">CSV 檔案</param>
        [HttpPost("import")]
        public async Task<IActionResult> Import([FromForm] Microsoft.AspNetCore.Http.IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("請上傳 CSV 檔案。");

            using var stream = file.OpenReadStream();
            var records = MonthlyRevenueApi.Utils.CsvUtils.ParseMonthlyRevenue(stream).ToList();
            if (records.Count == 0)
                return BadRequest("CSV 無有效資料。");

            // 呼叫 Service 進行批次寫入
            var result = await _mediator.Send(new MonthlyRevenueApi.Features.ImportMonthlyRevenueCommand(records));
            return Ok(result);
        }
    }
}
