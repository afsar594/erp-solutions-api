using Inspire.Erp.Application.Store.Interfaces;
using Inspire.Erp.Domain.Modals.Stock;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers.Store
{
    [Route("api/StockLedger")]
    [Produces("application/json")]
    [ApiController]
    public class StockLedgerController : ControllerBase
    {
        private IStockLedger stockLedger;

        public StockLedgerController(IStockLedger stockLedger)
        {
            this.stockLedger = stockLedger;
        }

        [HttpPost]
        [Route("GetStockLedger")] 
        public async Task<IActionResult> GetStockLedger(StockFilterModel stockFilterModel)
        {
            return Ok(await stockLedger.GetStockLedgerReport(stockFilterModel));
        }


    }
}
