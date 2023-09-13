using Inspire.Erp.Application.Account.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Inspire.Erp.Application.Sales.Interfaces;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Web.ViewModels.sales;
using Inspire.Erp.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Web.Common;
using Inspire.Erp.Web.MODULE;

using Microsoft.AspNetCore.Mvc.Rendering;



namespace Inspire.Erp.Web.Controllers
{
    [Route("api/SalesReturn")]
    [Produces("application/json")]
    [ApiController]
    public class SalesReturnController : ControllerBase
    {
        private ISalesReturnService _salesReturnService;
        private readonly IMapper _mapper;
        public SalesReturnController(ISalesReturnService salesReturnService, IMapper mapper)
        {
            _salesReturnService = salesReturnService;
            _mapper = mapper;
        }

        //[HttpGet]
        //[Route("SalesReturn_GetReportSalesReturn")]

        //public List<ReportSalesReturn> SalesReturn_GetReportSalesReturn()
        //{
        //    return _mapper.Map<List<ReportSalesReturn>>(_salesReturnService.SalesReturn_GetReportSalesReturn());


        //}



        //[HttpGet]
        //[Route("SalesReturn_GetReportSalesReturn")]
        //public ApiResponse<List<ReportSalesReturn>> SalesReturn_GetReportSalesReturn()
        //{



        //    ApiResponse<List<ReportSalesReturn>> apiResponse = new ApiResponse<List<ReportSalesReturn>>
        //    {
        //        Valid = true,
        //        Result = _mapper.Map<List<ReportSalesReturn>>(_salesReturnService.SalesReturn_GetReportSalesReturn()),
        //        Message = ""
        //    };
        //    return apiResponse;




        //}



