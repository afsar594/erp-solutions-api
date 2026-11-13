using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Models.Master
{
    public class AssetCodes
    {
        public long GroupID { get; set; }
        public int AssetMasterAssetId { get; set; }
        public long SubAssetID { get; set; }
        public string AssetMasterAssetName { get; set; }
        public string AssetDescription { get; set; }
        public string AssetCode { get; set; }
    }
}
