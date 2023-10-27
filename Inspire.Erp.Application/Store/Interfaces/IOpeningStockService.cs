using System;
using System.Collections.Generic;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Application.Common;
using System.Text;

namespace Inspire.Erp.Application.Store.Interfaces
{
    public interface IOpeningStockService
    {
        public OpeningStockModel InsertOpeningStock(OpeningStock openingStock, List<AccountsTransactions> accountsTransactions, List<StockRegister> stockRegister);
        public OpeningStockModel UpdateOpeningStock(OpeningStock openingStock, List<AccountsTransactions> accountsTransactions, List<StockRegister> stockRegister);
        public int DeleteOpeningStock(OpeningStock openingStock, List<AccountsTransactions> accountsTransactions, List<StockRegister> stockRegister);
        public IEnumerable<AccountsTransactions> GetAllTransaction();
        public OpeningStockModel GetSavedOPStockDetails(string pvno);
        public IEnumerable<OpeningStock> GetOpeningStocks();
        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate);

        public VouchersNumbers GetVouchersNumbers(string pvno);

        public OpeningStockVoucherModel InsertOpeningStockVoucher(OpeningStockVoucher openstockVoucher, List<AccountsTransactions> accountsTransactions, List<OpeningStockVoucherDetails> openstockVoucherDetails
        , List<StockRegister> stockRegister
       );
    }
}
