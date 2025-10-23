using AutoMapper;
using Inspire.Erp.Application.Store.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Inspire.Erp.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.HPSF;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Inspire.Erp.Web.Controllers.Store
{    
    [Route("api/[controller]")]
    [ApiController]
    public class StockAdjustmentController : ControllerBase
    {
        private IStockAdjustmentService _sa;
        private readonly IMapper _mapper;
        private IRepository<JobMaster> _jobrepository;
        private IRepository<LocationMaster> _locationrepository;
        private IRepository<ItemMaster> _itemrepository;
        private IRepository<UnitMaster> _unitrepository;
        private IRepository<UnitDetails> _unitDetailRepository;
        private IRepository<StockRegister> _stockRegisterRepository;

        public StockAdjustmentController(IStockAdjustmentService sa, IMapper mapper, IRepository<UnitMaster> unitrepository,
            IRepository<JobMaster> jobrepository, IRepository<LocationMaster> locationrepository, IRepository<ItemMaster> itemrepository, IRepository<UnitDetails> unitDetailRepository, IRepository<StockRegister> stockRegisterRepository)
        {
            _sa = sa;
            _mapper = mapper; _itemrepository = itemrepository; _unitrepository = unitrepository;
            _jobrepository = jobrepository; _locationrepository = locationrepository;
            _unitDetailRepository = unitDetailRepository;
            _stockRegisterRepository = stockRegisterRepository;
        }

        [HttpGet]
        [Route("GetAllStockAdjustment")]
        public ApiResponse<List<StockAdjustmentVoucher>> GetAllStockAdjustment()
        {
            ApiResponse<List<StockAdjustmentVoucher>> apiResponse = new ApiResponse<List<StockAdjustmentVoucher>>
            {
                Valid = true,
                Result = _mapper.Map<List<StockAdjustmentVoucher>>(_sa.GetStockAdjustment()),
                Message = ""
            };
            return apiResponse;
        }

        [HttpPost]
        [Route("InsertStockAdjustment")]
        public async Task<ApiResponse<StockAdjustmentVoucher>> InsertStockAdjustmentVoucher([FromBody] StockAdjustmentVoucher voucherCompositeView)
        {
            ApiResponse<StockAdjustmentVoucher> apiResponse = new ApiResponse<StockAdjustmentVoucher>();
            var param1 = _mapper.Map<StockAdjustmentVoucher>(voucherCompositeView);
            var param2 = _mapper.Map<List<StockAdjustmentVoucherDetails>>(voucherCompositeView.stockAdjustmentVoucherDetails);
            var param3 = _mapper.Map<List<AccountsTransactions>>(voucherCompositeView.AccountsTransactions);
            var xs = await _sa.InsertStockAdjustmentVoucher(param1, param2, param3);
            apiResponse = new ApiResponse<StockAdjustmentVoucher>
            {
                Valid = true,
                Result = xs,
                Message = StockeAdjustmentMessage.SaveStockAdjustmentVoucher
            };
            return apiResponse;
        }
        [HttpPost]
        [Route("UpdateStockAdjustment")]
        public async Task<ApiResponse<StockAdjustmentVoucher>> UpdateStockAdjustmentVoucher([FromBody] StockAdjustmentVoucher voucherCompositeView)
        {
            ApiResponse<StockAdjustmentVoucher> apiResponse = new ApiResponse<StockAdjustmentVoucher>();
            var param1 = _mapper.Map<StockAdjustmentVoucher>(voucherCompositeView);
            var param2 = _mapper.Map<List<StockAdjustmentVoucherDetails>>(voucherCompositeView.stockAdjustmentVoucherDetails);
            var xs = await _sa.UpdateStockAdjustmentVoucher(param1, param2);
            apiResponse = new ApiResponse<StockAdjustmentVoucher>
            {
                Valid = true,
                Result = xs,
                Message = StockeAdjustmentMessage.UpdateStockAdjustmentVoucher
            };
            return apiResponse;
        }

        [HttpGet]
        [Route("GetSavedStockAdjustmentDetails/{id}")]
        public ApiResponse<StockAdjustmentVoucher> GetSavedStockAdjustmentDetails(int id)
        {
            StockAdjustmentVoucher stockAdjustmentVoucher = _sa.GetSavedStockAdjustmentferDetails(id);
            if (stockAdjustmentVoucher != null)
            {
                ApiResponse<StockAdjustmentVoucher> apiResponse = new ApiResponse<StockAdjustmentVoucher>
                {
                    Valid = true,
                    Result = stockAdjustmentVoucher,
                    Message = ""
                };
                return apiResponse;
            }
            return null;
        }

        [HttpPost]
        [Route("DeleteStockAdjustmentVoucher")]
        public ApiResponse<int> DeleteStockAdjustmentVoucher([FromBody] StockAdjustmentVoucher voucherCompositeView)
        {

            var param1 = _mapper.Map<StockAdjustmentVoucher>(voucherCompositeView);
            var param2 = _mapper.Map<List<StockAdjustmentVoucherDetails>>(voucherCompositeView.stockAdjustmentVoucherDetails);
            var xs = _sa.DeleteStockAdjustmentVoucher(param1, param2);
            ApiResponse<int> apiResponse = new ApiResponse<int>
            {
                Valid = true,
                Result = 0,
                Message = StockeAdjustmentMessage.DeleteStockAdjustmentVoucher
            };
            return apiResponse;
        }


        [HttpPost("StockAdjustmentReport")]
        public async Task<IActionResult> StockAdjustmentReport(StockAdjustmentReportFilter model)
        {
            return Ok(await _sa.StockAdjustmentReport(model));
        }

        [HttpGet]
        [Route("StockAdjustmentDropdown")]
        public ResponseInfo StockAdjustmentDropdown()
        {
            var objresponse = new ResponseInfo();
            
            var jobMasters = _jobrepository.GetAsQueryable().AsNoTracking().Where(a => a.JobMasterJobDelStatus != true).Select(c => new
            {
                c.JobMasterJobId,
                c.JobMasterJobNumber,
                JobMasterJobName = c.JobMasterJobName.Trim(),
            }).OrderBy(x => x.JobMasterJobName).ToList();
            var LocationMaster = _locationrepository.GetAsQueryable().AsNoTracking().Where(a => a.LocationMasterLocationDelStatus != true).Select(c => new
            {
                c.LocationMasterLocationId,
                c.LocationMasterLocationStatus,
                LocationMasterLocationName = c.LocationMasterLocationName.Trim(),
            }).OrderBy(x => x.LocationMasterLocationName).ToList();

           var itemMaster = (from item in _itemrepository.GetAsQueryable().AsNoTracking()
                              join unit in _unitDetailRepository.GetAsQueryable().AsNoTracking()
                              on item.ItemMasterItemId equals unit.UnitDetailsItemId
                              where item.ItemMasterAccountNo != 0
                              && item.ItemMasterDelStatus != true
                              && item.ItemMasterItemType != ItemMasterStatus.Group
                              select new
                              {
                                  item.ItemMasterItemId,
                                  item.ItemMasterItemName,
                                  item.ItemMasterVenderId,
                                  item.ItemMasterItemSize,
                                  itemMasterBarcode = unit.UnitDetailsBarcode,
                                  ItemMasterCurrentStock =_stockRegisterRepository.GetAsQueryable().Where(x => x.StockRegisterMaterialID ==item.ItemMasterItemId).Sum(y => (y.StockRegisterSIN ?? 0) - (y.StockRegisterSout ?? 0)),

                              }).Distinct().OrderBy(x => x.ItemMasterItemName).ToList();

            var unitMasters = _unitrepository.GetAsQueryable().AsNoTracking().Where(a => a.UnitMasterUnitDelStatus != true).Select(x => new
            {
                x.UnitMasterUnitId,
                UnitMasterUnitFullName = x.UnitMasterUnitFullName.Trim(),
                UnitMasterUnitShortName = x.UnitMasterUnitShortName.Trim()
            }).OrderBy(x => x.UnitMasterUnitShortName).ToList();
            objresponse.ResultSet = new
            {
                jobMasters = jobMasters,
                LocationMaster = LocationMaster,
                itemMaster = itemMaster,
                unitMasters = unitMasters,
               
            };
            return objresponse;
        }
        [HttpGet]
        [Route("StockAdjustmentDropdownByLocation")]
        public ResponseInfo StockAdjustmentDropdownByLocation(int location)
        {
            var objresponse = new ResponseInfo();

            var jobMasters = _jobrepository.GetAsQueryable().AsNoTracking().Where(a => a.JobMasterJobDelStatus != true).Select(c => new
            {
                c.JobMasterJobId,
                c.JobMasterJobNumber,
                JobMasterJobName = c.JobMasterJobName.Trim(),
            }).OrderBy(x => x.JobMasterJobName).ToList();
            var LocationMaster = _locationrepository.GetAsQueryable().AsNoTracking().Where(a => a.LocationMasterLocationDelStatus != true).Select(c => new
            {
                c.LocationMasterLocationId,
                c.LocationMasterLocationStatus,
                LocationMasterLocationName = c.LocationMasterLocationName.Trim(),
            }).OrderBy(x => x.LocationMasterLocationName).ToList();

            var itemMaster = (from item in _itemrepository.GetAsQueryable().AsNoTracking()
                              join unit in _unitDetailRepository.GetAsQueryable().AsNoTracking()
                              on item.ItemMasterItemId equals unit.UnitDetailsItemId
                              where item.ItemMasterAccountNo != 0
                              && item.ItemMasterDelStatus != true
                              && item.ItemMasterItemType != ItemMasterStatus.Group
                              select new
                              {
                                  item.ItemMasterItemId,
                                  item.ItemMasterItemName,
                                  item.ItemMasterVenderId,
                                  item.ItemMasterItemSize,
                                  itemMasterBarcode = unit.UnitDetailsBarcode,
                                  ItemMasterCurrentStock = _stockRegisterRepository.GetAsQueryable().Where(x => x.StockRegisterMaterialID == item.ItemMasterItemId && x.StockRegisterLocationID == location).Sum(y => (y.StockRegisterSIN ?? 0) - (y.StockRegisterSout ?? 0)),

                              }).Distinct().OrderBy(x => x.ItemMasterItemName).ToList();

            var unitMasters = _unitrepository.GetAsQueryable().AsNoTracking().Where(a => a.UnitMasterUnitDelStatus != true).Select(x => new
            {
                x.UnitMasterUnitId,
                UnitMasterUnitFullName = x.UnitMasterUnitFullName.Trim(),
                UnitMasterUnitShortName = x.UnitMasterUnitShortName.Trim()
            }).OrderBy(x => x.UnitMasterUnitShortName).ToList();
            objresponse.ResultSet = new
            {
                jobMasters = jobMasters,
                LocationMaster = LocationMaster,
                itemMaster = itemMaster,
                unitMasters = unitMasters,

            };
            return objresponse;
        }

        [HttpGet]
        [Route("GetStockByLocation")]
        public ResponseInfo GetStockByLocation([FromQuery] int id, [FromQuery] DateTime asOnDate)
        {
                    var objresponse = new ResponseInfo();
            objresponse = _sa.GetStockByLocation(id, asOnDate);

            return objresponse;
        }

    }
}
