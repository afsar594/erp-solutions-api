using Inspire.Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static Inspire.Erp.Domain.Entities.StoreWareHouse;

namespace Inspire.Erp.Application.Common
{
    public partial class StockTransferModel
    {
        public StockTransfer stockTransfer { get; set; }
        public List<AccountsTransactions> accountsTransactions { get; set; }

        public List<StockTransferDetails> stockTransferDetails { get; set; }
        public List<StockRegister> stockRegister { get; set; }
    }
    public partial class SRTransferViewModel
    {
        public int StockRegisterMaterialID { get; set; }
        public int? StockRegisterQuantity { get; set; }
        public int? StockRegisterRate { get; set; }
        public int? StockRegisterUnitID { get; set; }
        public List<UnitDetailsViewModel> UnitDetails { get; set; }

    }
}
