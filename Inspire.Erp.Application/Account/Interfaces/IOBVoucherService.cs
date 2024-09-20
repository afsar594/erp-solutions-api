using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace Inspire.Erp.Application.Account.Interfaces
{
    public interface IOBVoucherService
    {
        public OBVoucherModel InsertOBVoucher(OpeningVoucherMaster openingVoucher, List<AccountsTransactions> accountsTransactions, List<OpeningVoucherDetails> openingVoucherDetails);
        public OBVoucherModel UpdateOBVoucher(OpeningVoucherMaster openingVoucher, List<AccountsTransactions> accountsTransactions, List<OpeningVoucherDetails> openingVoucherDetails);
        public int DeleteOBVoucher(OpeningVoucherMaster openingVoucher, List<AccountsTransactions> accountsTransactions, List<OpeningVoucherDetails> openingVoucherDetails);
        public IEnumerable<AccountsTransactions> GetAllTransaction();
        public OBVoucherModel GetSavedOBVoucherDetails(string pvno);
        public IEnumerable<OpeningVoucherMaster> GetOBVouchers();
        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate);

        public VouchersNumbers GetVouchersNumbers(string pvno);
        public Task<ResponseInfo> LoadAccounts();

    }
}
