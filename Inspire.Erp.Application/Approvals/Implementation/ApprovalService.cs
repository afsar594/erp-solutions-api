using Inspire.Erp.Application.Approvals.Interface;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Application.Common.Email;
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
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Text.Json.Serialization;
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
        public async Task<Response<ApproverLevelSettingsDto>> SaveAsync2(ApproverLevelSettingsDto approverLevelSettingsDto)
        {
            try
            {
                var existingSettings = await _approverLevelSettings.GetAsQueryable().AsNoTracking()
                    .Where(x => x.FormType == approverLevelSettingsDto.FormType &&
                                x.JobId == approverLevelSettingsDto.JobId)
                    .ToListAsync();

                var incomingApprovers = approverLevelSettingsDto.ApproverLevels;
                var userIds = approverLevelSettingsDto.ApproverLevels.Select(x => x.ApproverId).ToList();
                var users = _userFileRepository.GetAsQueryable()
                                               .AsNoTracking()
                                               .Where(x => userIds.Contains(x.UserId))
                                               .Select(s => new
                                               {
                                                   Id = s.UserId,
                                                   Name = s.UserName,
                                                   Email = s.emailId,
                                               }).ToList();

                var settingsToDelete = new List<ApproverLevelSettings>();
                var settingsToAdd = new List<ApproverLevelSettings>();
                incomingApprovers.ForEach(incoming =>
                {
                    var user = users.FirstOrDefault(u => u.Id == incoming.ApproverId);
                    incoming.MetaData.ForEach(x =>
                    {
                        x.Name = user?.Name;
                        x.Email = user?.Email;
                    });
                    var serializedMeta = JsonConvert.SerializeObject(incoming.MetaData);

                    var match = existingSettings.FirstOrDefault(x =>
                        x.ApproverId == incoming.ApproverId &&
                        x.ApproverLevel == incoming.ApproverLevel &&
                        x.MinAmount == incoming.MinAmount &&
                        x.MaxAmount == incoming.MaxAmount &&
                        x.NotificationDays == incoming.NotificationDays &&
                        x.OptEmail == incoming.OptEmail &&
                        x.MetaData == serializedMeta &&
                        x.JobId == approverLevelSettingsDto.JobId);

                    if (match == null)
                    {
                        var toRemove = existingSettings.Where(x =>
                            x.ApproverId == incoming.ApproverId &&
                        x.ApproverLevel == incoming.ApproverLevel &&
                        x.MinAmount == incoming.MinAmount &&
                        x.MaxAmount == incoming.MaxAmount &&
                        x.NotificationDays == incoming.NotificationDays &&
                        x.OptEmail == incoming.OptEmail &&
                        x.MetaData == serializedMeta &&
                        x.JobId == approverLevelSettingsDto.JobId).ToList();

                        settingsToDelete.AddRange(toRemove);

                        settingsToAdd.Add(new ApproverLevelSettings
                        {
                            ApproverId = incoming.ApproverId,
                            ApproverLevel = incoming.ApproverLevel,
                            FormType = approverLevelSettingsDto.FormType,
                            MinAmount = incoming.MinAmount,
                            MaxAmount = incoming.MaxAmount,
                            NotificationDays = incoming.NotificationDays,
                            OptEmail = incoming.OptEmail ? true : false,
                            JobId = approverLevelSettingsDto.JobId,
                            MetaData = serializedMeta
                        });
                    }
                });

                if (settingsToDelete.Any())
                    await _approverLevelSettings.DeleteListAsync(settingsToDelete);

                if (settingsToAdd.Any())
                    await _approverLevelSettings.InsertListAsync(settingsToAdd);

                return Response<ApproverLevelSettingsDto>.Success(approverLevelSettingsDto, "Saved Successfully");
            }
            catch (Exception ex)
            {
                // You can log ex here if needed
                throw;
            }
        }
        public async Task<Response<ApproverLevelSettingsDto>> SaveAsync(ApproverLevelSettingsDto approverLevelSettingsDto)
        {
            try
            {
                var existingSettings = await _approverLevelSettings.GetAsQueryable().AsNoTracking()
                    .Where(x => x.FormType == approverLevelSettingsDto.FormType &&
                                x.JobId == approverLevelSettingsDto.JobId)
                    .ToListAsync();

                var incomingApprovers = approverLevelSettingsDto.ApproverLevels;
                var userIds = incomingApprovers.Select(x => x.ApproverId).Distinct().ToList();
                var users = _userFileRepository.GetAsQueryable().AsNoTracking()
                    .Where(x => userIds.Contains(x.UserId))
                    .Select(s => new { Id = s.UserId, Name = s.UserName, Email = s.emailId })
                    .ToList();

                var settingsToAdd = new List<ApproverLevelSettings>();
                var incomingSerialized = new List<string>();

                foreach (var incoming in incomingApprovers)
                {
                    var user = users.FirstOrDefault(u => u.Id == incoming.ApproverId);
                    incoming.MetaData.ForEach(x =>
                    {
                        x.Name = user?.Name;
                        x.Email = user?.Email;
                    });

                    var serializedMeta = JsonConvert.SerializeObject(incoming.MetaData);
                    incomingSerialized.Add(serializedMeta);

                    bool exists = existingSettings.Any(x =>
                        x.ApproverId == incoming.ApproverId &&
                        x.ApproverLevel == incoming.ApproverLevel &&
                        x.MinAmount == incoming.MinAmount &&
                        x.MaxAmount == incoming.MaxAmount &&
                        x.NotificationDays == incoming.NotificationDays &&
                        x.OptEmail == incoming.OptEmail &&
                        x.MetaData == serializedMeta &&
                        x.JobId == approverLevelSettingsDto.JobId);

                    if (!exists)
                    {
                        settingsToAdd.Add(new ApproverLevelSettings
                        {
                            ApproverId = incoming.ApproverId,
                            ApproverLevel = incoming.ApproverLevel,
                            FormType = approverLevelSettingsDto.FormType,
                            MinAmount = incoming.MinAmount,
                            MaxAmount = incoming.MaxAmount,
                            NotificationDays = incoming.NotificationDays,
                            OptEmail = incoming.OptEmail,
                            JobId = approverLevelSettingsDto.JobId,
                            MetaData = serializedMeta
                        });
                    }
                }

                var settingsToDelete = existingSettings.Where(existing =>
                    !incomingApprovers.Any(incoming =>
                    {
                        var user = users.FirstOrDefault(u => u.Id == incoming.ApproverId);
                        incoming.MetaData.ForEach(x =>
                        {
                            x.Name = user?.Name;
                            x.Email = user?.Email;
                        });
                        var serializedMeta = JsonConvert.SerializeObject(incoming.MetaData);
                        return existing.ApproverId == incoming.ApproverId &&
                               existing.ApproverLevel == incoming.ApproverLevel &&
                               existing.MinAmount == incoming.MinAmount &&
                               existing.MaxAmount == incoming.MaxAmount &&
                               existing.NotificationDays == incoming.NotificationDays &&
                               existing.OptEmail == incoming.OptEmail &&
                               existing.MetaData == serializedMeta &&
                               existing.JobId == approverLevelSettingsDto.JobId;
                    })).ToList();

                if (settingsToDelete.Any())
                    await _approverLevelSettings.DeleteListAsync(settingsToDelete);

                if (settingsToAdd.Any())
                    await _approverLevelSettings.InsertListAsync(settingsToAdd);

                return await GetAsync(approverLevelSettingsDto.FormType);
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
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
                            MaxAmount = (int)a.MaxAmount,
                            MinAmount = (int)a.MinAmount,
                            NotificationDays = a.NotificationDays,
                            OptEmail = a.OptEmail,
                            MetaData = JsonConvert.DeserializeObject<List<UserMetaDataDto>>(a.MetaData)
                        }).OrderBy(a => a.ApproverLevel).ToList()
                    })
                    .FirstOrDefault();


                return Response<ApproverLevelSettingsDto>.Success(result, "Data found");
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #endregion

    }
}
