using AutoMapper;
using Inspire.Erp.Application.Master;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Web.Common;
using Inspire.Erp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

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
    }
    
}