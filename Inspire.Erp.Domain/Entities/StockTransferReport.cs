using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspire.Erp.Domain.Entities
{
    public partial class StockTransferReport
    {

        public string StockTransferVoucherNo { get; set; }
        public DateTime? StockTransferStDate { get; set; }
        public string ItemMasteritemName { get; set; }
     

    }
    public partial class StockTransferParamter
    {

      
        public string? StocklcationTo { get; set; }
        public string? StocklcationFrom { get; set; }
    }
}
