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
using Inspire.Erp.Application.Procurement.Implimentation;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Application.Procurement.Implementation;
using Microsoft.AspNetCore.Authorization;

namespace Inspire.Erp.Web.Controllers
{

    [Route("api/PurchaseRequisition")]
    [Produces("application/json")]
    [ApiController]
    public class PurchaseRequisitionController : ControllerBase
    {
        private IPurchaseRequisitionService _purchaseRequisitionService;
        private readonly IMapper _mapper;
        public PurchaseRequisitionController(IPurchaseRequisitionService purchaseRequisitionService, IMapper mapper)
        {
            _purchaseRequisitionService = purchaseRequisitionService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("InsertPurchaseRequisition")]
        public async Task<ApiResponse<PurchaseRequisitionViewModel>> InsertPurchaseRequisition([FromBody] PurchaseRequisitionViewModel voucherCompositeView)
        {
            ApiResponse<PurchaseRequisitionViewModel> apiResponse = new ApiResponse<PurchaseRequisitionViewModel>();
            if (_purchaseRequisitionService.GetVouchersNumbers(voucherCompositeView.PurchaseRequisitionNo) == null)
            {

                var param1 = _mapper.Map<PurchaseRequisition>(voucherCompositeView);
                var param2 = _mapper.Map<List<PurchaseRequisitionDetails>>(voucherCompositeView.PurchaseRequisitionDetails);
                var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
                
                var xs = await _purchaseRequisitionService.InsertPurchaseRequisition(param1, param3, param2 );

                if (!xs.Valid)
                {
                    return  new ApiResponse<PurchaseRequisitionViewModel>
                    {
                        Valid = true,
                        Result = voucherCompositeView,
                        Message = xs.Message
                    };
                }

                PurchaseRequisitionViewModel purchaseRequisitionViewModel = new PurchaseRequisitionViewModel
                {

                    PurchaseRequisitionId = xs.Result.purchaseRequisition.PurchaseRequisitionId,
                    PurchaseRequisitionNo = xs.Result.purchaseRequisition.PurchaseRequisitionNo,
                    PurchaseRequisitionDate = xs.Result.purchaseRequisition.PurchaseRequisitionDate,
                    PurchaseRequisitionType = xs.Result.purchaseRequisition.PurchaseRequisitionType,
                    PurchaseRequisitionPartyId = xs.Result.purchaseRequisition.PurchaseRequisitionPartyId,
                    PurchaseRequisitionPartyName = xs.Result.purchaseRequisition.PurchaseRequisitionPartyName,
                    PurchaseRequisitionPartyAddress = xs.Result.purchaseRequisition.PurchaseRequisitionPartyAddress,
                    PurchaseRequisitionPartyVatNo = xs.Result.purchaseRequisition.PurchaseRequisitionPartyVatNo,
                    PurchaseRequisitionSupInvNo = xs.Result.purchaseRequisition.PurchaseRequisitionSupInvNo,
                    PurchaseRequisitionRefNo = xs.Result.purchaseRequisition.PurchaseRequisitionRefNo,
                    PurchaseRequisitionDescription = xs.Result.purchaseRequisition.PurchaseRequisitionDescription,
                    PurchaseRequisitionGrno = xs.Result.purchaseRequisition.PurchaseRequisitionGrno,
                    PurchaseRequisitionGrdate = xs.Result.purchaseRequisition.PurchaseRequisitionGrdate,
                    PurchaseRequisitionLpono = xs.Result.purchaseRequisition.PurchaseRequisitionLpono,
                    PurchaseRequisitionLpodate = xs.Result.purchaseRequisition.PurchaseRequisitionLpodate,
                    PurchaseRequisitionQtnNo = xs.Result.purchaseRequisition.PurchaseRequisitionQtnNo,
                    PurchaseRequisitionQtnDate = xs.Result.purchaseRequisition.PurchaseRequisitionQtnDate,
                    PurchaseRequisitionExcludeVat = xs.Result.purchaseRequisition.PurchaseRequisitionExcludeVat,
                    PurchaseRequisitionPono = xs.Result.purchaseRequisition.PurchaseRequisitionPono,
                    PurchaseRequisitionBatchCode = xs.Result.purchaseRequisition.PurchaseRequisitionBatchCode,
                    PurchaseRequisitionDayBookNo = xs.Result.purchaseRequisition.PurchaseRequisitionDayBookNo,
                    PurchaseRequisitionLocationId = xs.Result.purchaseRequisition.PurchaseRequisitionLocationId,
                    PurchaseRequisitionUserId = xs.Result.purchaseRequisition.PurchaseRequisitionUserId,
                    PurchaseRequisitionCurrencyId = xs.Result.purchaseRequisition.PurchaseRequisitionCurrencyId,
                    PurchaseRequisitionCompanyId = xs.Result.purchaseRequisition.PurchaseRequisitionCompanyId,
                    PurchaseRequisitionJobId = xs.Result.purchaseRequisition.PurchaseRequisitionJobId,
                    PurchaseRequisitionFsno = xs.Result.purchaseRequisition.PurchaseRequisitionFsno,
                    PurchaseRequisitionFcRate = xs.Result.purchaseRequisition.PurchaseRequisitionFcRate,
                    PurchaseRequisitionStatus = xs.Result.purchaseRequisition.PurchaseRequisitionStatus,
                    PurchaseRequisitionTotalGrossAmount = xs.Result.purchaseRequisition.PurchaseRequisitionTotalGrossAmount,
                    PurchaseRequisitionTotalItemDisAmount = xs.Result.purchaseRequisition.PurchaseRequisitionTotalItemDisAmount,
                    PurchaseRequisitionTotalActualAmount = xs.Result.purchaseRequisition.PurchaseRequisitionTotalActualAmount,
                    PurchaseRequisitionTotalDisPer = xs.Result.purchaseRequisition.PurchaseRequisitionTotalDisPer,
                    PurchaseRequisitionTotalDisAmount = xs.Result.purchaseRequisition.PurchaseRequisitionTotalDisAmount,
                    PurchaseRequisitionVatAmt = xs.Result.purchaseRequisition.PurchaseRequisitionVatAmt,
                    PurchaseRequisitionVatPer = xs.Result.purchaseRequisition.PurchaseRequisitionVatPer,
                    PurchaseRequisitionVatRoundSign = xs.Result.purchaseRequisition.PurchaseRequisitionVatRoundSign,
                    PurchaseRequisitionVatRountAmt = xs.Result.purchaseRequisition.PurchaseRequisitionVatRountAmt,
                    PurchaseRequisitionNetDisAmount = xs.Result.purchaseRequisition.PurchaseRequisitionNetDisAmount,
                    PurchaseRequisitionNetAmount = xs.Result.purchaseRequisition.PurchaseRequisitionNetAmount,
                    PurchaseRequisitionTransportCost = xs.Result.purchaseRequisition.PurchaseRequisitionTransportCost,
                    PurchaseRequisitionHandlingcharges = xs.Result.purchaseRequisition.PurchaseRequisitionHandlingcharges,
                    PurchaseRequisitionIssueId = xs.Result.purchaseRequisition.PurchaseRequisitionIssueId,
                    PurchaseRequisitionJobDirectPur = xs.Result.purchaseRequisition.PurchaseRequisitionJobDirectPur,
                    PurchaseRequisitionDelStatus = xs.Result.purchaseRequisition.PurchaseRequisitionDelStatus,
                    deliveryterms = xs.Result.purchaseRequisition.deliveryterms,
                    deliveryatsite = xs.Result.purchaseRequisition.deliveryatsite,
                    confirmationemaildetails = xs.Result.purchaseRequisition.confirmationemaildetails,
                    paymentTerms = xs.Result.purchaseRequisition.paymentTerms,
                    IsPrintEnabled = !xs.Result.purchaseRequisition.IsPrintEnabled,
                    IsEditEnabled = !xs.Result.purchaseRequisition.IsEditEnabled
                };

                purchaseRequisitionViewModel.PurchaseRequisitionDetails = _mapper.Map<List<PurchaseRequisitionDetailsViewModel>>(xs.Result.purchaseRequisitionDetails);
                purchaseRequisitionViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionViewModel>>(xs.Result.accountsTransactions);
                apiResponse = new ApiResponse<PurchaseRequisitionViewModel>
                {
                    Valid = true,
                    Result = _mapper.Map<PurchaseRequisitionViewModel>(purchaseRequisitionViewModel),
                    Message = xs.Message
                };
            }
            else
            {
                apiResponse = new ApiResponse<PurchaseRequisitionViewModel>
                {
                    Valid = false,
                    Error = true,
                    Exception = null,
                    Message = PurchaseRequisitionMessage.VoucherAlreadyExist
                };

            }
            return apiResponse;
        }

