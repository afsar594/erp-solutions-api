using Inspire.Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Assets.Interface
{
    public interface IAssetSubListService
    {

        public IQueryable GetAllAssets();
        List<AssetSubMaster> InsertPermissionApproval(Erp.Domain.Models.ViewModels.AssetSubMasterViewModel request);
    }


}
