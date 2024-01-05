using Inspire.Erp.Application.Account.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Inspire.Erp.Application.Sales.Interfaces;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Web.ViewModels;
using Inspire.Erp.Web.ViewModels.Sales;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Web.Common;
using Inspire.Erp.Web.MODULE;

using Microsoft.AspNetCore.Mvc.Rendering;


namespace Inspire.Erp.Web.Controllers.Sales
{
    [Route("api/DeliveryNote")]
    [Produces("application/json")]
    [ApiController]
    public class DeliveryNoteController : ControllerBase
    {
        private IDeliveryNoteServices _deliveryNoteService;
        private readonly IMapper _mapper;
        public DeliveryNoteController(IDeliveryNoteServices deliveryNoteService, IMapper mapper)
        {
            _deliveryNoteService = deliveryNoteService;
            _mapper = mapper;
        }
       
        [HttpPost]
        [Route("InsertDeliveryNote")]
        public ApiResponse<CustomerDeliveryNoteViewModel> InsertSalesVoucher([FromBody] CustomerDeliveryNoteViewModel voucherCompositeView)
        {


            ApiResponse<CustomerDeliveryNoteViewModel> apiResponse = new ApiResponse<CustomerDeliveryNoteViewModel>();
           // if (_deliveryNoteService.GetVouchersNumbers(voucherCompositeView.CustomerDeliveryNoteDeliveryId) == null)
            {
                var param1 = _mapper.Map<CustomerDeliveryNote>(voucherCompositeView);
                var param2 = _mapper.Map<List<CustomerDeliveryNoteDetails>>(voucherCompositeView.CustomerDeliveryNoteDetails);
                ////var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
                //var param4 = _mapper.Map<List<StockRegister>>(voucherCompositeView.StockRegister);
                //var xs = _deliveryNoteService.InsertSalesVoucher(param1, param3, param2
                //    //, param4
                //    );

                //==============
               // param3 = new List<AccountsTransactions>();
               //// List<StockRegister> param4 = new List<StockRegister>();
                ////clsAccountAndStock.SalesVoucher_Accounts_STOCK_Transactions("", "", param1, param2, ref param4);

                var xs = _deliveryNoteService.InsertDeliveryNote(param1, param2);
                //========================


                CustomerDeliveryNoteViewModel deliveryNoteViewModel = new CustomerDeliveryNoteViewModel
                {

                    CustomerDeliveryNoteDeliveryId = xs.deliveryNote.CustomerDeliveryNoteDeliveryId,
                    CustomerDeliveryNoteDeliveryDate = xs.deliveryNote.CustomerDeliveryNoteDeliveryDate,
                    CustomerDeliveryNoteCpoId = xs.deliveryNote.CustomerDeliveryNoteCpoId,
                    CustomerDeliveryNoteCpoDate = xs.deliveryNote.CustomerDeliveryNoteCpoDate,
                    CustomerDeliveryNoteLocationId = xs.deliveryNote.CustomerDeliveryNoteLocationId,
                    CustomerDeliveryNoteCustomerCode = xs.deliveryNote.CustomerDeliveryNoteCustomerCode,
                    CustomerDeliveryNoteCustomerName = xs.deliveryNote.CustomerDeliveryNoteCustomerName,
                    CustomerDeliveryNoteSalesManId = xs.deliveryNote.CustomerDeliveryNoteSalesManId,
                    CustomerDeliveryNoteCurrencyId = xs.deliveryNote.CustomerDeliveryNoteCurrencyId,
                    CustomerDeliveryNoteDeliveryAddress = xs.deliveryNote.CustomerDeliveryNoteDeliveryAddress,
                    CustomerDeliveryNoteRemarks = xs.deliveryNote.CustomerDeliveryNoteRemarks,
                    CustomerDeliveryNoteFsno = xs.deliveryNote.CustomerDeliveryNoteFsno,
                    CustomerDeliveryNoteUserId = xs.deliveryNote.CustomerDeliveryNoteUserId,
                    CustomerDeliveryNoteNote = xs.deliveryNote.CustomerDeliveryNoteNote,
                    CustomerDeliveryNoteWarranty = xs.deliveryNote.CustomerDeliveryNoteWarranty,
                    CustomerDeliveryNoteTraining = xs.deliveryNote.CustomerDeliveryNoteTraining,
                    CustomerDeliveryNoteTechnicalDetails = xs.deliveryNote.CustomerDeliveryNoteTechnicalDetails,
                    CustomerDeliveryNoteTerms = xs.deliveryNote.CustomerDeliveryNoteTerms,
                    CustomerDeliveryNotePacking = xs.deliveryNote.CustomerDeliveryNotePacking,
                    CustomerDeliveryNoteQuality = xs.deliveryNote.CustomerDeliveryNoteQuality,
                    CustomerDeliveryNoteIssuedStatus = xs.deliveryNote.CustomerDeliveryNoteIssuedStatus,
                    CustomerDeliveryNoteAttention = xs.deliveryNote.CustomerDeliveryNoteAttention,
                    CustomerDeliveryNotePodId = xs.deliveryNote.CustomerDeliveryNotePodId,
                    CustomerDeliveryNoteDelDetId = xs.deliveryNote.CustomerDeliveryNoteDelDetId,
                    CustomerDeliveryNoteManuelPo = xs.deliveryNote.CustomerDeliveryNoteManuelPo,
                    CustomerDeliveryNoteDeliveryStatus = xs.deliveryNote.CustomerDeliveryNoteDeliveryStatus,
                    CustomerDeliveryNoteDelStatus = xs.deliveryNote.CustomerDeliveryNoteDelStatus,

                };

                deliveryNoteViewModel.CustomerDeliveryNoteDetails = _mapper.Map<List<CustomerDeliveryNoteDetailsViewModel>>(xs.deliveryNoteDetails);
                //deliveryNoteViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionsViewModel>>(xs.accountsTransactions);
               //// apiResponse = new ApiResponse<CustomerDeliveryNoteViewModel>
                ////{
                ////    Valid = true,
                ////    Result = _mapper.Map<CustomerDeliveryNoteViewModel>(deliveryNoteViewModel),
                ////    Message = SalesVoucherMessage.SaveVoucher
                ////};
            }
            ////else
            ////{
            ////    apiResponse = new ApiResponse<CustomerDeliveryNoteViewModel>
            ////    {
            ////        Valid = false,
            ////        Error = true,
            ////        Exception = null,
            ////        Message = SalesVoucherMessage.VoucherAlreadyExist
            ////    };

            ////}


            return apiResponse;


        }

