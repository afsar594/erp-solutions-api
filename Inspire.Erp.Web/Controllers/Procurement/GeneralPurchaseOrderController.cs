using Inspire.Erp.Application.Account.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Inspire.Erp.Application.Procurement.Interfaces;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Web.ViewModels;
using Inspire.Erp.Web.ViewModels.Procurement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Web.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using Inspire.Erp.Application.Procurement.Implementation;

namespace Inspire.Erp.Web.Controllers.Procurement
{
    [Route("api/GeneralPurchaseOrder")]
    [Produces("application/json")]
    [ApiController]
    public class GeneralPurchaseOrderController : ControllerBase
    {
        private IGeneralPurchaseOrderService _GeneralpurchaseOrderService;
        private readonly IMapper _mapper;
        public GeneralPurchaseOrderController(IGeneralPurchaseOrderService GeneralpurchaseOrderService, IMapper mapper)
        {
            _GeneralpurchaseOrderService = GeneralpurchaseOrderService;
            _mapper = mapper;
        }
        [HttpGet]
        [Route("GetAllUserFile")]
        public List<UserFile> GetAllUserFile()
        {
            return _mapper.Map<List<UserFile>>(_GeneralpurchaseOrderService.GetAllUserFile());
        }
        [HttpPost]
        [Route("InsertGeneralPurchaseOrder")]
        public ApiResponse<GeneralPurchaseOrderViewModel> InsertGeneralPurchaseOrder([FromBody] GeneralPurchaseOrderViewModel voucherCompositeView)
        {

            ApiResponse<GeneralPurchaseOrderViewModel> apiResponse = new ApiResponse<GeneralPurchaseOrderViewModel>();
            if (_GeneralpurchaseOrderService.GetVouchersNumbers(voucherCompositeView.GeneralPurchaseOrderPono) == null)
            {
                var param1 = _mapper.Map<GeneralPurchaseOrder>(voucherCompositeView);
                var param2 = _mapper.Map<List<GeneralPurchaseOrderDetails>>(voucherCompositeView.GeneralPurchaseOrderDetails);
                var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
                var xs = _GeneralpurchaseOrderService.InsertGeneralPurchaseOrder(param1, param3, param2

                    );

                GeneralPurchaseOrderViewModel GeneralpurchaseOrderViewModel = new GeneralPurchaseOrderViewModel();
                GeneralpurchaseOrderViewModel = _mapper.Map<GeneralPurchaseOrderViewModel>(xs.GeneralpurchaseOrder);

                GeneralpurchaseOrderViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionViewModel>>(xs.accountsTransactions);
                apiResponse = new ApiResponse<GeneralPurchaseOrderViewModel>
                {
                    Valid = true,
                    Result = GeneralpurchaseOrderViewModel,
                    Message = PurchaseOrderMessage.SaveVoucher
                };
            }
            return apiResponse;
        }
        [HttpPost]
        [Route("UpdateGeneralPurchaseOrder")]
        public ApiResponse<GeneralPurchaseOrderViewModel> UpdateGeneralPurchaseOrder([FromBody] GeneralPurchaseOrderViewModel voucherCompositeView)
        {
            var param1 = _mapper.Map<GeneralPurchaseOrder>(voucherCompositeView);
            var param2 = _mapper.Map<List<GeneralPurchaseOrderDetails>>(voucherCompositeView.GeneralPurchaseOrderDetails);
            var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
            var xs = _GeneralpurchaseOrderService.UpdateGeneralPurchaseOrder(param1, param3, param2

                );
            GeneralPurchaseOrderViewModel GeneralpurchaseOrderViewModel = new GeneralPurchaseOrderViewModel();
            GeneralpurchaseOrderViewModel = _mapper.Map<GeneralPurchaseOrderViewModel>(xs.GeneralpurchaseOrder);
            GeneralpurchaseOrderViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionViewModel>>(xs.accountsTransactions);

            ApiResponse<GeneralPurchaseOrderViewModel> apiResponse = new ApiResponse<GeneralPurchaseOrderViewModel>
            {
                Valid = true,
                Result = GeneralpurchaseOrderViewModel,
                Message = GeneralPurchaseOrderMessage.UpdateVoucher
            };
            return apiResponse;
        }
        [HttpPost]
        [Route("DeleteGeneralPurchaseOrder")]
        public ApiResponse<int> DeleteGeneralPurchaseOrder([FromBody] GeneralPurchaseOrderViewModel voucherCompositeView)
        {
            var param1 = _mapper.Map<GeneralPurchaseOrder>(voucherCompositeView);
            var param2 = _mapper.Map<List<GeneralPurchaseOrderDetails>>(voucherCompositeView.GeneralPurchaseOrderDetails);
            var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
            var xs = _GeneralpurchaseOrderService.DeleteGeneralPurchaseOrder(param1, param3, param2
                );
            ApiResponse<int> apiResponse = new ApiResponse<int>
            {
                Valid = true,
                Result = 0,
                Message = GeneralPurchaseOrderMessage.DeleteVoucher
            };
            return apiResponse;
        }
        [HttpGet]
        [Route("GetAllAccountTransaction")]
        public ApiResponse<List<AccountsTransactions>> GetAllAccountTransaction()
        {
            ApiResponse<List<AccountsTransactions>> apiResponse = new ApiResponse<List<AccountsTransactions>>
            {
                Valid = true,
                Result = _mapper.Map<List<AccountsTransactions>>(_GeneralpurchaseOrderService.GetAllTransaction()),
                Message = ""
            };
            return apiResponse;
        }
        [HttpGet]
        [Route("GetGeneralPurchaseOrder")]
        public ApiResponse<List<GeneralPurchaseOrder>> GetAllGeneralPurchaseOrder()
        {
            ApiResponse<List<GeneralPurchaseOrder>> apiResponse = new ApiResponse<List<GeneralPurchaseOrder>>
            {
                Valid = true,
                Result = _GeneralpurchaseOrderService.GetGeneralPurchaseOrder().ToList(),
                Message = ""
            };
            return apiResponse;
        }
        [HttpGet]
        [Route("GetSavedGeneralPurchaseOrderDetails/{id}")]
        public ApiResponse<GeneralPurchaseOrderViewModel> GetSavedGeneralPurchaseOrderDetails(string id)
        {
            GeneralPurchaseOrderModel GeneralpurchaseOrder = _GeneralpurchaseOrderService.GetSavedGeneralPurchaseOrderDetails(id);
            if (GeneralpurchaseOrder != null)
            {
                GeneralPurchaseOrderViewModel GeneralpurchaseOrderViewModel = new GeneralPurchaseOrderViewModel
                {
                    //PurchaseOrderId = GeneralpurchaseOrder.purchaseOrder.PurchaseOrderId,
                    //PurchaseOrderNo = purchaseOrder.purchaseOrder.PurchaseOrderNo,
                    //PurchaseOrderDate = purchaseOrder.purchaseOrder.PurchaseOrderDate,
                    //PurchaseOrderType = purchaseOrder.purchaseOrder.PurchaseOrderType,
                    //PurchaseOrderPartyId = purchaseOrder.purchaseOrder.PurchaseOrderPartyId,
                    //PurchaseOrderPartyName = purchaseOrder.purchaseOrder.PurchaseOrderPartyName,
                    //PurchaseOrderPartyAddress = purchaseOrder.purchaseOrder.PurchaseOrderPartyAddress,
                    //PurchaseOrderPartyVatNo = purchaseOrder.purchaseOrder.PurchaseOrderPartyVatNo,
                    //PurchaseOrderSupInvNo = purchaseOrder.purchaseOrder.PurchaseOrderSupInvNo,
                    //PurchaseOrderRefNo = purchaseOrder.purchaseOrder.PurchaseOrderRefNo,
                    //PurchaseOrderDescription = purchaseOrder.purchaseOrder.PurchaseOrderDescription,
                    //PurchaseOrderGrno = purchaseOrder.purchaseOrder.PurchaseOrderGrno,
                    //PurchaseOrderGrdate = purchaseOrder.purchaseOrder.PurchaseOrderGrdate,
                    //PurchaseOrderLpono = purchaseOrder.purchaseOrder.PurchaseOrderLpono,
                    //PurchaseOrderLpodate = purchaseOrder.purchaseOrder.PurchaseOrderLpodate,
                    //PurchaseOrderQtnNo = purchaseOrder.purchaseOrder.PurchaseOrderQtnNo,
                    //PurchaseOrderQtnDate = purchaseOrder.purchaseOrder.PurchaseOrderQtnDate,
                    //PurchaseOrderExcludeVat = purchaseOrder.purchaseOrder.PurchaseOrderExcludeVat,
                    //PurchaseOrderPono = purchaseOrder.purchaseOrder.PurchaseOrderPono,
                    //PurchaseOrderBatchCode = purchaseOrder.purchaseOrder.PurchaseOrderBatchCode,
                    //PurchaseOrderDayBookNo = purchaseOrder.purchaseOrder.PurchaseOrderDayBookNo,
                    //PurchaseOrderLocationId = purchaseOrder.purchaseOrder.PurchaseOrderLocationId,
                    //PurchaseOrderUserId = purchaseOrder.purchaseOrder.PurchaseOrderUserId,
                    //PurchaseOrderCurrencyId = purchaseOrder.purchaseOrder.PurchaseOrderCurrencyId,
                    //PurchaseOrderCompanyId = purchaseOrder.purchaseOrder.PurchaseOrderCompanyId,
                    //PurchaseOrderJobId = purchaseOrder.purchaseOrder.PurchaseOrderJobId,
                    //PurchaseOrderFsno = purchaseOrder.purchaseOrder.PurchaseOrderFsno,
                    //PurchaseOrderFcRate = purchaseOrder.purchaseOrder.PurchaseOrderFcRate,
                    //PurchaseOrderStatus = purchaseOrder.purchaseOrder.PurchaseOrderStatus,
                    //PurchaseOrderTotalGrossAmount = purchaseOrder.purchaseOrder.PurchaseOrderTotalGrossAmount,
                    //PurchaseOrderTotalItemDisAmount = purchaseOrder.purchaseOrder.PurchaseOrderTotalItemDisAmount,
                    //PurchaseOrderTotalActualAmount = purchaseOrder.purchaseOrder.PurchaseOrderTotalActualAmount,
                    //PurchaseOrderTotalDisPer = purchaseOrder.purchaseOrder.PurchaseOrderTotalDisPer,
                    //PurchaseOrderTotalDisAmount = purchaseOrder.purchaseOrder.PurchaseOrderTotalDisAmount,
                    //PurchaseOrderVatAmt = purchaseOrder.purchaseOrder.PurchaseOrderVatAmt,
                    //PurchaseOrderVatPer = purchaseOrder.purchaseOrder.PurchaseOrderVatPer,
                    //PurchaseOrderVatRoundSign = purchaseOrder.purchaseOrder.PurchaseOrderVatRoundSign,
                    //PurchaseOrderVatRountAmt = purchaseOrder.purchaseOrder.PurchaseOrderVatRountAmt,
                    //PurchaseOrderNetDisAmount = purchaseOrder.purchaseOrder.PurchaseOrderNetDisAmount,
                    //PurchaseOrderNetAmount = purchaseOrder.purchaseOrder.PurchaseOrderNetAmount,
                    //PurchaseOrderTransportCost = purchaseOrder.purchaseOrder.PurchaseOrderTransportCost,
                    //PurchaseOrderHandlingcharges = purchaseOrder.purchaseOrder.PurchaseOrderHandlingcharges,
                    //PurchaseOrderIssueId = purchaseOrder.purchaseOrder.PurchaseOrderIssueId,
                    //PurchaseOrderJobDirectPur = purchaseOrder.purchaseOrder.PurchaseOrderJobDirectPur,
                    //PurchaseOrderDelStatus = purchaseOrder.purchaseOrder.PurchaseOrderDelStatus,
                    //PurchaseOrderIndentNo = purchaseOrder.purchaseOrder.PurchaseOrderIndentNo,
                    //PurchaseOrderContPerson = purchaseOrder.purchaseOrder.PurchaseOrderContPerson,
                    //PurchaseOrderPreparedBy = purchaseOrder.purchaseOrder.PurchaseOrderPreparedBy,
                    //PurchaseOrderRecommendedBy = purchaseOrder.purchaseOrder.PurchaseOrderRecommendedBy,
                    //PurchaseOrderApproveEnable = purchaseOrder.purchaseOrder.PurchaseOrderApproveEnable,
                    //PurchaseOrderApproveDate = purchaseOrder.purchaseOrder.PurchaseOrderApproveDate,
                    //PurchaseOrderApproveStatus = purchaseOrder.purchaseOrder.PurchaseOrderApproveStatus,
                    //PurchaseOrderApproveUserId = purchaseOrder.purchaseOrder.PurchaseOrderApproveUserId,
                    //PurchaseOrderHeader = purchaseOrder.purchaseOrder.PurchaseOrderHeader,
                    //PurchaseOrderDelivery = purchaseOrder.purchaseOrder.PurchaseOrderDelivery,
                    //PurchaseOrderNotes = purchaseOrder.purchaseOrder.PurchaseOrderNotes,
                    //PurchaseOrderFooter = purchaseOrder.purchaseOrder.PurchaseOrderFooter,
                    //PurchaseOrderPaymentTerms = purchaseOrder.purchaseOrder.PurchaseOrderPaymentTerms,
                    //PurchaseOrderSubject = purchaseOrder.purchaseOrder.PurchaseOrderSubject,
                    //PurchaseOrderContent = purchaseOrder.purchaseOrder.PurchaseOrderContent,
                    //PurchaseOrderRemarks1 = purchaseOrder.purchaseOrder.PurchaseOrderRemarks1,
                    //PurchaseOrderRemarks2 = purchaseOrder.purchaseOrder.PurchaseOrderRemarks2,
                    //PurchaseOrderRemarks3 = purchaseOrder.purchaseOrder.PurchaseOrderRemarks3,
                    //PurchaseOrderDelivFromDate = purchaseOrder.purchaseOrder.PurchaseOrderDelivFromDate,
                    //PurchaseOrderDelivToDate = purchaseOrder.purchaseOrder.PurchaseOrderDelivToDate,
                    //PurchaseOrderDelivStatus = purchaseOrder.purchaseOrder.PurchaseOrderDelivStatus,
                    //PurchaseOrderDelivPlace = purchaseOrder.purchaseOrder.PurchaseOrderDelivPlace,
                };
                //GeneralpurchaseOrderViewModel.PurchaseOrderDetails = _mapper.Map<List<GeneralPurchaseOrderDetailsViewModel>>(GeneralpurchaseOrder.GeneralpurchaseOrderDetails);
                //GeneralpurchaseOrderViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionViewModel>>(GeneralpurchaseOrder.accountsTransactions);
                //ApiResponse<PurchaseOrderViewModel> apiResponse = new ApiResponse<GeneralPurchaseOrderViewModel>
                //{
                //    Valid = true,
                //    Result = purchaseOrderViewModel,
                //    Message = ""
                //};
                //return apiResponse;
            }
            return null;
        }
        [HttpGet]
        [Route("CheckVnoExist/{id}")]
        public ApiResponse<bool> CheckVnoExist(string id)
        {
            ApiResponse<bool> apiResponse = new ApiResponse<bool>
            {
                Valid = true,
                Result = true,
                Message = GeneralPurchaseOrderMessage.VoucherNumberExist
            };
            var x = _GeneralpurchaseOrderService.GetVouchersNumbers(id);
            if (x == null)
            {
                apiResponse.Result = false;
                apiResponse.Message = "";
            }
            return apiResponse;
        }
        [HttpGet]
        [Route("GetGPODetailsForGRN")]
        public IActionResult GetPODetailsForGRN()
        {
            try
            {
                var item = _GeneralpurchaseOrderService.GetGPODetailsForGRN();
                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

    }
}
