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
using Inspire.Erp.Application.Common.Email;
using Newtonsoft.Json;

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
        public async Task<List<EmailMetaDataDto>> GetOrderApproverNameAndEmail(EnumOrderForms orderType, EnumApproverLevels approverLevel, int jobId)
        {
            try
            {
                var result = _approverLevelSettings.GetAsQueryable().AsNoTracking()
                              .Where(x=>x.FormType == orderType && x.JobId == jobId)
                              .Select(a => new EmailMetaDataDto()
                              {
                                  FormType = a.FormType,
                                  ApproverId = a.ApproverId,
                                  ApproverLevel = a.ApproverLevel,
                                  OptEmail = a.OptEmail,
                                  JobId = a.JobId,
                                  NotificationDays = a.NotificationDays,
                                  MinAmount = a.MinAmount,
                                  MaxAmount = a.MaxAmount,
                                  MetaData = JsonConvert.DeserializeObject<List<UserEmail>>(a.MetaData),
                                  Sno = a.SNo
                              }).ToList();
                

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
    
    public async Task<List<EmailMetaDataDto>> GetOrderApproverNameAndEmail(EnumOrderForms orderType, int jobId)
        {
            try
            {
                var result = _approverLevelSettings.GetAsQueryable().AsNoTracking()
                              .Where(x=>x.FormType == orderType && x.JobId == jobId)
                              .Select(a => new EmailMetaDataDto()
                              {
                                  FormType = a.FormType,
                                  ApproverId = a.ApproverId,
                                  ApproverLevel = a.ApproverLevel,
                                  OptEmail = a.OptEmail,
                                  JobId = a.JobId,
                                  NotificationDays = a.NotificationDays,
                                  MinAmount = a.MinAmount,
                                  MaxAmount = a.MaxAmount,
                                  MetaData = JsonConvert.DeserializeObject<List<UserEmail>>(a.MetaData),
                                  Sno = a.SNo
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
