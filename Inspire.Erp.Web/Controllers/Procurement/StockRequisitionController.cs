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

namespace Inspire.Erp.Web.Controllers
{
    [Route("api/StockRequisition")]
    [Produces("application/json")]
    [ApiController]
    public class StockRequisitionController : ControllerBase
    {
        private IStockRequisitionService _StockRequisitionService;
        private readonly IMapper _mapper;
        public StockRequisitionController(IStockRequisitionService StockRequisitionService, IMapper mapper)
        {
            _StockRequisitionService = StockRequisitionService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("InsertStockRequisition")]
        public ApiResponse<StockRequisitionViewModel> InsertStockRequisition([FromBody] StockRequisitionViewModel voucherCompositeView)
        {
            ApiResponse<StockRequisitionViewModel> apiResponse = new ApiResponse<StockRequisitionViewModel>();
            if (_StockRequisitionService.GetVouchersNumbers(voucherCompositeView.StockRequisitionNo) == null)
            {
                var param1 = _mapper.Map<StockRequisition>(voucherCompositeView);
                var param2 = _mapper.Map<List<StockRequisitionDetails>>(voucherCompositeView.StockRequisitionDetails);
                var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);

                var xs = _StockRequisitionService.InsertStockRequisition(param1, param3, param2

                    );
                StockRequisitionViewModel StockRequisitionViewModel = new StockRequisitionViewModel();
                StockRequisitionViewModel = _mapper.Map<StockRequisitionViewModel>(StockRequisitionViewModel);
                apiResponse = new ApiResponse<StockRequisitionViewModel>
                {
                    Valid = true,
                    Result = StockRequisitionViewModel,
                    Message = StockRequisitionMessage.SaveVoucher
                };
            }
            else
            {
                apiResponse = new ApiResponse<StockRequisitionViewModel>
                {
                    Valid = false,
                    Error = true,
                    Exception = null,
                    Message = StockRequisitionMessage.VoucherAlreadyExist
                };

            }
            return apiResponse;
        }

