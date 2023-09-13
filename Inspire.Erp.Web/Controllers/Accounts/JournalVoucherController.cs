using Inspire.Erp.Application.Account.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Web.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace Inspire.Erp.Web.Controllers.Accounts
{
    [Route("api/JournalVoucher")]
    [Produces("application/json")]
    [ApiController]
    public class JournalVoucherController : ControllerBase
    {
        private IJournalVoucherService _journalVoucherService;
        private readonly IMapper _mapper;
        public JournalVoucherController(IJournalVoucherService journalVoucherService, IMapper mapper)
        {
            _journalVoucherService = journalVoucherService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("InsertJournalVoucher")]
        public ApiResponse<JournalVoucher> InsertJournalVoucher([FromBody] JournalVoucher voucherCompositeView)
        {
            ApiResponse<JournalVoucher> apiResponse = new ApiResponse<JournalVoucher>();
            var param1 = voucherCompositeView;
            var param2 = voucherCompositeView.JournalVoucherDetails;
            var param3 = voucherCompositeView.AccountsTransactions;
            var xs = _journalVoucherService.InsertJournalVoucher(param1, param3, param2);
            apiResponse = new ApiResponse<JournalVoucher>
            {
                Valid = true,
                Result = _mapper.Map<JournalVoucher>(xs),
                Message = JournalVoucherMessage.SaveVoucher
            };
            return apiResponse;
        }

        [HttpPost]
        [Route("UpdateJournalVoucher")]
        public ApiResponse<JournalVoucher> UpdateJournalVoucher([FromBody] JournalVoucher voucherCompositeView)
        {
            var param1 = _mapper.Map<JournalVoucher>(voucherCompositeView);
            var param2 = _mapper.Map<List<JournalVoucherDetails>>(voucherCompositeView.JournalVoucherDetails);
            var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
            var xs = _journalVoucherService.UpdateJournalVoucher(param1, param3, param2);
            ApiResponse<JournalVoucher> apiResponse = new ApiResponse<JournalVoucher>
            {
                Valid = true,
                Result = _mapper.Map<JournalVoucher>(xs),
                Message = JournalVoucherMessage.UpdateVoucher
            };
            return apiResponse;
        }

        [HttpPost]
        [Route("DeleteJournalVoucher")]
        public ApiResponse<int> DeleteJournalVoucher([FromBody] JournalVoucher voucherCompositeView)
        {
            var param1 = _mapper.Map<JournalVoucher>(voucherCompositeView);
            var param2 = _mapper.Map<List<JournalVoucherDetails>>(voucherCompositeView.JournalVoucherDetails);
            var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
            var xs = _journalVoucherService.DeleteJournalVoucher(param1, param3, param2);
            ApiResponse<int> apiResponse = new ApiResponse<int>
            {
                Valid = true,
                Result = 0,
                Message = JournalVoucherMessage.DeleteVoucher
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
                Result = _mapper.Map<List<AccountsTransactions>>(_journalVoucherService.GetAllTransaction()),
                Message = ""
            };
            return apiResponse;
        }

        [HttpGet]
        [Route("GetAllJournalVoucher")]
        public ApiResponse<IEnumerable<JournalVoucher>> GetAllJournalVoucher()
        {
            ApiResponse<IEnumerable<JournalVoucher>> apiResponse = new ApiResponse<IEnumerable<JournalVoucher>>
            {
                Valid = true,
                Result = _journalVoucherService.GetJournalVoucher(),
                Message = ""
            };
            return apiResponse;
        }

        [HttpGet]
        [Route("GetSavedJournalVoucherDetails/{id}")]
        public ApiResponse<JournalVoucher> GetSavedJournalVoucherDetails(string id)
        {
            JournalVoucher journalVoucher = _journalVoucherService.GetSavedJournalVoucherDetails(id);

            if (journalVoucher != null)
            {
                ApiResponse<JournalVoucher> apiResponse = new ApiResponse<JournalVoucher>
                {
                    Valid = true,
                    Result = journalVoucher,
                    Message = ""
                };
                return apiResponse;
            }
            return null;
        }

        [HttpGet]
        [Route("GetJVTRacking")]
        public IActionResult GetJVTRacking(string JournalVoucher_VNO)
        {
            try
            {
                return Ok(_journalVoucherService.GetJVTRacking(JournalVoucher_VNO));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

        [HttpGet]
        [Route("GenerateVoucherNo")]
        public IActionResult GenerateVoucherNo()
        {
            try
            {
                return Ok(_journalVoucherService.GenerateVoucherNo(null));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("CheckVnoExist/{id}")]
        public ApiResponse<bool> CheckVnoExist(string id)
        {
            ApiResponse<bool> apiResponse = new ApiResponse<bool>
            {
                Valid = true,
                Result = true,
                Message = JournalVoucherMessage.VoucherNumberExist
            };
            var x = _journalVoucherService.GetVouchersNumbers(id);
            if (x == null)
            {
                apiResponse.Result = false;
                apiResponse.Message = "";
            }
            return apiResponse;
        }
    }
}
