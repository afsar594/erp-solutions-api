using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Inspire.Erp.Application.Account;
using Inspire.Erp.Application.Account.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Inspire.Erp.Web.Common;
using Microsoft.AspNetCore.Mvc;

namespace Inspire.Erp.Web.Controllers
{
    [Route("api/paymentrequest")]
    [Produces("application/json")]
    [ApiController]
    public class PaymentRequestController : ControllerBase
    {
        private readonly IPaymentRequestService _paymentRequestService;
        private readonly IMapper _mapper;

        private readonly IRepository<CostCenterMaster> _costCenterRepository;
        private readonly IRepository<JobMaster> _jobRepository;
        private readonly IRepository<CurrencyMaster> _currencyRepository;
        private readonly IRepository<GeneralSettings> _generalSettingsRepo;
        private readonly IRepository<AccountSettings> _accountSettingRepo;
        private readonly IChartofAccountsService _chartOfAccountsService;

        public PaymentRequestController(
            IPaymentRequestService paymentRequestService,
            IMapper mapper,
            IRepository<CostCenterMaster> costCenterRepository,
            IRepository<JobMaster> jobRepository,
            IRepository<CurrencyMaster> currencyRepository,
            IRepository<GeneralSettings> generalSettingsRepo,
            IRepository<AccountSettings> accountSettingRepo,
            IChartofAccountsService chartOfAccountsService)
        {
            _paymentRequestService = paymentRequestService;
            _mapper = mapper;
            _costCenterRepository = costCenterRepository;
            _jobRepository = jobRepository;
            _currencyRepository = currencyRepository;
            _generalSettingsRepo = generalSettingsRepo;
            _accountSettingRepo = accountSettingRepo;
            _chartOfAccountsService = chartOfAccountsService;
        }

        // ======================== INSERT =========================
        [HttpPost("InsertPaymentRequest")]
        public ApiResponse<PaymentRequest> InsertPaymentRequest([FromBody] PaymentRequest paymentRequest)
        {
            var main = _mapper.Map<PaymentRequest>(paymentRequest);
            var details = _mapper.Map<List<PaymentRequestDetails>>(paymentRequest.PaymentRequestDetails);

            var result = _paymentRequestService.InsertPaymentRequest(main, details);

            return new ApiResponse<PaymentRequest>
            {
                Valid = true,
                Result = result,
                Message = "Payment Request saved successfully."
            };
        }

        // ======================== UPDATE =========================
        [HttpPost("UpdatePaymentRequest")]
        public ApiResponse<PaymentRequest> UpdatePaymentRequest([FromBody] PaymentRequest paymentRequest)
        {
            var main = _mapper.Map<PaymentRequest>(paymentRequest);
            var details = _mapper.Map<List<PaymentRequestDetails>>(paymentRequest.PaymentRequestDetails);

            var result = _paymentRequestService.UpdatePaymentRequest(main, details);

            return new ApiResponse<PaymentRequest>
            {
                Valid = true,
                Result = result,
                Message = "Payment Request updated successfully."
            };
        }

        // ======================== DELETE =========================
        [HttpPost("DeletePaymentRequest")]
        public ApiResponse<int> DeletePaymentRequest([FromBody] PaymentRequest paymentRequest)
        {
            var main = _mapper.Map<PaymentRequest>(paymentRequest);
            var details = _mapper.Map<List<PaymentRequestDetails>>(paymentRequest.PaymentRequestDetails);

            var result = _paymentRequestService.DeletePaymentRequest(main, details);

            return new ApiResponse<int>
            {
                Valid = true,
                Result = result,
                Message = "Payment Request deleted successfully."
            };
        }

        // ======================== GET SAVED DETAILS =========================
        [HttpGet("GetSavedPaymentRequestDetails/{id}")]
        public ApiResponse<PaymentRequest> GetSavedPaymentRequestDetails(int id)
        {
            var request = _paymentRequestService.GetSavedPaymentRequestDetails(id);

            if (request == null)
                return new ApiResponse<PaymentRequest> { Valid = false, Message = "Payment Request not found." };

            return new ApiResponse<PaymentRequest>
            {
                Valid = true,
                Result = request,
                Message = ""
            };
        }
        [HttpGet]
        [Route("GetPaymentRequests")]
        public ApiResponse<List<PaymentRequest>> GetPaymentRequests()
        {
            var result = _paymentRequestService.GetPaymentRequests().ToList();

            return new ApiResponse<List<PaymentRequest>>
            {
                Valid = true,
                Result = result,
                Message = "Payment Requests fetched successfully."
            };
        }
    }
}
