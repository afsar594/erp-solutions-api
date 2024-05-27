using AutoMapper;
using Inspire.Erp.Application.Authentication.Inerfaces;
using Inspire.Erp.Application.Sales.Interfaces;
using Inspire.Erp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers.POS
{
    [Route("api/pos/item")]
    [Produces("application/json")]
    [ApiController]
    public class PosReportController : ControllerBase
    {
        private readonly IMapper _mapper;
        private ISalesVoucherService _salesVoucherService;
        private IPosReportService _posReportService;
        private readonly IloginService _loginsvc;
        public PosReportController(IMapper mapper,
            ISalesVoucherService salesVoucherService, IloginService loginsvc, IPosReportService posReportService)
        {

            _mapper = mapper;
            _salesVoucherService = salesVoucherService;
            _loginsvc = loginsvc;
            _posReportService = posReportService;
        }

        [HttpPost]
        //[HttpGet]
        [Route("GetSalesTransactionCash")]
        public async Task<List<POS_SummaryDateWiseReport>> GetSalesTransactionCash(PosReportFilterModel model)
        {
            return await _posReportService.GetSalesTransactionCash(model.workPeriodId, model.stationId, model.fromDate, model.toDate);

        }

        [HttpPost]
        //[HttpGet]
        [Route("GetSalesTransactionCard")]
        public async Task<List<POS_SummaryDateWiseReport>> GetSalesTransactionCard(PosReportFilterModel model)
        {
            return await _posReportService.GetSalesTransactionCard(model.workPeriodId, model.stationId, model.fromDate, model.toDate);

        }

        [HttpPost]
        //[HttpGet]
        [Route("GetTodaySalesByDate")]
        public async Task<POS_MaintodaySales> GetTodaySalesByDate(PosReportFilterModel model)
        {
            return await _posReportService.GetTodaySalesByDate(model.workPeriodId, model.stationId, model.fromDate, model.toDate);

        }
        [HttpPost]
        //[HttpGet]
        [Route("GetTodaySalesByCounterWise")]
        public async Task<List<TodaySalesByCounterWise>> GetTodaySalesByCounterWise(PosReportFilterModel model)
        {
            return await _posReportService.GetTodaySalesByCounterWise(model.fromDate, model.toDate);

        }

    }
}