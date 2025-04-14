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

namespace Inspire.Erp.Application.Helpers
{
    public class PurchaseOrderRequisitionHelper
    {
        private readonly IRepository<Permissionapproval> _permissionapproval;
        private readonly IRepository<UserFile> _userFile;

        public PurchaseOrderRequisitionHelper(IRepository<Permissionapproval> permissionapproval, IRepository<UserFile> userFile)
        {
            _permissionapproval = permissionapproval;
            _userFile = userFile;
        }

        public async Task<List<EmailAddress>> GetApproverNameAndEmails(int levelId, int formId)
        {
            try
            {
                var result = (from p in await _permissionapproval.GetAllAsync()
                              join u in await _userFile.GetAllAsync() on (long)p.UserId equals u.UserId
                              where p.LevelId == levelId && p.ApprovalformId == formId
                              select new EmailAddress(u.emailId?.ToString() ?? "", u.UserName)).ToList();
                return result;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