        [HttpPost]
        [Route("UpdatePurchaseRequisition")]
        public async Task<ApiResponse<PurchaseRequisitionViewModel>> UpdatePurchaseRequisition([FromBody] PurchaseRequisitionViewModel voucherCompositeView)
        {
            var param1 = _mapper.Map<PurchaseRequisition>(voucherCompositeView);
            var param2 = _mapper.Map<List<PurchaseRequisitionDetails>>(voucherCompositeView.PurchaseRequisitionDetails);
            var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
            List<AccountsTransactions> accountsTransactions = new List<AccountsTransactions>();

            //var boqId = param2.Select(x => x.boqdId).Distinct().FirstOrDefault();
            //var reqItems = param2.Select(x => new PurchaseReqItesm()
            //{
            //    ItemCode = (int)x.PurchaseRequisitionDetailsMatId,
            //    ItemQty = (int)x.PurchaseRequisitionDetailsQuantity
            //}).ToList();

            //if (_purchaseRequisitionService.IsPurchaseRequisitionQtyExcededFromBOQ(boqId, reqItems))
            //{
            //    return new ApiResponse<PurchaseRequisitionViewModel>
            //    {
            //        Valid = false,
            //        Error = false,
            //        Result = voucherCompositeView,
            //        Exception = null,
            //        Message = "Sorry You can't update Purchase requisition as item quantity exceded from total BoQ quantity"
            //    };
            //}

            //var param4 = _mapper.Map<List<StockRegister>>(voucherCompositeView.StockRegister);
            var xs = await _purchaseRequisitionService.UpdatePurchaseRequisition(param1, accountsTransactions, param2
                //, param4
                );

            //var xs = _purchaseRequisitionService.UpdatePurchaseRequisition(param1, param3, param2
            //    //, param4
            //    );

            PurchaseRequisitionViewModel purchaseRequisitionViewModel = new PurchaseRequisitionViewModel
            {
                PurchaseRequisitionId = xs.Result.purchaseRequisition.PurchaseRequisitionId,
                PurchaseRequisitionNo = xs.Result.purchaseRequisition.PurchaseRequisitionNo,
                PurchaseRequisitionDate = xs.Result.purchaseRequisition.PurchaseRequisitionDate,
                PurchaseRequisitionType = xs.Result.purchaseRequisition.PurchaseRequisitionType,
                PurchaseRequisitionPartyId = xs.Result.purchaseRequisition.PurchaseRequisitionPartyId,
                PurchaseRequisitionPartyName = xs.Result.purchaseRequisition.PurchaseRequisitionPartyName,
                PurchaseRequisitionPartyAddress = xs.Result.purchaseRequisition.PurchaseRequisitionPartyAddress,
                PurchaseRequisitionPartyVatNo = xs.Result.purchaseRequisition.PurchaseRequisitionPartyVatNo,
                PurchaseRequisitionSupInvNo = xs.Result.purchaseRequisition.PurchaseRequisitionSupInvNo,
                PurchaseRequisitionRefNo = xs.Result.purchaseRequisition.PurchaseRequisitionRefNo,
                PurchaseRequisitionDescription = xs.Result.purchaseRequisition.PurchaseRequisitionDescription,
                PurchaseRequisitionGrno = xs.Result.purchaseRequisition.PurchaseRequisitionGrno,
                PurchaseRequisitionGrdate = xs.Result.purchaseRequisition.PurchaseRequisitionGrdate,
                PurchaseRequisitionLpono = xs.Result.purchaseRequisition.PurchaseRequisitionLpono,
                PurchaseRequisitionLpodate = xs.Result.purchaseRequisition.PurchaseRequisitionLpodate,
                PurchaseRequisitionQtnNo = xs.Result.purchaseRequisition.PurchaseRequisitionQtnNo,
                PurchaseRequisitionQtnDate = xs.Result.purchaseRequisition.PurchaseRequisitionQtnDate,
                PurchaseRequisitionExcludeVat = xs.Result.purchaseRequisition.PurchaseRequisitionExcludeVat,
                PurchaseRequisitionPono = xs.Result.purchaseRequisition.PurchaseRequisitionPono,
                PurchaseRequisitionBatchCode = xs.Result.purchaseRequisition.PurchaseRequisitionBatchCode,
                PurchaseRequisitionDayBookNo = xs.Result.purchaseRequisition.PurchaseRequisitionDayBookNo,
                PurchaseRequisitionLocationId = xs.Result.purchaseRequisition.PurchaseRequisitionLocationId,
                PurchaseRequisitionUserId = xs.Result.purchaseRequisition.PurchaseRequisitionUserId,
                PurchaseRequisitionCurrencyId = xs.Result.purchaseRequisition.PurchaseRequisitionCurrencyId,
                PurchaseRequisitionCompanyId = xs.Result.purchaseRequisition.PurchaseRequisitionCompanyId,
                PurchaseRequisitionJobId = xs.Result.purchaseRequisition.PurchaseRequisitionJobId,
                PurchaseRequisitionFsno = xs.Result.purchaseRequisition.PurchaseRequisitionFsno,
                PurchaseRequisitionFcRate = xs.Result.purchaseRequisition.PurchaseRequisitionFcRate,
                PurchaseRequisitionStatus = xs.Result.purchaseRequisition.PurchaseRequisitionStatus,
                PurchaseRequisitionTotalGrossAmount = xs.Result.purchaseRequisition.PurchaseRequisitionTotalGrossAmount,
                PurchaseRequisitionTotalItemDisAmount = xs.Result.purchaseRequisition.PurchaseRequisitionTotalItemDisAmount,
                PurchaseRequisitionTotalActualAmount = xs.Result.purchaseRequisition.PurchaseRequisitionTotalActualAmount,
                PurchaseRequisitionTotalDisPer = xs.Result.purchaseRequisition.PurchaseRequisitionTotalDisPer,
                PurchaseRequisitionTotalDisAmount = xs.Result.purchaseRequisition.PurchaseRequisitionTotalDisAmount,
                PurchaseRequisitionVatAmt = xs.Result.purchaseRequisition.PurchaseRequisitionVatAmt,
                PurchaseRequisitionVatPer = xs.Result.purchaseRequisition.PurchaseRequisitionVatPer,
                PurchaseRequisitionVatRoundSign = xs.Result.purchaseRequisition.PurchaseRequisitionVatRoundSign,
                PurchaseRequisitionVatRountAmt = xs.Result.purchaseRequisition.PurchaseRequisitionVatRountAmt,
                PurchaseRequisitionNetDisAmount = xs.Result.purchaseRequisition.PurchaseRequisitionNetDisAmount,
                PurchaseRequisitionNetAmount = xs.Result.purchaseRequisition.PurchaseRequisitionNetAmount,
                PurchaseRequisitionTransportCost = xs.Result.purchaseRequisition.PurchaseRequisitionTransportCost,
                PurchaseRequisitionHandlingcharges = xs.Result.purchaseRequisition.PurchaseRequisitionHandlingcharges,
                PurchaseRequisitionIssueId = xs.Result.purchaseRequisition.PurchaseRequisitionIssueId,
                PurchaseRequisitionJobDirectPur = xs.Result.purchaseRequisition.PurchaseRequisitionJobDirectPur,
                PurchaseRequisitionDelStatus = xs.Result.purchaseRequisition.PurchaseRequisitionDelStatus,
                deliveryterms = xs.Result.purchaseRequisition.deliveryterms,
                deliveryatsite = xs.Result.purchaseRequisition.deliveryatsite,
                confirmationemaildetails = xs.Result.purchaseRequisition.confirmationemaildetails,
                paymentTerms = xs.Result.purchaseRequisition.paymentTerms,
            };

            purchaseRequisitionViewModel.PurchaseRequisitionDetails = _mapper.Map<List<PurchaseRequisitionDetailsViewModel>>(xs.Result.purchaseRequisitionDetails);
            purchaseRequisitionViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionViewModel>>(xs.Result.accountsTransactions);

            ApiResponse<PurchaseRequisitionViewModel> apiResponse = new ApiResponse<PurchaseRequisitionViewModel>
            {
                Valid = true,
                Result = _mapper.Map<PurchaseRequisitionViewModel>(purchaseRequisitionViewModel),
                Message = xs.Message
            };

            return apiResponse;

        }


