//using Inspire.Erp.Application.Account.Interfaces;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using AutoMapper;
//using Inspire.Erp.Application.Sales.Interfaces;
//using Inspire.Erp.Application.Common;
//using Inspire.Erp.Domain.Entities;
//using Inspire.Erp.Web.ViewModels;
//using Inspire.Erp.Web.ViewModels.sales;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Inspire.Erp.Domain.Enums;
//using Inspire.Erp.Web.Common;
//using Inspire.Erp.Web.MODULE;

//using Microsoft.AspNetCore.Mvc.Rendering;
//using Inspire.Erp.WebReport.ReportEngine;
//using Inspire.Erp.Domain.Modals.Sales;
//using Inspire.Erp.Web.ViewModels.Accounts;

//namespace Inspire.Erp.Web.Controllers
//{
//    [Route("api/AssetPurchase")]
//    [Produces("application/json")]
//    [ApiController]
//    public class AssetPurchaseController : ControllerBase
//    {
//        private IAssetsOpeningService _salesOrderService;
//        private readonly IMapper _mapper;
//        public AssetPurchaseController(IAssetsOpeningService salesOrderService, IMapper mapper)
//        {
//            _salesOrderService = salesOrderService;
//            _mapper = mapper;
//        }

//        //[HttpPost]
//        //[Route("InsertAssetPurchase")]
//        //public ApiResponse<AssetPurchaseVoucherViewModel> InsertAssetPurchase([FromBody] AssetPurchaseVoucherViewModel voucherCompositeView)
//        //{
//        //    ApiResponse<AssetPurchaseVoucherViewModel> apiResponse = new ApiResponse<AssetPurchaseVoucherViewModel>();
    
//        //        var param1 = _mapper.Map<AssetPurchaseVoucher>(voucherCompositeView);
                
//        //        var xs = _salesOrderService.InsertAssetPurchaseVoucher(param1);
   
