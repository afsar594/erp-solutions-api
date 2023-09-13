using Inspire.Erp.Application.Account.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Inspire.Erp.Application.Account.Interfaces;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Web.Common;


using Microsoft.AspNetCore.Mvc.Rendering;






namespace Inspire.Erp.Web.Controllers
{
    [Route("api/PurchaseJournal")]
    [Produces("application/json")]
    [ApiController]
    public class PurchaseJournalController : ControllerBase
    {
        private IPurchaseJournalService _purchaseJournalService;
        private readonly IMapper _mapper;
        public PurchaseJournalController(IPurchaseJournalService purchaseJournalService, IMapper mapper)
        {
            _purchaseJournalService = purchaseJournalService;
            _mapper = mapper;
        }
        //[HttpPost]
        //[Route("InsertPurchaseJournal")]
        //public PurchaseJournalViewModel InsertPurchaseJournal([FromBody] PurchaseJournalCompositeView voucherCompositeView)
        //{
        //    List <PurchaseJournalDetailsViewModel> purchaseJournalDetailsViewModels = new List<PurchaseJournalDetailsViewModel>();
        //    var param1 = _mapper.Map<PurchaseJournal>(voucherCompositeView.PurchaseJournal);
        //    var param2 = _mapper.Map<List<PurchaseJournalDetails>>(voucherCompositeView.PurchaseJournalDetails);    
        //    var xs = _purchaseJournalService.InsertPurchaseJournal(param1, param2);
        //    return _mapper.Map<PurchaseJournalViewModel>(xs);

        //}

        [HttpPost]
        [Route("InsertPurchaseJournal")]
        public ApiResponse<PurchaseJournalViewModel> InsertPurchaseJournal([FromBody] PurchaseJournalViewModel voucherCompositeView)
        {
            //var param1 = _mapper.Map<PurchaseJournal>(voucherCompositeView);
            //var param2 = _mapper.Map<List<PurchaseJournalDetails>>(voucherCompositeView.PurchaseJournalDetails);
            //var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
            //var xs = _purchaseJournalService.InsertPurchaseJournal(param1, param3, param2);
            //PurchaseJournalViewModel purchaseJournalViewModel = new PurchaseJournalViewModel
            //{
            //    PurchaseJournalVno = xs.purchaseJournal.PurchaseJournalVno,
            //    PurchaseJournalCrAmount = xs.purchaseJournal.PurchaseJournalCrAmount,
            //    PurchaseJournalFcCrAmount = xs.purchaseJournal.PurchaseJournalFcCrAmount,
            //    PurchaseJournalDrAmount = xs.purchaseJournal.PurchaseJournalDrAmount,
            //    PurchaseJournalFcDrAmount = xs.purchaseJournal.PurchaseJournalFcDrAmount,
            //    PurchaseJournalAcNo = xs.purchaseJournal.PurchaseJournalAcNo,
            //    PurchaseJournalDate = xs.purchaseJournal.PurchaseJournalDate,
            //    PurchaseJournalId = xs.purchaseJournal.PurchaseJournalId,
            //    PurchaseJournalNarration = xs.purchaseJournal.PurchaseJournalNarration,
            //    PurchaseJournalCurrencyId = xs.purchaseJournal.PurchaseJournalCurrencyId,

            //    PurchaseJournalRefNo = xs.purchaseJournal.PurchaseJournalRefNo 

            //};

            //purchaseJournalViewModel.PurchaseJournalDetails = _mapper.Map<List<PurchaseJournalDetailsViewModel>>(xs.purchaseJournalDetails);
            //purchaseJournalViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionsViewModel>>(xs.accountsTransactions);
            //return _mapper.Map<PurchaseJournalViewModel>(purchaseJournalViewModel);


            ApiResponse<PurchaseJournalViewModel> apiResponse = new ApiResponse<PurchaseJournalViewModel>();
            if (_purchaseJournalService.GetVouchersNumbers(voucherCompositeView.PurchaseJournalVno) == null)
            {
                var param1 = _mapper.Map<PurchaseJournal>(voucherCompositeView);
                var param2 = _mapper.Map<List<PurchaseJournalDetails>>(voucherCompositeView.PurchaseJournalDetails);
                var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
                var xs = _purchaseJournalService.InsertPurchaseJournal(param1, param3, param2);
                PurchaseJournalViewModel purchaseJournalViewModel = new PurchaseJournalViewModel
                {
                    PurchaseJournalVno = xs.purchaseJournal.PurchaseJournalVno,
                    PurchaseJournalCrAmount = xs.purchaseJournal.PurchaseJournalCrAmount,
                    PurchaseJournalFcCrAmount = xs.purchaseJournal.PurchaseJournalFcCrAmount,
                    PurchaseJournalDrAmount = xs.purchaseJournal.PurchaseJournalDrAmount,
                    PurchaseJournalFcDrAmount = xs.purchaseJournal.PurchaseJournalFcDrAmount,
                    //PurchaseJournalAcNo = xs.purchaseJournal.PurchaseJournalAcNo,
                    PurchaseJournalDate = xs.purchaseJournal.PurchaseJournalDate,
                    PurchaseJournalId = xs.purchaseJournal.PurchaseJournalId,
                    PurchaseJournalNarration = xs.purchaseJournal.PurchaseJournalNarration,
                    PurchaseJournalCurrencyId = xs.purchaseJournal.PurchaseJournalCurrencyId,

                    PurchaseJournalRefNo = xs.purchaseJournal.PurchaseJournalRefNo
                };

                purchaseJournalViewModel.PurchaseJournalDetails = _mapper.Map<List<PurchaseJournalDetailsViewModel>>(xs.purchaseJournalDetails);
                purchaseJournalViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionViewModel>>(xs.accountsTransactions);
                apiResponse = new ApiResponse<PurchaseJournalViewModel>
                {
                    Valid = true,
                    Result = _mapper.Map<PurchaseJournalViewModel>(purchaseJournalViewModel),
                    Message = PurchaseJournalMessage.SaveVoucher
                };
            }
            else
            {
                apiResponse = new ApiResponse<PurchaseJournalViewModel>
                {
                    Valid = false,
                    Error = true,
                    Exception = null,
                    Message = PurchaseJournalMessage.VoucherAlreadyExist
                };

            }


            return apiResponse;






        }

