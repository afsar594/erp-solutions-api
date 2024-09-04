using Inspire.Erp.Application.Approvals.Interface;
using Inspire.Erp.Application.Settings.Interface;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using Inspire.Erp.Domain.Entities;
using System.Collections.Generic;
using Inspire.Erp.Domain.Models;
using AutoMapper;
using NPOI.SS.Formula.Functions;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Web.ViewModels;
namespace Inspire.Erp.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApprovalsController : ControllerBase
    {
        private readonly IApprovalService _approvalService;
        private readonly IMapper _mapper;
        public ApprovalsController(IApprovalService approvalService, IMapper mapper )
        {
            _approvalService = approvalService;
            _mapper = mapper;
        }
        [HttpPost("GetApprovalsRecords")]
        public async Task<IActionResult> GetApprovalsRecords(GenericGridViewModel model)
        {
            return Ok(await _approvalService.GetApprovalsRecords(model));
        }

        [HttpGet]
        [Route("GetAllUsers")]
        public ApiResponse<List<UserFile>> GetAllUsers()
        {
            ApiResponse<List<UserFile>> apiResponse = new ApiResponse<List<UserFile>>
            {
                Valid = true,
                Result = _mapper.Map<List<UserFile>>(_approvalService.GetAllUsers()),
                Message = ""
            };
            return apiResponse;
        }
        [HttpGet]
        [Route("GetAllapprovalforms")]
        public ApiResponse<List<PermissionApproval>> GetAllapprovalforms()
        {
            ApiResponse<List<PermissionApproval>> apiResponse = new ApiResponse<List<PermissionApproval>>
            {
                Valid = true,
                Result = _mapper.Map<List<PermissionApproval>>(_approvalService.GetAllapprovalforms()),
                Message = ""
            };
            return apiResponse;
        }
        [HttpGet]
        [Route("GetPermissionapById")]

        public PermissionApproval GetPermissionapById(long id)
        {
            var GetPermission = _approvalService.GetPermissionapById(id);

            return GetPermission;
        }
        [HttpPost]
        [Route("InsertPermissionApproval")]
        public ApiResponse<PermissionApproval> InserUser([FromBody] List<PermissionApproval> Perm)
        {
            ApiResponse<PermissionApproval> apiResponse = new ApiResponse<PermissionApproval>();

            var param1 = _mapper.Map<List<PermissionApproval>>(Perm);
            var xs = _approvalService.InsertPermissionApproval(param1);

            PermissionApproval Permission = new PermissionApproval();
            Permission = _mapper.Map<PermissionApproval>(xs);

            apiResponse = new ApiResponse<PermissionApproval>
            {
                Valid = true,
                Result = Permission,
                Message =""
            };
            return apiResponse;
        }
    }
}
