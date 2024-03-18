using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Modals.Stock
{
   public class StockReportBaseUnitResponse
    {
        public long? ItemId { get; set; }

        public string PartNo { get; set; }
        public string ItemName { get; set; }
        public int? Stock { get; set; }
        public int? Unit { get; set; }
        public decimal? AverageRate { get; set; }
        public decimal? AveragePurchaseValue { get; set; }
        public decimal? Amount { get; set; }

    }
 
}
