using Inspire.Erp.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Inspire.Erp.Domain.Models.ViewModels
{
    public class AssetSubMasterViewModel
    {
        //public string VoucherNo { get; set; }      // VARCHAR(30) NULL
        //public DateTime? VoucherDate { get; set; } // DATETIME NULL
        //public string RefNo { get; set; }          // VARCHAR(100) NULL
        //public string Description { get; set; }    // VARCHAR(2000) NULL
        //public double? GrandTotal { get; set; }    // FLOAT NULL
        public Entities.AssetSubListHeader AssetSubListHeader { get; set; }
        public List<AssetSubMaster> AssetSubMasterDetails { get; set; }
    }
}
