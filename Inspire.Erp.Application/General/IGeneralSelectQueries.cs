using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.General
{
    public interface IGeneralSelectQueries
    {
        public Task<bool> GetBooleanValue(string id);
    }
}
