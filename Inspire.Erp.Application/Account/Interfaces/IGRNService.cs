using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.StoreWareHouse.Interface
{
    public interface IGRNService
    {
        public Task<bool> saveGRN(PurchaseVoucher grn, List<AccountsTransactions> accountsTransactions);
        public Task<bool> DeleteGRN(GRNRequest grn);
        public Task<bool> updateGRN(PurchaseVoucher grn, List<AccountsTransactions> accountsTransactions);
        public IEnumerable<GRNResponse> loadPo(GRNRequest obj);
        public IEnumerable<GRNVoucherRquest> saveGRNVoucher(List<GRNVoucherRquest> obj);
        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate);
        public PurchaseVoucher GetByID(string voucherNo);
        public IQueryable GetAllGRN();

    }
}
