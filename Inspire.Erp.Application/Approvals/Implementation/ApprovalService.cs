using Inspire.Erp.Application.Approvals.Interface;
using Inspire.Erp.Application.Common;
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
        
        public ApprovalService(ILogger<ApprovalService> logger, IRepository<ApprovalDetail> approvalDetailService, IRepository<Approval> approvalService, IRepository<UserFile> userFileRepository, IRepository<tbl_approvalforms> approvalforms, IRepository<Permissionapproval> Permissionapproval)
        {
            _logger = logger;
            _approvalDetailService = approvalDetailService;
            _approvalService = approvalService;
            _userFileRepository = userFileRepository;
            _approvalforms = approvalforms;
            _Permissionapproval= Permissionapproval;

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
                        }).ToList() : new List<ApprovalDetailResponse>(),

                    }).Skip(model.Skip).Take(model.Take).ToListAsync();
                var gridResult = new GridWrapperResponse<List<ApprovalResponse>>();
                gridResult.Data = result;
                gridResult.Total=total;

                return Response<GridWrapperResponse<List<ApprovalResponse>>>.Success(gridResult, "Record found.");
            }
            catch (Exception ex)
            {
                return Response<GridWrapperResponse<List<ApprovalResponse>>>.Fail(new GridWrapperResponse<List<ApprovalResponse>>(), ex.Message);
            }
        }

        public IEnumerable<UserFile> GetAllUsers()
        {
            IEnumerable<UserFile> UserFile;
            try
            {
                UserFile = _userFileRepository.GetAll().Where(a => a.Deleted != true).Select(k => k);

            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
            }
            return UserFile;
        }

        public IEnumerable<tbl_approvalforms> GetAllapprovalforms()
        {
            IEnumerable<tbl_approvalforms> approvalforms;
            try
            {
                approvalforms = _approvalforms.GetAll().Where(a => a.is_active != true).Select(k => k);

            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
            }
            return approvalforms;
        }

        public Permissionapproval GetPermissionapById(long id)
        {
            var GetPermissionapById = _Permissionapproval.GetAll().Where(x => x.ApprovalformId == id).SingleOrDefault();
            return GetPermissionapById;
        }
        public Permissionapproval Permissionapproval(Permissionapproval Permissionapproval)
        {
            try
            {
                _Permissionapproval.BeginTransaction();

                //insert Permission approval
                var Permissionapprovals = new List<Permissionapproval>();
                foreach (var PA in Permissionapprovals)
                {


                    var tr = new Permissionapproval
                    {
                        ApprovalformId = (int)Permissionapproval.ApprovalformId,
                        LevelId = Permissionapproval.LevelId,
                        UserId = (int)Permissionapproval.UserId,
                        Username = Permissionapproval.Username,
                        CreatedDate = (DateTime)Permissionapproval.CreatedDate,                    

                    };
                    _Permissionapproval.Insert(tr);
                }
              
                _Permissionapproval.TransactionCommit();
                return this.GetPermissionapById(Permissionapproval.ApprovalformId);

            }
            catch (Exception ex)
            {
                _Permissionapproval.TransactionRollback();
                throw ex;
            }

        }

        IEnumerable<PermissionApproval> IApprovalService.GetAllapprovalforms()
        {
            throw new NotImplementedException();
        }
        PermissionApproval IApprovalService.GetPermissionapById(long a)
        {
            throw new NotImplementedException();
        }

        public PermissionApproval InsertPermissionApproval(List<PermissionApproval> Perm)
        {
            throw new NotImplementedException();
        }
    }
}