        [HttpPost]
        [Route("InsertSalesReturn")]
        public ApiResponse<SalesReturnViewModel> InsertSalesReturn([FromBody] SalesReturnViewModel voucherCompositeView)
        {
           

            ApiResponse<SalesReturnViewModel> apiResponse = new ApiResponse<SalesReturnViewModel>();
            if (_salesReturnService.GetVouchersNumbers(voucherCompositeView.SalesReturnNo) == null)
            {
                var param1 = _mapper.Map<SalesReturn>(voucherCompositeView);
                var param2 = _mapper.Map<List<SalesReturnDetails>>(voucherCompositeView.SalesReturnDetails);
                var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
                //var param4 = _mapper.Map<List<StockRegister>>(voucherCompositeView.StockRegister);
                //var xs = _salesReturnService.InsertSalesReturn(param1, param3, param2
                //    //, param4
                //    );

                //==============
                param3 = new List<AccountsTransactions>();
                List<StockRegister> param4 = new List<StockRegister>();
                clsAccountAndStock.SalesReturn_Accounts_STOCK_Transactions("", "", param1, param2, ref param4, ref param3);

                var xs = _salesReturnService.InsertSalesReturn(param1, param3, param2
               , param4
               );
                //========================



                SalesReturnViewModel salesReturnViewModel = new SalesReturnViewModel
                {





                    SalesReturnId = xs.salesReturn.SalesReturnId,
                    SalesReturnNo = xs.salesReturn.SalesReturnNo,
                    SalesReturnDate = xs.salesReturn.SalesReturnDate,
                    SalesReturnType = xs.salesReturn.SalesReturnType,
                    SalesReturnPartyId = xs.salesReturn.SalesReturnPartyId,
                    SalesReturnPartyName = xs.salesReturn.SalesReturnPartyName,
                    SalesReturnPartyAddress = xs.salesReturn.SalesReturnPartyAddress,
                    SalesReturnPartyVatNo = xs.salesReturn.SalesReturnPartyVatNo,
                    SalesReturnContPerson = xs.salesReturn.SalesReturnContPerson,
                    SalesReturnRefNo = xs.salesReturn.SalesReturnRefNo,
                    SalesReturnDescription = xs.salesReturn.SalesReturnDescription,
                    SalesReturnDlvNo = xs.salesReturn.SalesReturnDlvNo,
                    SalesReturnDlvDate = xs.salesReturn.SalesReturnDlvDate,
                    SalesReturnSono = xs.salesReturn.SalesReturnSono,
                    SalesReturnSodate = xs.salesReturn.SalesReturnSodate,
                    SalesReturnQtnNo = xs.salesReturn.SalesReturnQtnNo,
                    SalesReturnQtnDate = xs.salesReturn.SalesReturnQtnDate,
                    SalesReturnExcludeVat = xs.salesReturn.SalesReturnExcludeVat,
                    SalesReturnSalesManId = xs.salesReturn.SalesReturnSalesManId,
                    SalesReturnDptId = xs.salesReturn.SalesReturnDptId,
                    SalesReturnEnqNo = xs.salesReturn.SalesReturnEnqNo,
                    SalesReturnLocationId = xs.salesReturn.SalesReturnLocationId,
                    SalesReturnUserId = xs.salesReturn.SalesReturnUserId,
                    SalesReturnCurrencyId = xs.salesReturn.SalesReturnCurrencyId,
                    SalesReturnCompanyId = xs.salesReturn.SalesReturnCompanyId,
                    SalesReturnJobId = xs.salesReturn.SalesReturnJobId,
                    SalesReturnFsno = xs.salesReturn.SalesReturnFsno,
                    SalesReturnFcRate = xs.salesReturn.SalesReturnFcRate,
                    SalesReturnStatus = xs.salesReturn.SalesReturnStatus,
                    SalesReturnTotalGrossAmount = xs.salesReturn.SalesReturnTotalGrossAmount,
                    SalesReturnTotalItemDisAmount = xs.salesReturn.SalesReturnTotalItemDisAmount,
                    SalesReturnTotalActualAmount = xs.salesReturn.SalesReturnTotalActualAmount,
                    SalesReturnTotalDisPer = xs.salesReturn.SalesReturnTotalDisPer,
                    SalesReturnTotalDisAmount = xs.salesReturn.SalesReturnTotalDisAmount,
                    SalesReturnVatAmt = xs.salesReturn.SalesReturnVatAmt,
                    SalesReturnVatPer = xs.salesReturn.SalesReturnVatPer,
                    SalesReturnVatRoundSign = xs.salesReturn.SalesReturnVatRoundSign,
                    SalesReturnVatRountAmt = xs.salesReturn.SalesReturnVatRountAmt,
                    SalesReturnNetDisAmount = xs.salesReturn.SalesReturnNetDisAmount,
                    SalesReturnNetAmount = xs.salesReturn.SalesReturnNetAmount,
                    SalesReturnEnqDate = xs.salesReturn.SalesReturnEnqDate,
                    SalesReturnShippinAddress = xs.salesReturn.SalesReturnShippinAddress,
                    SalesReturnTermsAndCondition = xs.salesReturn.SalesReturnTermsAndCondition,
                    SalesReturnRemarks2  = xs.salesReturn.SalesReturnRemarks2 ,


                    SalesReturnHeader = xs.salesReturn.SalesReturnHeader,
                    SalesReturnDelivery = xs.salesReturn.SalesReturnDelivery,




                    SalesReturnNotes = xs.salesReturn.SalesReturnNotes,
                    SalesReturnFooter = xs.salesReturn.SalesReturnFooter,
                    SalesReturnPaymentTerms = xs.salesReturn.SalesReturnPaymentTerms,
                    SalesReturnSubject = xs.salesReturn.SalesReturnSubject,
                    SalesReturnContent = xs.salesReturn.SalesReturnContent,
                    SalesReturnRemarks1 = xs.salesReturn.SalesReturnRemarks1,

                    SalesReturnSvno = xs.salesReturn.SalesReturnSvno,
                    SalesReturnSvdate = xs.salesReturn.SalesReturnSvdate,
                 

                    SalesReturnDelStatus = xs.salesReturn.SalesReturnDelStatus,





                };

                salesReturnViewModel.SalesReturnDetails = _mapper.Map<List<SalesReturnDetailsViewModel>>(xs.salesReturnDetails);
                salesReturnViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionViewModel>>(xs.accountsTransactions);
                apiResponse = new ApiResponse<SalesReturnViewModel>
                {
                    Valid = true,
                    Result = _mapper.Map<SalesReturnViewModel>(salesReturnViewModel),
                    Message = SalesReturnMessage.SaveVoucher
                };
            }
            else
            {
                apiResponse = new ApiResponse<SalesReturnViewModel>
                {
                    Valid = false,
                    Error = true,
                    Exception = null,
                    Message = SalesReturnMessage.VoucherAlreadyExist
                };

            }


            return apiResponse;






        }

