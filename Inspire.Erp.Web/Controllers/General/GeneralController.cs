using Inspire.Erp.Application.Settings.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers.General
{
    [Route("api/General")]
    [Produces("application/json")]
    [ApiController]
    public class GeneralController : ControllerBase
    {
        private readonly IReportTypeService _service;
        public GeneralController(IReportTypeService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetReportByTypeAsync(string type)
        {
            return Ok(await _service.GetReportByTypeAsync(type));
        }
    }
}
