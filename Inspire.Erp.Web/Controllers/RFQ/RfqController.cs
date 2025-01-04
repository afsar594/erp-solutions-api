using AutoMapper;
using Inspire.Erp.Application.RFQ.Interface;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Models;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Inspire.Erp.Web.ViewModels;
using Inspire.Erp.Web.ViewModels.sales;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Inspire.Erp.Web.Controllers
{
    [Route("api")]
    [Produces("application/json")]
    [ApiController]
    public class RfqController : ControllerBase
    {
        private IRepository<RfqEntry> _rfqEntryrepository;
        private IRepository<RfqEntryDetails> _rfqEntryDetailsrepository;
        private IRfqService _rfq;
        private readonly IMapper _mapper;
        public RfqController(IMapper mapper, IRepository<RfqEntry> rfqEntryrepository, IRepository<RfqEntryDetails> rfqEntryDetailsrepository, IRfqService rfq)
        {

            _rfqEntryrepository = rfqEntryrepository;
            _rfqEntryDetailsrepository = rfqEntryDetailsrepository;
            _mapper = mapper;
            _rfq = rfq;
        }
        [HttpGet]
        [Route("GetRfq")]
        public ApiResponse<List<RfqEntry>> GetRfq()
        {
            ApiResponse<List<RfqEntry>> apiResponse = new ApiResponse<List<RfqEntry>>
            {
                Valid = true,
                Result = _mapper.Map<List<RfqEntry>>(_rfq.GetRfq()),
                Message = ""
            };
            return apiResponse;
        }
        [HttpGet]
        [Route("GetRfqById")]
        public IActionResult GetRfqById(int id)
        {
            try
            {
                var item = _rfq.GetRfqById(id);
                return Ok(item);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }
        [HttpPost]
        [Route("InsertRfq")]
        public async Task<ApiResponse<RFQViewModel>> InsertRfq([FromBody] RFQViewModel RfqEntryModule)
        {

            var param1 = _mapper.Map<RfqEntry>(RfqEntryModule);
            var param2 =_mapper.Map<List<RfqEntryDetails>>(RfqEntryModule.RfqEntryDetailViewModel);

            var rfq = await _rfq.InsertRfq(param1, param2);

            ApiResponse<RFQViewModel> apiResponse = new ApiResponse<RFQViewModel>
            {
                Valid = true,
                Result = _mapper.Map<RFQViewModel>(rfq),
                Message = "RFQ has been Saved."
            };
            return apiResponse;
        }
        [HttpPost]
        [Route("UpdatetRfq")]
        public async Task<ApiResponse<RFQViewModel>> UpdatetRfq([FromBody] RFQViewModel RfqEntryModule)

        {
            var param1 = _mapper.Map<RfqEntry>(RfqEntryModule);
            var param2 = _mapper.Map<List<RfqEntryDetails>>(RfqEntryModule.RfqEntryDetailViewModel);
            var rfq = await _rfq.UpdatetRfq(param1, param2);
            ApiResponse<RFQViewModel> apiResponse = new ApiResponse<RFQViewModel>
            {
                Valid = true,
                Result = _mapper.Map<RFQViewModel>(rfq),
                Message = "RFQ has been Updated."
            };
            return apiResponse;
        }
        [HttpGet]
        [Route("DeleteRfq")]
        public IActionResult DeleteRfq(string vouchaseNo)
        {
            try
            {
                return Ok(_rfq.DeleteRfq(vouchaseNo));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }
    }
}