        [HttpPost]
        [Route("UpdatePurchaseJournal")]
        public ApiResponse<PurchaseJournalViewModel> UpdatePurchaseJournal([FromBody] PurchaseJournalViewModel voucherCompositeView)
        {
            var param1 = _mapper.Map<PurchaseJournal>(voucherCompositeView);
            var param2 = _mapper.Map<List<PurchaseJournalDetails>>(voucherCompositeView.PurchaseJournalDetails);
            var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
            var xs = _purchaseJournalService.UpdatePurchaseJournal(param1, param3, param2);

            PurchaseJournalViewModel purchaseJournalViewModel = new PurchaseJournalViewModel
            {
                PurchaseJournalVno = xs.purchaseJournal.PurchaseJournalVno,
                PurchaseJournalCrAmount = xs.purchaseJournal.PurchaseJournalCrAmount,
                //PurchaseJournalAcNo = xs.purchaseJournal.PurchaseJournalAcNo,
                PurchaseJournalDate = xs.purchaseJournal.PurchaseJournalDate,
                PurchaseJournalId = xs.purchaseJournal.PurchaseJournalId,
                PurchaseJournalNarration = xs.purchaseJournal.PurchaseJournalNarration,
                PurchaseJournalCurrencyId = xs.purchaseJournal.PurchaseJournalCurrencyId,
                PurchaseJournalRefNo = xs.purchaseJournal.PurchaseJournalRefNo,
                PurchaseJournalDrAmount = xs.purchaseJournal.PurchaseJournalDrAmount
            };

            purchaseJournalViewModel.PurchaseJournalDetails = _mapper.Map<List<PurchaseJournalDetailsViewModel>>(xs.purchaseJournalDetails);
            purchaseJournalViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionViewModel>>(xs.accountsTransactions);

            ApiResponse<PurchaseJournalViewModel> apiResponse = new ApiResponse<PurchaseJournalViewModel>
            {
                Valid = true,
                Result = _mapper.Map<PurchaseJournalViewModel>(purchaseJournalViewModel),
                Message = PurchaseJournalMessage.UpdateVoucher
            };

            return apiResponse;

        }

