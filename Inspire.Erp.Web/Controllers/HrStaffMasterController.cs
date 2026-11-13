using Inspire.Erp.Application.Master;
using Inspire.Erp.Application.Master.Implementations;
using Inspire.Erp.Application.Master.Interfaces;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inspire.Erp.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HrStaffMasterController : ControllerBase
    {
        private IHrStaffMasterService hrstaffmasterservice;
        public HrStaffMasterController(IHrStaffMasterService _hrstaffmasterservice )
        {
            hrstaffmasterservice = _hrstaffmasterservice;   
        }
        [HttpGet]
        [Route("GetAllHrStaff")]
        public IActionResult GetAllHrStaff() { 
            var data = hrstaffmasterservice.GetAllHrStaff();
            return Ok(data);
        
        }

    }
}