        [HttpPost]
        [Route("UpdateDeliveryNote")]
        public ApiResponse<CustomerDeliveryNoteViewModel> UpdateSalesVoucher([FromBody] CustomerDeliveryNoteViewModel voucherCompositeView)
        {
            var param1 = _mapper.Map<CustomerDeliveryNote>(voucherCompositeView);
            var param2 = _mapper.Map<List<CustomerDeliveryNoteDetails>>(voucherCompositeView.CustomerDeliveryNoteDetails);
           // var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
            //var param4 = _mapper.Map<List<StockRegister>>(voucherCompositeView.StockRegister);
            //var xs = _deliveryNoteService.UpdateSalesVoucher(param1, param3, param2
            //    //, param4
            //    );

            //==============
           // param3 = new List<AccountsTransactions>();
            ////List<StockRegister> param4 = new List<StockRegister>();
            ////clsAccountAndStock.SalesVoucher_Accounts_STOCK_Transactions("", "", param1, param2, ref param4, ref param3);

            var xs = _deliveryNoteService.UpdateDeliveryNote(param1, param2);
            //========================


            CustomerDeliveryNoteViewModel deliveryNoteViewModel = new CustomerDeliveryNoteViewModel
            {
                CustomerDeliveryNoteDeliveryId = xs.deliveryNote.CustomerDeliveryNoteDeliveryId,
                CustomerDeliveryNoteDeliveryDate = xs.deliveryNote.CustomerDeliveryNoteDeliveryDate,
                CustomerDeliveryNoteCpoId = xs.deliveryNote.CustomerDeliveryNoteCpoId,
                CustomerDeliveryNoteCpoDate = xs.deliveryNote.CustomerDeliveryNoteCpoDate,
                CustomerDeliveryNoteLocationId = xs.deliveryNote.CustomerDeliveryNoteLocationId,
                CustomerDeliveryNoteCustomerCode = xs.deliveryNote.CustomerDeliveryNoteCustomerCode,
                CustomerDeliveryNoteCustomerName = xs.deliveryNote.CustomerDeliveryNoteCustomerName,
                CustomerDeliveryNoteSalesManId = xs.deliveryNote.CustomerDeliveryNoteSalesManId,
                CustomerDeliveryNoteCurrencyId = xs.deliveryNote.CustomerDeliveryNoteCurrencyId,
                CustomerDeliveryNoteDeliveryAddress = xs.deliveryNote.CustomerDeliveryNoteDeliveryAddress,
                CustomerDeliveryNoteRemarks = xs.deliveryNote.CustomerDeliveryNoteRemarks,
                CustomerDeliveryNoteFsno = xs.deliveryNote.CustomerDeliveryNoteFsno,
                CustomerDeliveryNoteUserId = xs.deliveryNote.CustomerDeliveryNoteUserId,
                CustomerDeliveryNoteNote = xs.deliveryNote.CustomerDeliveryNoteNote,
                CustomerDeliveryNoteWarranty = xs.deliveryNote.CustomerDeliveryNoteWarranty,
                CustomerDeliveryNoteTraining = xs.deliveryNote.CustomerDeliveryNoteTraining,
                CustomerDeliveryNoteTechnicalDetails = xs.deliveryNote.CustomerDeliveryNoteTechnicalDetails,
                CustomerDeliveryNoteTerms = xs.deliveryNote.CustomerDeliveryNoteTerms,
                CustomerDeliveryNotePacking = xs.deliveryNote.CustomerDeliveryNotePacking,
                CustomerDeliveryNoteQuality = xs.deliveryNote.CustomerDeliveryNoteQuality,
                CustomerDeliveryNoteIssuedStatus = xs.deliveryNote.CustomerDeliveryNoteIssuedStatus,
                CustomerDeliveryNoteAttention = xs.deliveryNote.CustomerDeliveryNoteAttention,
                CustomerDeliveryNotePodId = xs.deliveryNote.CustomerDeliveryNotePodId,
                CustomerDeliveryNoteDelDetId = xs.deliveryNote.CustomerDeliveryNoteDelDetId,
                CustomerDeliveryNoteManuelPo = xs.deliveryNote.CustomerDeliveryNoteManuelPo,
                CustomerDeliveryNoteDeliveryStatus = xs.deliveryNote.CustomerDeliveryNoteDeliveryStatus,
                CustomerDeliveryNoteDelStatus = xs.deliveryNote.CustomerDeliveryNoteDelStatus,
            };

            deliveryNoteViewModel.CustomerDeliveryNoteDetails = _mapper.Map<List<CustomerDeliveryNoteDetailsViewModel>>(xs.deliveryNoteDetails);
            //salesVoucherViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionsViewModel>>(xs.accountsTransactions);

            ApiResponse<CustomerDeliveryNoteViewModel> apiResponse = new ApiResponse<CustomerDeliveryNoteViewModel>
            {
                Valid = true,
                Result = _mapper.Map<CustomerDeliveryNoteViewModel>(deliveryNoteViewModel),
                Message = SalesVoucherMessage.UpdateVoucher
            };

            return apiResponse;

        }

