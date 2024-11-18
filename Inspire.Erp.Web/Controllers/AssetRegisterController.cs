using Inspire.Erp.Application.Master.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

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
        [HttpGet("AssetRigister")]
        public async Task<IActionResult> Show(DateTime? fromDate, DateTime? toDate, string searchText)
        {
            var result = await _assetRegisterService.GetAssetsRegisterAsync(fromDate, toDate, searchText);

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }
    }
}
