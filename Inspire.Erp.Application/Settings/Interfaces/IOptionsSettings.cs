using Inspire.Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Settings.Interfaces
{
    public interface IOptionsSettings
    {
        IEnumerable<OptionsMaster> GetOptionsSettings(string search);
        IEnumerable<OptionsMaster> GetOptionsSettingsbyKey(string key);
        Boolean updateOptionsSettings(List<OptionsMaster> optionsSettings);
        Boolean ImportOptionsSettingsFromFile(List<OptionsMaster> optionsSettings);
        
    }
}
