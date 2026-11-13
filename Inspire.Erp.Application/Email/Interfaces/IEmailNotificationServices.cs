using Inspire.Erp.Application.Common.Email;
using Inspire.Erp.Domain.Entities;
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
        Task<bool> SendTemplatedEmailAsync(List<UserEmail> toEmails, string templateId, List<object> dynamicTemplateData, List<string> filePaths, FileResultDto fileResult = null);
        Task<bool> SendTextEmailAsync(List<UserEmail> toEmails, string subject, string message, List<string> filePaths);
    }
}
