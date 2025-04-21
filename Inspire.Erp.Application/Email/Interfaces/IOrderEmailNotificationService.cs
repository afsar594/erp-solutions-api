using Inspire.Erp.Application.Common.Email;
using Inspire.Erp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Email.Interfaces
{
    public interface IOrderEmailNotificationService
    {
        Task SendOrderEmailNotificationAsync(EnumOrderForms orderType, EnumApproverLevels level, string subTitle, string poNumber, DateTime date, string supplier, decimal totalAmount, int orderId, bool IsNewEntry = true);
        Task SendQuotationEmailNotificationAsync(EnumOrderForms orderType, EnumApproverLevels level,QuotationEmailModel emailModel, bool IsNewEntry = true);
    }

}
