using AutoMapper;
using Inspire.Erp.Application.Master.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Models;
using Inspire.Erp.Domain.Models.Approvals;
using Inspire.Erp.Infrastructure.Database;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Application.Account.Implementations;
using Inspire.Erp.Application.MODULE;
using Inspire.Erp.Application.StoreWareHouse.Interface;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals;
using System.Linq;
using Inspire.Erp.Application.Assets.Interface;
using Inspire.Erp.Web.ViewModels;

namespace Inspire.Erp.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsSubMasterController : ControllerBase
    {

        private InspireErpDBContext _context;
        private IRepository<AssetSubMaster> _assetsubmaster;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;
        private IRepository<Erp.Domain.Entities.ProgramSettings> _programsettingsRepository;
      
        private readonly IMapper _mapper;
        private readonly IAssetSubListService _Assetsubmaster;
        public AssetsSubMasterController(IAssetSubListService Assetsubmaster, IMapper mapper, IRepository<AssetSubMaster> assetsubmaster, IRepository<VouchersNumbers> vouchersNumbers, IRepository<AssetSubMaster> assetSubMaster)
        {
            _mapper = mapper;
            _Assetsubmaster = Assetsubmaster;
            _voucherNumbersRepository = vouchersNumbers;
            
        }


      


        [HttpGet("GetAllAssetMasterList")]
        public IActionResult GetAssets()
        {
            try
            {
                var item = _Assetsubmaster.GetAllAssets();
                return Ok(item);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }
        [HttpPost]
        [Route("InsertAssetsSubMaster")]
        public ApiResponse<List<AssetSubMaster>> InsertPermissionApproval([FromBody] Erp.Domain.Models.ViewModels.AssetSubMasterViewModel request)
        {
            ApiResponse<List<AssetSubMaster>> apiResponse = new ApiResponse<List<AssetSubMaster>>();
            if (request.AssetSubMasterDetails == null)
            {
                return new ApiResponse<List<AssetSubMaster>>
                {
                    Valid = false,
                    Message = "Invalid data",
                };
            }
            var param1 = _mapper.Map<List<AssetSubMaster>>(request.AssetSubMasterDetails);
            var param2 = _mapper.Map<Erp.Domain.Entities.AssetSubListHeader>(request.AssetSubListHeader);

            var xs = _Assetsubmaster.InsertPermissionApproval(request);
            List<AssetSubMaster> Permission = new List<AssetSubMaster>();
            Permission = _mapper.Map<List<AssetSubMaster>>(xs);
            apiResponse = new ApiResponse<List<AssetSubMaster>>
            {
                Valid = true,
                Result = Permission,
                Message = ""
            };
            return apiResponse;
        }

    }
}
