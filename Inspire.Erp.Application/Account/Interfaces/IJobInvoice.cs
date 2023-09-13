using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Account.Interfaces
{
    public interface IJobInvoice
    {
        public Task<List<MasterAccountsTable>> GetSalesAccount();
        public Task<List<JobInvoiceModel>> GetJobInvoice();
        public Task<bool> InsertJobInvoice(JobInvoiceRequest obj);
        public Task<bool> UpdateJobInvoice(JobInvoiceRequest obj);
        public Task<bool> DeleteJobInvoice(JobInvoiceRequest obj);
    }
}
