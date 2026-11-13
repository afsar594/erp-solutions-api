using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Account.Interfaces
{
    public interface IAccountsSettingsService
    {
        IEnumerable<AccountSettings> GetAccountsSettings();
        IEnumerable<AccountSettings> GetAccountsSettingsbyKey(string key);
        Boolean updateAccountsSetting(List<AccountSettings> accountsSettings);
        public ResponseInfo DEFAULTACC();
    }
}
