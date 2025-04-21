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
                var result = (from a in _approverLevelSettings.GetAll()
                              join u in _userFile.GetAll() on a.ApproverId equals u.UserId
                              where a.FormType == orderType && a.ApproverLevel == approverLevel
                              group new { a, u } by new { a.FormType, a.ApproverId, u.UserName, a.ApproverLevel } into g
                              select new OrderApproversDto()
                              {
                                  Id = g.Key.ApproverId,
                                  Name = g.Key.UserName,
                                  Level = g.Key.ApproverLevel,
                                  Emails = g.Select(x => new ApproverOptEmail()
                                  {
                                      OptEmail = x.a.OptEmail,
                                      Email = new EmailAddress(x.u.emailId ?? "", x.u.UserName)
                                  }).ToList()
                              }).ToList();
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
