using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;

namespace Inspire.Erp.Application.Master.Interfaces
{
    public interface IPurchaseVoucherService
    {
        public IEnumerable<PurchaseVoucher> GetPurchaseVoucher();
        public PurchaseVoucherModel InsertPurchaseVoucher(PurchaseVoucher purchaseVoucher,
                                                                  List<AccountsTransactions> accountsTransactions,
                                                                  List<PurchaseVoucherDetails> salesVoucherDetails, 
                                                                  List<StockRegister> stockRegister);
        public PurchaseVoucherModel UpdatePurchaseVoucher(PurchaseVoucher purchaseVoucher, List<AccountsTransactions> accountsTransactions, List<PurchaseVoucherDetails> purchaseVoucherDetails
            , List<StockRegister> stockRegister
            );
        //public IEnumerable<PurchaseVoucher> DeletePurchaseVoucher(PurchaseVoucher DataObj);

        public int DeletePurchaseVoucher(PurchaseVoucher purchaseVoucher, List<AccountsTransactions> accountsTransactions, List<PurchaseVoucherDetails> purchaseVoucherDetails
           , List<StockRegister> stockRegister
           );
        public IEnumerable<PurchaseVoucherDetails> GetPurchaseVoucherDetails();

        public PurchaseVoucherModel GetSavedPurchaseVoucherDetails(string id);
        public IQueryable GetPurchaseReturnReport();

        public VouchersNumbers GetVouchersNumbers(string pvno);
    }
}