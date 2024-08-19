using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Account.Interfaces
{
    public interface IBankReconcilation
    {
        public Task<List<MasterAccountsTable>> GetAccountListForDropdown();
        public Task<IEnumerable<BankLedger>> BankLedger(string bankAccNo);
        public Task<Response<bool>> SavebankReconciliation(ReconciliationVoucher reconciliation);

    }
}
