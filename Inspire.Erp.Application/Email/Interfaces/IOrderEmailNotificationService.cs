using Inspire.Erp.Application.Common.Email;
using Inspire.Erp.Domain.Enums;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Email.Interfaces
{
    public interface IOrderEmailNotificationService
    {
        Task SendOrderEmailNotificationAsync(EnumOrderForms orderType, EnumApproverLevels level, string subTitle, string poNumber, DateTime date, string supplier, decimal totalAmount, int orderId,int jobId, string jobNo, string jobName, bool IsNewEntry, string requestedBy, string settingValue, int approverId=-1);
        Task SendQuotationEmailNotificationAsync(EnumOrderForms orderType, EnumApproverLevels level, QuotationEmailModel emailModel, bool IsNewEntry = true);
        Task<bool> SendTextEmailAsync(List<UserEmail> toEmails, string subject, string message, List<string> filePaths);
    }

}
