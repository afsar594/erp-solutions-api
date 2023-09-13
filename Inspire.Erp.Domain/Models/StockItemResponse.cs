using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Modals
{
    public class StockItemResponse
    {
        public long Item_Master_Item_Id { get; set; }
        public string? Item_Master_Item_Name { get; set; }
        public string? Item_Master_Batch_Code { get; set; }
        public string? Item_Master_Part_No { get; set; }
        public string? Item_Master_Barcode { get; set; }
        public decimal? Stock { get; set; }
    }
}