        [HttpPost]
        [Route("UpdateStockRequisition")]
        public ApiResponse<StockRequisitionViewModel> UpdateStockRequisition([FromBody] StockRequisitionViewModel voucherCompositeView)
        {
            var param1 = _mapper.Map<StockRequisition>(voucherCompositeView);
            var param2 = _mapper.Map<List<StockRequisitionDetails>>(voucherCompositeView.StockRequisitionDetails);
            var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
            List<AccountsTransactions> accountsTransactions = new List<AccountsTransactions>();

            //var param4 = _mapper.Map<List<StockRegister>>(voucherCompositeView.StockRegister);
            var xs = _StockRequisitionService.UpdateStockRequisition(param1, accountsTransactions, param2
                //, param4
                );

            //var xs = _StockRequisitionService.UpdateStockRequisition(param1, param3, param2
            //    //, param4
            //    );

            StockRequisitionViewModel StockRequisitionViewModel = new StockRequisitionViewModel();
            //{
            //    StockRequisitionId = xs.StockRequisition.StockRequisitionId,
            //    StockRequisitionNo = xs.StockRequisition.StockRequisitionNo,
            //    StockRequisitionDate = xs.StockRequisition.StockRequisitionDate,
            //    StockRequisitionType = xs.StockRequisition.StockRequisitionType,
            //    StockRequisitionPartyId = xs.StockRequisition.StockRequisitionPartyId,
            //    StockRequisitionPartyName = xs.StockRequisition.StockRequisitionPartyName,
            //    StockRequisitionPartyAddress = xs.StockRequisition.StockRequisitionPartyAddress,
            //    StockRequisitionPartyVatNo = xs.StockRequisition.StockRequisitionPartyVatNo,
            //    StockRequisitionSupInvNo = xs.StockRequisition.StockRequisitionSupInvNo,
            //    StockRequisitionRefNo = xs.StockRequisition.StockRequisitionRefNo,
            //    StockRequisitionDescription = xs.StockRequisition.StockRequisitionDescription,
            //    StockRequisitionGrno = xs.StockRequisition.StockRequisitionGrno,
            //    StockRequisitionGrdate = xs.StockRequisition.StockRequisitionGrdate,
            //    StockRequisitionLpono = xs.StockRequisition.StockRequisitionLpono,
            //    StockRequisitionLpodate = xs.StockRequisition.StockRequisitionLpodate,
            //    StockRequisitionQtnNo = xs.StockRequisition.StockRequisitionQtnNo,
            //    StockRequisitionQtnDate = xs.StockRequisition.StockRequisitionQtnDate,
            //    StockRequisitionExcludeVat = xs.StockRequisition.StockRequisitionExcludeVat,
            //    StockRequisitionPono = xs.StockRequisition.StockRequisitionPono,
            //    StockRequisitionBatchCode = xs.StockRequisition.StockRequisitionBatchCode,
            //    StockRequisitionDayBookNo = xs.StockRequisition.StockRequisitionDayBookNo,
            //    StockRequisitionLocationId = xs.StockRequisition.StockRequisitionLocationId,
            //    StockRequisitionUserId = xs.StockRequisition.StockRequisitionUserId,
            //    StockRequisitionCurrencyId = xs.StockRequisition.StockRequisitionCurrencyId,
            //    StockRequisitionCompanyId = xs.StockRequisition.StockRequisitionCompanyId,
            //    StockRequisitionJobId = xs.StockRequisition.StockRequisitionJobId,
            //    StockRequisitionFsno = xs.StockRequisition.StockRequisitionFsno,
            //    StockRequisitionFcRate = xs.StockRequisition.StockRequisitionFcRate,
            //    StockRequisitionStatus = xs.StockRequisition.StockRequisitionStatus,
            //    StockRequisitionTotalGrossAmount = xs.StockRequisition.StockRequisitionTotalGrossAmount,
            //    StockRequisitionTotalItemDisAmount = xs.StockRequisition.StockRequisitionTotalItemDisAmount,
            //    StockRequisitionTotalActualAmount = xs.StockRequisition.StockRequisitionTotalActualAmount,
            //    StockRequisitionTotalDisPer = xs.StockRequisition.StockRequisitionTotalDisPer,
            //    StockRequisitionTotalDisAmount = xs.StockRequisition.StockRequisitionTotalDisAmount,
            //    StockRequisitionVatAmt = xs.StockRequisition.StockRequisitionVatAmt,
            //    StockRequisitionVatPer = xs.StockRequisition.StockRequisitionVatPer,
            //    StockRequisitionVatRoundSign = xs.StockRequisition.StockRequisitionVatRoundSign,
            //    StockRequisitionVatRountAmt = xs.StockRequisition.StockRequisitionVatRountAmt,
            //    StockRequisitionNetDisAmount = xs.StockRequisition.StockRequisitionNetDisAmount,
            //    StockRequisitionNetAmount = xs.StockRequisition.StockRequisitionNetAmount,
            //    StockRequisitionTransportCost = xs.StockRequisition.StockRequisitionTransportCost,
            //    StockRequisitionHandlingcharges = xs.StockRequisition.StockRequisitionHandlingcharges,
            //    StockRequisitionIssueId = xs.StockRequisition.StockRequisitionIssueId,
            //    StockRequisitionJobDirectPur = xs.StockRequisition.StockRequisitionJobDirectPur,
            //    StockRequisitionDelStatus = xs.StockRequisition.StockRequisitionDelStatus,

            //};

            StockRequisitionViewModel.StockRequisitionDetails = _mapper.Map<List<StockRequisitionDetailsViewModel>>(xs.StockRequisitionDetails);
            StockRequisitionViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionViewModel>>(xs.accountsTransactions);

            ApiResponse<StockRequisitionViewModel> apiResponse = new ApiResponse<StockRequisitionViewModel>
            {
                Valid = true,
                Result = _mapper.Map<StockRequisitionViewModel>(StockRequisitionViewModel),
                Message = StockRequisitionMessage.UpdateVoucher
            };

            return apiResponse;

        }

