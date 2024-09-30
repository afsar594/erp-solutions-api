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
using Inspire.Erp.Domain.Models.ViewModels;
using Inspire.Erp.Application.Account.Interfaces;
using Inspire.Erp.Domain.Modals.Common;

namespace Inspire.Erp.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsController : ControllerBase
    {

        private readonly IAssetsMasterService _assetsMasterService;
        private readonly IMapper _mapper;
        private IRepository<AssetMaster> _assetMasterRepository;
        public AssetsController(IMapper mapper, IRepository<AssetMaster> assetMaster,
            IAssetsMasterService assetMasterService)
        {
            _mapper = mapper;
            _assetsMasterService = assetMasterService;
            _assetMasterRepository = assetMaster;
        }


        [HttpGet("GetFixedAssets")]
        public async Task<IActionResult> GetFixedAssets(int relativeNo)
        {
            try
            {
                return Ok(await _assetsMasterService.GetFixedAssets(relativeNo));
            }
            catch (Exception ex)
            {
                // Log the full exception details here
                return StatusCode(500, $"An error occurred while retrieving asset sub-master details: {ex.Message}");
            }
        }
    }
}