        [HttpPost]
        [Route("UpdateSalesReturn")]
        public ApiResponse<SalesReturnViewModel> UpdateSalesReturn([FromBody] SalesReturnViewModel voucherCompositeView)
        {
            var param1 = _mapper.Map<SalesReturn>(voucherCompositeView);
            var param2 = _mapper.Map<List<SalesReturnDetails>>(voucherCompositeView.SalesReturnDetails);
            var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
            //var param4 = _mapper.Map<List<StockRegister>>(voucherCompositeView.StockRegister);
            //var xs = _salesReturnService.UpdateSalesReturn(param1, param3, param2
            //    //, param4
            //    );





            //==============
            param3 = new List<AccountsTransactions>();
            List<StockRegister> param4 = new List<StockRegister>();
            clsAccountAndStock.SalesReturn_Accounts_STOCK_Transactions("", "", param1, param2, ref param4, ref param3);

            var xs = _salesReturnService.UpdateSalesReturn(param1, param3, param2
           , param4
           );
            //========================




            SalesReturnViewModel salesReturnViewModel = new SalesReturnViewModel
            {
                SalesReturnId = xs.salesReturn.SalesReturnId,
                SalesReturnNo = xs.salesReturn.SalesReturnNo,
                SalesReturnDate = xs.salesReturn.SalesReturnDate,
                SalesReturnType = xs.salesReturn.SalesReturnType,
                SalesReturnPartyId = xs.salesReturn.SalesReturnPartyId,
                SalesReturnPartyName = xs.salesReturn.SalesReturnPartyName,
                SalesReturnPartyAddress = xs.salesReturn.SalesReturnPartyAddress,
                SalesReturnPartyVatNo = xs.salesReturn.SalesReturnPartyVatNo,
                SalesReturnContPerson = xs.salesReturn.SalesReturnContPerson,
                SalesReturnRefNo = xs.salesReturn.SalesReturnRefNo,
                SalesReturnDescription = xs.salesReturn.SalesReturnDescription,
                SalesReturnDlvNo = xs.salesReturn.SalesReturnDlvNo,
                SalesReturnDlvDate = xs.salesReturn.SalesReturnDlvDate,
                SalesReturnSono = xs.salesReturn.SalesReturnSono,
                SalesReturnSodate = xs.salesReturn.SalesReturnSodate,
                SalesReturnQtnNo = xs.salesReturn.SalesReturnQtnNo,
                SalesReturnQtnDate = xs.salesReturn.SalesReturnQtnDate,
                SalesReturnExcludeVat = xs.salesReturn.SalesReturnExcludeVat,
                SalesReturnSalesManId = xs.salesReturn.SalesReturnSalesManId,
                SalesReturnDptId = xs.salesReturn.SalesReturnDptId,
                SalesReturnEnqNo = xs.salesReturn.SalesReturnEnqNo,
                SalesReturnLocationId = xs.salesReturn.SalesReturnLocationId,
                SalesReturnUserId = xs.salesReturn.SalesReturnUserId,
                SalesReturnCurrencyId = xs.salesReturn.SalesReturnCurrencyId,
                SalesReturnCompanyId = xs.salesReturn.SalesReturnCompanyId,
                SalesReturnJobId = xs.salesReturn.SalesReturnJobId,
                SalesReturnFsno = xs.salesReturn.SalesReturnFsno,
                SalesReturnFcRate = xs.salesReturn.SalesReturnFcRate,
                SalesReturnStatus = xs.salesReturn.SalesReturnStatus,
                SalesReturnTotalGrossAmount = xs.salesReturn.SalesReturnTotalGrossAmount,
                SalesReturnTotalItemDisAmount = xs.salesReturn.SalesReturnTotalItemDisAmount,
                SalesReturnTotalActualAmount = xs.salesReturn.SalesReturnTotalActualAmount,
                SalesReturnTotalDisPer = xs.salesReturn.SalesReturnTotalDisPer,
                SalesReturnTotalDisAmount = xs.salesReturn.SalesReturnTotalDisAmount,
                SalesReturnVatAmt = xs.salesReturn.SalesReturnVatAmt,
                SalesReturnVatPer = xs.salesReturn.SalesReturnVatPer,
                SalesReturnVatRoundSign = xs.salesReturn.SalesReturnVatRoundSign,
                SalesReturnVatRountAmt = xs.salesReturn.SalesReturnVatRountAmt,
                SalesReturnNetDisAmount = xs.salesReturn.SalesReturnNetDisAmount,
                SalesReturnNetAmount = xs.salesReturn.SalesReturnNetAmount,
                SalesReturnEnqDate = xs.salesReturn.SalesReturnEnqDate,
                SalesReturnShippinAddress = xs.salesReturn.SalesReturnShippinAddress,
                SalesReturnTermsAndCondition = xs.salesReturn.SalesReturnTermsAndCondition,
                SalesReturnRemarks2  = xs.salesReturn.SalesReturnRemarks2 ,


                SalesReturnNotes = xs.salesReturn.SalesReturnNotes,
                SalesReturnFooter = xs.salesReturn.SalesReturnFooter,
                SalesReturnPaymentTerms = xs.salesReturn.SalesReturnPaymentTerms,
                SalesReturnSubject = xs.salesReturn.SalesReturnSubject,
                SalesReturnContent = xs.salesReturn.SalesReturnContent,
                SalesReturnRemarks1 = xs.salesReturn.SalesReturnRemarks1,

                SalesReturnSvno = xs.salesReturn.SalesReturnSvno,
                SalesReturnSvdate = xs.salesReturn.SalesReturnSvdate,


                SalesReturnDelStatus = xs.salesReturn.SalesReturnDelStatus,
            };

            salesReturnViewModel.SalesReturnDetails = _mapper.Map<List<SalesReturnDetailsViewModel>>(xs.salesReturnDetails);
            salesReturnViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionViewModel>>(xs.accountsTransactions);

            ApiResponse<SalesReturnViewModel> apiResponse = new ApiResponse<SalesReturnViewModel>
            {
                Valid = true,
                Result = _mapper.Map<SalesReturnViewModel>(salesReturnViewModel),
                Message = SalesReturnMessage.UpdateVoucher
            };

            return apiResponse;

        }

