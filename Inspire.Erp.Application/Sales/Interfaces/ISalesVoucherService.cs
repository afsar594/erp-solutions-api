using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Modals.Sales;
using Inspire.Erp.Domain.Modals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inspire.Erp.Domain.Entities.POS;
using Inspire.Erp.Domain.Models;
using Inspire.Erp.Domain.DTO;

namespace Inspire.Erp.Application.Sales.Interfaces
{
    public interface ISalesVoucherService
    {


        public IEnumerable<DepartmentMaster> SalesVoucher_GetAllDepartmentMaster();
        public object GetSalesReportDetailWise(salesReportFilter filter);
        public object GetSalesReportSummaryWise(salesReportFilter filter );

        public IEnumerable<CustomerMaster> SalesVoucher_GetAllCustomerMaster();

        public IEnumerable<SalesManMaster> SalesVoucher_GetAllSalesManMaster();



        Task<Response<GridWrapperResponse<List<GetSalesVoucherResponse>>>> GetSalesVoucher(GenericGridViewModel model);
        public IEnumerable<ItemMaster> SalesVoucher_GetAllItemMaster();
        public IEnumerable<UnitMaster> SalesVoucher_GetAllUnitMaster();

        public IEnumerable<SuppliersMaster> SalesVoucher_GetAllSuppliersMaster();
        public IEnumerable<LocationMaster> SalesVoucher_GetAllLocationMaster();
        

        public Task<SalesVoucherModel>  InsertSalesVoucher(SalesVoucher salesVoucher, List<AccountsTransactions> accountsTransactions, List<SalesVoucherDetails> salesVoucherDetails
            , List<StockRegister> stockRegister
            , List<SalesCharges> salesChargesList
            );
        Task<VanSales> InsertVanSalesAsync(VanSales model);


        public Task<SalesVoucherModel> UpdateSalesVoucher(SalesVoucher salesVoucher, List<AccountsTransactions> accountsTransactions, List<SalesVoucherDetails> salesVoucherDetails
            , List<StockRegister> stockRegister
            , List<SalesCharges> salesChargesList
            );
        public int DeleteSalesVoucher(SalesVoucher salesVoucher, List<AccountsTransactions> accountsTransactions, List<SalesVoucherDetails> salesVoucherDetails
            , List<StockRegister> stockRegister
            );
        public IEnumerable<AccountsTransactions> GetAllTransaction();
        public SalesVoucherModel GetSavedSalesVoucherDetails(decimal pvno);
        public SalesVoucherModel GetSavedSalesVoucherDetailsBySaleVoucher(string svNo);
        public IEnumerable<SalesVoucher> GetSalesVoucher();
        public Task<Response<GridWrapperResponse<List<SalesVoucher>>>> GetSalesVoucherList(GenericGridViewModel model);
        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate);
        public VouchersNumbers GetVouchersNumbers(string pvno);
        public SalesVoucher GetSalesVoucherDetailsByMasterNo(string SalesVoucherNo);
        public Task<Response<(List<decimal> PurchasePrices, List<decimal> SalesPrices)>> GetLast3PurchaseAndSalesPricesAsync(int itemId, int? locationId);


        #region POS sales voucher
        //public VouchersNumbers GeneratePosVoucherNo(DateTime? VoucherGenDate);
        //public POS_Sales_Voucher InsertPosSalesVoucher(POS_Sales_Voucher salesVoucher, List<POS_Sales_Voucher_Details> PosSalesVoucherDetails);
        //public POS_Sales_Voucher UpdatePosSalesVoucher(POS_Sales_Voucher salesVoucher, List<POS_Sales_Voucher_Details> PosSalesVoucherDetails);
        //public POS_Sales_Voucher GetSavedPosSalesVoucherDetails(string pvno);
        //public IEnumerable<POS_Sales_Voucher> GetPosSalesVoucher();
        #endregion


        #region POS Sales Hold

        public VouchersNumbers GeneratePosSaleHoldVoucherNo(DateTime? VoucherGenDate);
        public SalesHold InsertPosSalesHold(SalesHold salesHold, List<SalesHoldDetails> salesHoldDetails);
        public List<SalesHold> DeletePosSalesHold(decimal id);
        public SalesHold GetPosSalesHold(decimal id);

        public IEnumerable<SalesHold> GetPosSalesHoldList();



        #endregion


        public POS_WorkPeriod StartPosWP(POS_WorkPeriod startWP);
        public POS_WorkPeriod EndPosWP(POS_WorkPeriod endWP);


        Task<ApiResponse<List<SalesVoucherDto>>> GetSalesVoucherForGrnBrowse(GeneralFilter model);
        IQueryable GetCustomerDetailWiseSalesReport(int? customerId, DateTime? fromDate, DateTime? toDate);
        IQueryable GetCustomerMaterialWiseSalesReport(int? customerId, DateTime? fromDate, DateTime? toDate);
        IQueryable GetCustomerSummaryWiseSalesReport(int? customerId, DateTime? fromDate, DateTime? toDate);
    }
}
