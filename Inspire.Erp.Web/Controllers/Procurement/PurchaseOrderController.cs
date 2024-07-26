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
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Infrastructure.Database.Repositoy;

namespace Inspire.Erp.Web.Controllers.Procurement
{
    [Route("api/PurchaseOrder")]
    [Produces("application/json")]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        private IPurchaseOrderService _purchaseOrderService;
        private IRepository<ItemMaster> itemrepository; private IRepository<UnitMaster> unitrepository; private IRepository<PurOrderRegister> _purOrderRegisterRepository;
        private IRepository<PurchaseOrder> _purchaseOrderRepository; private IRepository<PurchaseOrderDetails> _purchaseOrderDetailsRepository;

        private readonly IMapper _mapper;
        public PurchaseOrderController(IPurchaseOrderService purchaseOrderService, IMapper mapper, IRepository<ItemMaster> _itemrepository, IRepository<UnitMaster> _unitrepository, IRepository<PurchaseOrder> purchaseOrderRepository, IRepository<PurOrderRegister> purOrderRegisterRepository, IRepository<PurchaseOrderDetails> purchaseOrderDetailsRepository)
        {
            _purchaseOrderService = purchaseOrderService;
            _mapper = mapper;
            itemrepository = _itemrepository; unitrepository = _unitrepository; _purchaseOrderRepository = purchaseOrderRepository; _purOrderRegisterRepository = purOrderRegisterRepository;
            _purchaseOrderDetailsRepository = purchaseOrderDetailsRepository;
        }

        //[HttpGet]
        //[Route("PurchaseOrder_GetReportPurchaseOrder")]
        //public ApiResponse<List<ReportPurchaseOrder>> PurchaseOrder_GetReportPurchaseOrder()
        //{
        //    ApiResponse<List<ReportPurchaseOrder>> apiResponse = new ApiResponse<List<ReportPurchaseOrder>>
        //    {
        //        Valid = true,
        //        Result = _mapper.Map<List<ReportPurchaseOrder>>(_purchaseOrderService.PurchaseOrder_GetReportPurchaseOrder()),
        //        Message = ""
        //    };
        //    return apiResponse;
        //}
        [HttpGet]
        [Route("GetAllUserFile")]

        public List<UserFile> GetAllUserFile()
        {
            return _mapper.Map<List<UserFile>>(_purchaseOrderService.GetAllUserFile());
        }

