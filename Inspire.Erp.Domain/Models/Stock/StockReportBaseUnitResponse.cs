using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Modals.Stock
{
   public class StockReportBaseUnitResponse
    {
        public int? StockRegisterMaterialId { get; set; }
        public string ItemMasterItemName { get; set; }
        public int? PartNo { get; set; }
        public int? Stock { get; set; }
        public string ItemMasterPartNo { get; set; }
   
    }
 
}
