using AutoMapper;
using Inspire.Erp.Application.Store.Interface;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers.Accounts
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class BalanceSheetController : ControllerBase
    {
        private Inspire.Erp.Application.Store.Interface.IBalanceSheet bs;
        private readonly IMapper _mapper;
        public BalanceSheetController(IBalanceSheet _bs, IMapper mapper)
        {
            bs = _bs;
            _mapper = mapper;
        }

        [HttpGet("BalanceSheetResponse")]
        public async Task<dynamic> BalanceSheetResponse()
        {
            try
            {
                BalanceSheetRequest obj = new BalanceSheetRequest();
                obj.StartDate = DateTime.Now.AddMonths(-10);
                obj.EndDate = DateTime.Now;
                return bs.BalanceSheetResponse(obj);
                //.Select(k => new StatementOfAccountsSummaryViewModel
                //return sts.StatementOfAccountSummaryResponse(obj).Select(k => new StatementOfAccountsSummaryViewModel
                //{
                //    Dr = k.Dr,
                //    Cr = k.Cr

                //}).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("BalanceSheetResponse1")]
        public List<BalanceSheetResponse> BalanceSheetResponse1()
        {
            BalanceSheetRequest obj = new BalanceSheetRequest();
            obj.StartDate =DateTime.Now.AddMonths(-10);
            obj.EndDate = DateTime.Now;
            try
            {
                var response = bs.BalanceSheetResponse(obj);
                return _mapper.Map<List<BalanceSheetResponse>>(response);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}