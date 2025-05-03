using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.RecurringJob
{
    public interface IEmailSchedulerService
    {
        Task CheckAndSendEmailForJobScops();
        Task CheckAndSendEmailForJobBankGuarantees();
    }
}
