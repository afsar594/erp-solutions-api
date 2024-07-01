using Inspire.Erp.Application.Procurement.Interface;
using Inspire.Erp.Domain.Modals.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers.Procurement
{
    [Route("api/PurchassOrderTracking")]
    [Produces("application/json")]
    [ApiController]
    public class PurchassOrderTrackingController : Controller
    {

        private IPurchaseOrdertracking purchaseOrdertracking;

        public PurchassOrderTrackingController(IPurchaseOrdertracking purchaseOrdertracking)
        {
            this.purchaseOrdertracking = purchaseOrdertracking;
        }

        //Irfan 08 Dec 2023
        /// <summary>
        /// Name : getPurchaseOrderTrackingDetails
        /// Desc : used to get purchase order tracking details 
        /// </summary>
        /// <param name="reportFilter">
        /// need to set these attrubutes to load data accordingly
        /// PONO, fromDate, toDate, SupplierId, JobId, LocationId, ItemId
        /// </param>
        /// <returns> 
        /// </returns>
        [HttpPost]
        [Route("GetPurchaseOrderTrackingDetails")]
        public async Task<IActionResult> getPurchaseOrderTrackingDetails(ReportFilter reportFilter)
        {
            return Ok(await purchaseOrdertracking.GetPurchaseOrderTrackingDetails(reportFilter));
        }

        //Irfan 08 Dec 2023
        /// <summary>
        /// Name : GetPurchaseOrderTrackingSummary
        /// Desc : used to get purchase order tracking Summary 
        /// </summary>
        /// <param name="reportFilter">
        ///  need to set these attrubutes to load data accordingly
        /// PONO, fromDate, toDate, SupplierId, JobId, LocationId, ItemId</param>
        /// <returns> 
        /// </returns>
        [HttpPost]
        [Route("GetPurchaseOrderTrackingSummary")]
        public async Task<IActionResult> getPurchaseOrderTrackingSummary(ReportFilter reportFilter)
        {
            return Ok(await purchaseOrdertracking.GetPurchaseOrderTrackingSummary(reportFilter));
        }

        [HttpPost]
        [Route("GetUserTrackingDetails")]
        public async Task<IActionResult> GetUserTrackingDetails(UserTrackFilter reportFilter)
        {
            return Ok(await purchaseOrdertracking.GetUserTrackingDetails(reportFilter));
        }

    }
}
