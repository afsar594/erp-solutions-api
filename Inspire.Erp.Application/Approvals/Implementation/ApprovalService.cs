using Inspire.Erp.Application.Approvals.Interface;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.DTO;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Entities.POS;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Models.Approvals;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Approvals.Implementation
{
    public class ApprovalService : IApprovalService
    {
        private readonly ILogger<ApprovalService> _logger;
        private readonly IRepository<Approval> _approvalService;
        private readonly IRepository<ApprovalDetail> _approvalDetailService;
        private IRepository<UserFile> _userFileRepository;
        private IRepository<tbl_approvalforms> _approvalforms;
        private IRepository<Permissionapproval> _Permissionapproval;
        private IRepository<ApproverLevelSettings> _approverLevelSettings;

        public ApprovalService(ILogger<ApprovalService> logger, IRepository<ApprovalDetail> approvalDetailService, IRepository<Approval> approvalService, IRepository<UserFile> userFileRepository, IRepository<tbl_approvalforms> approvalforms, IRepository<Permissionapproval> Permissionapproval, IRepository<ApproverLevelSettings> approverLevelSettings)
        {
            _logger = logger;
            _approvalDetailService = approvalDetailService;
            _approvalService = approvalService;
            _userFileRepository = userFileRepository;
            _approvalforms = approvalforms;
            _Permissionapproval = Permissionapproval;
            _approverLevelSettings = approverLevelSettings;
        }
        public async Task<Response<GridWrapperResponse<List<ApprovalResponse>>>> GetApprovalsRecords(GenericGridViewModel model)
        {
            try
            {
                string filter = $@"1==1 {model.Filter}";
                int total = await _approvalService.GetAsQueryable().CountAsync();
                var result = await _approvalService.GetAsQueryable().Include(x => x.ApprovalDetails).Where(filter)
                    .Select(x => new ApprovalResponse()
                    {
                        CreatedAt = x.CreatedAt,
                        CreatedBy = x.CreatedBy,
                        LocationId = x.LocationId,
                        VoucherType = x.VoucherType,
                        Status = x.Status,
                        Id = x.Id,
                        ApprovalDetails = x.ApprovalDetails != null && x.ApprovalDetails.Count > 0 ? x.ApprovalDetails.Select(c => new ApprovalDetailResponse
                        {
                            Id = c.Id,
                            ApprovalId = c.ApprovalId,
                            ApprovedAt = c.ApprovedAt,
                            Status = c.Status,
                            UserId = c.UserId,
                        }).OrderBy(x => x.ApprovedAt).AsEnumerable().ToList() : new List<ApprovalDetailResponse>(),

                    }).Skip(model.Skip).Take(model.Take).ToListAsync();
                var gridResult = new GridWrapperResponse<List<ApprovalResponse>>();
                gridResult.Data = result;
                gridResult.Total = total;

                return Response<GridWrapperResponse<List<ApprovalResponse>>>.Success(gridResult, "Record found.");
            }
            catch (Exception ex)
            {
                return Response<GridWrapperResponse<List<ApprovalResponse>>>.Fail(new GridWrapperResponse<List<ApprovalResponse>>(), ex.Message);
            }
        }



        public List<Permissionapproval> GetPermissionapById(long id, int level)
        {
            var GetPermissionapById = _Permissionapproval.GetAsQueryable().AsNoTracking().Where(x => x.ApprovalformId == id && x.LevelId == level).ToList();
            return GetPermissionapById;
        }
        public List<Permissionapproval> InsertPermissionApproval(List<Permissionapproval> Permissionapproval)
        {
            try
            {
                _Permissionapproval.BeginTransaction();

                foreach (var item in Permissionapproval)
                {
                    if (item.isActive)
                    {
                        if (item.ApId == 0)
                        {
                            _Permissionapproval.Insert(item);
                        }
                        else
                        {
                            _Permissionapproval.Update(item);
                        }
                    }
                    else
                    {
                        if (item.ApId == 0)
                        {

                        }
                        else
                        {
                            _Permissionapproval.Delete(item);
                        }
                    }
                }
                _Permissionapproval.TransactionCommit();
                return Permissionapproval;
            }
            catch (Exception ex)
            {
                _Permissionapproval.TransactionRollback();
                throw ex;
            }
        }

        #region Not to remove/replace this code related to apporver level settings 
        public async Task<Response<ApproverLevelSettingsDto>> SaveAsync(ApproverLevelSettingsDto approverLevelSettingsDto)
        {
            try
            {
                var existingSettings = await this._approverLevelSettings.GetAsQueryable().AsNoTracking()
                                                        .Where(x => x.FormType == approverLevelSettingsDto.FormType)
                                                        .ToListAsync();

                var incomingApprovers = approverLevelSettingsDto.ApproverLevels;
                var settingsToAdd = incomingApprovers
                    .Where(x => !existingSettings.Any(a =>
                            a.ApproverLevel == x.ApproverLevel &&
                            a.ApproverId == x.ApproverId &&
                            a.MinAmount == x.MinAmount &&
                            a.MaxAmount == x.MaxAmount &&
                            a.NotificationDays == x.NotificationDays &&
                            a.OptEmail == x.OptEmail
                        ))
                    .Select(x => new ApproverLevelSettings
                    {
                        ApproverId = x.ApproverId,
                        ApproverLevel = x.ApproverLevel,
                        FormType = approverLevelSettingsDto.FormType,
                        MinAmount = x.MinAmount,
                        MaxAmount = x.MaxAmount,
                        NotificationDays = x.NotificationDays,
                        OptEmail = x.OptEmail,
                    }).ToList();

                var settingsToDelete = existingSettings
                    .Where(a => !incomingApprovers.Any(x =>
                        x.ApproverId == a.ApproverId && 
                        x.ApproverLevel == a.ApproverLevel && 
                        x.MinAmount == a.MinAmount &&
                        x.MaxAmount == a.MaxAmount &&
                        x.NotificationDays == a.NotificationDays &&
                        x.OptEmail == a.OptEmail))
                    .ToList();

                // Insert new records
                if (settingsToAdd.Any())
                    await this._approverLevelSettings.InsertListAsync(settingsToAdd);

                // Delete old records if needed
                if (settingsToDelete.Any())
                    await this._approverLevelSettings.DeleteListAsync(settingsToDelete);

                return Response<ApproverLevelSettingsDto>.Success(approverLevelSettingsDto, "Saved Successfully");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Response<ApproverLevelSettingsDto>> GetAsync(EnumOrderForms formType)
        {
            try
            {
                var existingSettings = await this._approverLevelSettings
                    .GetAsQueryable()
                    .AsNoTracking()
                    .Where(x => x.FormType == formType)
                    .ToListAsync();

                var result = existingSettings
                    .GroupBy(x => x.FormType)
                    .Select(x => new ApproverLevelSettingsDto()
                    {
                        FormType = x.Key,
                        ApproverLevels = x.Select(a => new Approvers()
                        {
                            ApproverId = a.ApproverId,
                            ApproverLevel = a.ApproverLevel,
                            MaxAmount = a.MaxAmount,
                            MinAmount = a.MinAmount,
                            NotificationDays = a.NotificationDays,
                            OptEmail = a.OptEmail
                        }).ToList()
                    })
                    .FirstOrDefault();


                return Response<ApproverLevelSettingsDto>.Success(result, "Data found");
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

    }
}
