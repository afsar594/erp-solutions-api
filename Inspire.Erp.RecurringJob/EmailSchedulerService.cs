using Inspire.Erp.Application.Common.Email;
using Inspire.Erp.Application.Email.Interfaces;
using Inspire.Erp.Domain.Constants;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Inspire.Erp.RecurringJob.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
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
        private readonly IRepository<JobMaster> _jobMasterRepo;
        private readonly IRepository<JobMasterJobDetails> _jobdetailsRepository;
        private readonly IRepository<JobMasterJobWiseBankGuarantees> _jobBankGuaruRepository;
        private readonly IRepository<ApproverLevelSettings> _approverLevelSettings;
        private readonly ILogger<EmailSchedulerService> _logger;

        public EmailSchedulerService(IEmailNotificationServices emailSender, IRepository<JobMasterJobDetails> jobdetailsRepository, IRepository<JobMasterJobWiseBankGuarantees> jobBankGuaruRepository, IRepository<ApproverLevelSettings> approverLevelSettings, ILogger<EmailSchedulerService> logger, IRepository<JobMaster> jobMasterRepo)
        {
            _emailSender = emailSender;
            _jobdetailsRepository = jobdetailsRepository;
            _jobBankGuaruRepository = jobBankGuaruRepository;
            _approverLevelSettings = approverLevelSettings;
            _logger = logger;
            _jobMasterRepo = jobMasterRepo;
        }

        public async Task CheckAndSendEmailForJobBankGuarantees()
        {
            try
            {
                var today = DateTime.Today;
                _logger.LogInformation("CheckAndSendEmailForJobBankGuarantees: Started");
                var groupedGuarantees = await GetGroupedDueBankGuarantees(today);
                if (!groupedGuarantees.Any())
                    return;

                var jobIds = groupedGuarantees
                                .Select(g => g.JobId)
                                .Distinct()
                                .ToList();
                var jobNo = groupedGuarantees.FirstOrDefault()?.JobNo ?? "";
                var projectName = groupedGuarantees.FirstOrDefault()?.ProjectName ?? "";



                var emailMetaDataDtos = await GetToEMails(jobIds);
                if (!emailMetaDataDtos.Any())
                    return;

                var toEmails = emailMetaDataDtos.SelectMany(e => e.MetaData).Distinct().ToList();

                var subject = $"Expiry of Bank Guarantee for Job No. {jobNo}";
                var messageBody = ComposeBankGuaranteesEmailBody(groupedGuarantees, projectName, jobNo);

                var emailStatus = await _emailSender.SendTextEmailAsync(toEmails, subject, messageBody, new List<string>());
                if (emailStatus)
                    await UpdateEmailSentFlag(jobIds, false);
                _logger.LogInformation("CheckAndSendEmailForJobBankGuarantees: Email Sent");


            }
            catch (Exception ex)
            {
                _logger.LogInformation("CheckAndSendEmailForJobBankGuarantees: Started", ex);
            }
        }

        private async Task UpdateEmailSentFlag(List<int> jobIds, bool isJobScope)
        {
            if (isJobScope)
            {
                var jobScopesTpUpdate = await this._jobdetailsRepository.GetAsQueryable().Where(x => jobIds.Contains((int)x.JobMasterJobDetailsJobId)).ToListAsync();
                if (jobScopesTpUpdate != null && jobScopesTpUpdate.Any())
                {
                    jobScopesTpUpdate.ForEach(jobScope =>
                    {
                        jobScope.EmailSent = true;
                    });
                    await this._jobdetailsRepository.UpdateListAsync(jobScopesTpUpdate);
                }
            }
            else
            {
                var jobBankGuarTpUpdate = await this._jobBankGuaruRepository.GetAsQueryable().Where(x => jobIds.Contains((int)x.JobMasterJobWiseBankGuaranteesJobId)).ToListAsync();
                if (jobBankGuarTpUpdate != null && jobBankGuarTpUpdate.Any())
                {
                    jobBankGuarTpUpdate.ForEach(jobScope =>
                    {
                        jobScope.EmailSent = true;
                    });
                    await this._jobBankGuaruRepository.UpdateListAsync(jobBankGuarTpUpdate);
                }
            }
        }

        private async Task<List<JobGroupDataDto>> GetGroupedDueBankGuarantees(DateTime today)
        {
            var rawData = await _jobBankGuaruRepository
                             .GetAsQueryable()
                             .Join(_approverLevelSettings.GetAsQueryable().AsNoTracking(),
                                 bg => bg.JobMasterJobWiseBankGuaranteesJobId,
                                 setting => setting.JobId,
                                 (bg, setting) => new { bg, setting })
                             .Join(_jobMasterRepo.GetAsQueryable().AsNoTracking(),
                                 combined => combined.bg.JobMasterJobWiseBankGuaranteesJobId,
                                 jm => jm.JobMasterJobId,
                                 (combined, jm) => new { combined.bg, combined.setting, JobMaster = jm })
                             .Where(x =>
                                 x.bg.Job_Master_JobWiseBankGuarantees_Valid_To_Date.HasValue &&
                                 x.bg.Job_Master_JobWiseBankGuarantees_Valid_To_Date.Value.Date.AddDays(-x.setting.NotificationDays) <= today &&
                                 x.setting.FormType == EnumOrderForms.PurchaseOrder &&
                                 (x.bg.EmailSent == null || x.bg.EmailSent == false))
                             .ToListAsync();

            var groupedData = rawData
                .GroupBy(x => new
                {
                    x.JobMaster.JobMasterJobId,
                    x.JobMaster.JobMasterJobNumber,
                    x.JobMaster.JobMasterJobName
                })
                .Select(group => new JobGroupDataDto
                {
                    JobId = group.Key.JobMasterJobId,
                    JobNo = group.Key.JobMasterJobNumber,
                    JobName = group.Key.JobMasterJobName,
                    Items = group.GroupBy(g => new { g.bg.JobMasterJobWiseBankGuaranteesAmount, g.bg.Job_Master_JobWiseBankGuarantees_Valid_To_Date })
                    .Select(item => new JobGroupItem
                    {
                        Amount = item.Key.JobMasterJobWiseBankGuaranteesAmount ?? 0,
                        //Description = item.bg.JobMasterJobWiseBankGuaranteesDesc,
                        Date = item.Key.Job_Master_JobWiseBankGuarantees_Valid_To_Date?.ToString("yyyy MMM dd") ?? "",
                    }).ToList()
                })
                .ToList();

            return groupedData;


        }

        private string ComposeBankGuaranteesEmailBody(List<JobGroupDataDto> groupedGuarantees, string projectName, string jobNameNo)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("  <meta charset=\"UTF-8\">");
            sb.AppendLine("  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            sb.AppendLine("  <title>Bank Guarantee Reminder</title>");
            sb.AppendLine("  <style>");
            sb.AppendLine("    body {");
            sb.AppendLine("      font-family: Arial, sans-serif;");
            sb.AppendLine("      background-color: #f9f9f9;");
            sb.AppendLine("      color: #333;");
            sb.AppendLine("      padding: 20px;");
            sb.AppendLine("    }");
            sb.AppendLine("    .email-container {");
            sb.AppendLine("      background-color: #ffffff;");
            sb.AppendLine("      padding: 20px;");
            sb.AppendLine("      border-radius: 6px;");
            sb.AppendLine("      box-shadow: 0 2px 4px rgba(0,0,0,0.1);");
            sb.AppendLine("      max-width: 600px;");
            sb.AppendLine("      margin: auto;");
            sb.AppendLine("    }");
            sb.AppendLine("    .content {");
            sb.AppendLine("      font-size: 16px;");
            sb.AppendLine("      line-height: 1.6;");
            sb.AppendLine("    }");
            sb.AppendLine("    .footer {");
            sb.AppendLine("      margin-top: 30px;");
            sb.AppendLine("      font-size: 14px;");
            sb.AppendLine("      color: #888;");
            sb.AppendLine("    }");
            sb.AppendLine("    ul {");
            sb.AppendLine("      padding-left: 20px;");
            sb.AppendLine("    }");
            sb.AppendLine("    li {");
            sb.AppendLine("      margin-bottom: 15px;");
            sb.AppendLine("    }");
            sb.AppendLine("  </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("  <div class=\"email-container\">");
            sb.AppendLine("    <div class=\"content\">");
            sb.AppendLine("      <p>Dear Team,</p>");
            sb.AppendLine($"      <p><br/>This is a courteous reminder regarding the bank guarantee under <strong> Job ID: {jobNameNo} </strong>.</p>");
            sb.AppendLine("      <p><br/>Please find the details of the Job Scope that are due for review:</p>");

            foreach (var group in groupedGuarantees)
            {
                group.Items.ForEach(item =>
                {
                    sb.AppendLine("      <ul>");
                    sb.AppendLine("        <li>");
                    sb.AppendLine($"          <strong>Amount:</strong> {item.Amount}<br/>");
                    sb.AppendLine($"          <strong>Date:</strong> {item.Date}<br/>");
                    //sb.AppendLine($"          <strong>Description:</strong> {group.Description}<br/>");
                    // sb.AppendLine($"          <strong>Due Date:</strong> {group.StatusDate:yyyy-MM-dd}"); // Uncomment if needed
                    sb.AppendLine("        </li>");
                    sb.AppendLine("      </ul>");
                });

            }

            sb.AppendLine("      <p>Please take the necessary actions to ensure timely progress.</p>");
            sb.AppendLine("      <p>Regards,<br/>Automated Notification Service</p>");
            sb.AppendLine("    </div>");
            sb.AppendLine("    <div class=\"footer\">");
            sb.AppendLine("      &copy; 2025 Water Bird Wtc. All rights reserved.");
            sb.AppendLine("    </div>");
            sb.AppendLine("  </div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        private string ComposeJobScopReminderEmailBody(List<JobGroupDataDto> groupedGuarantees, string projectName, string jobNameNo)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("  <meta charset=\"UTF-8\">");
            sb.AppendLine("  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            sb.AppendLine("  <title>Bank Guarantee Reminder</title>");
            sb.AppendLine("  <style>");
            sb.AppendLine("    body {");
            sb.AppendLine("      font-family: Arial, sans-serif;");
            sb.AppendLine("      background-color: #f9f9f9;");
            sb.AppendLine("      color: #333;");
            sb.AppendLine("      padding: 20px;");
            sb.AppendLine("    }");
            sb.AppendLine("    .email-container {");
            sb.AppendLine("      background-color: #ffffff;");
            sb.AppendLine("      padding: 20px;");
            sb.AppendLine("      border-radius: 6px;");
            sb.AppendLine("      box-shadow: 0 2px 4px rgba(0,0,0,0.1);");
            sb.AppendLine("      max-width: 600px;");
            sb.AppendLine("      margin: auto;");
            sb.AppendLine("    }");
            sb.AppendLine("    .content {");
            sb.AppendLine("      font-size: 16px;");
            sb.AppendLine("      line-height: 1.6;");
            sb.AppendLine("    }");
            sb.AppendLine("    .footer {");
            sb.AppendLine("      margin-top: 30px;");
            sb.AppendLine("      font-size: 14px;");
            sb.AppendLine("      color: #888;");
            sb.AppendLine("    }");
            sb.AppendLine("    ul {");
            sb.AppendLine("      padding-left: 20px;");
            sb.AppendLine("    }");
            sb.AppendLine("    li {");
            sb.AppendLine("      margin-bottom: 15px;");
            sb.AppendLine("    }");
            sb.AppendLine("  </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("  <div class=\"email-container\">");
            sb.AppendLine("    <div class=\"content\">");
            sb.AppendLine("      <p>Dear Team,</p>");
            sb.AppendLine($"      <p><br/>This is a gentle reminder that the Job Scope for the <strong> {projectName} </strong> under <strong> Job ID: {jobNameNo} </strong>.</p>");
            sb.AppendLine("      <p><br/>Please find the details of the Job Scope that are due for review:</p>");

            foreach (var group in groupedGuarantees)
            {
                group.Items.ForEach(item =>
                {
                    sb.AppendLine("      <ul>");
                    sb.AppendLine("        <li>");
                    sb.AppendLine($"          <strong>Amount:</strong> {item.Amount}<br/>");
                    sb.AppendLine($"          <strong>Date:</strong> {item.Date}<br/>");
                    //sb.AppendLine($"          <strong>Description:</strong> {group.Description}<br/>");
                    // sb.AppendLine($"          <strong>Due Date:</strong> {group.StatusDate:yyyy-MM-dd}"); // Uncomment if needed
                    sb.AppendLine("        </li>");
                    sb.AppendLine("      </ul>");
                });

            }

            sb.AppendLine("      <p>Please take the necessary actions to ensure timely progress.</p>");
            sb.AppendLine("      <p>Regards,<br/>Automated Notification Service</p>");
            sb.AppendLine("    </div>");
            sb.AppendLine("    <div class=\"footer\">");
            sb.AppendLine("      &copy; 2025 Water Bird Wtc. All rights reserved.");
            sb.AppendLine("    </div>");
            sb.AppendLine("  </div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        public async Task CheckAndSendEmailForJobScops()
        {
            try
            {
                var today = DateTime.Today;
                _logger.LogInformation("CheckAndSendEmailForJobScops: Started");
                var jobScopes = await GetGroupedDueJobScopesAsync(today);
                if (!jobScopes.Any())
                    return;

                var jobIds = jobScopes
                                .Select(g => g.JobId)
                                .Distinct()
                                .ToList();


                var jobNo = jobScopes.FirstOrDefault()?.JobNo ?? "";
                var jobName = jobScopes.FirstOrDefault()?.JobName ?? "";
                var projectName = jobScopes.FirstOrDefault()?.ProjectName ?? "";



                var emailMetaDataDtos = await GetToEMails(jobIds);
                if (!emailMetaDataDtos.Any())
                    return;

                var toEmails = emailMetaDataDtos.SelectMany(e => e.MetaData).Distinct().ToList();

                var subject = $"Soft Reminder – Job Scope for {projectName} (Job: {jobNo})";

                var messageBody = ComposeJobScopReminderEmailBody(jobScopes, projectName, jobNo);

                var emailStatus = await _emailSender.SendTextEmailAsync(toEmails, subject, messageBody, new List<string>());
                _logger.LogInformation("CheckAndSendEmailForJobScops: Email Sent");

                if (emailStatus)
                    await UpdateEmailSentFlag(jobIds, false);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("CheckAndSendEmailForJobScops: Started", ex);
            }
        }

        private async Task<List<JobGroupDataDto>> GetGroupedDueJobScopesAsync(DateTime today)
        {
            var rawData = await _jobdetailsRepository
                         .GetAsQueryable()
                         .Join(_approverLevelSettings.GetAsQueryable().AsNoTracking(),
                             jd => jd.JobMasterJobDetailsJobId,
                             setting => setting.JobId,
                             (jd, setting) => new { jd, setting })
                         .Join(_jobMasterRepo.GetAsQueryable().AsNoTracking(),
                             combined => combined.jd.JobMasterJobDetailsJobId,
                             jm => jm.JobMasterJobId,
                             (combined, jm) => new { combined.jd, combined.setting, JobMaster = jm })
                         .Where(x =>
                             x.jd.JobMasterJobDetailsStatusDate.HasValue &&
                             x.jd.JobMasterJobDetailsStatusDate.Value.Date.AddDays(-x.setting.NotificationDays) <= today &&
                             x.setting.FormType == EnumOrderForms.PurchaseOrder &&
                             x.jd.JobMasterJobDetailsStatus != "Cancel" &&
                             (x.jd.EmailSent == null || x.jd.EmailSent == false))
                         .ToListAsync();


            var groupedData = rawData
                            .GroupBy(x => new
                            {
                                x.jd.JobMasterJobDetailsJobId,
                                x.JobMaster.JobMasterJobNumber,
                                x.JobMaster.JobMasterJobName,
                                x.jd.JobMasterJobDetailsDescription,
                            })
                            .Select(group => new JobGroupDataDto
                            {
                                JobId = (int)group.Key.JobMasterJobDetailsJobId,
                                JobNo = group.Key.JobMasterJobNumber,
                                JobName = group.Key.JobMasterJobName,
                                ProjectName = group.Key.JobMasterJobDetailsDescription,
                                Items = group.GroupBy(g => new { g.jd.JobMasterJobDetailsJdAmt, g.jd.JobMasterJobDetailsStatusDate }).Select(item => new JobGroupItem
                                {
                                    Amount = item.Key.JobMasterJobDetailsJdAmt ?? 0,
                                    //Description = item.jd.JobMasterJobDetailsDescription ?? "",
                                    Date = item.Key.JobMasterJobDetailsStatusDate?.ToString("yyyy MMM dd") ?? "",
                                }).ToList()
                            })
                            .ToList();

            return groupedData;
        }

        public async Task<List<EmailMetaDataDto>> GetToEMails(List<int> jobId)
        {
            try
            {
                var result = await _approverLevelSettings.GetAsQueryable().AsNoTracking()
                              .Where(x => jobId.Contains((int)x.JobId) && x.OptEmail && x.FormType == EnumOrderForms.PurchaseOrder)
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
