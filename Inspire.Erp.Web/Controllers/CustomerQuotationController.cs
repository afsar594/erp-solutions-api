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
            if (_customerQuotationService.GetVouchersNumbers(customerQuotationCompositeView.CustomerQuotationQuotationId) == null)
            {
                var param1 = _mapper.Map<CustomerQuotation>(customerQuotationCompositeView);
                var param2 = _mapper.Map<List<CustomerQuotationDetails>>(customerQuotationCompositeView.CustomerQuotationDetails);
                var xs = _customerQuotationService.InsertCustomerQuotation(param1, param2);
                CustomerQuotationViewModel customerQuotationViewModel = new CustomerQuotationViewModel
                {

                    CustomerQuotationQid = xs.custQuotation.CustomerQuotationQid,
                    CustomerQuotationQuotationId = xs.custQuotation.CustomerQuotationQuotationId,
                    CustomerQuotationQuotationAddition = xs.custQuotation.CustomerQuotationQuotationAddition,
                    CustomerQuotationQuotationRoot = xs.custQuotation.CustomerQuotationQuotationRoot,
                    CustomerQuotationQuotationDate = xs.custQuotation.CustomerQuotationQuotationDate,
                    CustomerQuotationQuotationValidDate = xs.custQuotation.CustomerQuotationQuotationValidDate,
                    CustomerQuotationCustomerId = xs.custQuotation.CustomerQuotationCustomerId,
                    CustomerQuotationEnquiryId = xs.custQuotation.CustomerQuotationEnquiryId,
                    CustomerQuotationRemarks = xs.custQuotation.CustomerQuotationRemarks,
                    CustomerQuotationLocationId = xs.custQuotation.CustomerQuotationLocationId,
                    CustomerQuotationDiscountPercentage = xs.custQuotation.CustomerQuotationDiscountPercentage,
                    CustomerQuotationDiscountAmount = xs.custQuotation.CustomerQuotationDiscountAmount,
                    CustomerQuotationGrossTotal = xs.custQuotation.CustomerQuotationGrossTotal,
                    CustomerQuotationNetTotal = xs.custQuotation.CustomerQuotationNetTotal,
                    CustomerQuotationSalesManId = xs.custQuotation.CustomerQuotationSalesManId,
                    CustomerQuotationCurrencyId = xs.custQuotation.CustomerQuotationCurrencyId,
                    CustomerQuotationJobId = xs.custQuotation.CustomerQuotationJobId,
                    CustomerQuotationSubject = xs.custQuotation.CustomerQuotationSubject,
                    CustomerQuotationNote = xs.custQuotation.CustomerQuotationNote,
                    CustomerQuotationWarranty = xs.custQuotation.CustomerQuotationWarranty,
                    CustomerQuotationTraining = xs.custQuotation.CustomerQuotationTraining,
                    CustomerQuotationTechnicalDetails = xs.custQuotation.CustomerQuotationTechnicalDetails,
                    CustomerQuotationTerms = xs.custQuotation.CustomerQuotationTerms,
                    CustomerQuotationDeliveryTimeDate = xs.custQuotation.CustomerQuotationDeliveryTimeDate,
                    CustomerQuotationPacking = xs.custQuotation.CustomerQuotationPacking,
                    CustomerQuotationQuality = xs.custQuotation.CustomerQuotationQuality,
                    CustomerQuotationHeader = xs.custQuotation.CustomerQuotationHeader,
                    CustomerQuotationFooter = xs.custQuotation.CustomerQuotationFooter,
                    CustomerQuotationQuotationStatusId = xs.custQuotation.CustomerQuotationQuotationStatusId,
                    CustomerQuotationFsno = xs.custQuotation.CustomerQuotationFsno,
                    CustomerQuotationIsClose = xs.custQuotation.CustomerQuotationIsClose,
                    CustomerQuotationEnquiryNo = xs.custQuotation.CustomerQuotationEnquiryNo,
                    CustomerQuotationVoucherType = xs.custQuotation.CustomerQuotationVoucherType,
                    CustomerQuotationCashCustomerName = xs.custQuotation.CustomerQuotationCashCustomerName,
                    CustomerQuotationContactPersonV = xs.custQuotation.CustomerQuotationContactPersonV,
                    CustomerQuotationDiscountAmountTotal = xs.custQuotation.CustomerQuotationDiscountAmountTotal,
                    CustomerQuotationVendorId = xs.custQuotation.CustomerQuotationVendorId,
                    CustomerQuotationTypeId = xs.custQuotation.CustomerQuotationTypeId,
                    CustomerQuotationModelId = xs.custQuotation.CustomerQuotationModelId,
                    CustomerQuotationVatPercentage = xs.custQuotation.CustomerQuotationVatPercentage,
                    CustomerQuotationVatAmount = xs.custQuotation.CustomerQuotationVatAmount,
                    CustomerQuotationQuotationEnquiryN = xs.custQuotation.CustomerQuotationQuotationEnquiryN,
                    CustomerQuotationDelStatus = xs.custQuotation.CustomerQuotationDelStatus,
                    CustomerQuotationProjectN = xs.custQuotation.CustomerQuotationProjectN,
                    CustomerQuotationProjectRef = xs.custQuotation.CustomerQuotationProjectRef,
                    CustomerQuotationReviseQuotation = xs.custQuotation.CustomerQuotationReviseQuotation,
                    CustomerQuotationGrossProfitTotal = xs.custQuotation.CustomerQuotationGrossProfitTotal,

                };

                customerQuotationViewModel.CustomerQuotationDetails = _mapper.Map<List<CustomerQuotationDetailsViewModel>>(xs.customerQuotationDetails);
                apiResponse = new ApiResponse<CustomerQuotationViewModel>
                {
                    Valid = true,
                    Result = _mapper.Map<CustomerQuotationViewModel>(customerQuotationViewModel),
                    Message = CustomerQuotationMessage.SaveCustomerQuotation     
                };
            }
            else
            {
                apiResponse = new ApiResponse<CustomerQuotationViewModel>
                {
                    Valid = false,
                    Error = true,
                    Exception = null,
                    Message = CustomerQuotationMessage.CustomerQuotationAlreadyExist
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

            CustomerQuotationViewModel customerQuotationViewModel = new CustomerQuotationViewModel
            {
                CustomerQuotationQid = xs.custQuotation.CustomerQuotationQid,
                CustomerQuotationQuotationId = xs.custQuotation.CustomerQuotationQuotationId,
                CustomerQuotationQuotationAddition = xs.custQuotation.CustomerQuotationQuotationAddition,
                CustomerQuotationQuotationRoot = xs.custQuotation.CustomerQuotationQuotationRoot,
                CustomerQuotationQuotationDate = xs.custQuotation.CustomerQuotationQuotationDate,
                CustomerQuotationQuotationValidDate  = xs.custQuotation.CustomerQuotationQuotationValidDate,
                CustomerQuotationCustomerId = xs.custQuotation.CustomerQuotationCustomerId,
                CustomerQuotationEnquiryId = xs.custQuotation.CustomerQuotationEnquiryId,
                CustomerQuotationRemarks = xs.custQuotation.CustomerQuotationRemarks,
                CustomerQuotationLocationId = xs.custQuotation.CustomerQuotationLocationId,
                CustomerQuotationDiscountPercentage = xs.custQuotation.CustomerQuotationDiscountPercentage,
                CustomerQuotationDiscountAmount = xs.custQuotation.CustomerQuotationDiscountAmount,
                CustomerQuotationGrossTotal = xs.custQuotation.CustomerQuotationGrossTotal,
                CustomerQuotationNetTotal = xs.custQuotation.CustomerQuotationNetTotal,
                CustomerQuotationSalesManId = xs.custQuotation.CustomerQuotationSalesManId,
                CustomerQuotationCurrencyId = xs.custQuotation.CustomerQuotationCurrencyId,
                CustomerQuotationJobId = xs.custQuotation.CustomerQuotationJobId,
                CustomerQuotationSubject = xs.custQuotation.CustomerQuotationSubject,
                CustomerQuotationNote = xs.custQuotation.CustomerQuotationNote,
                CustomerQuotationWarranty = xs.custQuotation.CustomerQuotationWarranty,
                CustomerQuotationTraining = xs.custQuotation.CustomerQuotationTraining,
                CustomerQuotationTechnicalDetails = xs.custQuotation.CustomerQuotationTechnicalDetails,
                CustomerQuotationTerms = xs.custQuotation.CustomerQuotationTerms,
                CustomerQuotationDeliveryTimeDate = xs.custQuotation.CustomerQuotationDeliveryTimeDate,
                CustomerQuotationPacking = xs.custQuotation.CustomerQuotationPacking,
                CustomerQuotationQuality = xs.custQuotation.CustomerQuotationQuality,
                CustomerQuotationHeader = xs.custQuotation.CustomerQuotationHeader,
                CustomerQuotationFooter = xs.custQuotation.CustomerQuotationFooter,
                CustomerQuotationQuotationStatusId = xs.custQuotation.CustomerQuotationQuotationStatusId,
                CustomerQuotationFsno = xs.custQuotation.CustomerQuotationFsno,
                CustomerQuotationIsClose = xs.custQuotation.CustomerQuotationIsClose,
                CustomerQuotationEnquiryNo = xs.custQuotation.CustomerQuotationEnquiryNo,
                CustomerQuotationVoucherType = xs.custQuotation.CustomerQuotationVoucherType,
                CustomerQuotationCashCustomerName = xs.custQuotation.CustomerQuotationCashCustomerName,
                CustomerQuotationContactPersonV = xs.custQuotation.CustomerQuotationContactPersonV,
                CustomerQuotationDiscountAmountTotal = xs.custQuotation.CustomerQuotationDiscountAmountTotal,
                CustomerQuotationVendorId = xs.custQuotation.CustomerQuotationVendorId,
                CustomerQuotationTypeId = xs.custQuotation.CustomerQuotationTypeId,
                CustomerQuotationModelId = xs.custQuotation.CustomerQuotationModelId,
                CustomerQuotationVatPercentage = xs.custQuotation.CustomerQuotationVatPercentage,
                CustomerQuotationVatAmount = xs.custQuotation.CustomerQuotationVatAmount,
                CustomerQuotationQuotationEnquiryN = xs.custQuotation.CustomerQuotationQuotationEnquiryN,
                CustomerQuotationDelStatus = xs.custQuotation.CustomerQuotationDelStatus,
                CustomerQuotationProjectN   = xs.custQuotation.CustomerQuotationProjectN,
                CustomerQuotationProjectRef = xs.custQuotation.CustomerQuotationProjectRef,
                CustomerQuotationReviseQuotation = xs.custQuotation.CustomerQuotationReviseQuotation,
                CustomerQuotationGrossProfitTotal = xs.custQuotation.CustomerQuotationGrossProfitTotal,


            };

            customerQuotationViewModel.CustomerQuotationDetails = _mapper.Map<List<CustomerQuotationDetailsViewModel>>(xs.customerQuotationDetails);
            ApiResponse<CustomerQuotationViewModel> apiResponse = new ApiResponse<CustomerQuotationViewModel>
            {
                Valid = true,
                Result = _mapper.Map<CustomerQuotationViewModel>(customerQuotationViewModel),
                Message = CustomerQuotationMessage.UpdateCustomerQuotation
            };

            return apiResponse;
        }

        [HttpPost]
        [Route("DeleteCustomerQuotation")]
        public ApiResponse<int> DeleteCustomerQuotation([FromBody] CustomerQuotationViewModel customerQuotationCompositeView)
        {
            var param1 = _mapper.Map<CustomerQuotation>(customerQuotationCompositeView);
            var param2 = _mapper.Map<List<CustomerQuotationDetails>>(customerQuotationCompositeView.CustomerQuotationDetails);
            var xs = _customerQuotationService.DeleteCustomerQuotation(param1, param2);
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
        public ApiResponse<List<CustomerQuotation>> GetAllCustomerQuotations()
        {
            ApiResponse<List<CustomerQuotation>> apiResponse = new ApiResponse<List<CustomerQuotation>>
            {
                Valid = true,
                Result = _mapper.Map<List<CustomerQuotation>>(_customerQuotationService.GetCustomerQuotation()),
                Message = ""
            };
            return apiResponse;
        }

        [HttpGet]
        [Route("GetSavedCustQuotationDetails/{id}")]
        public ApiResponse<CustomerQuotationViewModel> GetSavedCustQuotationDetails(string id)
        {
            CustomerQuotationModel custQuotation = _customerQuotationService.GetSavedCustQuotationDetails(id);
            if (custQuotation != null)
            {
                CustomerQuotationViewModel customerQuotationViewModel = new CustomerQuotationViewModel
                {

                    CustomerQuotationQid = custQuotation.custQuotation.CustomerQuotationQid,
                    CustomerQuotationQuotationId = custQuotation.custQuotation.CustomerQuotationQuotationId,
                    CustomerQuotationQuotationAddition = custQuotation.custQuotation.CustomerQuotationQuotationAddition,
                    CustomerQuotationQuotationRoot = custQuotation.custQuotation.CustomerQuotationQuotationRoot,
                    CustomerQuotationQuotationDate = custQuotation.custQuotation.CustomerQuotationQuotationDate,
                    CustomerQuotationQuotationValidDate = custQuotation.custQuotation.CustomerQuotationQuotationValidDate,
                    CustomerQuotationCustomerId = custQuotation.custQuotation.CustomerQuotationCustomerId,
                    CustomerQuotationEnquiryId = custQuotation.custQuotation.CustomerQuotationEnquiryId,
                    CustomerQuotationRemarks = custQuotation.custQuotation.CustomerQuotationRemarks,
                    CustomerQuotationLocationId = custQuotation.custQuotation.CustomerQuotationLocationId,
                    CustomerQuotationDiscountPercentage = custQuotation.custQuotation.CustomerQuotationDiscountPercentage,
                    CustomerQuotationDiscountAmount = custQuotation.custQuotation.CustomerQuotationDiscountAmount,
                    CustomerQuotationGrossTotal = custQuotation.custQuotation.CustomerQuotationGrossTotal,
                    CustomerQuotationNetTotal = custQuotation.custQuotation.CustomerQuotationNetTotal,
                    CustomerQuotationSalesManId = custQuotation.custQuotation.CustomerQuotationSalesManId,
                    CustomerQuotationCurrencyId = custQuotation.custQuotation.CustomerQuotationCurrencyId,
                    CustomerQuotationJobId = custQuotation.custQuotation.CustomerQuotationJobId,
                    CustomerQuotationSubject = custQuotation.custQuotation.CustomerQuotationSubject,
                    CustomerQuotationNote = custQuotation.custQuotation.CustomerQuotationNote,
                    CustomerQuotationWarranty = custQuotation.custQuotation.CustomerQuotationWarranty,
                    CustomerQuotationTraining = custQuotation.custQuotation.CustomerQuotationTraining,
                    CustomerQuotationTechnicalDetails = custQuotation.custQuotation.CustomerQuotationTechnicalDetails,
                    CustomerQuotationTerms = custQuotation.custQuotation.CustomerQuotationTerms,
                    CustomerQuotationDeliveryTimeDate = custQuotation.custQuotation.CustomerQuotationDeliveryTimeDate,
                    CustomerQuotationPacking = custQuotation.custQuotation.CustomerQuotationPacking,
                    CustomerQuotationQuality = custQuotation.custQuotation.CustomerQuotationQuality,
                    CustomerQuotationHeader = custQuotation.custQuotation.CustomerQuotationHeader,
                    CustomerQuotationFooter = custQuotation.custQuotation.CustomerQuotationFooter,
                    CustomerQuotationQuotationStatusId = custQuotation.custQuotation.CustomerQuotationQuotationStatusId,
                    CustomerQuotationFsno = custQuotation.custQuotation.CustomerQuotationFsno,
                    CustomerQuotationIsClose = custQuotation.custQuotation.CustomerQuotationIsClose,
                    CustomerQuotationEnquiryNo = custQuotation.custQuotation.CustomerQuotationEnquiryNo,
                    CustomerQuotationVoucherType = custQuotation.custQuotation.CustomerQuotationVoucherType,
                    CustomerQuotationCashCustomerName = custQuotation.custQuotation.CustomerQuotationCashCustomerName,
                    CustomerQuotationContactPersonV = custQuotation.custQuotation.CustomerQuotationContactPersonV,
                    CustomerQuotationDiscountAmountTotal = custQuotation.custQuotation.CustomerQuotationDiscountAmountTotal,
                    CustomerQuotationVendorId = custQuotation.custQuotation.CustomerQuotationVendorId,
                    CustomerQuotationTypeId = custQuotation.custQuotation.CustomerQuotationTypeId,
                    CustomerQuotationModelId = custQuotation.custQuotation.CustomerQuotationModelId,
                    CustomerQuotationVatPercentage = custQuotation.custQuotation.CustomerQuotationVatPercentage,
                    CustomerQuotationVatAmount = custQuotation.custQuotation.CustomerQuotationVatAmount,
                    CustomerQuotationQuotationEnquiryN = custQuotation.custQuotation.CustomerQuotationQuotationEnquiryN,
                    CustomerQuotationDelStatus = custQuotation.custQuotation.CustomerQuotationDelStatus,
                    CustomerQuotationProjectN = custQuotation.custQuotation.CustomerQuotationProjectN,
                    CustomerQuotationProjectRef = custQuotation.custQuotation.CustomerQuotationProjectRef,
                    CustomerQuotationReviseQuotation = custQuotation.custQuotation.CustomerQuotationReviseQuotation,
                    CustomerQuotationGrossProfitTotal = custQuotation.custQuotation.CustomerQuotationGrossProfitTotal,



                };
                customerQuotationViewModel.CustomerQuotationDetails = _mapper.Map<List<CustomerQuotationDetailsViewModel>>(custQuotation.customerQuotationDetails);
                ApiResponse<CustomerQuotationViewModel> apiResponse = new ApiResponse<CustomerQuotationViewModel>
                {
                    Valid = true,
                    Result = customerQuotationViewModel,
                    Message = ""
                };
                return apiResponse;
            }
            return null;

        }

    }
}