        [HttpPost]
        [Route("DeleteDeliveryNote")]
        public ApiResponse<int> DeleteDeliveryNote([FromBody] CustomerDeliveryNoteViewModel voucherCompositeView)
        {
            var param1 = _mapper.Map<CustomerDeliveryNote>(voucherCompositeView);
            var param2 = _mapper.Map<List<CustomerDeliveryNoteDetails>>(voucherCompositeView.CustomerDeliveryNoteDetails);
           // var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
            //var param4 = _mapper.Map<List<StockRegister>>(voucherCompositeView.StockRegister);
            //var xs = _deliveryNoteService.DeleteSalesVoucher(  param1,    param3, param2
            //    //, param4
            //    );

            //==============
           /// param3 = new List<AccountsTransactions>();
            ////List<StockRegister> param4 = new List<StockRegister>();
            //clsAccountAndStock.SalesVoucher_Accounts_STOCK_Transactions("", "", param1, param2, ref param4, ref param3);

            var xs = _deliveryNoteService.DeleteDeliveryNote(param1, param2);
            //========================


            ApiResponse<int> apiResponse = new ApiResponse<int>
            {
                Valid = true,
                Result = 0,
                Message = SalesVoucherMessage.DeleteVoucher
            };

            return apiResponse;

        }


        [HttpGet]
        [Route("GetDeliveryNote")]
        public ApiResponse<List<CustomerDeliveryNote>> GetAllDeliveryNote()
        {
            ApiResponse<List<CustomerDeliveryNote>> apiResponse = new ApiResponse<List<CustomerDeliveryNote>>
            {
                Valid = true,
                Result = _mapper.Map<List<CustomerDeliveryNote>>(_deliveryNoteService.GetDeliveryNote()),
                Message = ""
            };
            return apiResponse;
        }

