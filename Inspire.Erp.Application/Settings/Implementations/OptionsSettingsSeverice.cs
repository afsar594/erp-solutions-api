using Inspire.Erp.Application.Settings.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Inspire.Erp.Application.Settings.Implementations
{
    public class OptionsSettingsSeverice : IOptionsSettings
    {
        private IRepository<OptionsMaster> _optionsSettingsRepo;
        public OptionsSettingsSeverice(IRepository<OptionsMaster> optionsSettingsRepo)
        {
            _optionsSettingsRepo = optionsSettingsRepo;
        }
        public IEnumerable<OptionsMaster> GetOptionsSettings(string search)
        {
            IEnumerable<OptionsMaster> optionsSettings = _optionsSettingsRepo.GetAsQueryable().Where(x => x.OptionsMasterFullDescription.Contains(search));
            return optionsSettings;
        }

        public IEnumerable<OptionsMaster> GetOptionsSettingsbyKey(string key)
        {

            IEnumerable<OptionsMaster> optionsSettings;
            try
            {
                optionsSettings = _optionsSettingsRepo.GetAll().Where(k => k.OptionsMasterDescription.Trim() == key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
            return optionsSettings;
        }

        public Boolean updateOptionsSettings(List<OptionsMaster> optionsSettings)
        {
            try
            {
                _optionsSettingsRepo.UpdateList(optionsSettings);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