//        //        CustomerPurchaseOrderViewModel salesVoucherViewModel = new CustomerPurchaseOrderViewModel
//        //        {
//        //            CustomerPurchaseOrderId = xs.customerPurchaseOrder.CustomerPurchaseOrderId,
//        //            CustomerPurchaseOrderVoucherNo = xs.customerPurchaseOrder.CustomerPurchaseOrderVoucherNo,
//        //            CustomerPurchaseOrderDate = xs.customerPurchaseOrder.CustomerPurchaseOrderDate,
//        //            CustomerPurchaseOrderLocationId = xs.customerPurchaseOrder.CustomerPurchaseOrderLocationId,
//        //            CustomerPurchaseOrderCustomerId= xs.customerPurchaseOrder.CustomerPurchaseOrderCustomerId,
//        //            CustomerPurchaseOrderDepartmentId = xs.customerPurchaseOrder.CustomerPurchaseOrderDepartmentId,
//        //            CustomerPurchaseOrderCustomerName = xs.customerPurchaseOrder.CustomerPurchaseOrderCustomerName,
//        //            CustomerPurchaseOrderSalesManId= xs.customerPurchaseOrder.CustomerPurchaseOrderSalesManId,
//        //            CustomerPurchaseOrderLpoNo= xs.customerPurchaseOrder.CustomerPurchaseOrderLpoNo,
//        //            CustomerPurchaseOrderLpoDate= xs.customerPurchaseOrder.CustomerPurchaseOrderLpoDate,
//        //            CustomerPurchaseOrderCurrencyId= xs.customerPurchaseOrder.CustomerPurchaseOrderCurrencyId,
//        //            CustomerPurchaseOrderRemarks= xs.customerPurchaseOrder.CustomerPurchaseOrderRemarks,
//        //            CustomerPurchaseOrderDiscountPercentage= xs.customerPurchaseOrder.CustomerPurchaseOrderDiscountPercentage,
//        //            CustomerPurchaseOrderDiscountAmount= xs.customerPurchaseOrder.CustomerPurchaseOrderDiscountAmount,
//        //            CustomerPurchaseOrderTotal= xs.customerPurchaseOrder.CustomerPurchaseOrderTotal,
//        //            CustomerPurchaseOrderGrandTotal= xs.customerPurchaseOrder.CustomerPurchaseOrderGrandTotal,
//        //            CustomerPurchaseOrderFsno= xs.customerPurchaseOrder.CustomerPurchaseOrderFsno,
//        //            CustomerPurchaseOrderUserId= xs.customerPurchaseOrder.CustomerPurchaseOrderUserId,
//        //            CustomerPurchaseOrderSubject= xs.customerPurchaseOrder.CustomerPurchaseOrderSubject,
//        //            CustomerPurchaseOrderNote= xs.customerPurchaseOrder.CustomerPurchaseOrderNote,
//        //            CustomerPurchaseOrderWarranty = xs.customerPurchaseOrder.CustomerPurchaseOrderWarranty,
//        //            CustomerPurchaseOrderTraining= xs.customerPurchaseOrder.CustomerPurchaseOrderTraining,
//        //            CustomerPurchaseOrderTechnicalDetails= xs.customerPurchaseOrder.CustomerPurchaseOrderTechnicalDetails,
//        //            CustomerPurchaseOrderTerms= xs.customerPurchaseOrder.CustomerPurchaseOrderTerms,
//        //            CustomerPurchaseOrderCpoAboutId= xs.customerPurchaseOrder.CustomerPurchaseOrderCpoAboutId,
//        //            CustomerPurchaseOrderCpoDeliveryTimeDate= xs.customerPurchaseOrder.CustomerPurchaseOrderCpoDeliveryTimeDate,
//        //            CustomerPurchaseOrderPacking= xs.customerPurchaseOrder.CustomerPurchaseOrderPacking,
//        //            CustomerPurchaseOrderQuotationId= xs.customerPurchaseOrder.CustomerPurchaseOrderQuotationId,
//        //            CustomerPurchaseOrderQuantity= xs.customerPurchaseOrder.CustomerPurchaseOrderQuantity,
//        //            CustomerPurchaseOrderIslocalPurchase= xs.customerPurchaseOrder.CustomerPurchaseOrderIslocalPurchase,
//        //            CustomerPurchaseOrderCpoTermsV= xs.customerPurchaseOrder.CustomerPurchaseOrderCpoTermsV,
//        //            CustomerPurchaseOrderJobId= xs.customerPurchaseOrder.CustomerPurchaseOrderJobId,
//        //            CustomerPurchaseOrderPoEnquiryIdN= xs.customerPurchaseOrder.CustomerPurchaseOrderPoEnquiryIdN,
//        //            CustomerPurchaseOrderPoQuotationIdN= xs.customerPurchaseOrder.CustomerPurchaseOrderPoQuotationIdN,
//        //            CustomerPurchaseOrderDelStatus= xs.customerPurchaseOrder.CustomerPurchaseOrderDelStatus,
                   
//        //        };

//        //        salesVoucherViewModel.CustomerPurchaseOrderDetails = _mapper.Map<List<CustomerPurchaseOrderDetailsViewModel>>(xs.customerPurchaseOrderDetails);
//        //        salesVoucherViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionViewModel>>(xs.accountsTransactions);
//        //        apiResponse = new ApiResponse<AssetPurchaseVoucherViewModel>
//        //        {
//        //            Valid = true,
//        //            Result = _mapper.Map<AssetPurchaseVoucherViewModel>(salesVoucherViewModel),
//        //            Message = CustomerPurchaseOrderMessage.SavePurchaseOrder
//        //        };
            
//        //    return apiResponse;
//        //}
//        [HttpPost]
//        [Route("UpdateSalesOrder")]
//        public ApiResponse<CustomerPurchaseOrderViewModel> UpdateSalesOrder([FromBody] CustomerPurchaseOrderViewModel voucherCompositeView)
//        {
//            var param1 = _mapper.Map<CustomerPurchaseOrder>(voucherCompositeView);
//            var param2 = _mapper.Map<List<CustomerPurchaseOrderDetails>>(voucherCompositeView.CustomerPurchaseOrderDetails);
//            var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);

