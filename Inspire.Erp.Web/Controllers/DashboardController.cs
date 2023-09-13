using AutoMapper;
using Inspire.Erp.Application.Dashboard.Interfaces;
using Inspire.Erp.Domain.Modals.Dashboard;
using Inspire.Erp.Web.Common;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class DashboardController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService, IMapper mapper)
        {
            _dashboardService = dashboardService;
            _mapper = mapper;
        }
        [HttpPost("GetDashboardPDCIssueRecieved")]
        public async Task<IActionResult> GetDashboardPDCIssueRecieved(DashboardFilterModel model)
        {
            return Ok(await _dashboardService.GetDashboardPDCIssueRecieved(model));
        }
        [HttpPost("GetSummaryChart")]
        public async Task<IActionResult> GetSummaryChart(DashboardFilterModel model)
        {
            return Ok(await _dashboardService.GetSummaryChart(model));
        }
        [HttpPost("GetCustomerSupplier")]
        public async Task<IActionResult> GetCustomerSupplier(DashboardFilterModel model)
        {
            return Ok(await _dashboardService.GetCustomerSupplier(model));
        }
        [HttpPost("GetDasboardIncomeExpense")]
        public async Task<IActionResult> GetDasboardIncomeExpense(DashboardFilterModel model)
        {
            return Ok(await _dashboardService.GetDasboardIncomeExpense(model));
        }
        [HttpPost("GetDasboardPurchaseGroupAccount")]
        public async Task<IActionResult> GetDasboardPurchaseGroupAccount(DashboardFilterModel model)
        {
            return Ok(await _dashboardService.GetDasboardPurchaseGroupAccount(model));
        }



    }
}
