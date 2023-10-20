using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Modals.Sales;
using Inspire.Erp.Domain.Modals;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Sales.Interfaces
{
    public interface ISalesVoucherService
    {
 

        public IEnumerable<DepartmentMaster> SalesVoucher_GetAllDepartmentMaster();



        public IEnumerable<CustomerMaster> SalesVoucher_GetAllCustomerMaster();

        public IEnumerable<CustomerMaster> SalesVoucher_GetAllSalesManMaster();



        Task<Response<GridWrapperResponse<List<GetSalesVoucherResponse>>>> GetSalesVoucher(GenericGridViewModel model);
        public IEnumerable<ItemMaster> SalesVoucher_GetAllItemMaster();
        public IEnumerable<UnitMaster> SalesVoucher_GetAllUnitMaster();

        public IEnumerable<SuppliersMaster> SalesVoucher_GetAllSuppliersMaster();
        public IEnumerable<LocationMaster> SalesVoucher_GetAllLocationMaster();

        //public SalesVoucher InsertSalesVoucher(SalesVoucher salesVoucher,
        //  List<SalesVoucherDetails> salesVoucherDetails);

        public SalesVoucherModel InsertSalesVoucher(SalesVoucher salesVoucher, List<AccountsTransactions> accountsTransactions, List<SalesVoucherDetails> salesVoucherDetails
            , List<StockRegister> stockRegister
            );
        public SalesVoucherModel UpdateSalesVoucher(SalesVoucher salesVoucher, List<AccountsTransactions> accountsTransactions, List<SalesVoucherDetails> salesVoucherDetails
            , List<StockRegister> stockRegister
            );
        public int DeleteSalesVoucher(SalesVoucher salesVoucher, List<AccountsTransactions> accountsTransactions, List<SalesVoucherDetails> salesVoucherDetails
            , List<StockRegister> stockRegister
            );
        public IEnumerable<AccountsTransactions> GetAllTransaction();
  

        
        public SalesVoucherModel GetSavedSalesVoucherDetails(string pvno);
        public IEnumerable<SalesVoucher> GetSalesVoucher();
        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate);

        public VouchersNumbers GetVouchersNumbers(string pvno);

    }
}
