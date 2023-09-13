using Inspire.Erp.Application.Account.Interfaces;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobInvoiceController : ControllerBase
    {
        private IJobInvoice _invoice;
        public JobInvoiceController(IJobInvoice invoice)
        {
            _invoice = invoice; 
        }
        [HttpGet("GetSalesAccount")]
        public async Task<List<MasterAccountsTable>> GetSalesAccount() {
            return await _invoice.GetSalesAccount();
        }
        [HttpGet("GetJobInvoice")]
        public async Task<List<JobInvoiceModel>> GetJobInvoice()
        {
            return await _invoice.GetJobInvoice();
        }
        [HttpPost("InsertJobInvoice")]
        public async Task<bool> InsertJobInvoice(JobInvoiceRequest obj)
        {
            return await _invoice.InsertJobInvoice(obj);
        }
        [HttpPost("UpdateJobInvoice")]
        public async Task<bool> UpdateJobInvoice(JobInvoiceRequest obj)
        {
            return await _invoice.UpdateJobInvoice(obj);
        }
        [HttpPost("DeleteJobInvoice")]
        public async Task<bool> DeleteJobInvoice(JobInvoiceRequest obj)
        {
            return await _invoice.DeleteJobInvoice(obj);
        }
    }
}
