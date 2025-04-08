using Inspire.Erp.Domain.Models.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Email.Interfaces
{
    public interface IEmailNotificationServices
    {
        Task<bool> SendTemplatedEmailAsync(string email, string templateId, object dynamicTemplateData, List<string> filePaths);
        Task<bool> SendTemplatedEmailAsync(List<string> toEmails, string templateId, object dynamicTemplateData, List<string> filePaths);
        Task<bool> SendTextEmailAsync(List<string> toEmails, string subject, string message, List<string> filePaths);
    }
}
