using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Infrastructure.Database;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Inspire.Erp.Domain.Models.Approvals;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Application.Common;

namespace Inspire.Erp.Application.Helpers
{
    public class PurchaseOrderRequisitionHelper
    {
        private readonly IRepository<ApproverLevelSettings> _approverLevelSettings;
        private readonly IRepository<UserFile> _userFile;

        public PurchaseOrderRequisitionHelper(IRepository<UserFile> userFile, IRepository<ApproverLevelSettings> approverLevelSettings)
        {
            _userFile = userFile;
            _approverLevelSettings = approverLevelSettings;
        }
        public async Task<List<OrderApproversDto>> GetOrderApproverNameAndEmail(EnumOrderForms orderType, EnumApproverLevels approverLevel)
        {
            try
            {

                var result = (from a in _approverLevelSettings.GetAsQueryable().AsNoTracking()
                              where a.FormType == orderType
                              join u in _userFile.GetAsQueryable().AsNoTracking()
                                  on a.ApproverId equals u.UserId
                              select new
                              {
                                  a.FormType,
                                  a.ApproverId,
                                  a.ApproverLevel,
                                  a.OptEmail,
                                  u.UserName,
                                  u.emailId
                              })
                 .ToList()
                 .GroupBy(x => new { x.FormType, x.ApproverId, x.UserName, x.ApproverLevel })
                 .Select(g => new OrderApproversDto()
                 {
                     Id = g.Key.ApproverId,
                     Name = g.Key.UserName,
                     Level = g.Key.ApproverLevel,
                     Emails = g.Select(x => new ApproverOptEmail()
                     {
                         OptEmail = x.OptEmail,
                         Email = new EmailAddress(x.emailId ?? "", x.UserName)
                     }).ToList()
                 })
                 .ToList();

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
