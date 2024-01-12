using Inspire.Erp.Domain.Modals;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Inspire.Erp.Domain.Modals.PDCResponse;

namespace Inspire.Erp.Application.Account.Interface
{
    public interface IPDC
    {
        public Task<IEnumerable<PDCPostedList>> PDCPostedList();
        public Task<IEnumerable<PDCGetList>> PDCGetList();
        public Task<IEnumerable<PDCGetList>> getFilteredPDCList(PDCFilters obj);
        public Task<List<BankNamesModel>> GetBankNames();
        Task<IEnumerable<PDCGetList>> PDCGetReturnList();
    }
}
