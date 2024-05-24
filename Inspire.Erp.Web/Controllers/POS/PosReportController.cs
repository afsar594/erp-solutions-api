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


        [HttpGet]
        [Route("GetSalesTransactionCash")]
        public async Task<List<POS_SummaryDateWiseReport>> GetSalesTransactionCash(int workPeriodId, int stationId, DateTime fromDate, DateTime toDate)
        {
            return await _posReportService.GetSalesTransactionCash(workPeriodId, stationId, fromDate, toDate);

        }


        [HttpGet]
        [Route("GetSalesTransactionCard")]
        public async Task<List<POS_SummaryDateWiseReport>> GetSalesTransactionCard(int workPeriodId, int stationId, DateTime fromDate, DateTime toDate)
        {
            return await _posReportService.GetSalesTransactionCard(workPeriodId, stationId, fromDate, toDate);

        }


        [HttpGet]
        [Route("GetTodaySalesByDate")]
        public async Task<List<POS_todaySalesByDate>> GetTodaySalesByDate(int workPeriodId, int stationId, DateTime fromDate, DateTime toDate)
        {
            return await _posReportService.GetTodaySalesByDate(workPeriodId, stationId, fromDate, toDate);

        }
    }
}