        [HttpPost]
        [Route("InsertPurchaseOrder")]
        public ApiResponse<PurchaseOrderViewModel> InsertPurchaseOrder([FromBody] PurchaseOrderViewModel voucherCompositeView)
        {

            ApiResponse<PurchaseOrderViewModel> apiResponse = new ApiResponse<PurchaseOrderViewModel>();
            //if (_purchaseOrderService.GetVouchersNumbers(voucherCompositeView.PurchaseOrderNo) == null)
            //{
            var param1 = _mapper.Map<PurchaseOrder>(voucherCompositeView);
            var param2 = _mapper.Map<List<PurchaseOrderDetails>>(voucherCompositeView.PurchaseOrderDetails);
            var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
            //var param4 = _mapper.Map<List<StockRegister>>(voucherCompositeView.StockRegister);
            var xs = _purchaseOrderService.InsertPurchaseOrder(param1, param3, param2
                //, param4
                );
            //PurchaseOrderViewModel purchaseOrderViewModel = new PurchaseOrderViewModel
            //{

            //    PurchaseOrderId = xs.purchaseOrder.PurchaseOrderId,
            //    PurchaseOrderNo = xs.purchaseOrder.PurchaseOrderNo,
            //    PurchaseOrderDate = xs.purchaseOrder.PurchaseOrderDate,
            //    PurchaseOrderType = xs.purchaseOrder.PurchaseOrderType,
            //    PurchaseOrderPartyId = xs.purchaseOrder.PurchaseOrderPartyId,
            //    PurchaseOrderPartyName = xs.purchaseOrder.PurchaseOrderPartyName,
            //    PurchaseOrderPartyAddress = xs.purchaseOrder.PurchaseOrderPartyAddress,
            //    PurchaseOrderPartyVatNo = xs.purchaseOrder.PurchaseOrderPartyVatNo,
            //    PurchaseOrderSupInvNo = xs.purchaseOrder.PurchaseOrderSupInvNo,
            //    PurchaseOrderRefNo = xs.purchaseOrder.PurchaseOrderRefNo,
            //    PurchaseOrderDescription = xs.purchaseOrder.PurchaseOrderDescription,
            //    PurchaseOrderGrno = xs.purchaseOrder.PurchaseOrderGrno,
            //    PurchaseOrderGrdate = xs.purchaseOrder.PurchaseOrderGrdate,
            //    PurchaseOrderLpono = xs.purchaseOrder.PurchaseOrderLpono,
            //    PurchaseOrderLpodate = xs.purchaseOrder.PurchaseOrderLpodate,
            //    PurchaseOrderQtnNo = xs.purchaseOrder.PurchaseOrderQtnNo,
            //    PurchaseOrderQtnDate = xs.purchaseOrder.PurchaseOrderQtnDate,
            //    PurchaseOrderExcludeVat = xs.purchaseOrder.PurchaseOrderExcludeVat,
            //    PurchaseOrderPono = xs.purchaseOrder.PurchaseOrderPono,
            //    PurchaseOrderBatchCode = xs.purchaseOrder.PurchaseOrderBatchCode,
            //    PurchaseOrderDayBookNo = xs.purchaseOrder.PurchaseOrderDayBookNo,
            //    PurchaseOrderLocationId = xs.purchaseOrder.PurchaseOrderLocationId,
            //    PurchaseOrderUserId = xs.purchaseOrder.PurchaseOrderUserId,
            //    PurchaseOrderCurrencyId = xs.purchaseOrder.PurchaseOrderCurrencyId,
            //    PurchaseOrderCompanyId = xs.purchaseOrder.PurchaseOrderCompanyId,
            //    PurchaseOrderJobId = xs.purchaseOrder.PurchaseOrderJobId,
            //    PurchaseOrderFsno = xs.purchaseOrder.PurchaseOrderFsno,
            //    PurchaseOrderFcRate = xs.purchaseOrder.PurchaseOrderFcRate,
            //    PurchaseOrderStatus = xs.purchaseOrder.PurchaseOrderStatus,
            //    PurchaseOrderTotalGrossAmount = xs.purchaseOrder.PurchaseOrderTotalGrossAmount,
            //    PurchaseOrderTotalItemDisAmount = xs.purchaseOrder.PurchaseOrderTotalItemDisAmount,
            //    PurchaseOrderTotalActualAmount = xs.purchaseOrder.PurchaseOrderTotalActualAmount,
            //    PurchaseOrderTotalDisPer = xs.purchaseOrder.PurchaseOrderTotalDisPer,
            //    PurchaseOrderTotalDisAmount = xs.purchaseOrder.PurchaseOrderTotalDisAmount,
            //    PurchaseOrderVatAmt = xs.purchaseOrder.PurchaseOrderVatAmt,
            //    PurchaseOrderVatPer = xs.purchaseOrder.PurchaseOrderVatPer,
            //    PurchaseOrderVatRoundSign = xs.purchaseOrder.PurchaseOrderVatRoundSign,
            //    PurchaseOrderVatRountAmt = xs.purchaseOrder.PurchaseOrderVatRountAmt,
            //    PurchaseOrderNetDisAmount = xs.purchaseOrder.PurchaseOrderNetDisAmount,
            //    PurchaseOrderNetAmount = xs.purchaseOrder.PurchaseOrderNetAmount,
            //    PurchaseOrderTransportCost = xs.purchaseOrder.PurchaseOrderTransportCost,
            //    PurchaseOrderHandlingcharges = xs.purchaseOrder.PurchaseOrderHandlingcharges,
            //    PurchaseOrderIssueId = xs.purchaseOrder.PurchaseOrderIssueId,
            //    PurchaseOrderJobDirectPur = xs.purchaseOrder.PurchaseOrderJobDirectPur,
            //    PurchaseOrderDelStatus = xs.purchaseOrder.PurchaseOrderDelStatus,



            //    PurchaseOrderIndentNo = xs.purchaseOrder.PurchaseOrderIndentNo,
            //    PurchaseOrderContPerson = xs.purchaseOrder.PurchaseOrderContPerson,
            //    PurchaseOrderPreparedBy = xs.purchaseOrder.PurchaseOrderPreparedBy,
            //    PurchaseOrderRecommendedBy = xs.purchaseOrder.PurchaseOrderRecommendedBy,
            //    PurchaseOrderApproveEnable = xs.purchaseOrder.PurchaseOrderApproveEnable,
            //    PurchaseOrderApproveDate = xs.purchaseOrder.PurchaseOrderApproveDate,
            //    PurchaseOrderApproveStatus = xs.purchaseOrder.PurchaseOrderApproveStatus,
            //    PurchaseOrderApproveUserId = xs.purchaseOrder.PurchaseOrderApproveUserId,



            //    PurchaseOrderHeader = xs.purchaseOrder.PurchaseOrderHeader,
            //    PurchaseOrderDelivery = xs.purchaseOrder.PurchaseOrderDelivery,
            //    PurchaseOrderNotes = xs.purchaseOrder.PurchaseOrderNotes,
            //    PurchaseOrderFooter = xs.purchaseOrder.PurchaseOrderFooter,
            //    PurchaseOrderPaymentTerms = xs.purchaseOrder.PurchaseOrderPaymentTerms,
            //    PurchaseOrderSubject = xs.purchaseOrder.PurchaseOrderSubject,

            //    PurchaseOrderContent = xs.purchaseOrder.PurchaseOrderContent,
            //    PurchaseOrderRemarks1 = xs.purchaseOrder.PurchaseOrderRemarks1,
            //    PurchaseOrderRemarks2 = xs.purchaseOrder.PurchaseOrderRemarks2,
            //    PurchaseOrderRemarks3 = xs.purchaseOrder.PurchaseOrderRemarks3,


            //    PurchaseOrderDelivFromDate = xs.purchaseOrder.PurchaseOrderDelivFromDate,
            //    PurchaseOrderDelivToDate = xs.purchaseOrder.PurchaseOrderDelivToDate,
            //    PurchaseOrderDelivStatus = xs.purchaseOrder.PurchaseOrderDelivStatus,
            //    PurchaseOrderDelivPlace = xs.purchaseOrder.PurchaseOrderDelivPlace,



            //};
            PurchaseOrderViewModel purchaseOrderViewModel = new PurchaseOrderViewModel();
            purchaseOrderViewModel = _mapper.Map<PurchaseOrderViewModel>(xs.purchaseOrder);

            //purchaseOrderViewModel.PurchaseOrderDetails = _mapper.Map<List<PurchaseOrderDetailsViewModel>>(xs.purchaseOrderDetails);
            purchaseOrderViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionViewModel>>(xs.accountsTransactions);
            apiResponse = new ApiResponse<PurchaseOrderViewModel>
            {
                Valid = true,
                Result = purchaseOrderViewModel, //_mapper.Map<PurchaseOrderViewModel>(purchaseOrderViewModel),
                Message = PurchaseOrderMessage.SaveVoucher
            };
            //}
            //else
            //{
            //    apiResponse = new ApiResponse<PurchaseOrderViewModel>
            //    {
            //        Valid = false,
            //        Error = true,
            //        Exception = null,
            //        Message = PurchaseOrderMessage.VoucherAlreadyExist
            //    };

            //}


            return apiResponse;
        }

