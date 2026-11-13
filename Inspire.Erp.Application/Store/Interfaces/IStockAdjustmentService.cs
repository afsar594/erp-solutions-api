using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Modals;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Inspire.Erp.Domain.Modals.Stock;

namespace Inspire.Erp.Application.Store.Interfaces
{
    public interface IStockAdjustmentService
    {
        public IEnumerable<StockAdjustmentVoucher> GetStockAdjustment();
        public Task<StockAdjustmentVoucher> InsertStockAdjustmentVoucher(StockAdjustmentVoucher stockAdjustmentVoucher, List<StockAdjustmentVoucherDetails> stockAdjustmentVoucherDetails, List<AccountsTransactions> AccountsTransactions);
        public Task<StockAdjustmentVoucher> UpdateStockAdjustmentVoucher(StockAdjustmentVoucher stockAdjustmentVoucher, List<StockAdjustmentVoucherDetails> stockAdjustmentVoucherDetails);
        public StockAdjustmentVoucher GetSavedStockAdjustmentferDetails(int id);
        public ResponseInfo GetStockByLocation(int id,DateTime asOnDate);
        public int DeleteStockAdjustmentVoucher(StockAdjustmentVoucher stockAdjustmentVoucher, List<StockAdjustmentVoucherDetails> stockAdjustmentVoucherDetails);

        Task<Response<List<StockAdjustmentReportResponse>>> StockAdjustmentReport(StockAdjustmentReportFilter model);

        IEnumerable<AccountSettings> GetAllAccountSettings();
        IEnumerable<AccountSettings> GetAllAccountStock();
    }
}
