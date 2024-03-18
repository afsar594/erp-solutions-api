using AutoMapper;
using Inspire.Erp.Application.Account.Interfaces;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Application.Sales.Interface;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Inspire.Erp.Application.Account.Interface;
using Inspire.Erp.Application.Account.Implementations;
using Inspire.Erp.Domain.Models;

namespace Inspire.Erp.Web.Controllers
{
    [ApiController]
    [Route("api/JobInvoice")]
    public class JobInvoiceController : ControllerBase
    {
        private IJobInvoice _invoice;
        private IMapper _mapper;
        public JobInvoiceController(IJobInvoice invoice, IMapper mapper)
        {
            _invoice = invoice;
            _mapper= mapper;
        }      
        [HttpGet]
        [Route("GetJobInvoice")]
        public ApiResponse<List<JobInvoice>> Getjobinvoice()
        {
            ApiResponse<List<JobInvoice>> apiResponse = new ApiResponse<List<JobInvoice>>
            {
                Valid = true,
                Result = _mapper.Map<List<JobInvoice>>(_invoice.Getjobinvoice()),
                Message = ""
            };
            return apiResponse;
        }
        [HttpGet]
        [Route("GetJobInvoiceById")]
        public IActionResult GetJobInvoiceById(int id)
        {
            try
            {
                var item = _invoice.GetJobInvoiceById(id);
                return Ok(item);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }
        [HttpPost]
        [Route("InsertJobInvoice")]
        public IActionResult InsertJobInvoice([FromBody] JobInvoice JobInvoiceViewModel)
        {
            try
            {
                return Ok(_invoice.InsertJobInvoice(JobInvoiceViewModel));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }
        [HttpPost]
        [Route("UpdateJobInvoice")]
        public IActionResult UpdateJobInvoice([FromBody] JobInvoice JobInvoiceViewModel)
        {
            try
            {
                return Ok(_invoice.UpdateJobInvoice(JobInvoiceViewModel));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }
        [HttpDelete]
        [Route("DeleteJobInvoice")]
        public IActionResult DeleteJobInvoice(int Id)
        {
            try
            {
                return Ok(_invoice.DeleteJobInvoice(Id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }
        [HttpGet]
        [Route("GetJobInvoiceVoucherNo")]
        public IActionResult GetJobInvoiceVoucherNo(string VNo)
        {
            try
            {
                return Ok(_invoice.GetJobInvoiceVoucherNo(VNo));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }
        //[HttpGet("GetSalesAccount")]
        //public async Task<List<MasterAccountsTable>> GetSalesAccount() {
        //    return await _invoice.GetSalesAccount();
        //}
        //[HttpGet("GetJobInvoice")]
        //public async Task<List<JobInvoiceModel>> GetJobInvoice()
        //{
        //    return await _invoice.GetJobInvoice();
        //}

        //[HttpPost("InsertJobInvoice")]
        //public async Task<bool> InsertJobInvoice(JobInvoiceRequest obj)
        //{
        //    return await _invoice.InsertJobInvoice(obj);
        //}
        //[HttpPost("UpdateJobInvoice")]
        //public async Task<bool> UpdateJobInvoice(JobInvoiceRequest obj)
        //{
        //    return await _invoice.UpdateJobInvoice(obj);
        //}
        //[HttpPost("DeleteJobInvoice")]
        //public async Task<bool> DeleteJobInvoice(JobInvoiceRequest obj)
        //{
        //    return await _invoice.DeleteJobInvoice(obj);
        //}
        //[HttpGet]
        //[Route("GetSaleVoucherNo")]
        //public IActionResult GetSaleVoucherNo(string VNo)
        //{
        //    try
        //    {
        //        return Ok(_invoice.GetSaleVoucherNo(VNo));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message.ToString());
        //    }
        //}

    }
}
