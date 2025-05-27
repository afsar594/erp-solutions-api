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
using System.Security.Cryptography;
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

        public async Task<Response<ApproverLevelSettingsDto>> SaveAsync(ApproverLevelSettingsDto approverLevelSettingsDto)
        {
            try
            {
                var existingSettings = await _approverLevelSettings.GetAsQueryable().AsNoTracking()
                    .Where(x => x.FormType == approverLevelSettingsDto.FormType &&
                                x.JobId == approverLevelSettingsDto.JobId)
                    .ToListAsync();

                var incomingApprovers = approverLevelSettingsDto.ApproverLevels;
                var userIds = incomingApprovers.SelectMany(x => x.ApproverIds).Distinct().ToList();


                var users = _userFileRepository.GetAsQueryable().AsNoTracking()
                    .Where(x => userIds.Contains(x.UserId))
                    .Select(s => new { Id = s.UserId, Name = s.UserName, Email = s.emailId })
                    .ToList();

                var settingsToAdd = new List<ApproverLevelSettings>();
                var incomingSerialized = new List<string>();
                

                foreach (var incoming in incomingApprovers)
                {
                    int sno = 1;
                    incoming.ApproverIds.ForEach(userId =>
                    {
                        var user = users.FirstOrDefault(u => u.Id == userId);
                        incoming.MetaData.ForEach(x =>
                        {
                            x.Name = user?.Name;
                            x.Email = user?.Email;
                        });

                        var serializedMeta = JsonConvert.SerializeObject(incoming.MetaData);
                        incomingSerialized.Add(serializedMeta);

                        bool exists = existingSettings.Any(x =>
                            x.ApproverId == userId &&
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
                                ApproverId = userId,
                                ApproverLevel = incoming.ApproverLevel,
                                FormType = approverLevelSettingsDto.FormType,
                                MinAmount = incoming.MinAmount,
                                MaxAmount = incoming.MaxAmount,
                                NotificationDays = incoming.NotificationDays,
                                OptEmail = incoming.OptEmail,
                                JobId = approverLevelSettingsDto.JobId,
                                MetaData = serializedMeta,
                                SNo = sno++
                            });
                        }
                    });
                }

                var settingsToDelete = existingSettings.Where(existing =>
                         !incomingApprovers.Any(incoming =>
                             incoming.ApproverIds.Any(userId =>
                             {
                                 var user = users.FirstOrDefault(u => u.Id == userId);
                                 incoming.MetaData.ForEach(x =>
                                 {
                                     x.Name = user?.Name;
                                     x.Email = user?.Email;
                                 });

                                 var serializedMeta = JsonConvert.SerializeObject(incoming.MetaData);

                                 return existing.ApproverId == userId &&
                                        existing.ApproverLevel == incoming.ApproverLevel &&
                                        existing.MinAmount == incoming.MinAmount &&
                                        existing.MaxAmount == incoming.MaxAmount &&
                                        existing.NotificationDays == incoming.NotificationDays &&
                                        existing.OptEmail == incoming.OptEmail &&
                                        existing.MetaData == serializedMeta &&
                                        existing.JobId == approverLevelSettingsDto.JobId;
                             })
                         )).ToList();


                if (settingsToDelete.Any())
                    await _approverLevelSettings.DeleteListAsync(settingsToDelete);

                if (settingsToAdd.Any())
                    await _approverLevelSettings.InsertListAsync(settingsToAdd);

                return await GetAsync(approverLevelSettingsDto.FormType, approverLevelSettingsDto.JobId);
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                throw;
            }
        }

        public async Task<Response<ApproverLevelSettingsDto>> GetAsync(EnumOrderForms formType, int jobId)
        {
            try
            {
                var existingSettings = await this._approverLevelSettings
                                                .GetAsQueryable()
                                                .AsNoTracking()
                                                .Where(x => x.FormType == formType && x.JobId == jobId)
                                                .ToListAsync();

                var groupedByLevel = existingSettings
                    .GroupBy(x => x.ApproverLevel)
                    .Select(g =>
                    {
                        var first = g.First(); 
                        return new Approvers
                        {
                            ApproverLevel = first.ApproverLevel,
                            ApproverIds = g.Select(x => (long)x.ApproverId).ToList(),
                            NotificationDays = first.NotificationDays,
                            OptEmail = first.OptEmail,
                            MinAmount = first.MinAmount,
                            MaxAmount = first.MaxAmount,
                            MetaData = JsonConvert.DeserializeObject<List<UserMetaDataDto>>(first.MetaData)
                        };
                    })
                    .OrderBy(x => x.ApproverLevel)
                    .ToList();

                var result = new ApproverLevelSettingsDto
                {
                    FormType = formType,
                    JobId = jobId,
                    ApproverLevels = groupedByLevel
                };


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