//            //==============
//            param3 = new List<AccountsTransactions>();
//            List<StockRegister> param4 = new List<StockRegister>();
//           // clsAccountAndStock.SalesOrder_Accounts_STOCK_Transactions("", "", param1, param2, ref param4, ref param3);

//            var xs = _salesOrderService.UpdateSalesOrder(param1, param3, param2
//           , param4
//           );
//            //========================



//            CustomerPurchaseOrderViewModel salesVoucherViewModel = new CustomerPurchaseOrderViewModel
//            {
//                CustomerPurchaseOrderId = xs.customerPurchaseOrder.CustomerPurchaseOrderId,
//                CustomerPurchaseOrderVoucherNo = xs.customerPurchaseOrder.CustomerPurchaseOrderVoucherNo,
//                CustomerPurchaseOrderDate = xs.customerPurchaseOrder.CustomerPurchaseOrderDate,
//                CustomerPurchaseOrderLocationId = xs.customerPurchaseOrder.CustomerPurchaseOrderLocationId,
//                CustomerPurchaseOrderCustomerId = xs.customerPurchaseOrder.CustomerPurchaseOrderCustomerId,
//                CustomerPurchaseOrderDepartmentId = xs.customerPurchaseOrder.CustomerPurchaseOrderDepartmentId,
//                CustomerPurchaseOrderCustomerName = xs.customerPurchaseOrder.CustomerPurchaseOrderCustomerName,
//                CustomerPurchaseOrderSalesManId = xs.customerPurchaseOrder.CustomerPurchaseOrderSalesManId,
//                CustomerPurchaseOrderLpoNo = xs.customerPurchaseOrder.CustomerPurchaseOrderLpoNo,
//                CustomerPurchaseOrderLpoDate = xs.customerPurchaseOrder.CustomerPurchaseOrderLpoDate,
//                CustomerPurchaseOrderCurrencyId = xs.customerPurchaseOrder.CustomerPurchaseOrderCurrencyId,
//                CustomerPurchaseOrderRemarks = xs.customerPurchaseOrder.CustomerPurchaseOrderRemarks,
//                CustomerPurchaseOrderDiscountPercentage = xs.customerPurchaseOrder.CustomerPurchaseOrderDiscountPercentage,
//                CustomerPurchaseOrderDiscountAmount = xs.customerPurchaseOrder.CustomerPurchaseOrderDiscountAmount,
//                CustomerPurchaseOrderTotal = xs.customerPurchaseOrder.CustomerPurchaseOrderTotal,
//                CustomerPurchaseOrderGrandTotal = xs.customerPurchaseOrder.CustomerPurchaseOrderGrandTotal,
//                CustomerPurchaseOrderFsno = xs.customerPurchaseOrder.CustomerPurchaseOrderFsno,
//                CustomerPurchaseOrderUserId = xs.customerPurchaseOrder.CustomerPurchaseOrderUserId,
//                CustomerPurchaseOrderSubject = xs.customerPurchaseOrder.CustomerPurchaseOrderSubject,
//                CustomerPurchaseOrderNote = xs.customerPurchaseOrder.CustomerPurchaseOrderNote,
//                CustomerPurchaseOrderWarranty = xs.customerPurchaseOrder.CustomerPurchaseOrderWarranty,
//                CustomerPurchaseOrderTraining = xs.customerPurchaseOrder.CustomerPurchaseOrderTraining,
//                CustomerPurchaseOrderTechnicalDetails = xs.customerPurchaseOrder.CustomerPurchaseOrderTechnicalDetails,
//                CustomerPurchaseOrderTerms = xs.customerPurchaseOrder.CustomerPurchaseOrderTerms,
//                CustomerPurchaseOrderCpoAboutId = xs.customerPurchaseOrder.CustomerPurchaseOrderCpoAboutId,
//                CustomerPurchaseOrderCpoDeliveryTimeDate = xs.customerPurchaseOrder.CustomerPurchaseOrderCpoDeliveryTimeDate,
//                CustomerPurchaseOrderPacking = xs.customerPurchaseOrder.CustomerPurchaseOrderPacking,
//                CustomerPurchaseOrderQuotationId = xs.customerPurchaseOrder.CustomerPurchaseOrderQuotationId,
//                CustomerPurchaseOrderQuantity = xs.customerPurchaseOrder.CustomerPurchaseOrderQuantity,
//                CustomerPurchaseOrderIslocalPurchase = xs.customerPurchaseOrder.CustomerPurchaseOrderIslocalPurchase,
//                CustomerPurchaseOrderCpoTermsV = xs.customerPurchaseOrder.CustomerPurchaseOrderCpoTermsV,
//                CustomerPurchaseOrderJobId = xs.customerPurchaseOrder.CustomerPurchaseOrderJobId,
//                CustomerPurchaseOrderPoEnquiryIdN = xs.customerPurchaseOrder.CustomerPurchaseOrderPoEnquiryIdN,
//                CustomerPurchaseOrderPoQuotationIdN = xs.customerPurchaseOrder.CustomerPurchaseOrderPoQuotationIdN,
//                CustomerPurchaseOrderDelStatus = xs.customerPurchaseOrder.CustomerPurchaseOrderDelStatus,

