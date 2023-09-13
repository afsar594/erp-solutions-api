////using System;
////using System.Collections.Generic;
////using System.Text;

////namespace Inspire.Erp.Application.Administration.Interfaces
////{
////    class IStationMasterServices
////    {
////    }
////}

using Inspire.Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Administration.Interfaces
{
    public interface IStationMasterServices
    {
        ////public IEnumerable<CurrencyMaster> InsertCurrency(CurrencyMaster currencyMaster);
        ////public IEnumerable<CurrencyMaster> UpdateCurrency(CurrencyMaster currencyMaster);
        ////public IEnumerable<CurrencyMaster> DeleteCurrency(CurrencyMaster currencyMaster);
        public IEnumerable<StationMaster> GetAllStation();
        ////public IEnumerable<CurrencyMaster> GetAllCurrencyById(int id);

    }
}