        [HttpPost]
        [Route("UpdatePurchaseOrder")]
        public ApiResponse<PurchaseOrderViewModel> UpdatePurchaseOrder([FromBody] PurchaseOrderViewModel voucherCompositeView)
        {
            var param1 = _mapper.Map<PurchaseOrder>(voucherCompositeView);
            var param2 = _mapper.Map<List<PurchaseOrderDetails>>(voucherCompositeView.PurchaseOrderDetails);
            var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
            //var param4 = _mapper.Map<List<StockRegister>>(voucherCompositeView.StockRegister);
            var xs = _purchaseOrderService.UpdatePurchaseOrder(param1, param3, param2
                //, param4
                );

            //PurchaseOrderViewModel purchaseOrderViewModel = new PurchaseOrderViewModel
            //{
            //    PurchaseOrderId = xs.purchaseOrder.PurchaseOrderId,
            //    PurchaseOrderNo = xs.purchaseOrder.PurchaseOrderNo,
            //    PurchaseOrderDate = xs.purchaseOrder.PurchaseOrderDate,
            //    PurchaseOrderType = xs.purchaseOrder.PurchaseOrderType,
            //    PurchaseOrderPartyId = xs.purchaseOrder.PurchaseOrderPartyId,
            //    PurchaseOrderPartyName = xs.purchaseOrder.PurchaseOrderPartyName,
            //    PurchaseOrderPartyAddress = xs.purchaseOrder.PurchaseOrderPartyAddress,
            //    PurchaseOrderPartyVatNo = xs.purchaseOrder.PurchaseOrderPartyVatNo,
            //    PurchaseOrderSupInvNo = xs.purchaseOrder.PurchaseOrderSupInvNo,
            //    PurchaseOrderRefNo = xs.purchaseOrder.PurchaseOrderRefNo,
            //    PurchaseOrderDescription = xs.purchaseOrder.PurchaseOrderDescription,
            //    PurchaseOrderGrno = xs.purchaseOrder.PurchaseOrderGrno,
            //    PurchaseOrderGrdate = xs.purchaseOrder.PurchaseOrderGrdate,
            //    PurchaseOrderLpono = xs.purchaseOrder.PurchaseOrderLpono,
            //    PurchaseOrderLpodate = xs.purchaseOrder.PurchaseOrderLpodate,
            //    PurchaseOrderQtnNo = xs.purchaseOrder.PurchaseOrderQtnNo,
            //    PurchaseOrderQtnDate = xs.purchaseOrder.PurchaseOrderQtnDate,
            //    PurchaseOrderExcludeVat = xs.purchaseOrder.PurchaseOrderExcludeVat,
            //    PurchaseOrderPono = xs.purchaseOrder.PurchaseOrderPono,
            //    PurchaseOrderBatchCode = xs.purchaseOrder.PurchaseOrderBatchCode,
            //    PurchaseOrderDayBookNo = xs.purchaseOrder.PurchaseOrderDayBookNo,
            //    PurchaseOrderLocationId = xs.purchaseOrder.PurchaseOrderLocationId,
            //    PurchaseOrderUserId = xs.purchaseOrder.PurchaseOrderUserId,
            //    PurchaseOrderCurrencyId = xs.purchaseOrder.PurchaseOrderCurrencyId,
            //    PurchaseOrderCompanyId = xs.purchaseOrder.PurchaseOrderCompanyId,
            //    PurchaseOrderJobId = xs.purchaseOrder.PurchaseOrderJobId,
            //    PurchaseOrderFsno = xs.purchaseOrder.PurchaseOrderFsno,
            //    PurchaseOrderFcRate = xs.purchaseOrder.PurchaseOrderFcRate,
            //    PurchaseOrderStatus = xs.purchaseOrder.PurchaseOrderStatus,
            //    PurchaseOrderTotalGrossAmount = xs.purchaseOrder.PurchaseOrderTotalGrossAmount,
            //    PurchaseOrderTotalItemDisAmount = xs.purchaseOrder.PurchaseOrderTotalItemDisAmount,
            //    PurchaseOrderTotalActualAmount = xs.purchaseOrder.PurchaseOrderTotalActualAmount,
            //    PurchaseOrderTotalDisPer = xs.purchaseOrder.PurchaseOrderTotalDisPer,
            //    PurchaseOrderTotalDisAmount = xs.purchaseOrder.PurchaseOrderTotalDisAmount,
            //    PurchaseOrderVatAmt = xs.purchaseOrder.PurchaseOrderVatAmt,
            //    PurchaseOrderVatPer = xs.purchaseOrder.PurchaseOrderVatPer,
            //    PurchaseOrderVatRoundSign = xs.purchaseOrder.PurchaseOrderVatRoundSign,
            //    PurchaseOrderVatRountAmt = xs.purchaseOrder.PurchaseOrderVatRountAmt,
            //    PurchaseOrderNetDisAmount = xs.purchaseOrder.PurchaseOrderNetDisAmount,
            //    PurchaseOrderNetAmount = xs.purchaseOrder.PurchaseOrderNetAmount,
            //    PurchaseOrderTransportCost = xs.purchaseOrder.PurchaseOrderTransportCost,
            //    PurchaseOrderHandlingcharges = xs.purchaseOrder.PurchaseOrderHandlingcharges,
            //    PurchaseOrderIssueId = xs.purchaseOrder.PurchaseOrderIssueId,
            //    PurchaseOrderJobDirectPur = xs.purchaseOrder.PurchaseOrderJobDirectPur,
            //    PurchaseOrderDelStatus = xs.purchaseOrder.PurchaseOrderDelStatus,

            //    PurchaseOrderIndentNo = xs.purchaseOrder.PurchaseOrderIndentNo,
            //    PurchaseOrderContPerson = xs.purchaseOrder.PurchaseOrderContPerson,
            //    PurchaseOrderPreparedBy = xs.purchaseOrder.PurchaseOrderPreparedBy,
            //    PurchaseOrderRecommendedBy = xs.purchaseOrder.PurchaseOrderRecommendedBy,
            //    PurchaseOrderApproveEnable = xs.purchaseOrder.PurchaseOrderApproveEnable,
            //    PurchaseOrderApproveDate = xs.purchaseOrder.PurchaseOrderApproveDate,
            //    PurchaseOrderApproveStatus = xs.purchaseOrder.PurchaseOrderApproveStatus,
            //    PurchaseOrderApproveUserId = xs.purchaseOrder.PurchaseOrderApproveUserId,



            //    PurchaseOrderHeader = xs.purchaseOrder.PurchaseOrderHeader,
            //    PurchaseOrderDelivery = xs.purchaseOrder.PurchaseOrderDelivery,
            //    PurchaseOrderNotes = xs.purchaseOrder.PurchaseOrderNotes,
            //    PurchaseOrderFooter = xs.purchaseOrder.PurchaseOrderFooter,
            //    PurchaseOrderPaymentTerms = xs.purchaseOrder.PurchaseOrderPaymentTerms,
            //    PurchaseOrderSubject = xs.purchaseOrder.PurchaseOrderSubject,

            //    PurchaseOrderContent = xs.purchaseOrder.PurchaseOrderContent,
            //    PurchaseOrderRemarks1 = xs.purchaseOrder.PurchaseOrderRemarks1,
            //    PurchaseOrderRemarks2 = xs.purchaseOrder.PurchaseOrderRemarks2,
            //    PurchaseOrderRemarks3 = xs.purchaseOrder.PurchaseOrderRemarks3,


            //    PurchaseOrderDelivFromDate = xs.purchaseOrder.PurchaseOrderDelivFromDate,
            //    PurchaseOrderDelivToDate = xs.purchaseOrder.PurchaseOrderDelivToDate,
            //    PurchaseOrderDelivStatus = xs.purchaseOrder.PurchaseOrderDelivStatus,
            //    PurchaseOrderDelivPlace = xs.purchaseOrder.PurchaseOrderDelivPlace,

            //};
            PurchaseOrderViewModel purchaseOrderViewModel = new PurchaseOrderViewModel();

            purchaseOrderViewModel = _mapper.Map<PurchaseOrderViewModel>(xs.purchaseOrder);
            // purchaseOrderViewModel.PurchaseOrderDetails = _mapper.Map<List<PurchaseOrderDetailsViewModel>>(xs.purchaseOrderDetails);
            purchaseOrderViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionViewModel>>(xs.accountsTransactions);

            ApiResponse<PurchaseOrderViewModel> apiResponse = new ApiResponse<PurchaseOrderViewModel>
            {
                Valid = true,
                Result = purchaseOrderViewModel,//_mapper.Map<PurchaseOrderViewModel>(xs.purchaseOrder),
                Message = PurchaseOrderMessage.UpdateVoucher
            };

            return apiResponse;

        }

