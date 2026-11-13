using Inspire.Erp.Application.Common.Email;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Models;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Email.Interfaces
{
    public interface IOrderEmailNotificationService
    {
        Task SendOrderEmailNotificationAsync(EnumOrderForms orderType, EnumApproverLevels level, string subTitle, string poNumber, DateTime date, string supplier, string totalAmount, int orderId, int jobId, string jobNo, string jobName, bool IsNewEntry, string requestedBy, string settingValue, List<string> attachments, int approverId = -1, FileResultDto fileResult = null, long originatorId = default);
        Task SendQuotationEmailNotificationAsync(EnumOrderForms orderType, EnumApproverLevels level, QuotationEmailModel emailModel, bool IsNewEntry = true);
        Task<bool> SendTextEmailAsync(List<UserEmail> toEmails, string subject, string message, List<string> filePaths);

        Task<ApiResponse<bool>> ResendEmailAsync(EnumOrderForms orderType, EnumApproverLevels level, long orderId,int approverId, CancellationToken cancellationToken = default);
    }

}
