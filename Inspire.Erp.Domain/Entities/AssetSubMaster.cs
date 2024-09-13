using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Entities
{
    public class AssetSubMaster
    {
        public long? GroupID { get; set; }
        public long SubAssetID { get; set; }
        public float? SlNo { get; set; }
        public string? AssetDescription { get; set; }
        public string? AssetCode { get; set; }
        public DateTime? PurchDate { get; set; }
        public float? PurchPrice { get; set; }
        public float? DepPerc { get; set; }
        public DateTime? DepDate { get; set; }
        public float? Days { get; set; }
        public float? PerDay { get; set; }
        public float? AccDep { get; set; }
        public float? BookVaue { get; set; }
        public string? VoucherNo { get; set; }
        public bool IsEdit { get; set; } = false;
        public string? Remarks { get; set; }
    }

}
