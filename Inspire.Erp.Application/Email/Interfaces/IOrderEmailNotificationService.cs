using Inspire.Erp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Email.Interfaces
{
    public interface IOrderEmailNotificationService
    {
        public Task SendOrderEmailNotificationAsync(EnumOrderForms orderType,EnumApproverLevels level,string subTitle,string poNumber,DateTime date,string supplier,decimal totalAmount,int orderId,bool IsNewEntry = true);
    }
}
