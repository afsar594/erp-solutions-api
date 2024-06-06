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

namespace Inspire.Erp.Web.Controllers
{
    [Route("api")]
    [Produces("application/json")]
    [ApiController]
    public class CustomerQuotationController : ControllerBase
    {
        private ICustomerQuotationService _customerQuotationService;
        private readonly IMapper _mapper;
        public CustomerQuotationController(ICustomerQuotationService customerQuotationService, IMapper mapper)
        {
            _customerQuotationService = customerQuotationService;
            _mapper = mapper;
        }
        [HttpPost]
        [Route("InsertCustomerQuotation")]
        public ApiResponse<CustomerQuotationViewModel> InsertCustomerQuotation([FromBody] CustomerQuotationViewModel customerQuotationCompositeView)
        {
            ApiResponse<CustomerQuotationViewModel> apiResponse = new ApiResponse<CustomerQuotationViewModel>();

            try
            {
                var param1 = _mapper.Map<CustomerQuotation>(customerQuotationCompositeView);
                var param2 = _mapper.Map<List<CustomerQuotationDetails>>(customerQuotationCompositeView.CustomerQuotationDetails);
                var xs = _customerQuotationService.InsertCustomerQuotation(param1, param2);
                //var apvm = new CustomerQuotationViewModel();
                //apvm = _mapper.Map<CustomerQuotationViewModel>(xs.custQuotation);
                //CustomerQuotationViewModel customerQuotationViewModel = new CustomerQuotationViewModel
                //{

                //    CustomerQuotationQid = xs.custQuotation.CustomerQuotationQid,
                //    CustomerQuotationQuotationId = xs.custQuotation.CustomerQuotationQuotationId,
                //    CustomerQuotationQuotationAddition = xs.custQuotation.CustomerQuotationQuotationAddition,
                //    CustomerQuotationQuotationRoot = xs.custQuotation.CustomerQuotationQuotationRoot,
                //    CustomerQuotationQuotationDate = xs.custQuotation.CustomerQuotationQuotationDate,
                //    CustomerQuotationQuotationValidDate = xs.custQuotation.CustomerQuotationQuotationValidDate,
                //    CustomerQuotationCustomerId = xs.custQuotation.CustomerQuotationCustomerId,
                //    CustomerQuotationEnquiryId = xs.custQuotation.CustomerQuotationEnquiryId,
                //    CustomerQuotationRemarks = xs.custQuotation.CustomerQuotationRemarks,
                //    CustomerQuotationLocationId = xs.custQuotation.CustomerQuotationLocationId,
                //    CustomerQuotationDiscountPercentage = xs.custQuotation.CustomerQuotationDiscountPercentage,
                //    CustomerQuotationDiscountAmount = xs.custQuotation.CustomerQuotationDiscountAmount,
                //    CustomerQuotationGrossTotal = xs.custQuotation.CustomerQuotationGrossTotal,
                //    CustomerQuotationNetTotal = xs.custQuotation.CustomerQuotationNetTotal,
                //    CustomerQuotationSalesManId = xs.custQuotation.CustomerQuotationSalesManId,
                //    CustomerQuotationCurrencyId = xs.custQuotation.CustomerQuotationCurrencyId,
                //    CustomerQuotationJobId = xs.custQuotation.CustomerQuotationJobId,
                //    CustomerQuotationSubject = xs.custQuotation.CustomerQuotationSubject,
                //    CustomerQuotationNote = xs.custQuotation.CustomerQuotationNote,
                //    CustomerQuotationWarranty = xs.custQuotation.CustomerQuotationWarranty,
                //    CustomerQuotationTraining = xs.custQuotation.CustomerQuotationTraining,
                //    CustomerQuotationTechnicalDetails = xs.custQuotation.CustomerQuotationTechnicalDetails,
                //    CustomerQuotationTerms = xs.custQuotation.CustomerQuotationTerms,
                //    CustomerQuotationDeliveryTimeDate = xs.custQuotation.CustomerQuotationDeliveryTimeDate,
                //    CustomerQuotationPacking = xs.custQuotation.CustomerQuotationPacking,
                //    CustomerQuotationQuality = xs.custQuotation.CustomerQuotationQuality,
                //    CustomerQuotationHeader = xs.custQuotation.CustomerQuotationHeader,
                //    CustomerQuotationFooter = xs.custQuotation.CustomerQuotationFooter,
                //    CustomerQuotationQuotationStatusId = xs.custQuotation.CustomerQuotationQuotationStatusId,
                //    CustomerQuotationFsno = xs.custQuotation.CustomerQuotationFsno,
                //    CustomerQuotationIsClose = xs.custQuotation.CustomerQuotationIsClose,
                //    CustomerQuotationEnquiryNo = xs.custQuotation.CustomerQuotationEnquiryNo,
                //    CustomerQuotationVoucherType = xs.custQuotation.CustomerQuotationVoucherType,
                //    CustomerQuotationCashCustomerName = xs.custQuotation.CustomerQuotationCashCustomerName,
                //    CustomerQuotationContactPersonV = xs.custQuotation.CustomerQuotationContactPersonV,
                //    CustomerQuotationDiscountAmountTotal = xs.custQuotation.CustomerQuotationDiscountAmountTotal,
                //    CustomerQuotationVendorId = xs.custQuotation.CustomerQuotationVendorId,
                //    CustomerQuotationTypeId = xs.custQuotation.CustomerQuotationTypeId,
                //    CustomerQuotationModelId = xs.custQuotation.CustomerQuotationModelId,
                //    CustomerQuotationVatPercentage = xs.custQuotation.CustomerQuotationVatPercentage,
                //    CustomerQuotationVatAmount = xs.custQuotation.CustomerQuotationVatAmount,
                //    CustomerQuotationQuotationEnquiryN = xs.custQuotation.CustomerQuotationQuotationEnquiryN,
                //    CustomerQuotationDelStatus = xs.custQuotation.CustomerQuotationDelStatus,
                //    CustomerQuotationProjectN = xs.custQuotation.CustomerQuotationProjectN,
                //    CustomerQuotationProjectRef = xs.custQuotation.CustomerQuotationProjectRef,
                //    CustomerQuotationReviseQuotation = xs.custQuotation.CustomerQuotationReviseQuotation,
                //    CustomerQuotationGrossProfitTotal = xs.custQuotation.CustomerQuotationGrossProfitTotal,

                //};


                CustomerQuotationViewModel CustomerQuotationViewModel = new CustomerQuotationViewModel();

                if (xs != null)
                {
                    CustomerQuotationViewModel = _mapper.Map<CustomerQuotationViewModel>(xs);
                    CustomerQuotationViewModel.CustomerQuotationDetails = _mapper.Map<List<CustomerQuotationDetailsViewModel>>(xs.CustomerQuotationDetails);

                    apiResponse = new ApiResponse<CustomerQuotationViewModel>
                    {
                        Valid = true,
                        Result = CustomerQuotationViewModel,
                        Message = CustomerQuotationMessage.SaveCustomerQuotation
                    };
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                apiResponse = new ApiResponse<CustomerQuotationViewModel>
                {
                    Valid = false,
                    Error = true,
                    Exception = ex.Message,
                    Message = CustomerQuotationMessage.CustomerQuotationError
                };

            }


            return apiResponse;

        }
        [HttpPost]
        [Route("UpdateCustomerQuotation")]
        public ApiResponse<CustomerQuotationViewModel> UpdateCustomerQuotation([FromBody] CustomerQuotationViewModel customerQuotationCompositeView)
        {
            var param1 = _mapper.Map<CustomerQuotation>(customerQuotationCompositeView);
            var param2 = _mapper.Map<List<CustomerQuotationDetails>>(customerQuotationCompositeView.CustomerQuotationDetails);
            var xs = _customerQuotationService.UpdateCustomerQuotation(param1, param2);
            if (xs != null)
            {
                CustomerQuotationViewModel CustomerQuotationViewModel = new CustomerQuotationViewModel();
                CustomerQuotationViewModel = _mapper.Map<CustomerQuotationViewModel>(xs);
                CustomerQuotationViewModel.CustomerQuotationDetails = _mapper.Map<List<CustomerQuotationDetailsViewModel>>(xs.CustomerQuotationDetails);

                ApiResponse<CustomerQuotationViewModel> apiResponse = new ApiResponse<CustomerQuotationViewModel>
                {
                    Valid = true,
                    Result = CustomerQuotationViewModel,
                    Message = CustomerQuotationMessage.UpdateCustomerQuotation
                };
                return apiResponse;
            }
            else
            {
                return null;
            }
        }
        [HttpPost]
        [Route("DeleteCustomerQuotation")]
        public ApiResponse<int> DeleteCustomerQuotation([FromBody] CustomerQuotationViewModel customerQuotationCompositeView)
        {
            var param1 = _mapper.Map<CustomerQuotation>(customerQuotationCompositeView);
            var param2 = _mapper.Map<List<CustomerQuotationDetails>>(customerQuotationCompositeView.CustomerQuotationDetails);
            var xs = _customerQuotationService.DeleteCustomerQuotation(param1);
            ApiResponse<int> apiResponse = new ApiResponse<int>
            {
                Valid = true,
                Result = 0,
                Message = CustomerQuotationMessage.DeleteCustomerQuotation
            };

            return apiResponse;

        }
        [HttpGet]
        [Route("GetCustomerQuotation")]
        public ApiResponse<List<CustomerQuotationViewModel>> GetAllCustomerQuotations()
        {
            ApiResponse<List<CustomerQuotationViewModel>> apiResponse = new ApiResponse<List<CustomerQuotationViewModel>>
            {
                Valid = true,
                Result = _mapper.Map<List<CustomerQuotationViewModel>>(_customerQuotationService.GetCustomerQuotation()),
                Message = ""
            };
            return apiResponse;
        }
        [HttpGet]
        [Route("GetSavedCustQuotationDetails/{id}")]
        public ApiResponse<CustomerQuotationViewModel> GetSavedCustQuotationDetails(int id)
        {
            //CustomerQuotation custQuotation = _customerQuotationService.GetSavedCustQuotationDetails(id);

            ApiResponse<CustomerQuotationViewModel> apiResponse = new ApiResponse<CustomerQuotationViewModel>
            {
                Valid = true,
                Result = _mapper.Map<CustomerQuotationViewModel>(_customerQuotationService.GetSavedCustQuotationDetails(id)),
                Message = ""
            };
            return apiResponse;
        }
        [HttpGet]
        [Route("GetCustomerQuotationForSaleOrder")]
        public async Task<IActionResult> GetCustomerQuotationForSaleOrder()
        {

            var response = await _customerQuotationService.GetCustomerQuotationForSaleOrder();
            ApiResponse<List<GetCustQuotationForSaleOrderResponse>> apiResponse = new ApiResponse<List<GetCustQuotationForSaleOrderResponse>>
            {
                Valid = true,
                Result = response.Result,
                Message = ""
            };
            return Ok(apiResponse);


        }


        [HttpPost]
        [Route("GetCustomerQuotationDetail")]
        public async Task<IActionResult> GetCustomerQuotationDetail(QuotationFilterModel FilterModel)
        {

            
                return Ok(await _customerQuotationService.GetCustomerQuotationDetail(FilterModel));
           

        }
    }
}
