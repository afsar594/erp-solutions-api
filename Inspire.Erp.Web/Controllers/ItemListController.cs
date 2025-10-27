using AutoMapper;
using Inspire.Erp.Application.Master;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Web.Common;
using Inspire.Erp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.POIFS.Crypt.Dsig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers
{
    [Route("api/ItemList")]
    [Produces("application/json")]
    [ApiController]
    public class ItemListController : ControllerBase
    {
        private readonly IMapper _mapper;
        private IItemListService IItemListService;
        public ItemListController(IItemListService _IItemListService, IMapper mapper)
        {

            IItemListService = _IItemListService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("GetAllItemListSearch")]
        public ApiResponse<List<Application.Master.ItemListViewModel>> GetAllItemListSearch(Application.Master.ItemFilterModel data)
        {
            IEnumerable<Application.Master.ItemListViewModel> listItemnater = IItemListService.GetAllItemListSearch(data);

            var x = _mapper.Map<List<Application.Master.ItemListViewModel>>(listItemnater);
            ApiResponse<List<Application.Master.ItemListViewModel>> apiResponse = new ApiResponse<List<Application.Master.ItemListViewModel>>
            {
                Valid = true,
                Result = x,
                Message = ""
            };
            return apiResponse;
        }
        [HttpPost]
        [Route("GetAllItemListSearchPaginated")]
        public async Task<IActionResult> GetAllItemListSearchPaginated(GenericGridViewModel data)
        {
            return Ok(await IItemListService.GetAllItemListSearchPaginated(data));   
        }

        [HttpPost]
        [Route("UpdateRateItemList")]
       public async Task<IActionResult> UpdateRateItemList(List<RateItemListrModel> dates, [FromQuery] GenericGridViewModel data)
        {
            return Ok( IItemListService.UpdateRateItemList(dates,data));
        }
    }
    
}