        [HttpPost]
        [Route("DeletePurchaseOrder")]
        public ApiResponse<int> DeletePurchaseOrder([FromBody] PurchaseOrderViewModel voucherCompositeView)
        {
            var param1 = _mapper.Map<PurchaseOrder>(voucherCompositeView);
            var param2 = _mapper.Map<List<PurchaseOrderDetails>>(voucherCompositeView.PurchaseOrderDetails);
            var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
            //var param4 = _mapper.Map<List<StockRegister>>(voucherCompositeView.StockRegister);
            var xs = _purchaseOrderService.DeletePurchaseOrder(param1, param3, param2
                //, param4
                );
            ApiResponse<int> apiResponse = new ApiResponse<int>
            {
                Valid = true,
                Result = 0,
                Message = PurchaseOrderMessage.DeleteVoucher
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
                Result = _mapper.Map<List<AccountsTransactions>>(_purchaseOrderService.GetAllTransaction()),
                Message = ""
            };
            return apiResponse;

        }

        [HttpGet]
        [Route("GetPurchaseOrder")]
        public ApiResponse<List<PurchaseOrder>> GetAllPurchaseOrder()
        {
            ApiResponse<List<PurchaseOrder>> apiResponse = new ApiResponse<List<PurchaseOrder>>
            {
                Valid = true,
                Result = _purchaseOrderService.GetPurchaseOrder().ToList(),
                Message = ""
            };
            return apiResponse;
        }

