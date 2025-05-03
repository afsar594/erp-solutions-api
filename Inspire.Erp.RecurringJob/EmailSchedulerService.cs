using Inspire.Erp.Application.Common.Email;
using Inspire.Erp.Application.Email.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.RecurringJob
{
    public class EmailSchedulerService : IEmailSchedulerService
    {
        private readonly IEmailNotificationServices _emailSender;
        private IRepository<JobMasterJobDetails> _jobdetailsRepository;
        private IRepository<JobMasterJobWiseBankGuarantees> _jobBankGuaruRepository;
        private IRepository<ApproverLevelSettings> _approverLevelSettings;

        public EmailSchedulerService(IEmailNotificationServices emailSender, IRepository<JobMasterJobDetails> jobdetailsRepository, IRepository<JobMasterJobWiseBankGuarantees> jobBankGuaruRepository, IRepository<ApproverLevelSettings> approverLevelSettings)
        {
            _emailSender = emailSender;
            _jobdetailsRepository = jobdetailsRepository;
            _jobBankGuaruRepository = jobBankGuaruRepository;
            _approverLevelSettings = approverLevelSettings;

        }

        public async Task CheckAndSendEmailForJobBankGuarantees()
        {
            try
            {
                var today = DateTime.Today;
                var BankGuarantees = await _jobBankGuaruRepository
                                       .GetAsQueryable()
                                       //.Where(x => x.Job_Master_JobWiseBankGuarantees_Valid_To_Date.Value.Date.AddDays(-14) == today)
                                       .AsNoTracking()
                                       .Select(x => new
                                       {
                                           JobId = x.JobMasterJobWiseBankGuaranteesJobId,
                                           JobNo = x.JobMasterNo ?? "",
                                           Description = x.Job_Master_JobWiseBankGuarantees_Remarks ?? "",
                                           StatusDate = x.Job_Master_JobWiseBankGuarantees_Valid_To_Date,
                                           Amount = x.JobMasterJobWiseBankGuaranteesAmount ?? 0
                                       }).ToListAsync();
                if (!BankGuarantees.Any())
                    return;
                var jobIds = BankGuarantees.Select(x => x.JobId).ToList();
                var emailMetaDataDtos = await GetToEMails(jobIds);
                if (!emailMetaDataDtos.Any()) return;

                var toMails = emailMetaDataDtos.SelectMany(email => email.MetaData).ToList();
                var subject = "Job Scops Reminder";
                var message = new StringBuilder();
                message.AppendLine("Dear Team,");
                message.AppendLine("Please find the details of the jobs that are due for review:");
                await _emailSender.SendTextEmailAsync(toMails, subject, message.ToString(), new List<string>());
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task CheckAndSendEmailForJobScops()
        {
            var today = DateTime.Today;
            var jobDetails = await _jobdetailsRepository
                                   .GetAsQueryable()
                                   //.Where(x => x.JobMasterJobDetailsStatusDate.Value.Date.AddDays(-14) == today)
                                   .AsNoTracking()
                                   .Select(x => new
                                   {
                                       JobId = x.JobMasterJobDetailsJobId,
                                       JobNo = x.JobMasterNo,
                                       Description = x.JobMasterJobDetailsDescription,
                                       Status = x.JobMasterJobDetailsStatus,
                                       StatusDate = x.JobMasterJobDetailsStatusDate,
                                       RefNo = x.JobMasterJobDetailsRefNo,
                                       Amount = x.JobMasterJobDetailsJdAmt
                                   }).ToListAsync();
            if (!jobDetails.Any())
                return;
            var jobIds = jobDetails.Select(x => (int)x.JobId).ToList();
            var emailMetaDataDtos = await GetToEMails(jobIds);
            if (!emailMetaDataDtos.Any()) return;
            var toMails = emailMetaDataDtos.SelectMany(email => email.MetaData).ToList();

            if (today == new DateTime(2025, 5, 20).AddDays(-14))
            {
                var subject = "Job Scops Reminder";
                var message = new StringBuilder();
                message.AppendLine("Dear Team,");
                message.AppendLine("Please find the details of the jobs that are due for review:");
                //await _emailSender.SendTextEmailAsync(toMails, subject, message.ToString(), new List<string>());
            }
        }
        public async Task<List<EmailMetaDataDto>> GetToEMails(List<int> jobId)
        {
            try
            {
                var result = await _approverLevelSettings.GetAsQueryable().AsNoTracking()
                              .Where(x => jobId.Contains((int)x.JobId) && x.OptEmail)
                              .Select(a => new EmailMetaDataDto()
                              {
                                  JobId = a.JobId,
                                  NotificationDays = a.NotificationDays,
                                  MinAmount = a.MinAmount,
                                  MaxAmount = a.MaxAmount,
                                  MetaData = JsonConvert.DeserializeObject<List<UserEmail>>(a.MetaData),
                              }).ToListAsync();
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
