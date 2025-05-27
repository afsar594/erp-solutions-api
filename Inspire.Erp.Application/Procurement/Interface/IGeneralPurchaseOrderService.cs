using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Application.Account.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Inspire.Erp.Domain.Modals;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Procurement.Interfaces
{
    public interface IGeneralPurchaseOrderService
    {
        public IEnumerable<UserFile> GetAllUserFile();
        public IEnumerable<ItemMaster> GeneralPurchaseOrder_GetAllItemMaster();
        public IEnumerable<UnitMaster> GeneralPurchaseOrder_GetAllUnitMaster();
        public IEnumerable<SuppliersMaster> GeneralPurchaseOrder_GetAllSuppliersMaster();
        public IEnumerable<LocationMaster> GeneralPurchaseOrder_GetAllLocationMaster();
        public Task<Response<GeneralPurchaseOrderModel>> InsertGeneralPurchaseOrder(GeneralPurchaseOrder generalpurchaseOrder, List<AccountsTransactions> accountsTransactions, List<GeneralPurchaseOrderDetails> GeneralpurchaseOrderDetails);
        public Task<Response<GeneralPurchaseOrderModel>> UpdateGeneralPurchaseOrder(GeneralPurchaseOrder generalpurchaseOrder, List<AccountsTransactions> accountsTransactions, List<GeneralPurchaseOrderDetails> GeneralpurchaseOrderDetails);
        public int DeleteGeneralPurchaseOrder(GeneralPurchaseOrder generalpurchaseOrder, List<AccountsTransactions> accountsTransactions, List<GeneralPurchaseOrderDetails> GeneralpurchaseOrderDetails);
        public IEnumerable<AccountsTransactions> GetAllTransaction();
        public Task<Response<GeneralPurchaseOrderModel>> GetSavedGeneralPurchaseOrderDetails(string pvno);
        public IEnumerable<GeneralPurchaseOrder> GetGeneralPurchaseOrder();
        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate);
        public VouchersNumbers GetVouchersNumbers(string pvno);
        public VouchersNumbers GetVouchersNumbers(string pvno, string vType);
        public IQueryable GetGPODetailsForGRN();

    }
}