        [HttpGet]
        [Route("GetSavedPurchaseOrderDetails/{id}")]
        public ApiResponse<PurchaseOrderViewModel> GetSavedPurchaseOrderDetails(string id)
        {
            PurchaseOrderModel purchaseOrder = _purchaseOrderService.GetSavedPurchaseOrderDetails(id);
            if (purchaseOrder != null)
            {
                PurchaseOrderViewModel purchaseOrderViewModel = new PurchaseOrderViewModel
                {
                    PurchaseOrderId = purchaseOrder.purchaseOrder.PurchaseOrderId,
                    PurchaseOrderNo = purchaseOrder.purchaseOrder.PurchaseOrderNo,
                    PurchaseOrderDate = purchaseOrder.purchaseOrder.PurchaseOrderDate,
                    PurchaseOrderType = purchaseOrder.purchaseOrder.PurchaseOrderType,
                    PurchaseOrderPartyId = purchaseOrder.purchaseOrder.PurchaseOrderPartyId,
                    PurchaseOrderPartyName = purchaseOrder.purchaseOrder.PurchaseOrderPartyName,
                    PurchaseOrderPartyAddress = purchaseOrder.purchaseOrder.PurchaseOrderPartyAddress,
                    PurchaseOrderPartyVatNo = purchaseOrder.purchaseOrder.PurchaseOrderPartyVatNo,
                    PurchaseOrderSupInvNo = purchaseOrder.purchaseOrder.PurchaseOrderSupInvNo,
                    PurchaseOrderRefNo = purchaseOrder.purchaseOrder.PurchaseOrderRefNo,
                    PurchaseOrderDescription = purchaseOrder.purchaseOrder.PurchaseOrderDescription,
                    PurchaseOrderGrno = purchaseOrder.purchaseOrder.PurchaseOrderGrno,
                    PurchaseOrderGrdate = purchaseOrder.purchaseOrder.PurchaseOrderGrdate,
                    PurchaseOrderLpono = purchaseOrder.purchaseOrder.PurchaseOrderLpono,
                    PurchaseOrderLpodate = purchaseOrder.purchaseOrder.PurchaseOrderLpodate,
                    PurchaseOrderQtnNo = purchaseOrder.purchaseOrder.PurchaseOrderQtnNo,
                    PurchaseOrderQtnDate = purchaseOrder.purchaseOrder.PurchaseOrderQtnDate,
                    PurchaseOrderExcludeVat = purchaseOrder.purchaseOrder.PurchaseOrderExcludeVat,
                    PurchaseOrderPono = purchaseOrder.purchaseOrder.PurchaseOrderPono,
                    PurchaseOrderBatchCode = purchaseOrder.purchaseOrder.PurchaseOrderBatchCode,
                    PurchaseOrderDayBookNo = purchaseOrder.purchaseOrder.PurchaseOrderDayBookNo,
                    PurchaseOrderLocationId = purchaseOrder.purchaseOrder.PurchaseOrderLocationId,
                    PurchaseOrderUserId = purchaseOrder.purchaseOrder.PurchaseOrderUserId,
                    PurchaseOrderCurrencyId = purchaseOrder.purchaseOrder.PurchaseOrderCurrencyId,
                    PurchaseOrderCompanyId = purchaseOrder.purchaseOrder.PurchaseOrderCompanyId,
                    PurchaseOrderJobId = purchaseOrder.purchaseOrder.PurchaseOrderJobId,
                    PurchaseOrderFsno = purchaseOrder.purchaseOrder.PurchaseOrderFsno,
                    PurchaseOrderFcRate = purchaseOrder.purchaseOrder.PurchaseOrderFcRate,
                    PurchaseOrderStatus = purchaseOrder.purchaseOrder.PurchaseOrderStatus,
                    PurchaseOrderTotalGrossAmount = purchaseOrder.purchaseOrder.PurchaseOrderTotalGrossAmount,
                    PurchaseOrderTotalItemDisAmount = purchaseOrder.purchaseOrder.PurchaseOrderTotalItemDisAmount,
                    PurchaseOrderTotalActualAmount = purchaseOrder.purchaseOrder.PurchaseOrderTotalActualAmount,
                    PurchaseOrderTotalDisPer = purchaseOrder.purchaseOrder.PurchaseOrderTotalDisPer,
                    PurchaseOrderTotalDisAmount = purchaseOrder.purchaseOrder.PurchaseOrderTotalDisAmount,
                    PurchaseOrderVatAmt = purchaseOrder.purchaseOrder.PurchaseOrderVatAmt,
                    PurchaseOrderVatPer = purchaseOrder.purchaseOrder.PurchaseOrderVatPer,
                    PurchaseOrderVatRoundSign = purchaseOrder.purchaseOrder.PurchaseOrderVatRoundSign,
                    PurchaseOrderVatRountAmt = purchaseOrder.purchaseOrder.PurchaseOrderVatRountAmt,
                    PurchaseOrderNetDisAmount = purchaseOrder.purchaseOrder.PurchaseOrderNetDisAmount,
                    PurchaseOrderNetAmount = purchaseOrder.purchaseOrder.PurchaseOrderNetAmount,
                    PurchaseOrderTransportCost = purchaseOrder.purchaseOrder.PurchaseOrderTransportCost,
                    PurchaseOrderHandlingcharges = purchaseOrder.purchaseOrder.PurchaseOrderHandlingcharges,
                    PurchaseOrderIssueId = purchaseOrder.purchaseOrder.PurchaseOrderIssueId,
                    PurchaseOrderJobDirectPur = purchaseOrder.purchaseOrder.PurchaseOrderJobDirectPur,
                    PurchaseOrderDelStatus = purchaseOrder.purchaseOrder.PurchaseOrderDelStatus,
                    PurchaseOrderIndentNo = purchaseOrder.purchaseOrder.PurchaseOrderIndentNo,
                    PurchaseOrderContPerson = purchaseOrder.purchaseOrder.PurchaseOrderContPerson,
                    PurchaseOrderPreparedBy = purchaseOrder.purchaseOrder.PurchaseOrderPreparedBy,
                    PurchaseOrderRecommendedBy = purchaseOrder.purchaseOrder.PurchaseOrderRecommendedBy,
                    PurchaseOrderApproveEnable = purchaseOrder.purchaseOrder.PurchaseOrderApproveEnable,
                    PurchaseOrderApproveDate = purchaseOrder.purchaseOrder.PurchaseOrderApproveDate,
                    PurchaseOrderApproveStatus = purchaseOrder.purchaseOrder.PurchaseOrderApproveStatus,
                    PurchaseOrderApproveUserId = purchaseOrder.purchaseOrder.PurchaseOrderApproveUserId,
                    PurchaseOrderHeader = purchaseOrder.purchaseOrder.PurchaseOrderHeader,
                    PurchaseOrderDelivery = purchaseOrder.purchaseOrder.PurchaseOrderDelivery,
                    PurchaseOrderNotes = purchaseOrder.purchaseOrder.PurchaseOrderNotes,
                    PurchaseOrderFooter = purchaseOrder.purchaseOrder.PurchaseOrderFooter,
                    PurchaseOrderPaymentTerms = purchaseOrder.purchaseOrder.PurchaseOrderPaymentTerms,
                    PurchaseOrderSubject = purchaseOrder.purchaseOrder.PurchaseOrderSubject,
                    PurchaseOrderContent = purchaseOrder.purchaseOrder.PurchaseOrderContent,
                    PurchaseOrderRemarks1 = purchaseOrder.purchaseOrder.PurchaseOrderRemarks1,
                    PurchaseOrderRemarks2 = purchaseOrder.purchaseOrder.PurchaseOrderRemarks2,
                    PurchaseOrderRemarks3 = purchaseOrder.purchaseOrder.PurchaseOrderRemarks3,
                    PurchaseOrderDelivFromDate = purchaseOrder.purchaseOrder.PurchaseOrderDelivFromDate,
                    PurchaseOrderDelivToDate = purchaseOrder.purchaseOrder.PurchaseOrderDelivToDate,
                    PurchaseOrderDelivStatus = purchaseOrder.purchaseOrder.PurchaseOrderDelivStatus,
                    PurchaseOrderDelivPlace = purchaseOrder.purchaseOrder.PurchaseOrderDelivPlace,
                };
                purchaseOrderViewModel.PurchaseOrderDetails = _mapper.Map<List<PurchaseOrderDetailsViewModel>>(purchaseOrder.purchaseOrderDetails);
                purchaseOrderViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionViewModel>>(purchaseOrder.accountsTransactions);
                ApiResponse<PurchaseOrderViewModel> apiResponse = new ApiResponse<PurchaseOrderViewModel>
                {
                    Valid = true,
                    Result = purchaseOrderViewModel,
                    Message = ""
                };
                return apiResponse;
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
                Message = PurchaseOrderMessage.VoucherNumberExist



            };
            var x = _purchaseOrderService.GetVouchersNumbers(id);
            if (x == null)
            {
                apiResponse.Result = false;
                apiResponse.Message = "";
            }

            return apiResponse;
        }

