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
    [Route("api/master/item")]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    public class ItemMasterController : ControllerBase
    {
        private readonly IMapper _mapper;
        private IItemMasterService itemMasterService;
        public ItemMasterController(IItemMasterService _itemMasterService, IMapper mapper)
        {

            itemMasterService = _itemMasterService;
            _mapper = mapper;
        }
        [HttpGet]
        [Route("GetAllItem")]
        public ApiResponse<List<ItemMasterViewModel>> GetAllItem()
        {
            IEnumerable<ItemMaster> listItemnater = itemMasterService.GetAllItem();
             var x = _mapper.Map<List<ItemMasterViewModel>>(listItemnater);
            ApiResponse<List<ItemMasterViewModel>> apiResponse = new ApiResponse<List<ItemMasterViewModel>>
            {
                Valid = true,
                Result = x,
                Message = ""
            };

            return apiResponse;
        }

        [HttpGet]
        [Route("GetAllItemMaster")]
        public ApiResponse<List<ItemMasterViewModel>> GetAllItemMaster()
        {
            IEnumerable<ItemMaster> listItemnater = itemMasterService.GetAllItemMaster();
            var x = _mapper.Map<List<ItemMasterViewModel>>(listItemnater);
            ApiResponse<List<ItemMasterViewModel>> apiResponse = new ApiResponse<List<ItemMasterViewModel>>
            {
                Valid = true,
                Result = x,
                Message = ""
            };

            return apiResponse;
        }

        [HttpGet]
        [Route("GetAllItemGroupAndSubGroup")]
        public ApiResponse<List<ItemMasterViewModel>> GetAllItemGroupAndSubGroup()
        {
            IEnumerable<ItemMaster> listItemnater = itemMasterService.GetAllItemGroupAndSubGroup();
            var x = _mapper.Map<List<ItemMasterViewModel>>(listItemnater);
            ApiResponse<List<ItemMasterViewModel>> apiResponse = new ApiResponse<List<ItemMasterViewModel>>
            {
                Valid = true,
                Result = x,
                Message = ""
            };

            return apiResponse;
        }

        [HttpGet]
        [Route("itemmax")]
        public ApiResponse<int> ItemMax()
        {
            int max = itemMasterService.ItemMasterIdMax();
            ApiResponse<int> apiResponse = new ApiResponse<int>
            {
                Valid = true,
                Result = max,
                Message = ""
            };

            return apiResponse;
        }

        [HttpGet]
        [Route("GetAllItemNotGroup")]
        public ApiResponse<List<ItemMasterViewModel>> GetAllItemNotGroup()
        {
            IEnumerable<ItemMaster> listItemnater = itemMasterService.GetAllItemNotGroup();
            var x = _mapper.Map<List<ItemMasterViewModel>>(listItemnater);
            ApiResponse<List<ItemMasterViewModel>> apiResponse = new ApiResponse<List<ItemMasterViewModel>>
            {
                Valid = true,
                Result = x,
                Message = ""
            };

            return apiResponse;
        }

        [HttpGet]
        [Route("GetAllItemStockType")]
        public ApiResponse<List<ItemStockTypeViewModel>> GetAllItemStockType()
        {
            IEnumerable<ItemStockType> listItemnater = itemMasterService.GetAllStockType();
            var x = _mapper.Map<List<ItemStockTypeViewModel>>(listItemnater);
            ApiResponse<List<ItemStockTypeViewModel>> apiResponse = new ApiResponse<List<ItemStockTypeViewModel>>
            {
                Valid = true,
                Result = x,
                Message = ""
            };

            return apiResponse;
        }

        //[HttpGet("{id}", Name= "GetAllItemById")]
        [HttpGet]
        [Route("GetAllItemById/{id}")]
        public ApiResponse<List<ItemMasterViewModel>> GetAllItemById(int id)
        {
            IEnumerable<ItemMaster> listItemnater = itemMasterService.GetAllItemById(id);

            var x = _mapper.Map<List<ItemMasterViewModel>>(listItemnater);
            ApiResponse<List<ItemMasterViewModel>> apiResponse = new ApiResponse<List<ItemMasterViewModel>>
            {
                Valid = true,
                Result = x,
                Message = ""
            };

            return apiResponse;

        }

        //[HttpGet("{id}", Name = "GetAllItemMasterById")]
        [HttpGet]
        [Route("GetAllItemMasterById/{id}")]
        public ApiResponse<ItemMasterViewModel> GetAllItemMasterById(int id)
        {
            ItemMaster listItemnater = itemMasterService.GetAllItemMasterById(id);

            var x = _mapper.Map<ItemMasterViewModel>(listItemnater);
            ApiResponse<ItemMasterViewModel> apiResponse = new ApiResponse<ItemMasterViewModel>
            {
                Valid = true,
                Result = x,
                Message = ""
            };

            return apiResponse;

        }


        //[HttpGet("{id}", Name = "GetAllItemByName")]
        [HttpGet]
        [Route("GetAllItemByName/{id}")]
        public ApiResponse<List<ItemMasterViewModel>> GetAllItemByName(string id)
        {
            IEnumerable<ItemMaster> listItemnater = itemMasterService.GetItemMastersByName(id);

            var x = _mapper.Map<List<ItemMasterViewModel>>(listItemnater);
            ApiResponse<List<ItemMasterViewModel>> apiResponse = new ApiResponse<List<ItemMasterViewModel>>
            {
                Valid = true,
                Result = x,
                Message = ""
            };

            return apiResponse;

        }

        [HttpPost]
        [Route("GetAllItemSearchFilter")]
        public ApiResponse<List<ItemMasterViewModel>> GetAllItemSearchFilter(ItemFilterViewModel data)
        {
            IEnumerable<ItemMaster> listItemnater = itemMasterService.GetAllItemSearchFilter(data.ItemGroup,data.ItemName);

            var x = _mapper.Map<List<ItemMasterViewModel>>(listItemnater);
            ApiResponse<List<ItemMasterViewModel>> apiResponse = new ApiResponse<List<ItemMasterViewModel>>
            {
                Valid = true,
                Result = x,
                Message = ""
            };

            return apiResponse;

        }

        [HttpPost]
        [Route("InsertItem")]
        public ApiResponse<List<ItemMasterViewModel>> InsertItem([FromBody] ItemMasterViewModel itemMaster)
        {
            if(itemMaster.ItemMasterImageBase64 != null && (itemMaster.ItemMasterItemId == null || itemMaster.ItemMasterItemId == 0))
            {
                itemMaster.ItemImages = new List<ItemImagesViewModel> {
                                                        new ItemImagesViewModel {
                                                             ItemImagesItemImage = Convert.FromBase64String(itemMaster.ItemMasterImageBase64.Split(',')[1])
                                                        } };
            }
           
            var model = _mapper.Map<ItemMaster>(itemMaster);
          
            var data = (itemMaster.ItemMasterItemId == null || itemMaster.ItemMasterItemId == 0)? itemMasterService.InsertItem(model):itemMasterService.UpdateItem(model);
            List<ItemMasterViewModel> result =  _mapper.Map<List<ItemMasterViewModel>>(data) ;
            ApiResponse<List<ItemMasterViewModel>> apiResponse = new ApiResponse<List<ItemMasterViewModel>>
            {
                Valid = true,
                Result = result,
                Message = (itemMaster.ItemMasterItemId == null || itemMaster.ItemMasterItemId == 0) ? ItemMasterMessage.SaveItem: ItemMasterMessage.UpdateItem
            };
            return apiResponse;
        }

        [HttpPost]
        [Route("NewItem")]
        public ApiResponse<ItemMasterViewModel> NewItem([FromBody] ItemMasterViewModel itemMaster)
        {
            if (itemMaster.ItemMasterImageBase64 != null && (itemMaster.ItemMasterItemId == null || itemMaster.ItemMasterItemId == 0))
            {
                itemMaster.ItemImages = new List<ItemImagesViewModel> {
                                                        new ItemImagesViewModel {
                                                             ItemImagesItemImage = Convert.FromBase64String(itemMaster.ItemMasterImageBase64.Split(',')[1])
                                                        } };
            }

            var model = _mapper.Map<ItemMaster>(itemMaster);

            var data = (itemMaster.ItemMasterItemId == null || itemMaster.ItemMasterItemId == 0) ? itemMasterService.NewItem(model) : itemMasterService.UpdateNewItem(model);
            ItemMasterViewModel result = _mapper.Map<ItemMasterViewModel>(data);
            ApiResponse<ItemMasterViewModel> apiResponse = new ApiResponse<ItemMasterViewModel>
            {
                Valid = true,
                Result = result,
                Message = (itemMaster.ItemMasterItemId == null || itemMaster.ItemMasterItemId == 0) ? ItemMasterMessage.SaveItem : ItemMasterMessage.UpdateItem
            };
            return apiResponse;
        }

        [HttpPost]
        [Route("DeleteNewItem")]
        public ApiResponse<ItemMasterViewModel> DeleteNewItem([FromBody] ItemMasterViewModel itemMaster)
        {
            ApiResponse<ItemMasterViewModel> apiResponse = null;
            if ((itemMaster.ItemMasterItemId != null && itemMaster.ItemMasterItemId != 0))
            {
                itemMaster.ItemMasterDelStatus = true;
                var model = _mapper.Map<ItemMaster>(itemMaster);

                var data = itemMasterService.UpdateNewItem(model);
                ItemMasterViewModel result = _mapper.Map<ItemMasterViewModel>(data);
                apiResponse = new ApiResponse<ItemMasterViewModel>
                {
                    Valid = true,
                    Result = result,
                    Message = (itemMaster.ItemMasterItemId == null || itemMaster.ItemMasterItemId == 0) ? ItemMasterMessage.SaveItem : ItemMasterMessage.UpdateItem
                };
            }
            else
            {
                apiResponse = new ApiResponse<ItemMasterViewModel>
                {
                    Valid = true,
                    Result = null,
                    Message = ItemMasterMessage.DeleteFailed
                };
            }


            return apiResponse;
        }

        [HttpPost]
        [Route("UpdateItem")]
        public ApiResponse<List<ItemMasterViewModel>> UpdateBrand([FromBody] ItemMasterViewModel itemMaster)
        {
            if (itemMaster.ItemMasterImageBase64 != null)
            {
                itemMaster.ItemImages = new List<ItemImagesViewModel> {
                                                        new ItemImagesViewModel {
                                                             ItemImagesItemImage = Convert.FromBase64String(itemMaster.ItemMasterImageBase64.Split(',')[1])
                                                        } };
            }

            var model = _mapper.Map<ItemMaster>(itemMaster);

            var data = itemMasterService.UpdateItem(model);
            List<ItemMasterViewModel> result = itemMaster.ItemMasterItemType == null ? null : _mapper.Map<List<ItemMasterViewModel>>(data);
            ApiResponse<List<ItemMasterViewModel>> apiResponse = new ApiResponse<List<ItemMasterViewModel>>
            {
                Valid = true,
                Result = result,
                Message = ""
            };
            return apiResponse;
        }

        [HttpPost]
        [Route("DeleteItem")]
        public ApiResponse<List<ItemMasterViewModel>> DeleteItem([FromBody] ItemMasterViewModel itemMaster)
        {
            ApiResponse<List<ItemMasterViewModel>> apiResponse = null;
            if ((itemMaster.ItemMasterItemId != null && itemMaster.ItemMasterItemId != 0))
            {
                itemMaster.ItemMasterDelStatus = true;
                var model = _mapper.Map<ItemMaster>(itemMaster);

                var data = itemMasterService.UpdateItem(model);
                List<ItemMasterViewModel> result = _mapper.Map<List<ItemMasterViewModel>>(data);
                apiResponse = new ApiResponse<List<ItemMasterViewModel>>
                {
                    Valid = true,
                    Result = result,
                    Message = (itemMaster.ItemMasterItemId == null || itemMaster.ItemMasterItemId == 0) ? ItemMasterMessage.SaveItem : ItemMasterMessage.UpdateItem
                };
            }
            else
            {
                apiResponse = new ApiResponse<List<ItemMasterViewModel>>
                {
                    Valid = true,
                    Result = null,
                    Message = ItemMasterMessage.DeleteFailed
                };
            }
          
            
            return apiResponse;
        }
    }
}