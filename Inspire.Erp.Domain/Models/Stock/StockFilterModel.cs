using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Modals.Stock
{
  public  class StockFilterModel
    {
        public int? ItemId { get; set; }
        public string ItemGroupId { get; set; }
        public string ItemName { get; set; }
        public int? LocationId { get; set; }

     }
}
