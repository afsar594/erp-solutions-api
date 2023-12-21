using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Application.Master.Interfaces;
using Inspire.Erp.Application.Procurement.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Inspire.Erp.Web.Common;
using Inspire.Erp.Web.MODULE;
using Inspire.Erp.Web.ViewModels.Procurement;
using Microsoft.AspNetCore.Mvc;

namespace Inspire.Erp.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseVoucherController : ControllerBase
    {

        private readonly IPurchaseVoucherService _service;

        private readonly IMapper _mapper;
        private IRepository<UnitDetails> _UnitDetailsRepository;
        public PurchaseVoucherController(IPurchaseVoucherService service, IMapper mapper, IRepository<UnitDetails> unitDetailsRepository)
        {
            _service = service;
            _mapper = mapper;
            _UnitDetailsRepository = unitDetailsRepository;
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

        //[HttpPost("InsertPurchaseVoucher")]
        //public ApiResponse<PurchaseVoucherViewModel> InsertPurchaseVoucher([FromBody] PurchaseVoucherViewModel dataObj)
        //{

        //        ApiResponse<PurchaseVoucherViewModel> apiResponse = new ApiResponse<PurchaseVoucherViewModel>();
        //        if (_service.GetVouchersNumbers(dataObj.PurchaseVoucherVoucherNo) == null)
        //        {
        //            var param1 = _mapper.Map<PurchaseVoucher>(dataObj);
        //            var param2 = _mapper.Map<List<PurchaseVoucherDetails>>(dataObj.PurchaseVoucherDetails);
        //            var param3 = _mapper.Map<List<AccountsTransactions>>(dataObj.AccountsTransactions);

        //            param3 = new List<AccountsTransactions>();
        //            List<StockRegister> param4 = new List<StockRegister>();

        //        var allUnits = new List<UnitDetails>();
        //        var unitDetails = new UnitDetails();
        //        foreach (var item in param1.PurchaseVoucherDetails)
        //        {
        //            unitDetails = _UnitDetailsRepository.GetAsQueryable().Where(c => c.UnitDetailsUnitId == item.PurchaseVoucherDetailsUnitId).FirstOrDefault();
        //            allUnits.Add(unitDetails);
        //        }

        //        clsAccountAndStock.PurchaseVoucher_Accounts_STOCK_Transactions("", "", param1, param2, ref param4, ref param3);

        //            var xs = _service.InsertPurchaseVoucher(param1, param3, param2
        //           , param4
        //           );


        //            PurchaseVoucherViewModel purchaseVoucherViewModel = new PurchaseVoucherViewModel
        //            {


        //                PurchaseVoucherPurId = xs.purchaseVoucher.PurchaseVoucherPurID,
        //                PurchaseVoucherVoucherNo = xs.purchaseVoucher.PurchaseVoucherVoucherNo,
        //                PurchaseVoucherPurchaseDate = xs.purchaseVoucher.PurchaseVoucherPurchaseDate,
        //                PurchaseVoucherPurchaseType = xs.purchaseVoucher.PurchaseVoucherPurchaseType,
        //                PurchaseVoucherPartyId = xs.purchaseVoucher.PurchaseVoucherPartyId,
        //                PurchaseVoucherPartyName = xs.purchaseVoucher.PurchaseVoucherPartyName,
        //                PurchaseVoucherPartyAddress = xs.purchaseVoucher.PurchaseVoucherPartyAddress,
        //                PurchaseVoucherPartyVatNo = xs.purchaseVoucher.PurchaseVoucherPartyVatNo,
        //               // PurchaseVoucherContPerson = xs.purchaseVoucher.SalesVoucherContPerson,
        //                PurchaseVoucherPurchaseRef = xs.purchaseVoucher.PurchaseVoucherPurchaseRef,
        //                PurchaseVoucherDescription = xs.purchaseVoucher.PurchaseVoucherDescription,
        //                PurchaseVoucherDisAcNo = xs.purchaseVoucher.PurchaseVoucherDisAcNo,
        //               // PurchaseVoucherDlvDate = xs.purchaseVoucher.PurchaseVoucherDlvDate,
        //               // PurchaseVoucherSono = xs.purchaseVoucher.PurchaseVoucherSono,
        //               // PurchaseVoucherSodate = xs.purchaseVoucher.PurchaseVoucherSodate,
        //               // PurchaseVoucherQtnNo = xs.purchaseVoucher.PurchaseVoucherQtnNo,
        //               // PurchaseVoucherQtnDate = xs.purchaseVoucher.PurchaseVoucherQtnDate,
        //                PurchaseVoucherExcludeVat = xs.purchaseVoucher.PurchaseVoucherExcludeVat,
        //               // PurchasesVoucherSalesManId = xs.purchaseVoucher.PurchasesVoucherSalesManId,
        //               // PurchaseVoucherDptId = xs.purchaseVoucher.PurchaseVoucherDptId,
        //               // PurchaseVoucherEnqNo = xs.purchaseVoucher.PurchaseVoucherEnqNo,
        //                PurchaseVoucherLocationId = xs.purchaseVoucher.PurchaseVoucherLocationId,
        //                PurchaseVoucherUserId = xs.purchaseVoucher.PurchaseVoucherUserId,
        //                PurchaseVoucherCurrencyId = xs.purchaseVoucher.PurchaseVoucherCurrencyId,
        //                PurchaseVoucherCompanyId = xs.purchaseVoucher.PurchaseVoucherCompanyId,
        //                PurchaseVoucherJobId = xs.purchaseVoucher.PurchaseVoucherJobId,
        //                PurchaseVoucherFsno = xs.purchaseVoucher.PurchaseVoucherFsno,
        //                PurchaseVoucherFcRate = xs.purchaseVoucher.PurchaseVoucherFcRate,
        //                PurchaseVoucherStatus = xs.purchaseVoucher.PurchaseVoucherStatus,
        //               // PurchaseVoucherTotalGrossAmount = xs.purchaseVoucher.PurchaseVoucherTotalGrossAmount,
        //                PurchaseVoucherTotalItemDisAmount = xs.purchaseVoucher.PurchaseVoucherTotalItemDisAmount,
        //                PurchaseVoucherActualAmount = xs.purchaseVoucher.PurchaseVoucherActualAmount,
        //               // PurchaseVoucherTotalActualAmount = xs.purchaseVoucher.PurchaseVoucherTotalActualAmount,
        //               // PurchaseVoucherTotalDisPer = xs.purchaseVoucher.PurchaseVoucherTotalDisPer,
        //               // PurchaseVoucherTotalDisAmount = xs.purchaseVoucher.PurchaseVoucherTotalDisAmount,
        //               // PurchaseVoucherVatAmt = xs.purchaseVoucher.PurchaseVoucherVatAmt,
        //               // PurchaseVoucherVatPer = xs.purchaseVoucher.PurchaseVoucherVatPer,
        //                PurchaseVoucherVatRoundSign = xs.purchaseVoucher.PurchaseVoucherVatRoundSign,
        //                PurchaseVoucherVatAmount = xs.purchaseVoucher.PurchaseVoucherVatAmount,
        //                PurchaseVoucherVatPercentage = xs.purchaseVoucher.PurchaseVoucherVatPercentage,
        //                PurchaseVoucherVatRoundAmount = xs.purchaseVoucher.PurchaseVoucherVatRoundAmount,
        //               // PurchaseVoucherVatRountAmt = xs.purchaseVoucher.PurchaseVoucherVatRountAmt,
        //               // PurchaseVoucherNetDisAmount = xs.purchaseVoucher.PurchaseVoucherNetDisAmount,

        //                PurchaseVoucherNetAmount = xs.purchaseVoucher.PurchaseVoucherNetAmount,
        //               // PurchaseVoucherEnqDate = xs.purchaseVoucher.PurchaseVoucherEnqDate,
        //               // PurchaseVoucherShippinAddress = xs.purchaseVoucher.PurchaseVoucherShippinAddress,
        //               // PurchaseVoucherTermsAndCondition = xs.purchaseVoucher.PurchaseVoucherTermsAndCondition,
        //               // PurchaseVoucherRemarks2 = xs.purchaseVoucher.PurchaseVoucherRemarks2,


        //                // PurchaseVoucherHeader = xs.purchaseVoucher.PurchaseVoucherHeader,
        //                // PurchaseVoucherDelivery = xs.purchaseVoucher.PurchaseVoucherDelivery,

        //                // PurchaseVoucherNotes = xs.purchaseVoucher.PurchaseVoucherNotes,
        //                // PurchaseVoucherFooter = xs.purchaseVoucher.PurchaseVoucherFooter,
        //                // PurchaseVoucherPaymentTerms = xs.purchaseVoucher.PurchaseVoucherPaymentTerms,
        //                // PurchaseVoucherSubject = xs.purchaseVoucher.PurchaseVoucherSubject,
        //                // PurchaseVoucherContent = xs.purchaseVoucher.PurchaseVoucherContent,
        //                // PurchaseVoucherRemarks1 = xs.purchaseVoucher.PurchaseVoucherRemarks1,

        //                PurchaseVoucherDelStatus = xs.purchaseVoucher.PurchaseVoucherDelStatus,

        //                // new field mapping 

        //                PurchaseVoucherTotalGrossAmt = xs.purchaseVoucher.PurchaseVoucherTotalGrossAmt,
        //                PurchaseVoucherTotalDiscountAmt = xs.purchaseVoucher.PurchaseVoucherTotalDiscountAmt,
        //                PurchaseVoucherDiscountPercentage = xs.purchaseVoucher.PurchaseVoucherDiscountPercentage,
        //                PurchaseVoucherGrNo = xs.purchaseVoucher.PurchaseVoucherGRNo,
        //                PurchaseVoucherGrDate = xs.purchaseVoucher.PurchaseVoucherGRDate,
        //                PurchaseVoucherLpoNo = xs.purchaseVoucher.PurchaseVoucherLPONo,
        //                PurchaseVoucherLpoDate = xs.purchaseVoucher.PurchaseVoucherLPODate,
        //                PurchaseVoucherQuatationNo = xs.purchaseVoucher.PurchaseVoucherQuatationNo,
        //                PurchaseVoucherQuatationDate = xs.purchaseVoucher.PurchaseVoucherQuatationDate,
        //                PurchaseVoucherPoNo = xs.purchaseVoucher.PurchaseVoucherPoNo,
        //                PurchaseVoucherBatchCode = xs.purchaseVoucher.PurchaseVoucherBatchCode,
        //            };

        //            purchaseVoucherViewModel.PurchaseVoucherDetails = _mapper.Map<List<PurchaseVoucherDetailsViewModel>>(xs.purchaseVoucherDetails);
        //            purchaseVoucherViewModel.AccountsTransactions = _mapper.Map<List<ViewModels.AccountTransactionViewModel>>(xs.accountsTransactions);
        //            apiResponse = new ApiResponse<PurchaseVoucherViewModel>
        //            {
        //                Valid = true,
        //                Result = _mapper.Map<PurchaseVoucherViewModel>(purchaseVoucherViewModel),
        //                Message = PurchaseVoucherMessage.SaveVoucher
        //            };
        //        }
        //        else
        //        {
        //            apiResponse = new ApiResponse<PurchaseVoucherViewModel>
        //            {
        //                Valid = false,
        //                Error = true,
        //                Exception = null,
        //                Message = SalesVoucherMessage.VoucherAlreadyExist
        //            };

        //        }

        //        // var item = _service.InsertPurchaseVoucher(dataObj);
        //        return apiResponse;

        //}
        //[HttpPost("UpdatePurchaseVoucher")]
        public ApiResponse<PurchaseVoucherViewModel> UpdatePurchaseVoucher([FromBody] PurchaseVoucherViewModel dataObj)
        {
            var param1 = _mapper.Map<PurchaseVoucher>(dataObj);
            var param2 = _mapper.Map<List<PurchaseVoucherDetails>>(dataObj.PurchaseVoucherDetails);
            var param3 = _mapper.Map<List<AccountsTransactions>>(dataObj.AccountsTransactions);

            param3 = new List<AccountsTransactions>();
            List<StockRegister> param4 = new List<StockRegister>();

            var allUnits = new List<UnitDetails>();
            var unitDetails = new UnitDetails();
            foreach (var item in param1.PurchaseVoucherDetails)
            {
                unitDetails = _UnitDetailsRepository.GetAsQueryable().Where(c => c.UnitDetailsUnitId == item.PurchaseVoucherDetailsUnitId).FirstOrDefault();
                allUnits.Add(unitDetails);
            }

            clsAccountAndStock.PurchaseVoucher_Accounts_STOCK_Transactions("", "", param1, param2, ref param4, ref param3);

            var xs = _service.UpdatePurchaseVoucher(param1, param3, param2
           , param4
           );


            PurchaseVoucherViewModel purchaseVoucherViewModel = new PurchaseVoucherViewModel
            {


                PurchaseVoucherPurId = xs.purchaseVoucher.PurchaseVoucherPurID,
                PurchaseVoucherVoucherNo = xs.purchaseVoucher.PurchaseVoucherVoucherNo,
                PurchaseVoucherPurchaseDate = xs.purchaseVoucher.PurchaseVoucherPurchaseDate,
                PurchaseVoucherPurchaseType = xs.purchaseVoucher.PurchaseVoucherPurchaseType,
                PurchaseVoucherPartyId = xs.purchaseVoucher.PurchaseVoucherPartyId,
                PurchaseVoucherPartyName = xs.purchaseVoucher.PurchaseVoucherPartyName,
                PurchaseVoucherPartyAddress = xs.purchaseVoucher.PurchaseVoucherPartyAddress,
                PurchaseVoucherPartyVatNo = xs.purchaseVoucher.PurchaseVoucherPartyVatNo,
                // PurchaseVoucherContPerson = xs.purchaseVoucher.SalesVoucherContPerson,
                PurchaseVoucherPurchaseRef = xs.purchaseVoucher.PurchaseVoucherPurchaseRef,
                PurchaseVoucherDescription = xs.purchaseVoucher.PurchaseVoucherDescription,
                PurchaseVoucherDisAcNo = xs.purchaseVoucher.PurchaseVoucherDisAcNo,
                // PurchaseVoucherDlvDate = xs.purchaseVoucher.PurchaseVoucherDlvDate,
                // PurchaseVoucherSono = xs.purchaseVoucher.PurchaseVoucherSono,
                // PurchaseVoucherSodate = xs.purchaseVoucher.PurchaseVoucherSodate,
                // PurchaseVoucherQtnNo = xs.purchaseVoucher.PurchaseVoucherQtnNo,
                // PurchaseVoucherQtnDate = xs.purchaseVoucher.PurchaseVoucherQtnDate,
                PurchaseVoucherExcludeVat = xs.purchaseVoucher.PurchaseVoucherExcludeVat,
                // PurchasesVoucherSalesManId = xs.purchaseVoucher.PurchasesVoucherSalesManId,
                // PurchaseVoucherDptId = xs.purchaseVoucher.PurchaseVoucherDptId,
                // PurchaseVoucherEnqNo = xs.purchaseVoucher.PurchaseVoucherEnqNo,
                PurchaseVoucherLocationId = xs.purchaseVoucher.PurchaseVoucherLocationId,
                PurchaseVoucherUserId = xs.purchaseVoucher.PurchaseVoucherUserId,
                PurchaseVoucherCurrencyId = xs.purchaseVoucher.PurchaseVoucherCurrencyId,
                PurchaseVoucherCompanyId = xs.purchaseVoucher.PurchaseVoucherCompanyId,
                PurchaseVoucherJobId = xs.purchaseVoucher.PurchaseVoucherJobId,
                PurchaseVoucherFsno = xs.purchaseVoucher.PurchaseVoucherFsno,
                PurchaseVoucherFcRate = xs.purchaseVoucher.PurchaseVoucherFcRate,
                PurchaseVoucherStatus = xs.purchaseVoucher.PurchaseVoucherStatus,
                // PurchaseVoucherTotalGrossAmount = xs.purchaseVoucher.PurchaseVoucherTotalGrossAmount,
                PurchaseVoucherTotalItemDisAmount = xs.purchaseVoucher.PurchaseVoucherTotalItemDisAmount,
                // PurchaseVoucherTotalActualAmount = xs.purchaseVoucher.PurchaseVoucherTotalActualAmount,
                // PurchaseVoucherTotalDisPer = xs.purchaseVoucher.PurchaseVoucherTotalDisPer,
                // PurchaseVoucherTotalDisAmount = xs.purchaseVoucher.PurchaseVoucherTotalDisAmount,
                // PurchaseVoucherVatAmt = xs.purchaseVoucher.PurchaseVoucherVatAmt,
                // PurchaseVoucherVatPer = xs.purchaseVoucher.PurchaseVoucherVatPer,
                PurchaseVoucherVatRoundSign = xs.purchaseVoucher.PurchaseVoucherVatRoundSign,

                PurchaseVoucherVatPercentage = xs.purchaseVoucher.PurchaseVoucherVatPercentage,
                PurchaseVoucherVatRoundAmount = xs.purchaseVoucher.PurchaseVoucherVatRoundAmount,
                // PurchaseVoucherVatRountAmt = xs.purchaseVoucher.PurchaseVoucherVatRountAmt,
                // PurchaseVoucherNetDisAmount = xs.purchaseVoucher.PurchaseVoucherNetDisAmount,
                PurchaseVoucherNetAmount = xs.purchaseVoucher.PurchaseVoucherNetAmount,
                // PurchaseVoucherEnqDate = xs.purchaseVoucher.PurchaseVoucherEnqDate,
                // PurchaseVoucherShippinAddress = xs.purchaseVoucher.PurchaseVoucherShippinAddress,
                // PurchaseVoucherTermsAndCondition = xs.purchaseVoucher.PurchaseVoucherTermsAndCondition,
                // PurchaseVoucherRemarks2 = xs.purchaseVoucher.PurchaseVoucherRemarks2,


                // PurchaseVoucherHeader = xs.purchaseVoucher.PurchaseVoucherHeader,
                // PurchaseVoucherDelivery = xs.purchaseVoucher.PurchaseVoucherDelivery,

                // PurchaseVoucherNotes = xs.purchaseVoucher.PurchaseVoucherNotes,
                // PurchaseVoucherFooter = xs.purchaseVoucher.PurchaseVoucherFooter,
                // PurchaseVoucherPaymentTerms = xs.purchaseVoucher.PurchaseVoucherPaymentTerms,
                // PurchaseVoucherSubject = xs.purchaseVoucher.PurchaseVoucherSubject,
                // PurchaseVoucherContent = xs.purchaseVoucher.PurchaseVoucherContent,
                // PurchaseVoucherRemarks1 = xs.purchaseVoucher.PurchaseVoucherRemarks1,

                PurchaseVoucherDelStatus = xs.purchaseVoucher.PurchaseVoucherDelStatus,

                PurchaseVoucherVatAmount = xs.purchaseVoucher.PurchaseVoucherVatAmount,
                PurchaseVoucherTotalGrossAmt = xs.purchaseVoucher.PurchaseVoucherTotalGrossAmt,
                PurchaseVoucherTotalDiscountAmt = xs.purchaseVoucher.PurchaseVoucherTotalDiscountAmt,

                PurchaseVoucherDiscountPercentage = xs.purchaseVoucher.PurchaseVoucherDiscountPercentage,
                PurchaseVoucherGrNo = xs.purchaseVoucher.PurchaseVoucherGRNo,
                PurchaseVoucherGrDate = xs.purchaseVoucher.PurchaseVoucherGRDate,
                PurchaseVoucherLpoNo = xs.purchaseVoucher.PurchaseVoucherLPONo,
                PurchaseVoucherLpoDate = xs.purchaseVoucher.PurchaseVoucherLPODate,
                PurchaseVoucherQuatationNo = xs.purchaseVoucher.PurchaseVoucherQuatationNo,
                PurchaseVoucherQuatationDate = xs.purchaseVoucher.PurchaseVoucherQuatationDate,
                PurchaseVoucherPoNo = xs.purchaseVoucher.PurchaseVoucherPoNo,
                PurchaseVoucherBatchCode = xs.purchaseVoucher.PurchaseVoucherBatchCode,


                PurchaseVoucherActualAmount = xs.purchaseVoucher.PurchaseVoucherActualAmount,
            };

            purchaseVoucherViewModel.PurchaseVoucherDetails = _mapper.Map<List<PurchaseVoucherDetailsViewModel>>(xs.purchaseVoucherDetails);
            purchaseVoucherViewModel.AccountsTransactions = _mapper.Map<List<ViewModels.AccountTransactionViewModel>>(xs.accountsTransactions);

            ApiResponse<PurchaseVoucherViewModel> apiResponse = new ApiResponse<PurchaseVoucherViewModel>
            {
                Valid = true,
                Result = _mapper.Map<PurchaseVoucherViewModel>(purchaseVoucherViewModel),
                Message = PurchaseVoucherMessage.SaveVoucher
            };
            return apiResponse;

        }
        //[HttpPost("DeletePurchaseVoucher")]
        //public IActionResult DeletePurchaseVoucher([FromBody] PurchaseVoucher dataObj)
        //{
        //    try
        //    {
        //        var item = _service.DeletePurchaseVoucher(dataObj);
        //        return Ok(item);
        //    }
        //    catch (System.Exception ex)
        //    {
        //        return StatusCode(500, ex.Message.ToString());
        //    }
        //}

        [HttpPost]
        [Route("DeletePurchaseVoucher")]
        public ApiResponse<int> DeletePurchaseVoucher([FromBody] PurchaseVoucherViewModel dataObj)
        {

            var param1 = _mapper.Map<PurchaseVoucher>(dataObj);
            var param2 = _mapper.Map<List<PurchaseVoucherDetails>>(dataObj.PurchaseVoucherDetails);
            var param3 = _mapper.Map<List<AccountsTransactions>>(dataObj.AccountsTransactions);


            //==============
            param3 = new List<AccountsTransactions>();
            List<StockRegister> param4 = new List<StockRegister>();

            var xs = _service.DeletePurchaseVoucher(param1, param3, param2
           , param4
           );
            //========================


            ApiResponse<int> apiResponse = new ApiResponse<int>
            {
                Valid = true,
                Result = 0,
                Message = PurchaseVoucherMessage.DeleteVoucher
            };

            return apiResponse;

        }

        //[HttpGet("GetSavedPurchaseVoucherDetails/{id}")]
        //public ApiResponse<PurchaseVoucherViewModel> GetSavedPurchaseVoucherDetails(string id)
        //{
        //    PurchaseVoucherModel purchaseVoucher = _service.GetSavedPurchaseVoucherDetails(id);

        //    if (purchaseVoucher!= null)
        //    {
        //        PurchaseVoucherViewModel purchaseVoucherViewModel = new PurchaseVoucherViewModel
        //        {


        //            PurchaseVoucherPurId = purchaseVoucher.purchaseVoucher.PurchaseVoucherPurID,
        //            PurchaseVoucherVoucherNo = purchaseVoucher.purchaseVoucher.PurchaseVoucherVoucherNo,
        //            PurchaseVoucherPurchaseDate = purchaseVoucher.purchaseVoucher.PurchaseVoucherPurchaseDate,
        //            PurchaseVoucherPurchaseType = purchaseVoucher.purchaseVoucher.PurchaseVoucherPurchaseType,
        //            PurchaseVoucherPartyId = purchaseVoucher.purchaseVoucher.PurchaseVoucherPartyId,
        //            PurchaseVoucherPartyName = purchaseVoucher.purchaseVoucher.PurchaseVoucherPartyName,
        //            PurchaseVoucherPartyAddress = purchaseVoucher.purchaseVoucher.PurchaseVoucherPartyAddress,
        //            PurchaseVoucherPartyVatNo = purchaseVoucher.purchaseVoucher.PurchaseVoucherPartyVatNo,
        //            // PurchaseVoucherContPerson = xs.purchaseVoucher.SalesVoucherContPerson,
        //            PurchaseVoucherPurchaseRef = purchaseVoucher.purchaseVoucher.PurchaseVoucherPurchaseRef,
        //            PurchaseVoucherDescription = purchaseVoucher.purchaseVoucher.PurchaseVoucherDescription,
        //            PurchaseVoucherDisAcNo = purchaseVoucher.purchaseVoucher.PurchaseVoucherDisAcNo,
        //            // PurchaseVoucherDlvDate = xs.purchaseVoucher.PurchaseVoucherDlvDate,
        //            // PurchaseVoucherSono = xs.purchaseVoucher.PurchaseVoucherSono,
        //            // PurchaseVoucherSodate = xs.purchaseVoucher.PurchaseVoucherSodate,
        //            // PurchaseVoucherQtnNo = xs.purchaseVoucher.PurchaseVoucherQtnNo,
        //            // PurchaseVoucherQtnDate = xs.purchaseVoucher.PurchaseVoucherQtnDate,
        //            PurchaseVoucherExcludeVat = purchaseVoucher.purchaseVoucher.PurchaseVoucherExcludeVat,
        //            // PurchasesVoucherSalesManId = xs.purchaseVoucher.PurchasesVoucherSalesManId,
        //            // PurchaseVoucherDptId = xs.purchaseVoucher.PurchaseVoucherDptId,
        //            // PurchaseVoucherEnqNo = xs.purchaseVoucher.PurchaseVoucherEnqNo,
        //            PurchaseVoucherLocationId = purchaseVoucher.purchaseVoucher.PurchaseVoucherLocationId,
        //            PurchaseVoucherUserId = purchaseVoucher.purchaseVoucher.PurchaseVoucherUserId,
        //            PurchaseVoucherCurrencyId = purchaseVoucher.purchaseVoucher.PurchaseVoucherCurrencyId,
        //            PurchaseVoucherCompanyId = purchaseVoucher.purchaseVoucher.PurchaseVoucherCompanyId,
        //            PurchaseVoucherJobId = purchaseVoucher.purchaseVoucher.PurchaseVoucherJobId,
        //            PurchaseVoucherFsno = purchaseVoucher.purchaseVoucher.PurchaseVoucherFsno,
        //            PurchaseVoucherFcRate = purchaseVoucher.purchaseVoucher.PurchaseVoucherFcRate,
        //            PurchaseVoucherStatus = purchaseVoucher.purchaseVoucher.PurchaseVoucherStatus,
        //            //PurchaseVoucherTotalGrossAmount = xs.purchaseVoucher.PurchaseVoucherTotalGrossAmount,
        //            PurchaseVoucherTotalItemDisAmount = purchaseVoucher.purchaseVoucher.PurchaseVoucherTotalItemDisAmount,
        //            // PurchaseVoucherTotalActualAmount = xs.purchaseVoucher.PurchaseVoucherTotalActualAmount,
        //            // PurchaseVoucherTotalDisPer = xs.purchaseVoucher.PurchaseVoucherTotalDisPer,
        //            // PurchaseVoucherTotalDisAmount = xs.purchaseVoucher.PurchaseVoucherTotalDisAmount,
        //             //PurchaseVoucherVatAmt = purchaseVoucher.purchaseVoucher.PurchaseVoucherVatAmt,
        //            // PurchaseVoucherVatPer = xs.purchaseVoucher.PurchaseVoucherVatPer,
        //            PurchaseVoucherVatRoundSign = purchaseVoucher.purchaseVoucher.PurchaseVoucherVatRoundSign,
        //            PurchaseVoucherVatPercentage = purchaseVoucher.purchaseVoucher.PurchaseVoucherVatPercentage,
        //            PurchaseVoucherVatRoundAmount = purchaseVoucher.purchaseVoucher.PurchaseVoucherVatRoundAmount,
        //            // PurchaseVoucherVatRountAmt = xs.purchaseVoucher.PurchaseVoucherVatRountAmt,
        //            // PurchaseVoucherNetDisAmount = xs.purchaseVoucher.PurchaseVoucherNetDisAmount,
        //             PurchaseVoucherNetAmount = purchaseVoucher.purchaseVoucher.PurchaseVoucherNetAmount,
        //            // PurchaseVoucherEnqDate = xs.purchaseVoucher.PurchaseVoucherEnqDate,
        //            // PurchaseVoucherShippinAddress = xs.purchaseVoucher.PurchaseVoucherShippinAddress,
        //            // PurchaseVoucherTermsAndCondition = xs.purchaseVoucher.PurchaseVoucherTermsAndCondition,
        //            // PurchaseVoucherRemarks2 = xs.purchaseVoucher.PurchaseVoucherRemarks2,


        //            // PurchaseVoucherHeader = xs.purchaseVoucher.PurchaseVoucherHeader,
        //            // PurchaseVoucherDelivery = xs.purchaseVoucher.PurchaseVoucherDelivery,

        //            // PurchaseVoucherNotes = xs.purchaseVoucher.PurchaseVoucherNotes,
        //            // PurchaseVoucherFooter = xs.purchaseVoucher.PurchaseVoucherFooter,
        //            // PurchaseVoucherPaymentTerms = xs.purchaseVoucher.PurchaseVoucherPaymentTerms,
        //            // PurchaseVoucherSubject = xs.purchaseVoucher.PurchaseVoucherSubject,
        //            // PurchaseVoucherContent = xs.purchaseVoucher.PurchaseVoucherContent,
        //            // PurchaseVoucherRemarks1 = xs.purchaseVoucher.PurchaseVoucherRemarks1,

        //            PurchaseVoucherDelStatus = purchaseVoucher.purchaseVoucher.PurchaseVoucherDelStatus,

        //            // new field mapping 
        //            PurchaseVoucherVatAmount = purchaseVoucher.purchaseVoucher.PurchaseVoucherVatAmount,
        //            PurchaseVoucherTotalGrossAmt = purchaseVoucher.purchaseVoucher.PurchaseVoucherTotalGrossAmt,
        //            PurchaseVoucherTotalDiscountAmt = purchaseVoucher.purchaseVoucher.PurchaseVoucherTotalDiscountAmt,

        //            PurchaseVoucherGrNo = purchaseVoucher.purchaseVoucher.PurchaseVoucherGRNo,
        //            PurchaseVoucherGrDate = purchaseVoucher.purchaseVoucher.PurchaseVoucherGRDate,
        //            PurchaseVoucherLpoNo = purchaseVoucher.purchaseVoucher.PurchaseVoucherLPONo,
        //            PurchaseVoucherLpoDate = purchaseVoucher.purchaseVoucher.PurchaseVoucherLPODate,
        //            PurchaseVoucherQuatationNo = purchaseVoucher.purchaseVoucher.PurchaseVoucherQuatationNo,
        //            PurchaseVoucherQuatationDate = purchaseVoucher.purchaseVoucher.PurchaseVoucherQuatationDate,
        //            PurchaseVoucherPoNo = purchaseVoucher.purchaseVoucher.PurchaseVoucherPoNo,
        //            PurchaseVoucherBatchCode = purchaseVoucher.purchaseVoucher.PurchaseVoucherBatchCode,

        //            PurchaseVoucherActualAmount = purchaseVoucher.purchaseVoucher.PurchaseVoucherActualAmount,

        //            PurchaseVoucherDiscountPercentage = purchaseVoucher.purchaseVoucher.PurchaseVoucherDiscountPercentage,
        //        };
        //        purchaseVoucherViewModel.PurchaseVoucherDetails = _mapper.Map<List<PurchaseVoucherDetailsViewModel>>(purchaseVoucher.purchaseVoucherDetails);
        //        purchaseVoucherViewModel.AccountsTransactions = _mapper.Map<List<ViewModels.AccountTransactionViewModel>>(purchaseVoucher.accountsTransactions);
        //        ApiResponse<PurchaseVoucherViewModel> apiResponse = new ApiResponse<PurchaseVoucherViewModel>
        //        {
        //            Valid = true,
        //            Result = purchaseVoucherViewModel,
        //            Message = ""
        //        };
        //        return apiResponse;
        //    }
        //    return null;

        //}

        [HttpPost("GetPurchaseVoucherReport")]
        public async Task<IActionResult> GetPurchaseVoucherReport(GenericGridViewModel model)
        {
            return Ok(await _service.GetPurchaseVoucherReport(model));
        }

        [HttpPost]
        [Route("InsertPurchaseVoucher")]
        public ApiResponse<PurchaseVoucher> InsertPurchaseVoucher([FromBody] PurchaseVoucher purchaseVoucher)
        {
            var param1 = _mapper.Map<PurchaseVoucher>(purchaseVoucher);
            var param2 = _mapper.Map<List<PurchaseVoucherDetails>>(purchaseVoucher.PurchaseVoucherDetails);
            var param3 = _mapper.Map<List<AccountsTransactions>>(purchaseVoucher.AccountsTransactions);

            var response = _service.InsertPurchaseVoucher(param1, param3, param2);
            var res = new ApiResponse<PurchaseVoucher>
            {
                Valid = true,
                Message = "Purchase Inserted",
                Result = response,
            };

            return res;
        }

        [HttpPost]
        [Route("UpdatePurchaseVoucher")]
        public ApiResponse<PurchaseVoucher> UpdatePurchaseVoucher([FromBody] PurchaseVoucher purchaseVoucher)
        {
            var param1 = _mapper.Map<PurchaseVoucher>(purchaseVoucher);
            var param2 = _mapper.Map<List<PurchaseVoucherDetails>>(purchaseVoucher.PurchaseVoucherDetails);
            var param3 = _mapper.Map<List<AccountsTransactions>>(purchaseVoucher.AccountsTransactions);

            var response = _service.InsertPurchaseVoucher(param1, param3, param2);
            var res = new ApiResponse<PurchaseVoucher>
            {
                Valid = true,
                Message = "Purchase Inserted",
                Result = response,
            };

            return res;
        }

        [HttpGet("GetSavedPurchaseVoucherDetails/{id}")]
        public ApiResponse<PurchaseVoucher> GetSavedPurchaseVoucherDetails(string id)
        {
            var purchaseVoucher = _service.GetSavedPurchaseVoucherDetailsV2(id);

            ApiResponse<PurchaseVoucher> apiResponse = new ApiResponse<PurchaseVoucher>
            {
                Valid = true,
                Result = purchaseVoucher,
                Message = ""
            };
            return apiResponse;
        }
    }
}