using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Account.Interfaces
{
    public interface ISalesReturnService
    {
 

        ////public IEnumerable<DepartmentMaster> SalesReturn_GetAllDepartmentMaster();



        ////public IEnumerable<CustomerMaster> SalesReturn_GetAllCustomerMaster();

        ////public IEnumerable<CustomerMaster> SalesReturn_GetAllSalesManMaster();



        //////public IEnumerable<ReportSalesReturn> SalesReturn_GetReportSalesReturn();
        ////public IEnumerable<ItemMaster> SalesReturn_GetAllItemMaster();
        ////public IEnumerable<UnitMaster> SalesReturn_GetAllUnitMaster();

        ////public IEnumerable<SuppliersMaster> SalesReturn_GetAllSuppliersMaster();
        ////public IEnumerable<LocationMaster> SalesReturn_GetAllLocationMaster();

        //public SalesReturn InsertSalesReturn(SalesReturn salesReturn,
        //  List<SalesReturnDetails> salesReturnDetails);

        public SalesReturnModel InsertSalesReturn(SalesReturn salesReturn, List<AccountsTransactions> accountsTransactions, List<SalesReturnDetails> salesReturnDetails
            , List<StockRegister> stockRegister
            );
        public SalesReturnModel UpdateSalesReturn(SalesReturn salesReturn, List<AccountsTransactions> accountsTransactions, List<SalesReturnDetails> salesReturnDetails
           , List<StockRegister> stockRegister
            );
        public int DeleteSalesReturn(SalesReturn salesReturn, List<AccountsTransactions> accountsTransactions, List<SalesReturnDetails> salesReturnDetails
             , List<StockRegister> stockRegister
            );
        public IEnumerable<AccountsTransactions> GetAllTransaction();
  

        
        public SalesReturnModel GetSavedSalesReturnDetails(int pvno);
        public List<SalesReturn> GetSalesReturn();
        public Task<Response<GridWrapperResponse<List<SalesReturn>>>> GetSalesReturnList(GenericGridViewModel model);
        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate);

        public VouchersNumbers GetVouchersNumbers(string pvno);

    }
}
