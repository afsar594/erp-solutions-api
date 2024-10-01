using Inspire.Erp.Application.Procurement.Interface;
using Inspire.Erp.Domain.Modals.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers.Procurement
{
    [Authorize]
    [Route("api/PrecurementReports")]
    [Produces("application/json")]
    [ApiController]
    public class PrecurementReportsController : Controller
    {
        private IPurchaseReports _reports;

        public PrecurementReportsController(IPurchaseReports reports)
        {
            _reports = reports;
        }
       
        [HttpPost]
        [Route("GetPurchaseOrderReport")]
        public async Task<IActionResult> GetPurchaseOrderReport(GenericGridViewModel model)
        {
            return Ok(await _reports.getPurdhaseOrderReport(model));
        }
    }
}
