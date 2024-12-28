using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Account.Interface
{
    public interface IJobInvoice
    {
        public IEnumerable<JobInvoice> Getjobinvoice();
        public JobInvoice GetJobInvoiceById(int? id);
        public Task<JobInvoiceModel> InsertJobInvoice(JobInvoice JobInvoiceViewModel, List<JobInvoiceDetails> jobInvoiceDetails, List<AccountsTransactions> accountsTransactions);
        public Task<JobInvoiceModel> UpdateJobInvoice(JobInvoice JobInvoiceViewModel, List<JobInvoiceDetails> jobInvoiceDetails, List<AccountsTransactions> accountsTransactions);
        public IEnumerable<JobInvoice> DeleteJobInvoice(string Id);
        public JobInvoiceModel GetJobInvoiceVoucherNo(int id);
        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate);

    }
}
