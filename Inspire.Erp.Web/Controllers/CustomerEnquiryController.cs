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
    public class CustomerEnquiryController : ControllerBase
    {
        private ICustomerEnquiryService _customerEnquiryService;
        private readonly IMapper _mapper;
        public CustomerEnquiryController(ICustomerEnquiryService customerEnquiryService, IMapper mapper)
        {
            _customerEnquiryService = customerEnquiryService;
            _mapper = mapper;
        }


        [HttpPost]
        [Route("InsertCustomerEnquiry")]
        public ApiResponse<EnquiryMasterViewModel> InsertCustomerEnquiry([FromBody] EnquiryMasterViewModel customerEnquiryCompositeView)
        {
            ApiResponse<EnquiryMasterViewModel> apiResponse = new ApiResponse<EnquiryMasterViewModel>();
            if (_customerEnquiryService.GetVouchersNumbers(customerEnquiryCompositeView.EnquiryMasterEnquiryId) == null)
            {
                var param1 = _mapper.Map<EnquiryMaster>(customerEnquiryCompositeView);
                var param2 = _mapper.Map<List<EnquiryDetails>>(customerEnquiryCompositeView.CustomerEnquiryDetails);
                var xs = _customerEnquiryService.InsertCustomerEnquiry(param1, param2);
                EnquiryMasterViewModel customerEnquiryViewModel = new EnquiryMasterViewModel
                {

                    EnquiryMasterEnquiryId = xs.custEnquiry.EnquiryMasterEnquiryId,
                    EnquiryMasterEnquiryDate = xs.custEnquiry.EnquiryMasterEnquiryDate,
                    EnquiryMasterEnquiryAboutId = xs.custEnquiry.EnquiryMasterEnquiryAboutId,
                    EnquiryMasterRemarks = xs.custEnquiry.EnquiryMasterRemarks,
                    EnquiryMasterSalesManId = xs.custEnquiry.EnquiryMasterSalesManId,
                    EnquiryMasterCurrencyId = xs.custEnquiry.EnquiryMasterCurrencyId,
                    EnquiryMasterLocationId = xs.custEnquiry.EnquiryMasterLocationId,
                    EnquiryMasterCustomerReference = xs.custEnquiry.EnquiryMasterCustomerReference,
                    EnquiryMasterEnquiryStatusId = xs.custEnquiry.EnquiryMasterEnquiryStatusId,
                    EnquiryMasterConsultingEngineer = xs.custEnquiry.EnquiryMasterConsultingEngineer,
                    EnquiryMasterJobNo = xs.custEnquiry.EnquiryMasterJobNo,
                    EnquiryMasterBusineesTypeId = xs.custEnquiry.EnquiryMasterBusineesTypeId,
                    EnquiryMasterEnquiryVoucherNo = xs.custEnquiry.EnquiryMasterEnquiryVoucherNo,
                    EnquiryMasterVesselId = xs.custEnquiry.EnquiryMasterVesselId,
                    EnquiryMasterBuilder = xs.custEnquiry.EnquiryMasterBuilder,
                    EnquiryMasterSerialNo = xs.custEnquiry.EnquiryMasterSerialNo,
                    EnquiryMasterVendorId = xs.custEnquiry.EnquiryMasterVendorId,
                    EnquiryMasterTypeId = xs.custEnquiry.EnquiryMasterTypeId,
                    EnquiryMasterModelId = xs.custEnquiry.EnquiryMasterModelId,
                    EnquiryMasterContactId = xs.custEnquiry.EnquiryMasterContactId,
                    EnquiryMasterContactName = xs.custEnquiry.EnquiryMasterContactName,
                    EnquiryMasterContactEmail = xs.custEnquiry.EnquiryMasterContactEmail,
                    EnquiryMasterDelStatus = xs.custEnquiry.EnquiryMasterDelStatus

                };

                customerEnquiryViewModel.CustomerEnquiryDetails = _mapper.Map<List<EnquiryDetailsViewModel>>(xs.customerEnquiryDetails);
                apiResponse = new ApiResponse<EnquiryMasterViewModel>
                {
                    Valid = true,
                    Result = _mapper.Map<EnquiryMasterViewModel>(customerEnquiryViewModel),
                    Message = CustomerEnquiryMessage.SaveCustomerEnquiry
                };
            }
            else
            {
                apiResponse = new ApiResponse<EnquiryMasterViewModel>
                {
                    Valid = false,
                    Error = true,
                    Exception = null,
                    Message = PaymentVoucherMessage.VoucherAlreadyExist
                };

            }


            return apiResponse;

        }

        [HttpPost]
        [Route("UpdateCustomerEnquiry")]
        public ApiResponse<EnquiryMasterViewModel> UpdateCustomerEnquiry([FromBody] EnquiryMasterViewModel customerEnquiryCompositeView)
        {
            var param1 = _mapper.Map<EnquiryMaster>(customerEnquiryCompositeView);
            var param2 = _mapper.Map<List<EnquiryDetails>>(customerEnquiryCompositeView.CustomerEnquiryDetails);
            var xs = _customerEnquiryService.UpdateCustomerEnquiry(param1, param2);

            EnquiryMasterViewModel customerEnquiryViewModel = new EnquiryMasterViewModel
            {

                EnquiryMasterEnquiryId = xs.custEnquiry.EnquiryMasterEnquiryId,
                EnquiryMasterEnquiryDate = xs.custEnquiry.EnquiryMasterEnquiryDate,
                EnquiryMasterEnquiryAboutId = xs.custEnquiry.EnquiryMasterEnquiryAboutId,
                EnquiryMasterRemarks = xs.custEnquiry.EnquiryMasterRemarks,
                EnquiryMasterSalesManId = xs.custEnquiry.EnquiryMasterSalesManId,
                EnquiryMasterCurrencyId = xs.custEnquiry.EnquiryMasterCurrencyId,
                EnquiryMasterLocationId = xs.custEnquiry.EnquiryMasterLocationId,
                EnquiryMasterCustomerReference = xs.custEnquiry.EnquiryMasterCustomerReference,
                EnquiryMasterEnquiryStatusId = xs.custEnquiry.EnquiryMasterEnquiryStatusId,
                EnquiryMasterConsultingEngineer = xs.custEnquiry.EnquiryMasterConsultingEngineer,
                EnquiryMasterJobNo = xs.custEnquiry.EnquiryMasterJobNo,
                EnquiryMasterBusineesTypeId = xs.custEnquiry.EnquiryMasterBusineesTypeId,
                EnquiryMasterEnquiryVoucherNo = xs.custEnquiry.EnquiryMasterEnquiryVoucherNo,
                EnquiryMasterVesselId = xs.custEnquiry.EnquiryMasterVesselId,
                EnquiryMasterBuilder = xs.custEnquiry.EnquiryMasterBuilder,
                EnquiryMasterSerialNo = xs.custEnquiry.EnquiryMasterSerialNo,
                EnquiryMasterVendorId = xs.custEnquiry.EnquiryMasterVendorId,
                EnquiryMasterTypeId = xs.custEnquiry.EnquiryMasterTypeId,
                EnquiryMasterModelId = xs.custEnquiry.EnquiryMasterModelId,
                EnquiryMasterContactId = xs.custEnquiry.EnquiryMasterContactId,
                EnquiryMasterContactName = xs.custEnquiry.EnquiryMasterContactName,
                EnquiryMasterContactEmail = xs.custEnquiry.EnquiryMasterContactEmail,
                EnquiryMasterDelStatus = xs.custEnquiry.EnquiryMasterDelStatus
            };

            customerEnquiryViewModel.CustomerEnquiryDetails = _mapper.Map<List<EnquiryDetailsViewModel>>(xs.customerEnquiryDetails);
            ApiResponse<EnquiryMasterViewModel> apiResponse = new ApiResponse<EnquiryMasterViewModel>
            {
                Valid = true,
                Result = _mapper.Map<EnquiryMasterViewModel>(customerEnquiryViewModel),
                Message = CustomerEnquiryMessage.UpdateCustomerEnquiry
            };

            return apiResponse;
        }

        [HttpPost]
        [Route("DeleteCustomerEnquiry")]
        public ApiResponse<int> DeleteCustomerEnquiry([FromBody] EnquiryMasterViewModel customerEnquiryCompositeView)
        {
            var param1 = _mapper.Map<EnquiryMaster>(customerEnquiryCompositeView);
            var param2 = _mapper.Map<List<EnquiryDetails>>(customerEnquiryCompositeView.CustomerEnquiryDetails);
            var xs = _customerEnquiryService.DeleteCustomerEnquiry(param1, param2);
            ApiResponse<int> apiResponse = new ApiResponse<int>
            {
                Valid = true,
                Result = 0,
                Message = CustomerEnquiryMessage.DeleteCustomerEnquiry
            };

            return apiResponse;

        }


        [HttpGet]
        [Route("GetCustomerEnquiry")]
        public ApiResponse<List<EnquiryMaster>> GetAllCustomerEnquirys()
        {
            ApiResponse<List<EnquiryMaster>> apiResponse = new ApiResponse<List<EnquiryMaster>>
            {
                Valid = true,
                Result = _mapper.Map<List<EnquiryMaster>>(_customerEnquiryService.GetCustomerEnquiry()),
                Message = ""
            };
            return apiResponse;
        }

        
        [HttpGet]
        [Route("GetSavedCustEnquiryDetails/{id}")]
        public ApiResponse<EnquiryMasterViewModel> GetSavedCustEnquiryDetails(string id)
        {
            CustomerEnquiryModel custEnquiry = _customerEnquiryService.GetSavedCustEnquiryDetails(id);
            if (custEnquiry != null)
            {
                EnquiryMasterViewModel customerEnquiryViewModel = new EnquiryMasterViewModel
                {

                    EnquiryMasterEnquiryId = custEnquiry.custEnquiry.EnquiryMasterEnquiryId,
                    EnquiryMasterEnquiryDate = custEnquiry.custEnquiry.EnquiryMasterEnquiryDate,
                    EnquiryMasterEnquiryAboutId = custEnquiry.custEnquiry.EnquiryMasterEnquiryAboutId,
                    EnquiryMasterRemarks = custEnquiry.custEnquiry.EnquiryMasterRemarks,
                    EnquiryMasterSalesManId = custEnquiry.custEnquiry.EnquiryMasterSalesManId,
                    EnquiryMasterCurrencyId = custEnquiry.custEnquiry.EnquiryMasterCurrencyId,
                    EnquiryMasterLocationId = custEnquiry.custEnquiry.EnquiryMasterLocationId,
                    EnquiryMasterCustomerReference = custEnquiry.custEnquiry.EnquiryMasterCustomerReference,
                    EnquiryMasterEnquiryStatusId = custEnquiry.custEnquiry.EnquiryMasterEnquiryStatusId,
                    EnquiryMasterConsultingEngineer = custEnquiry.custEnquiry.EnquiryMasterConsultingEngineer,
                    EnquiryMasterJobNo = custEnquiry.custEnquiry.EnquiryMasterJobNo,
                    EnquiryMasterBusineesTypeId = custEnquiry.custEnquiry.EnquiryMasterBusineesTypeId,
                    EnquiryMasterEnquiryVoucherNo = custEnquiry.custEnquiry.EnquiryMasterEnquiryVoucherNo,
                    EnquiryMasterVesselId = custEnquiry.custEnquiry.EnquiryMasterVesselId,
                    EnquiryMasterBuilder = custEnquiry.custEnquiry.EnquiryMasterBuilder,
                    EnquiryMasterSerialNo = custEnquiry.custEnquiry.EnquiryMasterSerialNo,
                    EnquiryMasterVendorId = custEnquiry.custEnquiry.EnquiryMasterVendorId,
                    EnquiryMasterTypeId = custEnquiry.custEnquiry.EnquiryMasterTypeId,
                    EnquiryMasterModelId = custEnquiry.custEnquiry.EnquiryMasterModelId,
                    EnquiryMasterContactId = custEnquiry.custEnquiry.EnquiryMasterContactId,
                    EnquiryMasterContactName = custEnquiry.custEnquiry.EnquiryMasterContactName,
                    EnquiryMasterContactEmail = custEnquiry.custEnquiry.EnquiryMasterContactEmail,
                    EnquiryMasterDelStatus = custEnquiry.custEnquiry.EnquiryMasterDelStatus

                };
                customerEnquiryViewModel.CustomerEnquiryDetails = _mapper.Map<List<EnquiryDetailsViewModel>>(custEnquiry.customerEnquiryDetails);
                ApiResponse<EnquiryMasterViewModel> apiResponse = new ApiResponse<EnquiryMasterViewModel>
                {
                    Valid = true,
                    Result = customerEnquiryViewModel,
                    Message = ""
                };
               
                return apiResponse;
            }
            return null;

        }


    }
}