//            };

//            salesVoucherViewModel.CustomerPurchaseOrderDetails = _mapper.Map<List<CustomerPurchaseOrderDetailsViewModel>>(xs.customerPurchaseOrderDetails);
//            salesVoucherViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionViewModel>>(xs.accountsTransactions);

//            ApiResponse<CustomerPurchaseOrderViewModel> apiResponse = new ApiResponse<CustomerPurchaseOrderViewModel>
//            {
//                Valid = true,
//                Result = _mapper.Map<CustomerPurchaseOrderViewModel>(salesVoucherViewModel),
//                Message = CustomerPurchaseOrderMessage.UpdatePurchaseOrder
//            };

//            return apiResponse;

//        }

//        [HttpPost]
//        [Route("DeleteSalesOrder")]
//        public ApiResponse<int> DeletePurchaseOrder([FromBody] CustomerPurchaseOrderViewModel voucherCompositeView)
//        {
//            var param1 = _mapper.Map<CustomerPurchaseOrder>(voucherCompositeView);
//            var param2 = _mapper.Map<List<CustomerPurchaseOrderDetails>>(voucherCompositeView.CustomerPurchaseOrderDetails);
//            var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
//            //var param4 = _mapper.Map<List<StockRegister>>(voucherCompositeView.StockRegister);
//            //var xs = _salesOrderService.DeleteSalesOrder(  param1,    param3, param2
//            //    //, param4
//            //    );

//            //==============
//            param3 = new List<AccountsTransactions>();
//            List<StockRegister> param4 = new List<StockRegister>();
//            //clsAccountAndStock.SalesOrder_Accounts_STOCK_Transactions("", "", param1, param2, ref param4, ref param3);

//            var xs = _salesOrderService.DeleteSalesOrder(param1, param3, param2
//           , param4
//           );
//            //========================


//            ApiResponse<int> apiResponse = new ApiResponse<int>
//            {
//                Valid = true,
//                Result = 0,
//                Message = CustomerPurchaseOrderMessage.DeletePurchaseOrder
//            };

//            return apiResponse;

//        }
//        //[HttpGet]
//        //[Route("GetAllAccountTransaction")]
//        //public ApiResponse<List<AccountsTransactions>> GetAllAccountTransaction()
//        //{
//        //    ApiResponse<List<AccountsTransactions>> apiResponse = new ApiResponse<List<AccountsTransactions>>
//        //    {
//        //        Valid = true,
//        //        Result = _mapper.Map<List<AccountsTransactions>>(_salesOrderService.GetAllTransaction()),
//        //        Message = ""
//        //    };
//        //    return apiResponse;
//        //}

