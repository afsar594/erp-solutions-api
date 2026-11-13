
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Inspire.Erp.Application.StoreWareHouse.Interface;
using Inspire.Erp.Application.Sales.Interfaces;
using Inspire.Erp.Domain.Modals.Common;
using Microsoft.AspNetCore.Authorization;
using Inspire.Erp.Domain.Modals;
using System.Collections.Generic;

namespace Inspire.Erp.Web.Controllers.Sales
{   
    [Route("api/DeliveryNoteReport")]
    [Produces("application/json")]
    [ApiController]
    public class DeliveryNoteReportController : ControllerBase
    {
        private IDeliveryNoteServices _deliveryNoteServices;
        public DeliveryNoteReportController(IDeliveryNoteServices deliveryNoteServices)
        {
            _deliveryNoteServices = deliveryNoteServices;
        }
        [HttpPost]
        [Route("getDeliveryNoteReportDetails")]
        public  async Task<IActionResult> DeliveryNoteReportDetails(DeliveryNoteReportFilter filter)
        {
            return Ok(await _deliveryNoteServices.DeliveryNoteReportDetails(filter));
        }
        [HttpPost]
        [Route("DeliveryNoteByFilter")]
        public async Task<IActionResult> DeliveryNoteByFilter(DeliveryNotefilter filter)
        {
            return Ok(await _deliveryNoteServices.DeliveryNoteByFilter(filter));
        }

        [HttpPost]
        [Route("DeliveryNoteByFilterbyId")]
        public async Task<IActionResult> DeliveryNoteByFilterbyId(List<DeliverybyId> detail)
        {
            return Ok(await _deliveryNoteServices.DeliveryNoteByFilterbyId(detail));
        }
    }
}
