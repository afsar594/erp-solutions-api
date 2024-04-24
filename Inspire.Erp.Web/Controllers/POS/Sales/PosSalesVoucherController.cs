using AutoMapper;
using Inspire.Erp.Application.Account.Interfaces;
using Inspire.Erp.Application.Sales.Implementations;
using Inspire.Erp.Application.Sales.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Inspire.Erp.Web.Controllers.POS.Sales
{
    [Route("api/[controller]")]
    [ApiController]
    public class PosSalesVoucherController : ControllerBase
    {
        private IPosSalesVoucherService _posSalesVoucherService;
        private readonly IMapper _mapper;
        public PosSalesVoucherController(IPosSalesVoucherService posSalesVoucherService, IMapper mapper)
        {
            _mapper = mapper;
            _posSalesVoucherService = posSalesVoucherService;
        }


        [HttpPost]
        [Route("InsertPosSalesVoucher")]
        public ApiResponse<POS_Sales_Voucher> InsertPosSalesVoucher([FromBody] POS_Sales_Voucher voucherCompositeView)
        {
            ApiResponse<POS_Sales_Voucher> apiResponse = new ApiResponse<POS_Sales_Voucher>();
            var param1 = _mapper.Map<POS_Sales_Voucher>(voucherCompositeView);
            var param2 = _mapper.Map<List<POS_Sales_Voucher_Details>>(voucherCompositeView.posSalesVoucherDetails);
            var xs = _posSalesVoucherService.InsertPosSalesVoucher(param1, param2);
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
            var xs = _posSalesVoucherService.UpdatePosSalesVoucher(param1, param2);
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
                Result = _mapper.Map<IEnumerable<POS_Sales_Voucher>>(_posSalesVoucherService.GetPosSalesVoucher()),
                Message = ""
            };
            return apiResponse;

        }
    }
}
