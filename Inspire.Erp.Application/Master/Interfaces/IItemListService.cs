using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Master
{
    public interface IItemListService
    {
        
        public IEnumerable<ItemListViewModel> GetAllItemListSearch(ItemFilterModel filteModle);
        public Task<Response<GridWrapperResponse<List<ItemListViewModel>>>> UpdateRateItemList(List<RateItemListrModel> dates, GenericGridViewModel data);
        Task<Response<GridWrapperResponse<List<ItemListViewModel>>>> GetAllItemListSearchPaginated(GenericGridViewModel model);

    }
}