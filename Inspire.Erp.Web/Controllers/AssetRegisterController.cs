using Inspire.Erp.Application.Master.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;


using Inspire.Erp.Domain.DTO.Asset_Register;
using Inspire.Erp.Domain.Models.Master;

namespace Inspire.Erp.Web.Controllers
{
    [Route("api/master")]
    [Produces("application/json")]
    [ApiController]
    public class AssetRegisterController : ControllerBase
    {
        readonly IAssetRegisterService _assetRegisterService;
        public AssetRegisterController(IAssetRegisterService assetRegisterService)
        {
             _assetRegisterService = assetRegisterService;
        }
        [HttpPost("AssetRegister")]
        public async Task<IActionResult> Show(AssetRegisterModel model)
        {
            var result = await _assetRegisterService.GetAssetsRegisterAsync(model.fromDate, model.toDate, model.searchText);

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }
    }
}
