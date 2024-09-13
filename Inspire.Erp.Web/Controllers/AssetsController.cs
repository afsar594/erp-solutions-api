using AutoMapper;
using Inspire.Erp.Application.Assets;
using Inspire.Erp.Application.Master.Interfaces;
using Inspire.Erp.Domain.Entities;
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

namespace Inspire.Erp.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsController : ControllerBase
    {

        private InspireErpDBContext _context;
        private IRepository<AssetSubMaster> _assetsubmaster;
        private IRepository<VouchersNumbers> _vouchersNumbers;

        private readonly IMapper _mapper;
        private readonly IAssetSubListService _Assetsubmaster;

        public AssetsController(IAssetSubListService Assetsubmaster, IMapper mapper, IRepository<AssetSubMaster> assetsubmaster, IRepository<VouchersNumbers> vouchersNumbers, IRepository<AssetSubMaster> assetSubMaster)
        {
            _mapper = mapper;
            _Assetsubmaster = Assetsubmaster;
            _vouchersNumbers = vouchersNumbers;

        }


        [HttpGet("GetAllAssets")]
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
        [Route("InsertAssets")]
        public ApiResponse<List<AssetSubMaster>> InsertPermissionApproval([FromBody] List<AssetSubMaster> Perm)
        {
            ApiResponse<List<AssetSubMaster>> apiResponse = new ApiResponse<List<AssetSubMaster>>();
            if (Perm == null)
            {
                return new ApiResponse<List<AssetSubMaster>>
                {
                    Valid = false,
                    Message = "Invalid data",
                };
            }
            var param1 = _mapper.Map<List<AssetSubMaster>>(Perm);
            var xs = _Assetsubmaster.InsertPermissionApproval(param1);

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