        [HttpPost]
        [Route("DeleteSalesReturn")]
        public ApiResponse<int> DeleteSalesReturn([FromBody] SalesReturnViewModel voucherCompositeView)
        {
            var param1 = _mapper.Map<SalesReturn>(voucherCompositeView);
            var param2 = _mapper.Map<List<SalesReturnDetails>>(voucherCompositeView.SalesReturnDetails);
            var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
            //var param4 = _mapper.Map<List<StockRegister>>(voucherCompositeView.StockRegister);
            //var xs = _salesReturnService.DeleteSalesReturn(  param1,    param3, param2
            //    //, param4
            //    );

            //==============
            param3 = new List<AccountsTransactions>();
            List<StockRegister> param4 = new List<StockRegister>();
            //clsAccountAndStock.SalesReturn_Accounts_STOCK_Transactions("", "", param1, param2, ref param4, ref param3);

            var xs = _salesReturnService.DeleteSalesReturn(param1, param3, param2
           , param4
           );
            //========================


            ApiResponse<int> apiResponse = new ApiResponse<int>
            {
                Valid = true,
                Result = 0,
                Message = SalesReturnMessage.DeleteVoucher
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
                Result = _mapper.Map<List<AccountsTransactions>>(_salesReturnService.GetAllTransaction()),
                Message = ""
            };
            return apiResponse;

        }

