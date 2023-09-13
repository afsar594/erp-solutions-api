using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.AccountStatement;
using Inspire.Erp.Domain.Modals.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Account.Interfaces
{
    public interface IAssetsOpeningService
    {
        Task<Response<List<DropdownResponse>>> GetCurrencyMaster();
        Task<Response<List<GetAssetsMasterList>>> GetAssetMaster();
        Task<Response<bool>> AddEditAssetOpening(AddEditAssetPurchaseVoucher model);
        Task<Response<AddEditAssetPurchaseVoucher>> GetAssetOpening(int id = 0);
        Task<Response<GridWrapperResponse<List<GetAssetPurchaseVoucher>>>> GetAssetOpeningList(GenericGridViewModel model);
    }
}
