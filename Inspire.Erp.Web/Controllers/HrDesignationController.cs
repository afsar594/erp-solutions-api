using Inspire.Erp.Application.Master;
using Inspire.Erp.Application.Master.Implementation;
using Inspire.Erp.Application.Master.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Web.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers.C
{
    [Route("api/[Controller]")]
    [ApiController]
    public class HrDesignationController : ControllerBase
    {
        private IHrDesignationService _hrDesignationService;

        public HrDesignationController(IHrDesignationService hrDesignationService) {
            _hrDesignationService = hrDesignationService;
        }
        [HttpGet("GetAllDesignation")]
        public async Task<dynamic> GetAllDesignation()
        {
            return await _hrDesignationService.GetAllDesignation();
        }
        [HttpGet("GetDHrDesignationById/{id}")]
        public ApiResponse<HrDesignation> GetDHrDesignationById(int id)
        {
            var HrDesignation = _hrDesignationService.GetDHrDesignationById(id);

            ApiResponse<HrDesignation> apiResponse = new ApiResponse<HrDesignation>
            {
                Valid = true,
                Result = HrDesignation,
                Message = ""
            };
            return apiResponse;
        }
        [HttpPost("InsertDesignation")]
        
        public async Task<dynamic> InsertDesignation(HrDesignation obj) 
        {
            return await _hrDesignationService.InsertDesignation(obj);
         }

        [HttpPost("UpdateDesignation")]
        public async Task<dynamic> UpdateDepartment(HrDesignation obj)
        {
            return await _hrDesignationService.UpdateDesignation(obj);
        }

        //[HttpDelete("DeleteDesignation")]
        //public async Task<dynamic> DeleteDesignation(HrDesignation obj)
        //{
        //    return await _hrDesignationService.DeleteDesignation(obj);
        //}



        [HttpPost]
        [Route("DeleteDesignation/{id}")]
        public IActionResult DeleteDesignation(long Id)
        {
            try
            {
                return Ok(_hrDesignationService.DeleteDesignation(Id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }
    }
}