        [HttpPost]
        [Route("DeletePurchaseJournal")]
        public ApiResponse<int> DeletePurchaseJournal([FromBody] PurchaseJournalViewModel voucherCompositeView)
        {
            var param1 = _mapper.Map<PurchaseJournal>(voucherCompositeView);
            var param2 = _mapper.Map<List<PurchaseJournalDetails>>(voucherCompositeView.PurchaseJournalDetails);
            var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
            var xs = _purchaseJournalService.DeletePurchaseJournal(param1, param3, param2);
            ApiResponse<int> apiResponse = new ApiResponse<int>
            {
                Valid = true,
                Result = 0,
                Message = PurchaseJournalMessage.DeleteVoucher
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
                Result = _mapper.Map<List<AccountsTransactions>>(_purchaseJournalService.GetAllTransaction()),
                Message = ""
            };
            return apiResponse;




        }

        [HttpGet]
        [Route("GetPurchaseJournal")]
        public ApiResponse<List<PurchaseJournal>> GetAllPurchaseJournal()
        {


            ApiResponse<List<PurchaseJournal>> apiResponse = new ApiResponse<List<PurchaseJournal>>
            {
                Valid = true,
                Result = _mapper.Map<List<PurchaseJournal>>(_purchaseJournalService.GetPurchaseJournal()),
                Message = ""
            };
            return apiResponse;





        }

