using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Helpers.ExtensionMethods
{
    public static class DapperExtensions
    {
        public static object GetValueOrDefault(this IDictionary<string, object> dict, string key)
        {
            return dict.ContainsKey(key) ? dict[key] ?? 0 : 0;
        }
    }

}
