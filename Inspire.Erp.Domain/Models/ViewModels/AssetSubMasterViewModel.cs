using Inspire.Erp.Domain.Entities;
using System.Collections.Generic;

namespace Inspire.Erp.Domain.Models.ViewModels
{
    public class AssetSubMasterViewModel
    {
        public Erp.Domain.Entities.AssetSubListHeader AssetSubListHeader { get; set; }
        public List<AssetSubMaster> AssetSubMasterDetails { get; set; }
    }
}