        [HttpGet]
        [Route("GetPODetailsForGRN")]
        public IActionResult GetPODetailsForGRN()
        {
            try
            {
                var item = _purchaseOrderService.GetPODetailsForGRN();
                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

        [HttpGet]
        [Route("GetGRMPOs/{supplierId}")]
        public async Task<IActionResult> GetGRMPOs(int supplierId)
        {
            try
            {
                var item = await _purchaseOrderService.GetGRMPOs(supplierId);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }
        [HttpGet]
        [Route("GetAllPOForGRN/{id}")]
        public ResponseInfo GetAllPOForGRN(decimal id)
        {

            var objResponse = new ResponseInfo();

            var materialList = itemrepository.GetAsQueryable().Where(a => a.ItemMasterDelStatus != true).Select(c => new
            {
                c.ItemMasterItemName,
                c.ItemMasterItemId,
                c.ItemMasterLastPurchasePrice
            });
            var unitList = unitrepository.GetAsQueryable().Where(a => a.UnitMasterUnitDelStatus != true).Select(c => new
            {
                c.UnitMasterUnitId,
                UnitMasterUnitShortName = c.UnitMasterUnitShortName.Trim(),
                c.UnitMasterUnitFullName
            }).ToList();
            var tracking = _purOrderRegisterRepository.GetAsQueryable().Where(a => a.PurOrderRegisterDelStatus != true).ToList();
            var detailList = _purchaseOrderDetailsRepository.GetAsQueryable().ToList();
            var details = _purchaseOrderRepository.GetAsQueryable().Where(a => a.PurchaseOrderDelStatus != true && a.PurchaseOrderPartyId == id).Select(c => new
            {
                c.PurchaseOrderId,
                c.PurchaseOrderNo,
                c.PurchaseOrderDate
            }).ToList();
            var purchaseOrderNos = details.Select(d => d.PurchaseOrderNo).ToList();

            // Filtering detailList based on PurchaseOrderNos
            var data = (from d in detailList
                        join c in details on d.PurchaseOrderId equals c.PurchaseOrderId
                        where purchaseOrderNos.Contains(d.PurchaseOrderDetailsNo)
                        select new
                        {
                            PODID = d.PurchaseOrderDetailsId,
                            POID = d.PurchaseOrderId,
                            date = c.PurchaseOrderDate.Date,
                            PONO = d.PurchaseOrderDetailsNo,
                            itemId = d.PurchaseOrderDetailsMatId,
                            itemname = materialList
                        .Where(m => m.ItemMasterItemId == (long?)d.PurchaseOrderDetailsMatId)
                        .Select(c => c.ItemMasterItemName ?? "")
                        .FirstOrDefault() ?? "",
                            unitId = d.PurchaseOrderDetailsUnitId ?? 0,
                            unit = unitList
                        .Where(u => u.UnitMasterUnitId == (int?)d.PurchaseOrderDetailsUnitId)
                        .Select(c => c.UnitMasterUnitFullName.Trim() ?? "")
                        .FirstOrDefault() ?? "",
                            price = d.PurchaseOrderDetailsRate ?? 0,
                            poQty = d.PurchaseOrderDetailsQuantity ?? 0,
                            QtyIUssued = tracking.Where(a => a.PurOrderRegisterOrderNo == d.PurchaseOrderDetailsNo)
                    .Sum(c => c.PurOrderRegisterQtyIssued ?? 0),
                        })
                .ToList();

            objResponse.ResultSet = new
            {
                result = data
            };
            return objResponse;

        }
    }
}