        [HttpGet]
        [Route("GetSavedPurchaseJournalDetails/{id}")]
        public ApiResponse<PurchaseJournalViewModel> GetSavedPurchaseJournalDetails(string id)
        {
            PurchaseJournalModel purchaseJournal = _purchaseJournalService.GetSavedPurchaseJournalDetails(id);

            if (purchaseJournal != null)
            {
                PurchaseJournalViewModel purchaseJournalViewModel = new PurchaseJournalViewModel
                {
                    PurchaseJournalVno = purchaseJournal.purchaseJournal.PurchaseJournalVno,
                    PurchaseJournalCrAmount = purchaseJournal.purchaseJournal.PurchaseJournalCrAmount,
                    //PurchaseJournalAcNo = purchaseJournal.purchaseJournal.PurchaseJournalAcNo,
                    PurchaseJournalDate = purchaseJournal.purchaseJournal.PurchaseJournalDate,
                    PurchaseJournalId = purchaseJournal.purchaseJournal.PurchaseJournalId,
                    PurchaseJournalNarration = purchaseJournal.purchaseJournal.PurchaseJournalNarration,
                    PurchaseJournalCurrencyId = purchaseJournal.purchaseJournal.PurchaseJournalCurrencyId,
                    PurchaseJournalRefNo = purchaseJournal.purchaseJournal.PurchaseJournalRefNo,
                    PurchaseJournalDrAmount = purchaseJournal.purchaseJournal.PurchaseJournalDrAmount
                };
                purchaseJournalViewModel.PurchaseJournalDetails = _mapper.Map<List<PurchaseJournalDetailsViewModel>>(purchaseJournal.purchaseJournalDetails);
                purchaseJournalViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionViewModel>>(purchaseJournal.accountsTransactions);
                ApiResponse<PurchaseJournalViewModel> apiResponse = new ApiResponse<PurchaseJournalViewModel>
                {
                    Valid = true,
                    Result = purchaseJournalViewModel,
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
                Message = PurchaseJournalMessage.VoucherNumberExist
            };
            var x = _purchaseJournalService.GetVouchersNumbers(id);
            if (x == null)
            {
                apiResponse.Result = false;
                apiResponse.Message = "";
            }

            return apiResponse;
        }



    }
}



//using Inspire.Erp.Application.Account.Interfaces;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using AutoMapper;
//using Inspire.Erp.Application.Account.Interfaces;
//using Inspire.Erp.Application.Common;
//using Inspire.Erp.Domain.Entities;
//using Inspire.Erp.Web.ViewModels;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace Inspire.Erp.Web.Controllers
//{
//    [Route("api/PurchaseJournal")]
//    [Produces("application/json")]
//    [ApiController]
//    public class PurchaseJournalController : ControllerBase
//    {
//        private IPurchaseJournalService _purchaseJournalService;
//        private readonly IMapper _mapper;
//        public PurchaseJournalController(IPurchaseJournalService purchaseJournalService, IMapper mapper)
//        {
//            _purchaseJournalService = purchaseJournalService;
//            _mapper = mapper;
//        }
//        //[HttpPost]
//        //[Route("InsertPurchaseJournal")]
//        //public PurchaseJournalViewModel InsertPurchaseJournal([FromBody] PurchaseJournalCompositeView voucherCompositeView)
//        //{
//        //    List <PurchaseJournalDetailsViewModel> purchaseJournalDetailsViewModels = new List<PurchaseJournalDetailsViewModel>();
//        //    var param1 = _mapper.Map<PurchaseJournal>(voucherCompositeView.PurchaseJournal);
//        //    var param2 = _mapper.Map<List<PurchaseJournalDetails>>(voucherCompositeView.PurchaseJournalDetails);    
//        //    var xs = _purchaseJournalService.InsertPurchaseJournal(param1, param2);
//        //    return _mapper.Map<PurchaseJournalViewModel>(xs);

//        //}

//        [HttpPost]
//        [Route("InsertPurchaseJournal")]
//        public PurchaseJournalViewModel InsertPurchaseJournal([FromBody] PurchaseJournalViewModel voucherCompositeView)
//        {
//            var param1 = _mapper.Map<PurchaseJournal>(voucherCompositeView);
//            var param2 = _mapper.Map<List<PurchaseJournalDetails>>(voucherCompositeView.PurchaseJournalDetails);
//            var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
//            var xs = _purchaseJournalService.InsertPurchaseJournal(param1, param3, param2);
//            PurchaseJournalViewModel purchaseJournalViewModel = new PurchaseJournalViewModel
//            {
//                PurchaseJournalVno = xs.purchaseJournal.PurchaseJournalVno,
//                PurchaseJournalCrAmount = xs.purchaseJournal.PurchaseJournalCrAmount,
//                //PurchaseJournalAcNo = xs.purchaseJournal.PurchaseJournalAcNo,
//                PurchaseJournalDate = xs.purchaseJournal.PurchaseJournalDate,
//                PurchaseJournalId = xs.purchaseJournal.PurchaseJournalId,
//                PurchaseJournalNarration = xs.purchaseJournal.PurchaseJournalNarration,
//                PurchaseJournalCurrencyId = xs.purchaseJournal.PurchaseJournalCurrencyId,
//                PurchaseJournalRefNo = xs.purchaseJournal.PurchaseJournalRefNo,
//                PurchaseJournalDrAmount = xs.purchaseJournal.PurchaseJournalDrAmount
//            };

//            purchaseJournalViewModel.PurchaseJournalDetails = _mapper.Map<List<PurchaseJournalDetailsViewModel>>(xs.purchaseJournalDetails);
//            purchaseJournalViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionsViewModel>>(xs.accountsTransactions);
//            return _mapper.Map<PurchaseJournalViewModel>(purchaseJournalViewModel);

//        }

//        [HttpPost]
//        [Route("UpdatePurchaseJournal")]
//        public PurchaseJournalViewModel UpdatePurchaseJournal([FromBody] PurchaseJournalViewModel voucherCompositeView)
//        {
//            var param1 = _mapper.Map<PurchaseJournal>(voucherCompositeView);
//            var param2 = _mapper.Map<List<PurchaseJournalDetails>>(voucherCompositeView.PurchaseJournalDetails);
//            var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
//            var xs = _purchaseJournalService.UpdatePurchaseJournal(param1, param3, param2);

//            PurchaseJournalViewModel purchaseJournalViewModel = new PurchaseJournalViewModel
//            {
//                PurchaseJournalVno = xs.purchaseJournal.PurchaseJournalVno,
//                PurchaseJournalCrAmount = xs.purchaseJournal.PurchaseJournalCrAmount,
//                //PurchaseJournalAcNo = xs.purchaseJournal.PurchaseJournalAcNo,
//                PurchaseJournalDate = xs.purchaseJournal.PurchaseJournalDate,
//                PurchaseJournalId = xs.purchaseJournal.PurchaseJournalId,
//                PurchaseJournalNarration = xs.purchaseJournal.PurchaseJournalNarration,
//                PurchaseJournalCurrencyId = xs.purchaseJournal.PurchaseJournalCurrencyId,
//                PurchaseJournalRefNo = xs.purchaseJournal.PurchaseJournalRefNo,
//                PurchaseJournalDrAmount = xs.purchaseJournal.PurchaseJournalDrAmount
//            };

//            purchaseJournalViewModel.PurchaseJournalDetails = _mapper.Map<List<PurchaseJournalDetailsViewModel>>(xs.purchaseJournalDetails);
//            purchaseJournalViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionsViewModel>>(xs.accountsTransactions);

//            return _mapper.Map<PurchaseJournalViewModel>(purchaseJournalViewModel);

//        }

//        [HttpPost]
//        [Route("DeletePurchaseJournal")]
//        public int DeletePurchaseJournal([FromBody] PurchaseJournalViewModel voucherCompositeView)
//        {
//            var param1 = _mapper.Map<PurchaseJournal>(voucherCompositeView);
//            var param2 = _mapper.Map<List<PurchaseJournalDetails>>(voucherCompositeView.PurchaseJournalDetails);
//            var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
//            var xs = _purchaseJournalService.DeletePurchaseJournal(param1, param3, param2);
//            return xs;

//        }

//        [HttpGet]
//        [Route("GetAllAccountTransaction")]
//        public List<AccountsTransactions> GetAllAccountTransaction()
//        {
//            return _mapper.Map<List<AccountsTransactions>>(_purchaseJournalService.GetAllTransaction());
//        }

//        [HttpGet]
//        [Route("GetPurchaseJournal")]
//        public List<PurchaseJournal> GetAllPurchaseJournal()
//        {
//            return _mapper.Map<List<PurchaseJournal>>(_purchaseJournalService.GetPurchaseJournal());
//        }

//        [HttpGet]
//        [Route("GetSavedPurchaseJournalDetails/{id}")]
//        public PurchaseJournalViewModel GetSavedPurchaseJournalDetails(string id)
//        {
//            PurchaseJournalModel purchaseJournal = _purchaseJournalService.GetSavedPurchaseJournalDetails(id);
//            PurchaseJournalViewModel purchaseJournalViewModel = new PurchaseJournalViewModel
//            {
//                PurchaseJournalVno = purchaseJournal.purchaseJournal.PurchaseJournalVno,
//                PurchaseJournalCrAmount = purchaseJournal.purchaseJournal.PurchaseJournalCrAmount,
//                //PurchaseJournalAcNo = purchaseJournal.purchaseJournal.PurchaseJournalAcNo,
//                PurchaseJournalDate = purchaseJournal.purchaseJournal.PurchaseJournalDate,
//                PurchaseJournalId = purchaseJournal.purchaseJournal.PurchaseJournalId,
//                PurchaseJournalNarration = purchaseJournal.purchaseJournal.PurchaseJournalNarration,
//                PurchaseJournalCurrencyId = purchaseJournal.purchaseJournal.PurchaseJournalCurrencyId,
//                PurchaseJournalRefNo = purchaseJournal.purchaseJournal.PurchaseJournalRefNo,
//                PurchaseJournalDrAmount = purchaseJournal.purchaseJournal.PurchaseJournalDrAmount

//            };
//            purchaseJournalViewModel.PurchaseJournalDetails = _mapper.Map<List<PurchaseJournalDetailsViewModel>>(purchaseJournal.purchaseJournalDetails);
//            purchaseJournalViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionsViewModel>>(purchaseJournal.accountsTransactions);
//            return purchaseJournalViewModel;
//        }
//    }
//}
