using Inspire.Erp.Application.Master.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Inspire.Erp.Application.Master.Implementations
{
    public class SettingsService : IErpSettings
    {
        private IRepository<ProgramSettings> _programSettingsRepo;
        private IRepository<GeneralSettings> _generalSettingsRepo;
        public SettingsService(IRepository<ProgramSettings> programSettingsRepo, IRepository<GeneralSettings> generalSettingsRepo)
        {
            _programSettingsRepo = programSettingsRepo;
            _generalSettingsRepo = generalSettingsRepo;
        }
        public IEnumerable<ProgramSettings> GetProgramSettings()
        {

            IEnumerable<ProgramSettings> programSettings = _programSettingsRepo.GetAll();
            return programSettings;
        }

        public IEnumerable<GeneralSettings> GetGeneralSettings()
        {
            IEnumerable<GeneralSettings> generalSettings = _generalSettingsRepo.GetAll();
            return generalSettings;
        }

        public IEnumerable<ProgramSettings> GetProgramSettingsbyKey(string key)
        {

            IEnumerable<ProgramSettings> programSettings;
            try
            {
                programSettings = _programSettingsRepo.GetAll().Where(k => k.ProgramSettingsKeyValue.Trim() == key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
            return programSettings;
        }

        public IEnumerable<GeneralSettings> GetGeneralSettingsbyKey(string key)
        {

            IEnumerable<GeneralSettings> generalSettings;
            try
            {
                generalSettings = _generalSettingsRepo.GetAll().Where(k => k.GeneralSettingsKeyValue.Trim() == key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
            return generalSettings;
        }

        public Boolean updateProgramSettings(List<ProgramSettings> programSettings)
        {
            try
            {
                _programSettingsRepo.UpdateList(programSettings);
            } catch(Exception ex)
            {
                return false;
            }
            return true;
        }

        public Boolean updateGeneralSettings(List<GeneralSettings> generalSettings)
        {
            try
            {
                _generalSettingsRepo.UpdateList(generalSettings);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }




        public List<ProgramSettings> GetGeneralSettingsByMultiKeys(string keys)
        {
            if (string.IsNullOrWhiteSpace(keys))
                return new List<ProgramSettings>();

            var keyList = keys.Split(',', StringSplitOptions.RemoveEmptyEntries)
                              .Select(k => k.Trim())
                              .ToList();

            // Filter directly at DB level using IQueryable
            var result = _programSettingsRepo.GetAll()
                            .Where(g => keyList.Contains(g.ProgramSettingsKeyValue.Trim()))
                            .ToList();

            return result;
        }
    }
}
