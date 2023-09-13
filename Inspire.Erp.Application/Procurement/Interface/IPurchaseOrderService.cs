using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Application.Account.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Procurement.Interfaces
{
    public interface IPurchaseOrderService
    {
        
            public IEnumerable<UserFile> GetAllUserFile();
        //public IEnumerable<ReportPurchaseOrder> PurchaseOrder_GetReportPurchaseOrder();
        public IEnumerable<ItemMaster> PurchaseOrder_GetAllItemMaster();
        public IEnumerable<UnitMaster> PurchaseOrder_GetAllUnitMaster();

        public IEnumerable<SuppliersMaster> PurchaseOrder_GetAllSuppliersMaster();
        public IEnumerable<LocationMaster> PurchaseOrder_GetAllLocationMaster();

        //public PurchaseOrder InsertPurchaseOrder(PurchaseOrder purchaseOrder,
        //  List<PurchaseOrderDetails> purchaseOrderDetails);

        public PurchaseOrderModel InsertPurchaseOrder(PurchaseOrder purchaseOrder, List<AccountsTransactions> accountsTransactions, List<PurchaseOrderDetails> purchaseOrderDetails
            //, List<StockRegister> stockRegister
            );
        public PurchaseOrderModel UpdatePurchaseOrder(PurchaseOrder purchaseOrder, List<AccountsTransactions> accountsTransactions, List<PurchaseOrderDetails> purchaseOrderDetails
            //, List<StockRegister> stockRegister
            );
        public int DeletePurchaseOrder(PurchaseOrder purchaseOrder, List<AccountsTransactions> accountsTransactions, List<PurchaseOrderDetails> purchaseOrderDetails
            //, List<StockRegister> stockRegister
            );
        public IEnumerable<AccountsTransactions> GetAllTransaction();
  

        
        public PurchaseOrderModel GetSavedPurchaseOrderDetails(string pvno);
        public IEnumerable<PurchaseOrder> GetPurchaseOrder();
        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate);

        public VouchersNumbers GetVouchersNumbers(string pvno);
        public VouchersNumbers GetVouchersNumbers(string pvno,string vType);

    }
}
