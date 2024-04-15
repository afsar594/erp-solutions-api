using AutoMapper;
using Inspire.Erp.Application.Master;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Web.Common;
using Inspire.Erp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.POIFS.Crypt.Dsig;
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
        [HttpPost]
        [Route("UpdateRateItemList")]
       public ApiResponse<Application.Master.ItemListViewModel> UpdateRateItemList(Application.Master.RateItemListrModel dates)
        {
            var param1 = _mapper.Map<Application.Master.RateItemListrModel>(dates);
            var UpdateJobInvoiceViewModel = IItemListService.UpdateRateItemList(param1);


            ApiResponse<Application.Master.ItemListViewModel> apiResponse = new ApiResponse<Application.Master.ItemListViewModel>
            {
                Valid = true,
                Result = _mapper.Map<Application.Master.ItemListViewModel>(UpdateJobInvoiceViewModel),
                Message = JobInvoiceMessage.UpdateJobInvoice

            };
            return apiResponse;

        }
    }
    
}