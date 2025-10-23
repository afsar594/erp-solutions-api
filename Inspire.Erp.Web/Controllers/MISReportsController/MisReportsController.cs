using Inspire.Erp.Application.MIS.Interfaces;
using Inspire.Erp.Domain.Modals.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers.MISReportsController
{
    [Route("api/[controller]")]
    [ApiController]
    public class MisReportsController : ControllerBase
    {
        private readonly IMisReportServices _misReportServices;

        public MisReportsController(IMisReportServices misReportServices)
        {
            _misReportServices = misReportServices;
        }


        [HttpPost("profit-and-loss")]
        public async Task<IActionResult> GetProfitAndLoassAsync([FromBody] MisReportParams misReportParams) 
        { 
            return Ok(await _misReportServices.GetProfitAndLossAsync(misReportParams));
        }

        [HttpPost("balance-sheet")]
        public async Task<IActionResult> GetBalanceSheetAsync([FromBody] MisReportParams misReportParams) 
        { 
            return Ok(await _misReportServices.GetBalanceSheetAsync(misReportParams));
        }

        [HttpPost("trial-balance")]
        public async Task<IActionResult> GetTrialBalanceAsync([FromBody] MisReportParams misReportParams) 
        { 
            return Ok(await _misReportServices.GetTrialBalanceAsync(misReportParams));
        }


    }
}
