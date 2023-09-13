using Inspire.Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Common
{
    public partial class PurchaseOrderModel
    {
        public PurchaseOrder purchaseOrder { get; set; }
        public List<AccountsTransactions> accountsTransactions { get; set; }

        public List<PurchaseOrderDetails> purchaseOrderDetails { get; set; }
        public List<StockRegister> stockRegister { get; set; }
    }
}
