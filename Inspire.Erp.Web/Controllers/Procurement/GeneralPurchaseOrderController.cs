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
using Microsoft.AspNetCore.Authorization;
using Inspire.Erp.Domain.Models;

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
        public async Task<Common.ApiResponse<GeneralPurchaseOrderViewModel>> InsertGeneralPurchaseOrder([FromBody] GeneralPurchaseOrderViewModel voucherCompositeView)
        {

            Common.ApiResponse<GeneralPurchaseOrderViewModel> apiResponse = new Common.ApiResponse<GeneralPurchaseOrderViewModel>();
            if (_GeneralpurchaseOrderService.GetVouchersNumbers(voucherCompositeView.GeneralPurchaseOrderPono) == null)
            {
                var param1 = _mapper.Map<GeneralPurchaseOrder>(voucherCompositeView);
                var param2 = _mapper.Map<List<GeneralPurchaseOrderDetails>>(voucherCompositeView.GeneralPurchaseOrderDetails);
                var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
                var xs = await _GeneralpurchaseOrderService.InsertGeneralPurchaseOrder(param1, param3, param2

                    );
                if (!xs.Valid)
                {
                    return new Common.ApiResponse<GeneralPurchaseOrderViewModel>
                    {
                        Valid = true,
                        Result = voucherCompositeView,
                        Message = PurchaseOrderMessage.SaveVoucher
                    };
                }

                GeneralPurchaseOrderViewModel GeneralpurchaseOrderViewModel = new GeneralPurchaseOrderViewModel();
                GeneralpurchaseOrderViewModel = _mapper.Map<GeneralPurchaseOrderViewModel>(xs.Result.GeneralpurchaseOrder);

                GeneralpurchaseOrderViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionViewModel>>(xs.Result.accountsTransactions);
                apiResponse = new Common.ApiResponse<GeneralPurchaseOrderViewModel>
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
        public async Task<Common.ApiResponse<GeneralPurchaseOrderViewModel>> UpdateGeneralPurchaseOrder([FromBody] GeneralPurchaseOrderViewModel voucherCompositeView)
        {
            var param1 = _mapper.Map<GeneralPurchaseOrder>(voucherCompositeView);
            var param2 = _mapper.Map<List<GeneralPurchaseOrderDetails>>(voucherCompositeView.GeneralPurchaseOrderDetails);
            var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
            var xs = await _GeneralpurchaseOrderService.UpdateGeneralPurchaseOrder(param1, param3, param2

                );
            if (!xs.Valid)
            {
                return new Common.ApiResponse<GeneralPurchaseOrderViewModel>
                {
                    Valid = true,
                    Result = voucherCompositeView,
                    Message = PurchaseOrderMessage.SaveVoucher
                };
            }


            GeneralPurchaseOrderViewModel GeneralpurchaseOrderViewModel = new GeneralPurchaseOrderViewModel();
            GeneralpurchaseOrderViewModel = _mapper.Map<GeneralPurchaseOrderViewModel>(xs.Result.GeneralpurchaseOrder);

            GeneralpurchaseOrderViewModel.GeneralPurchaseOrderDetails = _mapper.Map<List<GeneralPurchaseOrderDetailsViewModel>>(xs.Result.GeneralpurchaseOrderDetails);
            GeneralpurchaseOrderViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionViewModel>>(xs.Result.accountsTransactions);

            Common.ApiResponse<GeneralPurchaseOrderViewModel> apiResponse = new Common.ApiResponse<GeneralPurchaseOrderViewModel>
            {
                Valid = true,
                Result = GeneralpurchaseOrderViewModel,
                Message = GeneralPurchaseOrderMessage.UpdateVoucher
            };
            return apiResponse;
        }

        [HttpPost]
        [Route("DeleteGeneralPurchaseOrder")]
        public Common.ApiResponse<int> DeleteGeneralPurchaseOrder([FromBody] GeneralPurchaseOrderViewModel voucherCompositeView)
        {
            var param1 = _mapper.Map<GeneralPurchaseOrder>(voucherCompositeView);
            var param2 = _mapper.Map<List<GeneralPurchaseOrderDetails>>(voucherCompositeView.GeneralPurchaseOrderDetails);
            var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
            var xs = _GeneralpurchaseOrderService.DeleteGeneralPurchaseOrder(param1, param3, param2
                );
            Common.ApiResponse<int> apiResponse = new Common.ApiResponse<int>
            {
                Valid = true,
                Result = 0,
                Message = GeneralPurchaseOrderMessage.DeleteVoucher
            };
            return apiResponse;
        }
        [HttpGet]
        [Route("GetAllAccountTransaction")]
        public Common.ApiResponse<List<AccountsTransactions>> GetAllAccountTransaction()
        {
            Common.ApiResponse<List<AccountsTransactions>> apiResponse = new Common.ApiResponse<List<AccountsTransactions>>
            {
                Valid = true,
                Result = _mapper.Map<List<AccountsTransactions>>(_GeneralpurchaseOrderService.GetAllTransaction()),
                Message = ""
            };
            return apiResponse;
        }
        [HttpGet]
        [Route("GetGeneralPurchaseOrder")]
        public Common.ApiResponse<List<GeneralPurchaseOrder>> GetAllGeneralPurchaseOrder()
        {
            Common.ApiResponse<List<GeneralPurchaseOrder>> apiResponse = new Common.ApiResponse<List<GeneralPurchaseOrder>>
            {
                Valid = true,
                Result = _GeneralpurchaseOrderService.GetGeneralPurchaseOrder().OrderBy(x => x.GeneralPurchaseOrderJobId).ToList(),
                Message = ""
            };
            return apiResponse;
        }
        [HttpGet]
        [Route("GetSavedGeneralPurchaseOrderDetails/{id}")]
        public async Task<Common.ApiResponse<GeneralPurchaseOrderViewModel>> GetSavedGeneralPurchaseOrderDetails(string id)
        {
            var xs = await _GeneralpurchaseOrderService.GetSavedGeneralPurchaseOrderDetails(id);
            if (xs != null)
            {
                GeneralPurchaseOrderViewModel GeneralpurchaseOrderViewModel = new GeneralPurchaseOrderViewModel();
                GeneralpurchaseOrderViewModel = _mapper.Map<GeneralPurchaseOrderViewModel>(xs.Result.GeneralpurchaseOrder);
                GeneralpurchaseOrderViewModel.GeneralPurchaseOrderDetails = _mapper.Map<List<GeneralPurchaseOrderDetailsViewModel>>(xs.Result.GeneralpurchaseOrderDetails);
                GeneralpurchaseOrderViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionViewModel>>(xs.Result.accountsTransactions);
                Common.ApiResponse<GeneralPurchaseOrderViewModel> apiResponse = new Common.ApiResponse<GeneralPurchaseOrderViewModel>
                {
                    Valid = true,
                    Result = GeneralpurchaseOrderViewModel,
                    Message = ""
                };
                return apiResponse;
            }
            return null;
        }
        [HttpGet]
        [Route("CheckVnoExist/{id}")]
        public Common.ApiResponse<bool> CheckVnoExist(string id)
        {
            Common.ApiResponse<bool> apiResponse = new Common.ApiResponse<bool>
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