        [HttpGet]
        [Route("GetSalesReturn")]
        public ApiResponse<List<SalesReturn>> GetAllSalesReturn()
        {


            ApiResponse<List<SalesReturn>> apiResponse = new ApiResponse<List<SalesReturn>>
            {
                Valid = true,
                Result = _mapper.Map<List<SalesReturn>>(_salesReturnService.GetSalesReturn()),
                Message = ""
            };
            return apiResponse;





        }

        [HttpGet]
        [Route("GetSavedSalesReturnDetails/{id}")]
        public ApiResponse<SalesReturnViewModel> GetSavedSalesReturnDetails(string id)
        {
            SalesReturnModel salesReturn = _salesReturnService.GetSavedSalesReturnDetails(id);

            if (salesReturn != null)
            {
                SalesReturnViewModel salesReturnViewModel = new SalesReturnViewModel
                {


                    SalesReturnId = salesReturn.salesReturn.SalesReturnId,
                    SalesReturnNo = salesReturn.salesReturn.SalesReturnNo,
                    SalesReturnDate = salesReturn.salesReturn.SalesReturnDate,
                    SalesReturnType = salesReturn.salesReturn.SalesReturnType,
                    SalesReturnPartyId = salesReturn.salesReturn.SalesReturnPartyId,
                    SalesReturnPartyName = salesReturn.salesReturn.SalesReturnPartyName,
                    SalesReturnPartyAddress = salesReturn.salesReturn.SalesReturnPartyAddress,
                    SalesReturnPartyVatNo = salesReturn.salesReturn.SalesReturnPartyVatNo,
                    SalesReturnContPerson = salesReturn.salesReturn.SalesReturnContPerson,
                    SalesReturnRefNo = salesReturn.salesReturn.SalesReturnRefNo,
                    SalesReturnDescription = salesReturn.salesReturn.SalesReturnDescription,
                    SalesReturnDlvNo = salesReturn.salesReturn.SalesReturnDlvNo,
                    SalesReturnDlvDate = salesReturn.salesReturn.SalesReturnDlvDate,
                    SalesReturnSono = salesReturn.salesReturn.SalesReturnSono,
                    SalesReturnSodate = salesReturn.salesReturn.SalesReturnSodate,
                    SalesReturnQtnNo = salesReturn.salesReturn.SalesReturnQtnNo,
                    SalesReturnQtnDate = salesReturn.salesReturn.SalesReturnQtnDate,
                    SalesReturnExcludeVat = salesReturn.salesReturn.SalesReturnExcludeVat,
                    SalesReturnSalesManId = salesReturn.salesReturn.SalesReturnSalesManId,
                    SalesReturnDptId = salesReturn.salesReturn.SalesReturnDptId,
                    SalesReturnEnqNo = salesReturn.salesReturn.SalesReturnEnqNo,
                    SalesReturnLocationId = salesReturn.salesReturn.SalesReturnLocationId,
                    SalesReturnUserId = salesReturn.salesReturn.SalesReturnUserId,
                    SalesReturnCurrencyId = salesReturn.salesReturn.SalesReturnCurrencyId,
                    SalesReturnCompanyId = salesReturn.salesReturn.SalesReturnCompanyId,
                    SalesReturnJobId = salesReturn.salesReturn.SalesReturnJobId,
                    SalesReturnFsno = salesReturn.salesReturn.SalesReturnFsno,
                    SalesReturnFcRate = salesReturn.salesReturn.SalesReturnFcRate,
                    SalesReturnStatus = salesReturn.salesReturn.SalesReturnStatus,
                    SalesReturnTotalGrossAmount = salesReturn.salesReturn.SalesReturnTotalGrossAmount,
                    SalesReturnTotalItemDisAmount = salesReturn.salesReturn.SalesReturnTotalItemDisAmount,
                    SalesReturnTotalActualAmount = salesReturn.salesReturn.SalesReturnTotalActualAmount,
                    SalesReturnTotalDisPer = salesReturn.salesReturn.SalesReturnTotalDisPer,
                    SalesReturnTotalDisAmount = salesReturn.salesReturn.SalesReturnTotalDisAmount,
                    SalesReturnVatAmt = salesReturn.salesReturn.SalesReturnVatAmt,
                    SalesReturnVatPer = salesReturn.salesReturn.SalesReturnVatPer,
                    SalesReturnVatRoundSign = salesReturn.salesReturn.SalesReturnVatRoundSign,
                    SalesReturnVatRountAmt = salesReturn.salesReturn.SalesReturnVatRountAmt,
                    SalesReturnNetDisAmount = salesReturn.salesReturn.SalesReturnNetDisAmount,
                    SalesReturnNetAmount = salesReturn.salesReturn.SalesReturnNetAmount,
                    SalesReturnEnqDate = salesReturn.salesReturn.SalesReturnEnqDate,
                    SalesReturnShippinAddress = salesReturn.salesReturn.SalesReturnShippinAddress,
                    SalesReturnTermsAndCondition = salesReturn.salesReturn.SalesReturnTermsAndCondition,
                    SalesReturnRemarks2  = salesReturn.salesReturn.SalesReturnRemarks2 ,






                    SalesReturnHeader = salesReturn.salesReturn.SalesReturnHeader,
                    SalesReturnDelivery = salesReturn.salesReturn.SalesReturnDelivery,

                  


        SalesReturnNotes = salesReturn.salesReturn.SalesReturnNotes,
                    SalesReturnFooter = salesReturn.salesReturn.SalesReturnFooter,
                    SalesReturnPaymentTerms = salesReturn.salesReturn.SalesReturnPaymentTerms,
                    SalesReturnSubject = salesReturn.salesReturn.SalesReturnSubject,
                    SalesReturnContent = salesReturn.salesReturn.SalesReturnContent,
                    SalesReturnRemarks1  = salesReturn.salesReturn.SalesReturnRemarks1 ,

                    SalesReturnSvno = salesReturn.salesReturn.SalesReturnSvno,
                    SalesReturnSvdate = salesReturn.salesReturn.SalesReturnSvdate,





                    SalesReturnDelStatus = salesReturn.salesReturn.SalesReturnDelStatus,









                };
                salesReturnViewModel.SalesReturnDetails = _mapper.Map<List<SalesReturnDetailsViewModel>>(salesReturn.salesReturnDetails);
                salesReturnViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionViewModel>>(salesReturn.accountsTransactions);
                ApiResponse<SalesReturnViewModel> apiResponse = new ApiResponse<SalesReturnViewModel>
                {
                    Valid = true,
                    Result = salesReturnViewModel,
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
                Message = SalesReturnMessage.VoucherNumberExist



            };
            var x = _salesReturnService.GetVouchersNumbers(id);
            if (x == null)
            {
                apiResponse.Result = false;
                apiResponse.Message = "";
            }

            return apiResponse;
        }



    }
}