        [HttpPost]
        [Route("DeletePurchaseRequisition")]
        public ApiResponse<int> DeletePurchaseRequisition([FromBody] PurchaseRequisitionViewModel voucherCompositeView)
        {
            var param1 = _mapper.Map<PurchaseRequisition>(voucherCompositeView);
            var param2 = _mapper.Map<List<PurchaseRequisitionDetails>>(voucherCompositeView.PurchaseRequisitionDetails);
            var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
            //var param4 = _mapper.Map<List<StockRegister>>(voucherCompositeView.StockRegister);
            var xs = _purchaseRequisitionService.DeletePurchaseRequisition(param1, param3, param2
                //, param4
                );
            ApiResponse<int> apiResponse = new ApiResponse<int>
            {
                Valid = true,
                Result = 0,
                Message = PurchaseRequisitionMessage.DeleteVoucher
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
                Result = _mapper.Map<List<AccountsTransactions>>(_purchaseRequisitionService.GetAllTransaction()),
                Message = ""
            };
            return apiResponse;




        }

        [HttpGet]
        [Route("GetPurchaseRequisition")]
        public ApiResponse<List<PurchaseRequisition>> GetAllPurchaseRequisition()
        {


            ApiResponse<List<PurchaseRequisition>> apiResponse = new ApiResponse<List<PurchaseRequisition>>
            {
                Valid = true,
                Result = _mapper.Map<List<PurchaseRequisition>>(_purchaseRequisitionService.GetPurchaseRequisition()),
                Message = ""
            };
            return apiResponse;





        }