        [HttpGet]
        [Route("GetSavedDeliveryNoteDetails/{id}")]
        public ApiResponse<CustomerDeliveryNoteViewModel> GetSavedDeliveryNoteDetails(long id)
        {
            DeliveryNoteModel deliveryNote = _deliveryNoteService.GetSavedDeliveryNoteDetails(id);

            if (deliveryNote != null)
            {
                CustomerDeliveryNoteViewModel deliveryNoteViewModel = new CustomerDeliveryNoteViewModel
                {

                    CustomerDeliveryNoteDeliveryId = deliveryNote.deliveryNote.CustomerDeliveryNoteDeliveryId,

                    CustomerDeliveryNoteDeliveryDate = deliveryNote.deliveryNote.CustomerDeliveryNoteDeliveryDate,
                    CustomerDeliveryNoteCpoId = deliveryNote.deliveryNote.CustomerDeliveryNoteCpoId,
                    CustomerDeliveryNoteCpoDate = deliveryNote.deliveryNote.CustomerDeliveryNoteCpoDate,
                    CustomerDeliveryNoteLocationId = deliveryNote.deliveryNote.CustomerDeliveryNoteLocationId,
                    CustomerDeliveryNoteCustomerCode = deliveryNote.deliveryNote.CustomerDeliveryNoteCustomerCode,
                    CustomerDeliveryNoteCustomerName = deliveryNote.deliveryNote.CustomerDeliveryNoteCustomerName,
                    CustomerDeliveryNoteSalesManId = deliveryNote.deliveryNote.CustomerDeliveryNoteSalesManId,
                    CustomerDeliveryNoteCurrencyId = deliveryNote.deliveryNote.CustomerDeliveryNoteCurrencyId,
                    CustomerDeliveryNoteDeliveryAddress = deliveryNote.deliveryNote.CustomerDeliveryNoteDeliveryAddress,
                    CustomerDeliveryNoteRemarks = deliveryNote.deliveryNote.CustomerDeliveryNoteRemarks,
                    CustomerDeliveryNoteFsno = deliveryNote.deliveryNote.CustomerDeliveryNoteFsno,
                    CustomerDeliveryNoteUserId = deliveryNote.deliveryNote.CustomerDeliveryNoteUserId,
                    CustomerDeliveryNoteNote = deliveryNote.deliveryNote.CustomerDeliveryNoteNote,
                    CustomerDeliveryNoteWarranty = deliveryNote.deliveryNote.CustomerDeliveryNoteWarranty,
                    CustomerDeliveryNoteTraining = deliveryNote.deliveryNote.CustomerDeliveryNoteTraining,
                    CustomerDeliveryNoteTechnicalDetails = deliveryNote.deliveryNote.CustomerDeliveryNoteTechnicalDetails,
                    CustomerDeliveryNoteTerms = deliveryNote.deliveryNote.CustomerDeliveryNoteTerms,
                    CustomerDeliveryNotePacking = deliveryNote.deliveryNote.CustomerDeliveryNotePacking,
                    CustomerDeliveryNoteQuality = deliveryNote.deliveryNote.CustomerDeliveryNoteQuality,
                    CustomerDeliveryNoteIssuedStatus = deliveryNote.deliveryNote.CustomerDeliveryNoteIssuedStatus,
                    CustomerDeliveryNoteAttention = deliveryNote.deliveryNote.CustomerDeliveryNoteAttention,
                    CustomerDeliveryNotePodId = deliveryNote.deliveryNote.CustomerDeliveryNotePodId,
                    CustomerDeliveryNoteDelDetId = deliveryNote.deliveryNote.CustomerDeliveryNoteDelDetId,
                    CustomerDeliveryNoteManuelPo = deliveryNote.deliveryNote.CustomerDeliveryNoteManuelPo,
                    CustomerDeliveryNoteDeliveryStatus = deliveryNote.deliveryNote.CustomerDeliveryNoteDeliveryStatus,
                    CustomerDeliveryNoteDelStatus = deliveryNote.deliveryNote.CustomerDeliveryNoteDelStatus,

                };
                deliveryNoteViewModel.CustomerDeliveryNoteDetails = _mapper.Map<List<CustomerDeliveryNoteDetailsViewModel>>(deliveryNote.deliveryNoteDetails);
                ApiResponse<CustomerDeliveryNoteViewModel> apiResponse = new ApiResponse<CustomerDeliveryNoteViewModel>
                {
                    Valid = true,
                    Result = deliveryNoteViewModel,
                    Message = ""
                };
                return apiResponse;
            }
            return null;



        }

        [HttpGet]
        [Route("CheckVnoExist/{id}")]
        public ApiResponse<bool> CheckVnoExist(long id)
        {
            ApiResponse<bool> apiResponse = new ApiResponse<bool>
            {
                Valid = true,
                Result = true,
                Message = SalesVoucherMessage.VoucherNumberExist



            };
            // var x = _deliveryNoteService.GetVouchersNumbers(id);
            var x = 0;
            if (x == null)
            {
                apiResponse.Result = false;
                apiResponse.Message = "";
            }

            return apiResponse;
        }



    }
}








