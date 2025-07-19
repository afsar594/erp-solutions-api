using AutoMapper;
using Inspire.Erp.Application.Master;
using Inspire.Erp.Application.Master.Interfaces;
using Inspire.Erp.Domain.DTO.ItemMaster;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Inspire.Erp.Web.Common;
using Inspire.Erp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.OpenXmlFormats.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers
{
    [Route("api/master/item")]
    [Produces("application/json")]
    [ApiController]
    public class ItemMasterController : ControllerBase
    {
        private readonly IMapper _mapper;
        private IItemMasterService itemMasterService;
        private ILocationMasterService locationMasterService;

        private readonly IRepository<SalesVoucher> _repoSalesVoucher;
        private readonly IRepository<SalesVoucherDetails> _repoSalesVoucherDetails;
        private readonly IRepository<StockRegister> _repoStockRegister;
        private IRepository<ItemMaster> itemrepository; private IRepository<VendorMaster> Brandrepository; private IRepository<ItemStockType> itemStockRepository;
        private readonly IRepository<SuppliersMaster> supplierrepository; private IRepository<TaxMaster> taxrepository; private IRepository<PriceLevelMaster> priceLevelrepository;
        private IRepository<UnitMaster> unitrepository; private IRepository<UnitDetails> _unitDetailsRepo;

        public ItemMasterController(IRepository<ItemMaster> _itemrepository, IRepository<VendorMaster> _Brandrepository, IRepository<ItemStockType> _itemStockRepository,
            IRepository<SuppliersMaster> _supplierrepository, IRepository<TaxMaster> _Taxrepository, IRepository<PriceLevelMaster> _priceLevelrepository,
            IRepository<UnitMaster> _unitrepository, IRepository<UnitDetails> unitDetailsRepo,
            IItemMasterService _itemMasterService, ILocationMasterService _locationMasterService, IMapper mapper, IRepository<SalesVoucher> repoSalesVoucher, IRepository<SalesVoucherDetails> repoSalesVoucherDetails, IRepository<StockRegister> repoStockRegister)
        {
            itemMasterService = _itemMasterService;
            locationMasterService = _locationMasterService;
            _mapper = mapper; _unitDetailsRepo = unitDetailsRepo;
            Brandrepository = _Brandrepository; itemStockRepository = _itemStockRepository; supplierrepository = _supplierrepository;
            itemrepository = _itemrepository; taxrepository = _Taxrepository; priceLevelrepository = _priceLevelrepository; unitrepository = _unitrepository;
            _repoSalesVoucher = repoSalesVoucher;
            _repoSalesVoucherDetails = repoSalesVoucherDetails;
            _repoStockRegister = repoStockRegister;
        }
        [HttpGet]
        [Route("GetAllItem")]
        public ApiResponse<List<ItemMasterViewModel>> GetAllItem()
        {
            List<ItemMaster> listItemnater = itemMasterService.GetAllItem();
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
        [HttpGet]
        [Route("GetAllItemMasterByBarCode/{barCode}")]
        public ApiResponse<ItemMasterViewModel> GetAllItemMasterByBarCode(string barCode)
        {
            ItemMaster listItemnater = itemMasterService.GetAllItemMasterByBarCode(barCode);
            var x = _mapper.Map<ItemMasterViewModel>(listItemnater);
            ApiResponse<ItemMasterViewModel> apiResponse = new ApiResponse<ItemMasterViewModel>
            {
                Valid = true,
                Result = x,
                Message = ""
            };
            return apiResponse;
        }
        [HttpGet]
        [Route("SearchItemByBarcodeLink/{barCode}")]
        public async Task<IActionResult> SearchItemByBarcodeLink(ItemBarCodeSearchFilter model)
        {
            return Ok(await itemMasterService.SearchItemByBarcode(model));

        }
        [HttpGet("GetItemsBySearch")]
        public async Task<IActionResult> GetItemsBySearch(string query)
        {
            return Ok(await itemMasterService.GetItemsBySearch(query));

        }
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
            IEnumerable<ItemMaster> listItemnater = itemMasterService.GetAllItemSearchFilter(data.ItemGroup, data.ItemName);

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
        public async Task<ApiResponse<List<ItemMasterViewModel>>> InsertItem([FromBody] ItemMasterViewModel itemMaster)
        {
            if (itemMaster.ItemMasterImageBase64 != null && (itemMaster.ItemMasterItemId == null || itemMaster.ItemMasterItemId == 0))
            {
                itemMaster.ItemImages = new List<ItemImagesViewModel> {
                                                        new ItemImagesViewModel {
                                                             ItemImagesItemImage = Convert.FromBase64String(itemMaster.ItemMasterImageBase64.Split(',')[1])
                                                        } };
            }

            var model = _mapper.Map<ItemMaster>(itemMaster);
            var data = (itemMaster.ItemMasterItemId == null || itemMaster.ItemMasterItemId == 0) ? await itemMasterService.InsertItem(model) : itemMasterService.UpdateItem(model);
            List<ItemMasterViewModel> result = _mapper.Map<List<ItemMasterViewModel>>(data);
            ApiResponse<List<ItemMasterViewModel>> apiResponse = new ApiResponse<List<ItemMasterViewModel>>
            {
                Valid = true,
                Result = result,
                Message = (itemMaster.ItemMasterItemId == null || itemMaster.ItemMasterItemId == 0) ? ItemMasterMessage.SaveItem : ItemMasterMessage.UpdateItem
            };
            return apiResponse;
        }

        [HttpPost]
        [Route("NewItem")]
        public async Task<ApiResponse<ItemMasterViewModel>> NewItem([FromBody] ItemMasterViewModel itemMaster)
        {
            if (itemMaster.ItemMasterImageBase64 != null && (itemMaster.ItemMasterItemId == null || itemMaster.ItemMasterItemId == 0))
            {
                itemMaster.ItemImages = new List<ItemImagesViewModel> {
                                                        new ItemImagesViewModel {
                                                             ItemImagesItemImage = Convert.FromBase64String(itemMaster.ItemMasterImageBase64.Split(',')[1])
                                                        } };
            }

            var model = _mapper.Map<ItemMaster>(itemMaster);

            var data = (itemMaster.ItemMasterItemId == null || itemMaster.ItemMasterItemId == 0) ? await itemMasterService.NewItem(model) : itemMasterService.UpdateNewItem(model);
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
                    Message = ItemMasterMessage.DeleteFaileds
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

        [HttpGet]
        [Route("GetAveragePrice")]
        public async Task<IActionResult> GetAveragePrice(int id)
        {
            return Ok(await itemMasterService.GetAveragePrice(id));
        }


        [HttpGet]
        [Route("LoadDropdown")]
        public ResponseInfo LoadDropdown()
        {
            var objectresponse = new ResponseInfo();
            var itemMaster = itemrepository.GetAsQueryable().Where(k => k.ItemMasterAccountNo == 0 && (k.ItemMasterDelStatus != true)
                     && k.ItemMasterItemType == ItemMasterStatus.Group).Select(k => new
                     {
                         k.ItemMasterItemId,
                         k.ItemMasterItemName
                     }).OrderBy(x => x.ItemMasterItemName).AsEnumerable().ToList();

            var VendorMasters = Brandrepository.GetAsQueryable().Where(c => c.VendorMasterVendorDelStatus != true).Select(a => new
            {
                a.VendorMasterVendorId,
                a.VendorMasterVendorName,
                a.VendorMasterVendorStatus,
            }).OrderBy(x => x.VendorMasterVendorName).ToList();
            var Stocktype = itemStockRepository.GetAsQueryable().Where(c => c.ItemStockTypeDelStatus != 1).Select(a => new
            {
                a.ItemStockTypeId,
                a.ItemStockTypeDescription
            }).OrderBy(x => x.ItemStockTypeDescription).ToList();
            var SuppliersMaster = supplierrepository.GetAsQueryable().Where(a => a.SuppliersMasterSupplierDelStatus != true).Select(c => new
            {
                c.SuppliersMasterSupplierId,
                c.SuppliersMasterSupplierName
            }).OrderBy(x => x.SuppliersMasterSupplierName).ToList();
            var taxMasters = taxrepository.GetAsQueryable().Where(a => a.TmDelStatus != true).Select(c => new
            {
                c.TmId,
                c.TmName,
                c.TmPercentage
            }).OrderBy(x => x.TmName).ToList();
            var priceLevelMasters = priceLevelrepository.GetAsQueryable().Where(a => a.PriveLevelMasterPriceLevelDelStatus != true).Select(c => new
            {
                c.PriceLevelMasterPriceLevelId,
                c.PriceLevelMasterPriceLevelName
            }).OrderBy(x => x.PriceLevelMasterPriceLevelName).ToList();
            var unitMasters = unitrepository.GetAsQueryable().Where(a => a.UnitMasterUnitDelStatus != true).Select(x => new
            {
                x.UnitMasterUnitId,
                x.UnitMasterUnitFullName,
                x.UnitMasterUnitShortName
            }).OrderBy(x => x.UnitMasterUnitShortName).AsEnumerable().ToList();
            objectresponse.ResultSet = new
            {
                itemMaster = itemMaster,
                VendorMasters = VendorMasters,
                StockTypes = Stocktype,
                SuppliersMaster = SuppliersMaster,
                taxMasters = taxMasters,
                priceLevelMasters = priceLevelMasters,
                unitMasters = unitMasters
            };

            objectresponse.IsSuccess = true;
            return objectresponse;
        }

        [HttpGet]
        [Route("LoadDropdownTaxMaster")]
        public ResponseInfo LoadDropdownTaxMaster()
        {
            var objectresponse = new ResponseInfo();
            var taxMasters = taxrepository.GetAsQueryable().Where(a => a.TmDelStatus != true).Select(c => new
            {
                c.TmId,
                c.TmName,
                c.TmPercentage
            }).OrderBy(x => x.TmName).ToList();

            objectresponse.ResultSet = new
            {
                taxMasters = taxMasters
            };

            objectresponse.IsSuccess = true;
            return objectresponse;
        }

        [HttpGet]
        [Route("LoadDropdownStockType")]
        public ResponseInfo LoadDropdownStockType()
        {
            var objectresponse = new ResponseInfo();
            var Stocktype = itemStockRepository.GetAsQueryable().Where(c => c.ItemStockTypeDelStatus != 1).Select(a => new
            {
                a.ItemStockTypeId,
                a.ItemStockTypeDescription
            }).OrderBy(x => x.ItemStockTypeDescription).ToList();
            objectresponse.ResultSet = new
            {
                StockTypes = Stocktype
            };

            objectresponse.IsSuccess = true;
            return objectresponse;
        }

        [HttpGet]
        [Route("LoadDropdownPriceleverMaster")]
        public ResponseInfo LoadDropdownPriceleverMaster()
        {
            var objectresponse = new ResponseInfo();
            var priceLevelMasters = priceLevelrepository.GetAsQueryable().Where(a => a.PriveLevelMasterPriceLevelDelStatus != true).Select(c => new
            {
                c.PriceLevelMasterPriceLevelId,
                c.PriceLevelMasterPriceLevelName
            }).OrderBy(x => x.PriceLevelMasterPriceLevelName).ToList();

            objectresponse.ResultSet = new
            {
                priceLevelMasters = priceLevelMasters
            };

            objectresponse.IsSuccess = true;
            return objectresponse;
        }

        [HttpGet]
        [Route("LoadDropdownVendorMaster")]
        public ResponseInfo LoadDropdownVendorMaster()
        {
            var objectresponse = new ResponseInfo();
            var VendorMasters = Brandrepository.GetAsQueryable().Where(c => c.VendorMasterVendorDelStatus != true).Select(a => new
            {
                a.VendorMasterVendorId,
                a.VendorMasterVendorName,
                a.VendorMasterVendorStatus,
            }).OrderBy(x => x.VendorMasterVendorName).ToList();

            objectresponse.ResultSet = new
            {
                VendorMasters = VendorMasters
            };

            objectresponse.IsSuccess = true;
            return objectresponse;
        }

        [HttpGet]
        [Route("searchItemByName")]
        public ResponseInfo searchItemByName(string name)
        {
            var objectresponse = new ResponseInfo();

            var itemMaster = itemrepository.GetAsQueryable().Where(k => k.ItemMasterAccountNo != 0 && (k.ItemMasterDelStatus != true)
                     && k.ItemMasterItemType != ItemMasterStatus.Group && k.ItemMasterItemName.ToLower().Contains(name.ToLower())).Select(k => new
                     {
                         k.ItemMasterItemId,
                         k.ItemMasterItemName
                     }).OrderBy(x => x.ItemMasterItemName).ToList();

            objectresponse.ResultSet = new
            {
                itemMaster = itemMaster,
            };
            return objectresponse;
        }

        [HttpGet]
        [Route("GetItemMasterById/{id}/{customerId?}")]
        public ResponseInfo GetItemMasterById(int id, int customerId = -1)
        {
            var response = new ResponseInfo();
            decimal? lastSalePrice = null;
            decimal? CurrentItemStock = null;
            var item = itemrepository.GetAsQueryable()
                .Where(x => x.ItemMasterItemId == id && x.ItemMasterDelStatus != true)
                .Select(a => new
                {
                    a.ItemMasterItemId,
                    a.ItemMasterItemName,
                    a.ItemMasterPartNo,
                    a.ItemMasterUnitPrice,
                    a.ItemMasterLastPurchasePrice
                })
                .FirstOrDefault();

            if (item == null)
            {
                response.ResultSet = null;
                return response;
            }

            var unitDetail = _unitDetailsRepo.GetAsQueryable()
                .Where(x => x.UnitDetailsItemId == id && x.UnitDetailsDelStatus != true)
                .OrderBy(x => x.UnitDetailsItem)
                .Select(c => new
                {
                    c.UnitDetailsId,
                    c.UnitDetailsUnitId,
                    c.UnitDetailsConversionType,
                    c.UnitDetailsRate,
                    c.UnitDetailsUnitPrice,
                    c.UnitDetailsUnitCost,
                    c.UnitDetailsProfitPer,
                })
                .FirstOrDefault();
            if (customerId > 0)
            {
                lastSalePrice = (from sv in _repoSalesVoucher.GetAsQueryable().AsNoTracking()
                                 join svd in _repoSalesVoucherDetails.GetAsQueryable().AsNoTracking()
                                 on sv.SalesVoucherId equals svd.SalesVoucherId
                                 where sv.SalesVoucherPartyId == customerId && svd.SalesVoucherDetailsMatId == id
                                 orderby sv.SalesVoucherDate descending
                                 select svd.SalesVoucherDetailsRate)
                                              .FirstOrDefault();
            }

            var stockData = _repoStockRegister.GetAsQueryable()
                                              .AsNoTracking()
                                              .Where(x => x.StockRegisterMaterialID == id && !(bool)x.StockRegisterDelStatus)
                                              .GroupBy(x => x.StockRegisterMaterialID)
                                              .Select(g => new
                                              {
                                                  StockIn = g.Sum(s => s.StockRegisterSIN ?? 0m),
                                                  StockOut = g.Sum(s => s.StockRegisterSout ?? 0m)
                                              })
                                              .FirstOrDefault();

            CurrentItemStock = stockData != null ? stockData.StockIn - stockData.StockOut : 0m;



            var result = new
            {
                item.ItemMasterItemId,
                item.ItemMasterItemName,
                item.ItemMasterPartNo,
                ItemMasterUnitPrice = lastSalePrice ?? item.ItemMasterUnitPrice,
                CurrentItemStock = CurrentItemStock ?? 0m,
                item.ItemMasterLastPurchasePrice,
                UnitDetails = unitDetail
            };

            response.ResultSet = new { itemMaster = result };
            return response;
        }

        [HttpPost("GetFilteredItems")]
        public async Task<IActionResult> GetFilteredItems([FromBody] PagedSearchRequest request)
        {
            return Ok(await itemMasterService.GetFilteredItems(request));
        }

        [HttpPost("GetLastItemsThreePrices")]
        public async Task<IActionResult> GetLastItemsThreePrices([FromBody] List<int> itemIds)
        {
            return Ok(await itemMasterService.GetLastItemThreePrices(itemIds));
        }

    }
}