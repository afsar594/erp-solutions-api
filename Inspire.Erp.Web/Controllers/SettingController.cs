using Inspire.Erp.Application.Common;
using Inspire.Erp.Application.Settings.Interfaces;
using Inspire.Erp.Domain.Modals.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingController : ControllerBase
    {
        private readonly IMenuService _menuService;
        public SettingController(IMenuService menuService)
        {
            _menuService = menuService;
        }
        #region MENU 
        [HttpPost("GetMenusByWorkgroupId")]
        public async Task<IActionResult> GetMenusByWorkgroupId(int id = 0)
        {
            return Ok(await _menuService.GetMenusByWorkgroupId(id));
        }
        #endregion
    }
}
