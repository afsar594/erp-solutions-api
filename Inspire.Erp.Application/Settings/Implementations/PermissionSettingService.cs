using Inspire.Erp.Application.Settings.Interfaces;
using Inspire.Erp.Domain.DTO.WorkGropPermission;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Inspire.Erp.Application.Settings.Implementations
{
    public class PermissionSettingService : IPermissionSettingService
    {

        private IRepository<WorkGroupPermissions> _groupPermissions;
        private IRepository<FormNames> _formNames;


        public PermissionSettingService(IRepository<WorkGroupPermissions> groupPermissions, IRepository<FormNames> formNames)
        {
            _groupPermissions = groupPermissions;
            _formNames = formNames;
            
        }
        public Response<List<WorkGroupPermissionsDto>> GetPermissionSetting(int? groupId)
        {
            List<WorkGroupPermissionsDto> permissionList = new List<WorkGroupPermissionsDto>();

            permissionList = (from wgp in _groupPermissions.GetAsQueryable().Where(x => x.WorkGroupPermissionsWorkGroupId == groupId)
                              join fn in _formNames.GetAsQueryable() on wgp.WorkGroupPermissionsFormId equals fn.FormNamesId into fnGroup
                              from fn in fnGroup.DefaultIfEmpty()
                              select new WorkGroupPermissionsDto
                              {
                                  WorkGroupPermissionsFormName = fn.FormNamesName,
                                  WorkGroupPermissionsUadd = wgp.WorkGroupPermissionsUadd,
                                  WorkGroupPermissionsUallow = wgp.WorkGroupPermissionsUallow,
                                  WorkGroupPermissionsUdelete = wgp.WorkGroupPermissionsUdelete,
                                  WorkGroupPermissionsUview =   wgp.WorkGroupPermissionsUview,
                                  WorkGroupPermissionsUprint = wgp.WorkGroupPermissionsUprint,
                                  WorkGroupPermissionsUedit = wgp.WorkGroupPermissionsUedit,
                                  WorkGroupPermissionsFormId = wgp.WorkGroupPermissionsFormId,
                                  WorkGroupPermissionsWorkGroupId = wgp.WorkGroupPermissionsWorkGroupId,
                                 

                              }).ToList();

            return new Response<List<WorkGroupPermissionsDto>>
            { 
              Valid = true,
              Result = permissionList,
              Message = "Permission Data Found"
            };
        }

        public async Task<Response<List<WorkGroupPermissionsDto>>> UpdatePermissionSetting(List<WorkGroupPermissions> permissionSetting)
        {
            
            _groupPermissions.UpdateList(permissionSetting);
           
            return new Response<List<WorkGroupPermissionsDto>>
            {
                Result = this.GetPermissionSetting(permissionSetting[0].WorkGroupPermissionsWorkGroupId).Result,
                Valid = true,
                Message = "saved Successfully"
            };
                 
        }
    }
}
