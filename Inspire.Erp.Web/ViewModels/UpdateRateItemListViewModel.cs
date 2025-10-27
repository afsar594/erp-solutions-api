using Inspire.Erp.Application.Master;
using Inspire.Erp.Domain.Modals.Common;
using System.Collections.Generic;

namespace Inspire.Erp.Web.ViewModels
{
    public class UpdateRateItemListViewModel
    {
        public List<RateItemListrModel> Dates { get; set; }
        public GenericGridViewModel Data { get; set; }
    }
}