        //public AccountsTransactions GenerateNewAccountTransactionEntry(
        //         long? AccountsTransactionsTransSno,
        //  string? AccountsTransactionsAccNo,
        // DateTime? AccountsTransactionsTransDate,
        //string AccountsTransactionsParticulars,
        //decimal AccountsTransactionsDebit,
        //decimal AccountsTransactionsCredit,
        //decimal? AccountsTransactionsFcDebit,
        //decimal? AccountsTransactionsFcCredit,
        //string AccountsTransactionsVoucherType,
        //string AccountsTransactionsVoucherNo,
        //string AccountsTransactionsDescription,
        //long AccountsTransactionsUserId,
        //string AccountsTransactionsStatus,
        //DateTime AccountsTransactionsTstamp,
        //string RefNo,
        //decimal AccountsTransactionsFsno,
        //decimal? AccountsTransactionsAllocDebit,
        //decimal? AccountsTransactionsAllocCredit,
        //decimal? AccountsTransactionsAllocBalance,
        //decimal? AccountsTransactionsFcAllocDebit,
        //decimal? AccountsTransactionsFcAllocCredit,
        //decimal? AccountsTransactionsFcAllocBalance,
        //int? AccountsTransactionsLocation,
        //long? AccountsTransactionsJobNo,
        //long? AccountsTransactionsCostCenterId,
        //DateTime? AccountsTransactionsApprovalDt,
        //int? AccountsTransactionsDepartment,
        //decimal? AccountsTransactionsFcRate,
        //int? AccountsTransactionsCompanyId,
        //int? AccountsTransactionsCurrencyId,
        //double? AccountsTransactionsDrGram,
        //double? AccountsTransactionsCrGram,
        //string AccountsTransactionsCheqNo,
        //string AccountsTransactionsLpoNo,
        //DateTime? AccountsTransactionsCheqDate,
        //string AccountsTransactionsOpposEntryDesc,
        //  double? AccountsTransactionsAllocUpdateBal,
        // long? AccountsTransactionsDeptId,
        //  string AccountsTransactionsVatno,
        //  decimal? AccountsTransactionsVatableAmount,
        //  bool? AccountstransactionsDelStatus 
        //     )
        //{

        //    AccountsTransactions accountsTransactions = new AccountsTransactions
        //    {

        //       //AccountsTransactionsTransSno = AccountsTransactionsTransSno,
        //        AccountsTransactionsAccNo = AccountsTransactionsAccNo,
        //        AccountsTransactionsTransDate = AccountsTransactionsTransDate,
        //        AccountsTransactionsParticulars = AccountsTransactionsParticulars,
        //        AccountsTransactionsDebit = AccountsTransactionsDebit,
        //        AccountsTransactionsCredit = AccountsTransactionsCredit,
        //        AccountsTransactionsFcDebit = AccountsTransactionsFcDebit,
        //        AccountsTransactionsFcCredit = AccountsTransactionsFcCredit,
        //        AccountsTransactionsVoucherType = AccountsTransactionsVoucherType,
        //        AccountsTransactionsVoucherNo = AccountsTransactionsVoucherNo,
        //        AccountsTransactionsDescription = AccountsTransactionsDescription,
        //        AccountsTransactionsUserId = AccountsTransactionsUserId,
        //        AccountsTransactionsStatus = AccountsTransactionsStatus,
        //        AccountsTransactionsTstamp = AccountsTransactionsTstamp,
        //        RefNo = RefNo,
        //        AccountsTransactionsFsno = AccountsTransactionsFsno,
        //        AccountsTransactionsAllocDebit = AccountsTransactionsAllocDebit,
        //        AccountsTransactionsAllocCredit = AccountsTransactionsAllocCredit,
        //        AccountsTransactionsAllocBalance = AccountsTransactionsAllocBalance,
        //        AccountsTransactionsFcAllocDebit = AccountsTransactionsFcAllocDebit,
        //        AccountsTransactionsFcAllocCredit = AccountsTransactionsFcAllocCredit,
        //        AccountsTransactionsFcAllocBalance = AccountsTransactionsFcAllocBalance,
        //        AccountsTransactionsLocation = AccountsTransactionsLocation,
        //        AccountsTransactionsJobNo = AccountsTransactionsJobNo,
        //        AccountsTransactionsCostCenterId = AccountsTransactionsCostCenterId,
        //        AccountsTransactionsApprovalDt = AccountsTransactionsApprovalDt,
        //        AccountsTransactionsDepartment = AccountsTransactionsDepartment,
        //        AccountsTransactionsFcRate = AccountsTransactionsFcRate,
        //        AccountsTransactionsCompanyId = AccountsTransactionsCompanyId,
        //        AccountsTransactionsCurrencyId = AccountsTransactionsCurrencyId,
        //        AccountsTransactionsDrGram = AccountsTransactionsDrGram,
        //        AccountsTransactionsCrGram = AccountsTransactionsCrGram,
        //        AccountsTransactionsCheqNo = AccountsTransactionsCheqNo,
        //        AccountsTransactionsLpoNo = AccountsTransactionsLpoNo,
        //        AccountsTransactionsCheqDate = AccountsTransactionsCheqDate,
        //        AccountsTransactionsOpposEntryDesc = AccountsTransactionsOpposEntryDesc,
        //        AccountsTransactionsAllocUpdateBal = AccountsTransactionsAllocUpdateBal,
        //        AccountsTransactionsDeptId = AccountsTransactionsDeptId,
        //        AccountsTransactionsVatno = AccountsTransactionsVatno,
        //        AccountsTransactionsVatableAmount = AccountsTransactionsVatableAmount,
        //        AccountstransactionsDelStatus=false