        [HttpGet]
        [Route("GetSavedPurchaseRequisitionDetails/{id}")]
        public async Task<ApiResponse<PurchaseRequisitionViewModel>> GetSavedPurchaseRequisitionDetails(int id)
        {
            var data = await _purchaseRequisitionService.GetSavedPurchaseRequisitionDetails(id);
            PurchaseRequisitionModel purchaseRequisition = data.Result;

            if (purchaseRequisition.purchaseRequisition != null)
            {
                PurchaseRequisitionViewModel purchaseRequisitionViewModel = new PurchaseRequisitionViewModel
                {
                    PurchaseRequisitionId = purchaseRequisition.purchaseRequisition.PurchaseRequisitionId,
                    PurchaseRequisitionNo = purchaseRequisition.purchaseRequisition.PurchaseRequisitionNo,
                    PurchaseRequisitionDate = purchaseRequisition.purchaseRequisition.PurchaseRequisitionDate,
                    PurchaseRequisitionType = purchaseRequisition.purchaseRequisition.PurchaseRequisitionType,
                    PurchaseRequisitionPartyId = purchaseRequisition.purchaseRequisition.PurchaseRequisitionPartyId,
                    PurchaseRequisitionPartyName = purchaseRequisition.purchaseRequisition.PurchaseRequisitionPartyName,
                    PurchaseRequisitionPartyAddress = purchaseRequisition.purchaseRequisition.PurchaseRequisitionPartyAddress,
                    PurchaseRequisitionPartyVatNo = purchaseRequisition.purchaseRequisition.PurchaseRequisitionPartyVatNo,
                    PurchaseRequisitionSupInvNo = purchaseRequisition.purchaseRequisition.PurchaseRequisitionSupInvNo,
                    PurchaseRequisitionRefNo = purchaseRequisition.purchaseRequisition.PurchaseRequisitionRefNo,
                    PurchaseRequisitionDescription = purchaseRequisition.purchaseRequisition.PurchaseRequisitionDescription,
                    PurchaseRequisitionGrno = purchaseRequisition.purchaseRequisition.PurchaseRequisitionGrno,
                    PurchaseRequisitionGrdate = purchaseRequisition.purchaseRequisition.PurchaseRequisitionGrdate,
                    PurchaseRequisitionLpono = purchaseRequisition.purchaseRequisition.PurchaseRequisitionLpono,
                    PurchaseRequisitionLpodate = purchaseRequisition.purchaseRequisition.PurchaseRequisitionLpodate,
                    PurchaseRequisitionQtnNo = purchaseRequisition.purchaseRequisition.PurchaseRequisitionQtnNo,
                    PurchaseRequisitionQtnDate = purchaseRequisition.purchaseRequisition.PurchaseRequisitionQtnDate,
                    PurchaseRequisitionExcludeVat = purchaseRequisition.purchaseRequisition.PurchaseRequisitionExcludeVat,
                    PurchaseRequisitionPono = purchaseRequisition.purchaseRequisition.PurchaseRequisitionPono,
                    PurchaseRequisitionBatchCode = purchaseRequisition.purchaseRequisition.PurchaseRequisitionBatchCode,
                    PurchaseRequisitionDayBookNo = purchaseRequisition.purchaseRequisition.PurchaseRequisitionDayBookNo,
                    PurchaseRequisitionLocationId = purchaseRequisition.purchaseRequisition.PurchaseRequisitionLocationId,
                    PurchaseRequisitionUserId = purchaseRequisition.purchaseRequisition.PurchaseRequisitionUserId,
                    PurchaseRequisitionCurrencyId = purchaseRequisition.purchaseRequisition.PurchaseRequisitionCurrencyId,
                    PurchaseRequisitionCompanyId = purchaseRequisition.purchaseRequisition.PurchaseRequisitionCompanyId,
                    PurchaseRequisitionJobId = purchaseRequisition.purchaseRequisition.PurchaseRequisitionJobId,
                    PurchaseRequisitionFsno = purchaseRequisition.purchaseRequisition.PurchaseRequisitionFsno,
                    PurchaseRequisitionFcRate = purchaseRequisition.purchaseRequisition.PurchaseRequisitionFcRate,
                    PurchaseRequisitionStatus = purchaseRequisition.purchaseRequisition.PurchaseRequisitionStatus,
                    PurchaseRequisitionTotalGrossAmount = purchaseRequisition.purchaseRequisition.PurchaseRequisitionTotalGrossAmount,
                    PurchaseRequisitionTotalItemDisAmount = purchaseRequisition.purchaseRequisition.PurchaseRequisitionTotalItemDisAmount,
                    PurchaseRequisitionTotalActualAmount = purchaseRequisition.purchaseRequisition.PurchaseRequisitionTotalActualAmount,
                    PurchaseRequisitionTotalDisPer = purchaseRequisition.purchaseRequisition.PurchaseRequisitionTotalDisPer,
                    PurchaseRequisitionTotalDisAmount = purchaseRequisition.purchaseRequisition.PurchaseRequisitionTotalDisAmount,
                    PurchaseRequisitionVatAmt = purchaseRequisition.purchaseRequisition.PurchaseRequisitionVatAmt,
                    PurchaseRequisitionVatPer = purchaseRequisition.purchaseRequisition.PurchaseRequisitionVatPer,
                    PurchaseRequisitionVatRoundSign = purchaseRequisition.purchaseRequisition.PurchaseRequisitionVatRoundSign,
                    PurchaseRequisitionVatRountAmt = purchaseRequisition.purchaseRequisition.PurchaseRequisitionVatRountAmt,
                    PurchaseRequisitionNetDisAmount = purchaseRequisition.purchaseRequisition.PurchaseRequisitionNetDisAmount,
                    PurchaseRequisitionNetAmount = purchaseRequisition.purchaseRequisition.PurchaseRequisitionNetAmount,
                    PurchaseRequisitionTransportCost = purchaseRequisition.purchaseRequisition.PurchaseRequisitionTransportCost,
                    PurchaseRequisitionHandlingcharges = purchaseRequisition.purchaseRequisition.PurchaseRequisitionHandlingcharges,
                    PurchaseRequisitionIssueId = purchaseRequisition.purchaseRequisition.PurchaseRequisitionIssueId,
                    PurchaseRequisitionJobDirectPur = purchaseRequisition.purchaseRequisition.PurchaseRequisitionJobDirectPur,
                    PurchaseRequisitionDelStatus = purchaseRequisition.purchaseRequisition.PurchaseRequisitionDelStatus,
                    PurchaseRequisitionApprovedBy = purchaseRequisition.purchaseRequisition.PurchaseRequisitionApprovedBy,
                    PurchaseRequisitionApproveEnable = purchaseRequisition.purchaseRequisition.PurchaseRequisitionApproveEnable,
                    deliveryterms = purchaseRequisition.purchaseRequisition.deliveryterms,
                    deliveryatsite = purchaseRequisition.purchaseRequisition.deliveryatsite,
                    confirmationemaildetails = purchaseRequisition.purchaseRequisition.confirmationemaildetails,
                    paymentTerms = purchaseRequisition.purchaseRequisition.paymentTerms,
                    IsPrintEnabled = !purchaseRequisition.purchaseRequisition.IsPrintEnabled,
                    IsEditEnabled = !purchaseRequisition.purchaseRequisition.IsEditEnabled
                };
                purchaseRequisitionViewModel.PurchaseRequisitionDetails = _mapper.Map<List<PurchaseRequisitionDetailsViewModel>>(purchaseRequisition.purchaseRequisitionDetails);
                purchaseRequisitionViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionViewModel>>(purchaseRequisition.accountsTransactions);
                ApiResponse<PurchaseRequisitionViewModel> apiResponse = new ApiResponse<PurchaseRequisitionViewModel>
                {
                    Valid = true,
                    Result = purchaseRequisitionViewModel,
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
                Message = PurchaseRequisitionMessage.VoucherNumberExist
            };
            var x = _purchaseRequisitionService.GetVouchersNumbers(id);
            if (x == null)
            {
                apiResponse.Result = false;
                apiResponse.Message = "";
            }

            return apiResponse;
        }

        [HttpPost]
        [Route("GetPurchaseRequisitionStatus")]
        public async Task<IActionResult> GetPurchaseRequisitionStatus(PurChaseRequisitionStatusFilterReport model)
        {

            var response = await _purchaseRequisitionService.GetPurchaseRequisitionStatus(model);
            ApiResponse<List<PurChaseRequisitionStatus>> apiResponse = new ApiResponse<List<PurChaseRequisitionStatus>>
            {
                Valid = true,
                Result = response.Result,
                Message = ""
            };
            return Ok(apiResponse);
        }

        [HttpPost]
        [Route("GetPurchaseReqJobId")]
        public async Task<IActionResult> GetPurchaseReqJobId(PurChaseRequisitionFilterReport model)
        {
            return Ok(await _purchaseRequisitionService.GetPurchaseReqJobId(model));
        }
        [HttpGet]
        [Route("GetBalanceAmount")]
        public ResponseInfo GetBalanceAmount(int jobId)
        {
            var obj = new ResponseInfo();
            obj = _purchaseRequisitionService.GetBalanceAmount(jobId);
            return obj;
        }

        [HttpPost]
        [Route("GetPurchaseReqSales")]
        public async Task<IActionResult> GetPurchaseReqSales(int prId)
        {
            return Ok(await _purchaseRequisitionService.GetPurchaseReqSale(prId));
        }
        [HttpGet]
        [Route("GetAllPReq")]
        public ResponseInfo GetAllPReq()
        {
            var obj = new ResponseInfo();
            obj = _purchaseRequisitionService.GetAllPReq();
            return obj;
        }


    }

}
