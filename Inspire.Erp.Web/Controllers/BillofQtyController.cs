using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Inspire.Erp.Application.Sales.Interfaces;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Web.Common;
using Inspire.Erp.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Inspire.Erp.Web.ViewModels.Accounts;
using Inspire.Erp.Domain.Modals.Sales;
using Inspire.Erp.Web.ViewModels.Procurement;
using Inspire.Erp.Application.Store.Implementation;
using Inspire.Erp.Domain.Modals.Stock;
using Inspire.Erp.Application.Account;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Infrastructure.Database.Repositoy;

namespace Inspire.Erp.Web.Controllers
{
    [Route("api")]
    [Produces("application/json")]
    [ApiController]
    public class BillofQtyController : ControllerBase
    {
        private IBillofQtyService _customerQuotationService;
        private readonly IMapper _mapper;
        public BillofQtyController(IBillofQtyService customerQuotationService, IMapper mapper)
        {
            _customerQuotationService = customerQuotationService;
            _mapper = mapper;
        }
        [HttpPost]
        [Route("InsertBillofQty")]
        public ApiResponse<BillofQtyMasterModel> InsertBillofQty([FromBody] BillofQtyMasterModel customerQuotationCompositeView)
        {
            ApiResponse<BillofQtyMasterModel> apiResponse = new ApiResponse<BillofQtyMasterModel>();

            try
            {
                var param1 = _mapper.Map<BillofQtyMaster>(customerQuotationCompositeView);
                var param2 = _mapper.Map<List<BillOfQTyDetails>>(customerQuotationCompositeView.BillOfQTyDetails);
                var xs = _customerQuotationService.InsertBillofQty(param1, param2);

                BillofQtyMasterModel CustomerQuotationViewModel = new BillofQtyMasterModel();

                if (xs != null)
                {
                    CustomerQuotationViewModel = _mapper.Map<BillofQtyMasterModel>(xs);
                    CustomerQuotationViewModel.BillOfQTyDetails = _mapper.Map<List<BillOfQTyDetailsModel>>(xs.BillOfQTyDetails);

                    apiResponse = new ApiResponse<BillofQtyMasterModel>
                    {
                        Valid = true,
                        Result = CustomerQuotationViewModel,
                        Message = BillofQtyMessage.SaveBillofQty
                    };
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                apiResponse = new ApiResponse<BillofQtyMasterModel>
                {
                    Valid = false,
                    Error = true,
                    Exception = ex.Message,
                    Message = BillofQtyMessage.BillofQtyError
                };
            }
            return apiResponse;

        }
        [HttpPost]
        [Route("UpdateBillofQty")]
        public ApiResponse<BillofQtyMasterModel> UpdateBillofQty([FromBody] BillofQtyMasterModel customerQuotationCompositeView)
        {
            var param1 = _mapper.Map<BillofQtyMaster>(customerQuotationCompositeView);
            var param2 = _mapper.Map<List<BillOfQTyDetails>>(customerQuotationCompositeView.BillOfQTyDetails);
            var xs = _customerQuotationService.UpdateBillofQty(param1, param2);
            if (xs != null)
            {
                BillofQtyMasterModel CustomerQuotationViewModel = new BillofQtyMasterModel();
                CustomerQuotationViewModel = _mapper.Map<BillofQtyMasterModel>(xs);
                CustomerQuotationViewModel.BillOfQTyDetails = _mapper.Map<List<BillOfQTyDetailsModel>>(xs.BillOfQTyDetails);

                ApiResponse<BillofQtyMasterModel> apiResponse = new ApiResponse<BillofQtyMasterModel>
                {
                    Valid = true,
                    Result = CustomerQuotationViewModel,
                    Message = BillofQtyMessage.UpdateBillofQty
                };
                return apiResponse;
            }
            else
            {
                return null;
            }
        }
        [HttpPost]
        [Route("DeleteBillofQty")]
        public ApiResponse<int> DeleteBillofQty( decimal id)
        {
            ApiResponse<int> apiResponse = new ApiResponse<int>
            {
                Valid = true,
                Result = _mapper.Map<int>(_customerQuotationService.DeleteBillofQty(id)),
                Message = ""
            };
            return apiResponse;                       

        }
        [HttpGet]
        [Route("GetBillofQty")]
        public ApiResponse<List<BillofQtyMaster>> GetBillofQty()
        {
            ApiResponse<List<BillofQtyMaster>> apiResponse = new ApiResponse<List<BillofQtyMaster>>
            {
                Valid = true,
                Result = _mapper.Map<List<BillofQtyMaster>>(_customerQuotationService.GetBillofQty()),
                Message = ""
            };
            return apiResponse;
        }
        [HttpGet]
        [Route("GetSavedBillofQty/{id}")]
        public ApiResponse<BillofQtyMasterModel> GetSavedBillofQty(decimal id)
        {            

            ApiResponse<BillofQtyMasterModel> apiResponse = new ApiResponse<BillofQtyMasterModel>
            {
                Valid = true,
                Result = _mapper.Map<BillofQtyMasterModel>(_customerQuotationService.GetSavedBillofQty(id)),
                Message = ""
            };
            return apiResponse;
        }


    }
}