        //    };
        //    return accountsTransactions;

        //}


        [HttpPost]
        [Route("DeleteStockRequisition")]
        public ApiResponse<int> DeleteStockRequisition([FromBody] StockRequisitionViewModel voucherCompositeView)
        {
            var param1 = _mapper.Map<StockRequisition>(voucherCompositeView);
            var param2 = _mapper.Map<List<StockRequisitionDetails>>(voucherCompositeView.StockRequisitionDetails);
            var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
            //var param4 = _mapper.Map<List<StockRegister>>(voucherCompositeView.StockRegister);
            var xs = _StockRequisitionService.DeleteStockRequisition(param1, param3, param2
                //, param4
                );
            ApiResponse<int> apiResponse = new ApiResponse<int>
            {
                Valid = true,
                Result = 0,
                Message = StockRequisitionMessage.DeleteVoucher
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
                Result = _mapper.Map<List<AccountsTransactions>>(_StockRequisitionService.GetAllTransaction()),
                Message = ""
            };
            return apiResponse;




        }

        [HttpGet]
        [Route("GetStockRequisition")]
        public ApiResponse<List<StockRequisition>> GetAllStockRequisition()
        {
            ApiResponse<List<StockRequisition>> apiResponse = new ApiResponse<List<StockRequisition>>
            {
                Valid = true,
                Result = _mapper.Map<List<StockRequisition>>(_StockRequisitionService.GetStockRequisition()),
                Message = ""
            };
            return apiResponse;
        }