//        [HttpGet]
//        [Route("GetAllSalesOrder")]
//        public IActionResult GetAllSalesOrder()
//        {
//           ApiResponse<List<CustomerPurchaseOrder>> apiResponse = new ApiResponse<List<CustomerPurchaseOrder>>
//            {
//                Valid = true,
//                Result = _mapper.Map<List<CustomerPurchaseOrder>>(_salesOrderService.GetCustomerPurchaseOrders()),
//                Message = ""
//            };
//             return Ok( apiResponse);
//        }

//        [HttpGet]
//        [Route("GetSavedSalesOrderDetails/{id}")]
//        public ApiResponse<CustomerPurchaseOrderViewModel> GetSavedSalesOrderDetails(string id)
//        {
//            CustomerPurchaseOrderModel xs = _salesOrderService.GetSavedCustomerPurchaseOrderDetails(id);

//            if (xs != null)
//            {
//                CustomerPurchaseOrderViewModel salesVoucherViewModel = new CustomerPurchaseOrderViewModel
//                {
//                    CustomerPurchaseOrderId = xs.customerPurchaseOrder.CustomerPurchaseOrderId,
//                    CustomerPurchaseOrderVoucherNo = xs.customerPurchaseOrder.CustomerPurchaseOrderVoucherNo,
//                    CustomerPurchaseOrderDate = xs.customerPurchaseOrder.CustomerPurchaseOrderDate,
//                    CustomerPurchaseOrderLocationId = xs.customerPurchaseOrder.CustomerPurchaseOrderLocationId,
//                    CustomerPurchaseOrderCustomerId = xs.customerPurchaseOrder.CustomerPurchaseOrderCustomerId,
//                    CustomerPurchaseOrderDepartmentId = xs.customerPurchaseOrder.CustomerPurchaseOrderDepartmentId,
//                    CustomerPurchaseOrderCustomerName = xs.customerPurchaseOrder.CustomerPurchaseOrderCustomerName,
//                    CustomerPurchaseOrderSalesManId = xs.customerPurchaseOrder.CustomerPurchaseOrderSalesManId,
//                    CustomerPurchaseOrderLpoNo = xs.customerPurchaseOrder.CustomerPurchaseOrderLpoNo,
//                    CustomerPurchaseOrderLpoDate = xs.customerPurchaseOrder.CustomerPurchaseOrderLpoDate,
//                    CustomerPurchaseOrderCurrencyId = xs.customerPurchaseOrder.CustomerPurchaseOrderCurrencyId,
//                    CustomerPurchaseOrderRemarks = xs.customerPurchaseOrder.CustomerPurchaseOrderRemarks,
//                    CustomerPurchaseOrderDiscountPercentage = xs.customerPurchaseOrder.CustomerPurchaseOrderDiscountPercentage,
//                    CustomerPurchaseOrderDiscountAmount = xs.customerPurchaseOrder.CustomerPurchaseOrderDiscountAmount,
//                    CustomerPurchaseOrderTotal = xs.customerPurchaseOrder.CustomerPurchaseOrderTotal,
//                    CustomerPurchaseOrderGrandTotal = xs.customerPurchaseOrder.CustomerPurchaseOrderGrandTotal,
//                    CustomerPurchaseOrderFsno = xs.customerPurchaseOrder.CustomerPurchaseOrderFsno,
//                    CustomerPurchaseOrderUserId = xs.customerPurchaseOrder.CustomerPurchaseOrderUserId,
//                    CustomerPurchaseOrderSubject = xs.customerPurchaseOrder.CustomerPurchaseOrderSubject,
//                    CustomerPurchaseOrderNote = xs.customerPurchaseOrder.CustomerPurchaseOrderNote,
//                    CustomerPurchaseOrderWarranty = xs.customerPurchaseOrder.CustomerPurchaseOrderWarranty,
//                    CustomerPurchaseOrderTraining = xs.customerPurchaseOrder.CustomerPurchaseOrderTraining,
//                    CustomerPurchaseOrderTechnicalDetails = xs.customerPurchaseOrder.CustomerPurchaseOrderTechnicalDetails,
//                    CustomerPurchaseOrderTerms = xs.customerPurchaseOrder.CustomerPurchaseOrderTerms,
//                    CustomerPurchaseOrderCpoAboutId = xs.customerPurchaseOrder.CustomerPurchaseOrderCpoAboutId,
//                    CustomerPurchaseOrderCpoDeliveryTimeDate = xs.customerPurchaseOrder.CustomerPurchaseOrderCpoDeliveryTimeDate,
//                    CustomerPurchaseOrderPacking = xs.customerPurchaseOrder.CustomerPurchaseOrderPacking,
//                    CustomerPurchaseOrderQuotationId = xs.customerPurchaseOrder.CustomerPurchaseOrderQuotationId,
//                    CustomerPurchaseOrderQuantity = xs.customerPurchaseOrder.CustomerPurchaseOrderQuantity,
//                    CustomerPurchaseOrderIslocalPurchase = xs.customerPurchaseOrder.CustomerPurchaseOrderIslocalPurchase,
//                    CustomerPurchaseOrderCpoTermsV = xs.customerPurchaseOrder.CustomerPurchaseOrderCpoTermsV,
//                    CustomerPurchaseOrderJobId = xs.customerPurchaseOrder.CustomerPurchaseOrderJobId,
//                    CustomerPurchaseOrderPoEnquiryIdN = xs.customerPurchaseOrder.CustomerPurchaseOrderPoEnquiryIdN,
//                    CustomerPurchaseOrderPoQuotationIdN = xs.customerPurchaseOrder.CustomerPurchaseOrderPoQuotationIdN,
//                    CustomerPurchaseOrderDelStatus = xs.customerPurchaseOrder.CustomerPurchaseOrderDelStatus,

