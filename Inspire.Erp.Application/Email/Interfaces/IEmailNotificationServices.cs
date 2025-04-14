using Inspire.Erp.Domain.Models.Common;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Email.Interfaces
{
    public interface IEmailNotificationServices
    {
        Task<bool> SendTemplatedEmailAsync(List<EmailAddress> toEmails, string templateId, List<object> dynamicTemplateData, List<string> filePaths);
        Task<bool> SendTextEmailAsync(List<EmailAddress> toEmails, string subject, string message, List<string> filePaths);
    }
}