        [HttpGet]
        [Route("GetSavedStockRequisitionDetails/{id}")]
        public ApiResponse<StockRequisitionViewModel> GetSavedStockRequisitionDetails(string id)
        {
            StockRequisitionModel StockRequisition = _StockRequisitionService.GetSavedStockRequisitionDetails(id);

            if (StockRequisition != null)
            {
                StockRequisitionViewModel StockRequisitionViewModel = new StockRequisitionViewModel();
                //{


                //    StockRequisitionId = StockRequisition.StockRequisition.StockRequisitionId,
                //    StockRequisitionNo = StockRequisition.StockRequisition.StockRequisitionNo,
                //    StockRequisitionDate = StockRequisition.StockRequisition.StockRequisitionDate,
                //    StockRequisitionType = StockRequisition.StockRequisition.StockRequisitionType,
                //    StockRequisitionPartyId = StockRequisition.StockRequisition.StockRequisitionPartyId,
                //    StockRequisitionPartyName = StockRequisition.StockRequisition.StockRequisitionPartyName,
                //    StockRequisitionPartyAddress = StockRequisition.StockRequisition.StockRequisitionPartyAddress,
                //    StockRequisitionPartyVatNo = StockRequisition.StockRequisition.StockRequisitionPartyVatNo,
                //    StockRequisitionSupInvNo = StockRequisition.StockRequisition.StockRequisitionSupInvNo,
                //    StockRequisitionRefNo = StockRequisition.StockRequisition.StockRequisitionRefNo,
                //    StockRequisitionDescription = StockRequisition.StockRequisition.StockRequisitionDescription,
                //    StockRequisitionGrno = StockRequisition.StockRequisition.StockRequisitionGrno,
                //    StockRequisitionGrdate = StockRequisition.StockRequisition.StockRequisitionGrdate,
                //    StockRequisitionLpono = StockRequisition.StockRequisition.StockRequisitionLpono,
                //    StockRequisitionLpodate = StockRequisition.StockRequisition.StockRequisitionLpodate,
                //    StockRequisitionQtnNo = StockRequisition.StockRequisition.StockRequisitionQtnNo,
                //    StockRequisitionQtnDate = StockRequisition.StockRequisition.StockRequisitionQtnDate,
                //    StockRequisitionExcludeVat = StockRequisition.StockRequisition.StockRequisitionExcludeVat,
                //    StockRequisitionPono = StockRequisition.StockRequisition.StockRequisitionPono,
                //    StockRequisitionBatchCode = StockRequisition.StockRequisition.StockRequisitionBatchCode,
                //    StockRequisitionDayBookNo = StockRequisition.StockRequisition.StockRequisitionDayBookNo,
                //    StockRequisitionLocationId = StockRequisition.StockRequisition.StockRequisitionLocationId,
                //    StockRequisitionUserId = StockRequisition.StockRequisition.StockRequisitionUserId,
                //    StockRequisitionCurrencyId = StockRequisition.StockRequisition.StockRequisitionCurrencyId,
                //    StockRequisitionCompanyId = StockRequisition.StockRequisition.StockRequisitionCompanyId,
                //    StockRequisitionJobId = StockRequisition.StockRequisition.StockRequisitionJobId,
                //    StockRequisitionFsno = StockRequisition.StockRequisition.StockRequisitionFsno,
                //    StockRequisitionFcRate = StockRequisition.StockRequisition.StockRequisitionFcRate,
                //    StockRequisitionStatus = StockRequisition.StockRequisition.StockRequisitionStatus,
                //    StockRequisitionTotalGrossAmount = StockRequisition.StockRequisition.StockRequisitionTotalGrossAmount,
                //    StockRequisitionTotalItemDisAmount = StockRequisition.StockRequisition.StockRequisitionTotalItemDisAmount,
                //    StockRequisitionTotalActualAmount = StockRequisition.StockRequisition.StockRequisitionTotalActualAmount,
                //    StockRequisitionTotalDisPer = StockRequisition.StockRequisition.StockRequisitionTotalDisPer,
                //    StockRequisitionTotalDisAmount = StockRequisition.StockRequisition.StockRequisitionTotalDisAmount,
                //    StockRequisitionVatAmt = StockRequisition.StockRequisition.StockRequisitionVatAmt,
                //    StockRequisitionVatPer = StockRequisition.StockRequisition.StockRequisitionVatPer,
                //    StockRequisitionVatRoundSign = StockRequisition.StockRequisition.StockRequisitionVatRoundSign,
                //    StockRequisitionVatRountAmt = StockRequisition.StockRequisition.StockRequisitionVatRountAmt,
                //    StockRequisitionNetDisAmount = StockRequisition.StockRequisition.StockRequisitionNetDisAmount,
                //    StockRequisitionNetAmount = StockRequisition.StockRequisition.StockRequisitionNetAmount,
                //    StockRequisitionTransportCost = StockRequisition.StockRequisition.StockRequisitionTransportCost,
                //    StockRequisitionHandlingcharges = StockRequisition.StockRequisition.StockRequisitionHandlingcharges,
                //    StockRequisitionIssueId = StockRequisition.StockRequisition.StockRequisitionIssueId,
                //    StockRequisitionJobDirectPur = StockRequisition.StockRequisition.StockRequisitionJobDirectPur,
                //    StockRequisitionDelStatus = StockRequisition.StockRequisition.StockRequisitionDelStatus,


                //};
                StockRequisitionViewModel.StockRequisitionDetails = _mapper.Map<List<StockRequisitionDetailsViewModel>>(StockRequisition.StockRequisitionDetails);
                StockRequisitionViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionViewModel>>(StockRequisition.accountsTransactions);
                ApiResponse<StockRequisitionViewModel> apiResponse = new ApiResponse<StockRequisitionViewModel>
                {
                    Valid = true,
                    Result = StockRequisitionViewModel,
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
                Message = StockRequisitionMessage.VoucherNumberExist
            };
            var x = _StockRequisitionService.GetVouchersNumbers(id);
            if (x == null)
            {
                apiResponse.Result = false;
                apiResponse.Message = "";
            }

            return apiResponse;
        }
    }
}
