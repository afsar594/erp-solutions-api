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
using Inspire.Erp.Web.ViewModels;
using Inspire.Erp.Domain.Enums;

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
        //public IActionResult InsertJobInvoice([FromBody] JobInvoice JobInvoiceViewModel)
        //{
        //    try
        //    {
        //        return Ok(_invoice.InsertJobInvoice(JobInvoiceViewModel));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message.ToString());
        //    }
        //}
        public ApiResponse<JobInvoice> InsertJobInvoice([FromBody] JobInvoice JobInvoiceViewModel)
        {

            var param1 = _mapper.Map<JobInvoice>(JobInvoiceViewModel);
            var InsertJobInvoiceViewModel = _invoice.InsertJobInvoice(param1);

            ApiResponse<JobInvoice> apiResponse = new ApiResponse<JobInvoice>
            {
                Valid = true,
                Result = _mapper.Map<JobInvoice>(InsertJobInvoiceViewModel),
                Message = JobInvoiceMessage.SaveJobInvoice
            };
            return apiResponse;
        }
        [HttpPost]
        [Route("UpdateJobInvoice")]
        //public IActionResult UpdateJobInvoice([FromBody] JobInvoice JobInvoiceViewModel)
        //{
        //    try
        //    {
        //        return Ok(_invoice.UpdateJobInvoice(JobInvoiceViewModel));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message.ToString());
        //    }
        //}
        public ApiResponse<JobInvoice> UpdateJobInvoice([FromBody] JobInvoice JobInvoiceViewModel)
        {
            var param1 = _mapper.Map<JobInvoice>(JobInvoiceViewModel);
            var UpdateJobInvoiceViewModel = _invoice.UpdateJobInvoice(param1);


            ApiResponse<JobInvoice> apiResponse = new ApiResponse<JobInvoice>
            {
                Valid = true,
                Result = _mapper.Map<JobInvoice>(UpdateJobInvoiceViewModel),
                Message = JobInvoiceMessage.UpdateJobInvoice

            };
            return apiResponse;

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
        //[HttpGet]
        //[Route("GetJobInvoiceVoucherNo")]
        //public IActionResult GetJobInvoiceVoucherNo(string VNo)
        //{
        //    try

        //    {
               
        //        return Ok(_invoice.GetJobInvoiceVoucherNo(VNo));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message.ToString());
        //    }
        //}

        [HttpGet]
        [Route("GetJobInvoiceVoucherNo/{id}")]
        public ApiResponse<JobInvoice> GetJobInvoiceVoucherNo(string id)
        {
            JobInvoice salesJournal = _invoice.GetJobInvoiceVoucherNo(id);

            if (salesJournal != null)
            {
               ApiResponse<JobInvoice> apiResponse = new ApiResponse<JobInvoice>
                {
                    Valid = true,
                    Result = salesJournal,
                    Message = ""
                };
                return apiResponse;
            }
            return null;
        }
        [HttpGet]
        [Route("GenerateVoucherNo")]
        public IActionResult GenerateVoucherNo()
        {
            try
            {
                var item = _invoice.GenerateVoucherNo(null);
                return Ok(item);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

    }
}
