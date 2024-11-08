using AutoMapper;
using Inspire.Erp.Application.RFQ.Interface;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Models;
using Inspire.Erp.Infrastructure.Database.Repositoy;
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
        public RfqController(IMapper mapper, IRepository<RfqEntry> rfqEntryrepository, IRepository<RfqEntryDetails> rfqEntryDetailsrepository)
        {

            _rfqEntryrepository = rfqEntryrepository;
            _rfqEntryDetailsrepository = rfqEntryDetailsrepository;
            _mapper = mapper;
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
        public async Task<ApiResponse<RfqEntry>> InsertRfq([FromBody] RfqEntry RfqEntry)
        {
           var rfq = await _rfq.InsertRfq(RfqEntry);

            ApiResponse<RfqEntry> apiResponse = new ApiResponse<RfqEntry>
            {
                Valid = true,
                Result = _mapper.Map<RfqEntry>(rfq),
                Message = "RFQ has been Saved."
            };
            return apiResponse;
        }
        [HttpPost]
        [Route("UpdatetRfq")]
        public async Task<ApiResponse<RfqEntry>> UpdatetRfq([FromBody] RfqEntry RfqEntry)
        {
            var rfq = await _rfq.InsertRfq(RfqEntry);

            ApiResponse<RfqEntry> apiResponse = new ApiResponse<RfqEntry>
            {
                Valid = true,
                Result = _mapper.Map<RfqEntry>(rfq),
                Message = "RFQ has been Updated."
            };
            return apiResponse;
        }
        [HttpGet]
        [Route("DeleteRfq")]
        public IActionResult DeleteRfq(string Id)
        {
            try
            {
                return Ok(_rfq.DeleteRfq(Id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }
    }
}