using Inspire.Erp.Application.Approvals.Interface;
using Inspire.Erp.Application.Settings.Interface;
using Inspire.Erp.Domain.Modals.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApprovalsController : ControllerBase
    {
        private readonly IApprovalService _approvalService;
        public ApprovalsController(IApprovalService approvalService)
        {
            _approvalService = approvalService;
        }
        [HttpPost("GetApprovalsRecords")]
        public async Task<IActionResult> GetApprovalsRecords(GenericGridViewModel model)
        {
            return Ok(await _approvalService.GetApprovalsRecords(model));
        }
    }
}
