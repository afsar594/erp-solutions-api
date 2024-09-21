using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Models.ViewModels
{
    public class AssetSubMasterListVM
    {
       
            public long? GroupID { get; set; }
            public long SubAssetID { get; set; }
            public double? SlNo { get; set; }  // Changed from float? to double?
            public string? AssetDescription { get; set; }
            public string? AssetCode { get; set; }
            public DateTime? PurchDate { get; set; }
            public double? PurchPrice { get; set; }  // Changed from float? to double?
            public double? DepPerc { get; set; }     // Changed from float? to double?
            public DateTime? DepDate { get; set; }
            public double? Days { get; set; }        // Changed from float? to double?
            public double? PerDay { get; set; }      // Changed from float? to double?
            public double? AccDep { get; set; }      // Changed from float? to double?
            public double? BookVaue { get; set; }    // Changed from float? to double?
            public string? VoucherNo { get; set; }
            public bool IsEdit { get; set; } = false;
            public string? Remarks { get; set; }
        
    }
}
