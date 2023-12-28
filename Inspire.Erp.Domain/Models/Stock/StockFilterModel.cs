using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Modals.Stock
{
  public  class StockFilterModel
    {
        public bool IsDateChecked { get; set; }
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public int? ItemId { get; set; }
        public string ItemGroupId { get; set; }
        public string ItemName { get; set; }
        public string PartNo { get; set; }
        public int? LocationId { get; set; }
        public bool OrderByGroup { get; set; }
        public bool ExcludeZeroOpeningBalance { get; set; }
        public bool ExcludeZeroBalance { get; set; }
        public bool PrintReceivedQtyReport { get; set; }


    }
}