//                };
//                salesVoucherViewModel.CustomerPurchaseOrderDetails = _mapper.Map<List<CustomerPurchaseOrderDetailsViewModel>>(xs.customerPurchaseOrderDetails);
//                salesVoucherViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionViewModel>>(xs.accountsTransactions);
//                ApiResponse<CustomerPurchaseOrderViewModel> apiResponse = new ApiResponse<CustomerPurchaseOrderViewModel>
//                {
//                    Valid = true,
//                    Result = salesVoucherViewModel,
//                    Message = ""
//                };
//                return apiResponse;
//            }
//            return null;



//        }
//        //[HttpGet]
//        //[Route("CheckVnoExist/{id}")]
//        //public ApiResponse<bool> CheckVnoExist(string id)
//        //{
//        //    ApiResponse<bool> apiResponse = new ApiResponse<bool>
//        //    {
//        //        Valid = true,
//        //        Result = true,
//        //        Message = SalesOrderMessage.VoucherNumberExist



//        //    };
//        //    var x = _salesOrderService.GetVouchersNumbers(id);
//        //    if (x == null)
//        //    {
//        //        apiResponse.Result = false;
//        //        apiResponse.Message = "";
//        //    }

//        //    return apiResponse;
//        //}


//        [HttpPost]
//        [Route("GetPurchaseOrderTracking")]
//        public async Task<IActionResult> GetPurchaseOrderTracking(CustPurchOrderFilterModel model)
//        {

//            var response = await _salesOrderService.GetCustomerPurchaseOrderTracking(model);

//            //return null;

//            ApiResponse<List<GetCustomerPurchaseOrderTrackingResponse>> apiResponse = new ApiResponse<List<GetCustomerPurchaseOrderTrackingResponse>>
//            {
//                Valid = true,
//                Result = response.Result,
//                Message = ""
//            };
//            return Ok(apiResponse);
//        }



//        [HttpPost]
//        [Route("GetPurchaseOrderStatuses")]
//        public async Task<IActionResult> GetPurchaseOrderStatuses(CustPurchOrderFilterModel model)
//        {

//            var response = await _salesOrderService.GetCustomerPOStatuses(model);

//            //return null;

//            ApiResponse<List<GetCustomerPurchaseOrderTrackingResponse>> apiResponse = new ApiResponse<List<GetCustomerPurchaseOrderTrackingResponse>>
//            {
//                Valid = true,
//                Result = response.Result,
//                Message = ""
//            };
//            return Ok(apiResponse);
//        }
//    }
//}








