using Inspire.Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static Inspire.Erp.Domain.Entities.StoreWareHouse;

namespace Inspire.Erp.Application.Common
{
    public partial class SRTransferViewModel
    {
        public int StockRegisterMaterialID { get; set; }
        public int? StockRegisterQuantity { get; set; }
        public int? StockRegisterRate { get; set; }
        public int? StockRegisterUnitID { get; set; }
        public List<UnitDetailsViewModel> UnitDetails { get; set; }

    }
}
