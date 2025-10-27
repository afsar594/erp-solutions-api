using Inspire.Erp.Application.Master;
using Inspire.Erp.Domain.Modals.Common;
using System.Collections.Generic;

namespace Inspire.Erp.Web.ViewModels
{
    public class UpdateRateItemListViewModel
    {
        public List<RateItemListrModel> itemlistData { get; set; }
        public GenericGridViewModel paginationData { get; set; }
    }
}
