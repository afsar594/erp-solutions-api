using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Entities
{
    public class AssetConsumptionReturnDetails
    {
        public string? VoucherNo { get; set; }
        public int? SlNo { get; set; }
        public long? AssetID { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? Days { get; set; }
        public float? Rate { get; set; }
        public float? Amount { get; set; }
        public string? Remarks { get; set; }
        public long? SubAssetID { get; set; }
        public string? ConsVoucherNo { get; set; }
    }

}
