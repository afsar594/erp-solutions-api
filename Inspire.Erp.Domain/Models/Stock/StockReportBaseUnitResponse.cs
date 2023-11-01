using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Modals.Stock
{
   public class StockReportBaseUnitResponse
    {
        public int? ItemId { get; set; }

        public string PartNo { get; set; }
        public string ItemName { get; set; }
        public int? Stock { get; set; }
        public int? Unit { get; set; }
        public decimal? AverageRate { get; set; }
        public decimal? Amount { get; set; }

    }
 
}
