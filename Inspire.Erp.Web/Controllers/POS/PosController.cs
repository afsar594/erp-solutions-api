using AutoMapper;
using Inspire.Erp.Application.Master;
using Inspire.Erp.Application.Sales.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Entities.POS;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Web.Common;
using Inspire.Erp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers.POS
{
    [Route("api/pos/item")]
    [Produces("application/json")]
    //[Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    public class POSController : ControllerBase
    {
        private readonly IMapper _mapper;
        private IItemMasterService itemMasterService;
        private ISalesVoucherService _salesVoucherService;
        public POSController(IItemMasterService _itemMasterService, IMapper mapper, ISalesVoucherService salesVoucherService)
        {

            itemMasterService = _itemMasterService;
            _mapper = mapper;
            _salesVoucherService = salesVoucherService;
        }
        

        [HttpGet]
        [Route("SearchItemByBarcodeLink/{barCode}")]
        public async Task<IActionResult> SearchItemByBarcodeLink(ItemBarCodeSearchFilter model)
        {
            return Ok(await itemMasterService.SearchItemByBarcode(model));
        }

        [HttpGet]
        [Route("GetWorkPeriods")]
        public async Task<IActionResult> GetWorkPeriods(WorkPeriodFilter model)
        {
            return Ok(await itemMasterService.GetWorkPeriod(model));

        }


        [HttpGet]
        [Route("GetItems")]
        public async Task<IActionResult> GetItems(string? barCode)
        {
            return Ok(await itemMasterService.GetItems(barCode));
        }

        [HttpGet]
        [Route("GetItems/{unitDetailsId}")]
        public async Task<IActionResult> SearchItemByItemLink(ItemBarCodeSearchFilter model)
        {
            return Ok(await itemMasterService.SearchItemByUnitDetailsId(model));

        }

        [HttpPost("AddSettlementDetails")]
        public async Task<IActionResult> AddSettlementDetails(SettlementDetailsRequest model)
        {
            try
            {
                return Ok(await itemMasterService.AddSettlementDetails(model));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }

        }


        [HttpPost("CalcualteCardAmount")]
        public async Task<IActionResult> CalcualteCardAmount(SettlementDetailsRequest model)
        {
            try
            {
                return Ok(await itemMasterService.CalcualteCardAmount(model));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }

        }


        [HttpPost("ProcessCard")]
        public async Task<IActionResult> ProcessCard(CardAmountResponse model)
        {
            try
            {
                return Ok(await itemMasterService.ProcessCard(model));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }

        }


        [HttpPost]
        [Route("InsertPosSalesVoucher")]
        public ApiResponse<POS_Sales_Voucher> InsertPosSalesVoucher([FromBody] POS_Sales_Voucher voucherCompositeView)
        {
            ApiResponse<POS_Sales_Voucher> apiResponse = new ApiResponse<POS_Sales_Voucher>();
            var param1 = _mapper.Map<POS_Sales_Voucher>(voucherCompositeView);
            var param2 = _mapper.Map<List<POS_Sales_Voucher_Details>>(voucherCompositeView.posSalesVoucherDetails);
            var xs = _salesVoucherService.InsertPosSalesVoucher(param1, param2);
            apiResponse = new ApiResponse<POS_Sales_Voucher>
            {
                Valid = true,
                Result = _mapper.Map<POS_Sales_Voucher>(xs),
                Message = SalesVoucherMessage.SaveVoucher
            };
            return apiResponse;
        }
        [HttpPost]
        [Route("UpdatePosSalesVoucher")]
        public ApiResponse<POS_Sales_Voucher> UpdatePosSalesVoucher([FromBody] POS_Sales_Voucher voucherCompositeView)
        {
            ApiResponse<POS_Sales_Voucher> apiResponse = new ApiResponse<POS_Sales_Voucher>();
            var param1 = _mapper.Map<POS_Sales_Voucher>(voucherCompositeView);
            var param2 = _mapper.Map<List<POS_Sales_Voucher_Details>>(voucherCompositeView.posSalesVoucherDetails);
            var xs = _salesVoucherService.UpdatePosSalesVoucher(param1, param2);
            apiResponse = new ApiResponse<POS_Sales_Voucher>
            {
                Valid = true,
                Result = _mapper.Map<POS_Sales_Voucher>(xs),
                Message = SalesVoucherMessage.SaveVoucher
            };
            return apiResponse;
        }

        [HttpGet]
        [Route("GetPosSalesVoucher")]
        public ApiResponse<IEnumerable<POS_Sales_Voucher>> GetAllSalesVoucher()
        {
            ApiResponse<IEnumerable<POS_Sales_Voucher>> apiResponse = new ApiResponse<IEnumerable<POS_Sales_Voucher>>
            {
                Valid = true,
                Result = _mapper.Map<IEnumerable<POS_Sales_Voucher>>(_salesVoucherService.GetPosSalesVoucher()),
                Message = ""
            };
            return apiResponse;

        }




    }
}