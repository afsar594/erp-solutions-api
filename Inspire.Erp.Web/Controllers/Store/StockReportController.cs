////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Threading.Tasks;
////using Microsoft.AspNetCore.Mvc;

////namespace Inspire.Erp.Web.Controllers.Store
////{
////    public class OpeningStockController : Controller
////    {
       
////    }
////}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Inspire.Erp.Application.Store.Interfaces;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Web.Common;
using Inspire.Erp.Web.ViewModels.Store;
using Inspire.Erp.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Inspire.Erp.Domain.Modals.Sales;
using Inspire.Erp.Domain.Modals.Common;

using Inspire.Erp.Domain.Modals.Stock;
using NPOI.OpenXmlFormats.Dml;

namespace Inspire.Erp.Web.Controllers.Store
{
    [Route("api/StockReport")]
    [Produces("application/json")]
    [ApiController]
    public class StockReportController : ControllerBase
    {
        private IStockService _openingStockService;
        private readonly IMapper _mapper;
        public StockReportController(IStockService openingStockService, IMapper mapper)
        {
            _openingStockService = openingStockService;
            _mapper = mapper;
        }


      

        [HttpPost]
        [Route("GetStockReportBaseUnitList")]
        public async Task<IActionResult> GetStockReportBaseUnitList(StockFilterModel model)
        {

            var results = await _openingStockService.GetStockReportBaseUnitList();
            return Ok(results);
            //ApiResponse<List<StockReportBaseUnitResponse>> apiResponse = new ApiResponse<List<StockReportBaseUnitResponse>>
            //{
            //    Valid = true,
            //    Result = _mapper.Map<List<StockReportBaseUnitResponse>>(_openingStockService.GetStockReportBaseUnitList()),
            //    Message = ""
            //};
            //return apiResponse;
        }

       
    }
}