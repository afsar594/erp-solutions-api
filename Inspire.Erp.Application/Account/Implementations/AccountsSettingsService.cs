using Inspire.Erp.Application.Account.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Inspire.Erp.Domain.Modals.Common;

namespace Inspire.Erp.Application.Account.Implementations
{
    public class AccountsSettingsService : IAccountsSettingsService
    {
        private IRepository<AccountSettings> _accountsSettingsRepo;
        public AccountsSettingsService(IRepository<AccountSettings> accountsSettingsRepo)
        {
            _accountsSettingsRepo = accountsSettingsRepo;

        }
        public IEnumerable<AccountSettings> GetAccountsSettings()
        {

            IEnumerable<AccountSettings> accountsSettings = _accountsSettingsRepo.GetAll();
            return accountsSettings;
        }

        public IEnumerable<AccountSettings> GetAccountsSettingsbyKey(string key)
        {

            IEnumerable<AccountSettings> accountsSettings;
            try
            {
                accountsSettings = _accountsSettingsRepo.GetAll().Where(k => k.AccountSettingsAccountKeyValue.Trim() == key);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
            return accountsSettings;
        }
        public Boolean updateAccountsSetting(List<AccountSettings> accountsSettings)
        {
            try
            {
                _accountsSettingsRepo.UpdateList(accountsSettings);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public ResponseInfo DEFAULTACC()
        {
            var obj = new ResponseInfo();
            try
            {
                var data = _accountsSettingsRepo.GetAsQueryable().Where(a => a.AccountSettingsAccountKeyValue.Trim().ToUpper() == "COSTOF_FINISHEDGOODS_AC_NO" && a.AccountSettingsAccountKeyValue.Trim().ToUpper() == "STOCK_AC_NO").ToList();
                obj.ResultSet = data;
                obj.IsSuccess = true;
            }
            catch (Exception ex)
            {
                obj.IsSuccess = false;
            }


            return obj;

        }

    }
}
