using Inspire.Erp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Settings.Interfaces
{
    public interface IGeneralSettings
    {
        public Task<string> GetValueByKey(string key);
        public Task<(string GeneralSettingValue, string ApproverStatus)> GetApproverStatusAsync(string jobId, string orderType);
    }
}
