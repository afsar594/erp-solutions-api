using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspire.Erp.Application.Master.Interfaces;
using Inspire.Erp.Application.Procurement.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals.Common;
using Microsoft.AspNetCore.Mvc;

namespace Inspire.Erp.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseVoucherController : ControllerBase
    {

        private readonly IPurchaseVoucherService _service;

        public PurchaseVoucherController(IPurchaseVoucherService service)
        {
            _service = service;
        }
        [HttpGet("GetPurchaseVoucher")]
        public IActionResult GetPurchaseVoucher()
        {
            try
            {
                var item = _service.GetPurchaseVoucher();
                return Ok(item);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

        [HttpGet("GetPurchaseReturnReport")]
        public IActionResult GetPurchaseReturnReport()
        {
            try
            {
                var item = _service.GetPurchaseReturnReport();
                return Ok(item);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

        [HttpGet("GetPurchaseVoucherDetails")]
        public IActionResult GetPurchaseVoucherDetails()
        {
            try
            {
                var item = _service.GetPurchaseVoucherDetails();
                return Ok(item);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

        [HttpPost("InsertPurchaseVoucher")]
        public IActionResult InsertPurchaseVoucher([FromBody] PurchaseVoucher dataObj)
        {
            try
            {
                var item = _service.InsertPurchaseVoucher(dataObj);
                return Ok(item);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }
        [HttpPost("UpdatePurchaseVoucher")]
        public IActionResult UpdatePurchaseVoucher([FromBody] PurchaseVoucher dataObj)
        {
            try
            {
                var item = _service.UpdatePurchaseVoucher(dataObj);
                return Ok(item);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }
        [HttpPost("DeletePurchaseVoucher")]
        public IActionResult DeletePurchaseVoucher([FromBody] PurchaseVoucher dataObj)
        {
            try
            {
                var item = _service.DeletePurchaseVoucher(dataObj);
                return Ok(item);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

        [HttpPost("GetPurchaseVoucherReport")]
        public async Task<IActionResult> GetPurchaseVoucherReport(GenericGridViewModel model)
        {
            return Ok(await _service.GetPurchaseVoucherReport(model));
        }
    }
}