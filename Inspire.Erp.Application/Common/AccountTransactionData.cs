using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Models;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inspire.Erp.Application.Common
{
    public class AccountTransactionData
    {

        private IRepository<AllocationVoucherDetails> allocationVDlts;
        private IRepository<AccountsTransactions> _accountTransactionRepository;

        public AccountTransactionData(IRepository<AllocationVoucherDetails> allocationVDlts, IRepository<AccountsTransactions> accountTransactionRepository)
        {
            this.allocationVDlts = allocationVDlts;
            _accountTransactionRepository = accountTransactionRepository;
        }

        
    }
}
