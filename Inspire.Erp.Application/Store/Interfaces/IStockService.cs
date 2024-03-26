using System;
using System.Collections.Generic;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Application.Common;
using System.Text;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Modals.Sales;
using Inspire.Erp.Domain.Modals;
using System.Threading.Tasks;
using Inspire.Erp.Domain.Modals.Stock;

namespace Inspire.Erp.Application.Store.Interfaces
{
    public interface IStockService
    {

        Task<Response<List<StockReportBaseUnitResponse>>> GetStockReportBaseUnitList(StockFilterModel model);
        Task<Response<List<StockReportBaseUnitResponse>>> GetStockReportBaseUnit(StockFilterModel model);

        Task<Response<StockMovementReportResponse>> GetStockMovementReport(StockFilterModel model);
    

    }
}
