using Inspire.Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Master.Interfaces
{
    public interface IErpSettings
    {
        IEnumerable<ProgramSettings> GetProgramSettings();
        IEnumerable<GeneralSettings> GetGeneralSettings();
        IEnumerable<ProgramSettings> GetProgramSettingsbyKey(string key);
        List<ProgramSettings> GetGeneralSettingsByMultiKeys(string keys);
        IEnumerable<GeneralSettings> GetGeneralSettingsbyKey(string key);
        Boolean updateProgramSettings(List<ProgramSettings> programSettings);
        Boolean updateGeneralSettings(List<GeneralSettings> generalSettings);
    }
}
