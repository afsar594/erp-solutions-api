using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Models.ViewModels;
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
        public List<AssetSubMasterListVM> GetAllAssetSubMasterDetails();
        bool DeleteAssetSubMasterById(int id); // New method for deletion
        public AssetSubMasterListVM GetAssetSubMasterDetailById(int SubAssetId);
        List<AssetSubMaster> InsertPermissionApproval(AssetSubMasterViewModel request);

    }


}
