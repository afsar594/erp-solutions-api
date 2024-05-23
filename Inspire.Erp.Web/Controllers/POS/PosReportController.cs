using AutoMapper;
using Inspire.Erp.Application.Authentication.Inerfaces;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Application.Master;
using Inspire.Erp.Application.Sales.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Entities.POS;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Web.Common;
using Inspire.Erp.Web.ViewModels;
using Inspire.Erp.Web.ViewModels.Procurement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers.POS
{
    [Route("api/pos/item")]
    [Produces("application/json")]
    [ApiController]
    public class PosReportController : ControllerBase
    {
        private string eBigCharOff = "BigCharOff";
        private readonly IMapper _mapper;
        private ISalesVoucherService _salesVoucherService;
        private IPosReportService _posReportService;
        private string eCentre = "Centre";
        private string eBigCharOn3 = "BigCharOn3";
        private string eBigCharOn2 = "BigCharOn2";
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
        [Route("GetSummaryDateWise")]
        public async Task<IActionResult> GetSummaryDateWise(string PrintHeader1, string PrintHeader2, string PrintHeader3, string PrintHeader4)
        {

            DateTime dateTime = DateTime.Now;
            DateTimeFormatInfo dateformat = new DateTimeFormatInfo { MonthDayPattern = "MMMM" };
            string strdate = dateTime.ToString("MMMM,dd,yyyy HH:mm", dateformat);

            StringBuilder strRen = new StringBuilder();
            strRen.AppendLine(eBigCharOff + "---------------------");
            strRen.AppendLine(eCentre + eBigCharOn3 + " Report Name");
            strRen.AppendLine(eCentre + eBigCharOn3 + " " + strdate);
            strRen.AppendLine(eBigCharOff + "---------------------");

            if (!string.IsNullOrEmpty(PrintHeader1))
            {
                strRen.AppendLine(eCentre + eBigCharOn2 + PrintHeader1);
            }
            if (!string.IsNullOrEmpty(PrintHeader2))
            {
                strRen.AppendLine(eCentre + eBigCharOn3 + PrintHeader2);
            }
            if (!string.IsNullOrEmpty(PrintHeader3))
            {
                strRen.AppendLine(eCentre + eBigCharOff + PrintHeader3);
            }
            if (!string.IsNullOrEmpty(PrintHeader4))
            {
                strRen.AppendLine(eCentre + eBigCharOff + PrintHeader4);
            }

            strRen.AppendLine(eBigCharOff + "-------------------");
            strRen.AppendLine(eCentre + eBigCharOn2 + " Cash Payment");
            strRen.AppendLine(eBigCharOff + "-------------------");


            strRen.AppendLine(eBigCharOff + "--------------------");
            strRen.AppendLine(eCentre + " BillNo    Payment   Total");
            strRen.AppendLine(eBigCharOff + "--------------------");

            var CashsalesDetails = await this.GetSalesTransactionCash(1, 1);

            decimal CashAmount = 0;
            decimal CashDiscount = 0;
            decimal SGrossAmount = 0;
            decimal CashVatAmount = 0;
            if (CashsalesDetails == null)
            {
                return NotFound(" Cash Sales transaction  not found");
            }
            foreach (var item in CashsalesDetails)
            {
                strRen.AppendLine($"       {item.VoucherNo}      {item.PaymentMode}      {item.Amount}");

                CashAmount += item.Amount;
                CashDiscount += item.Discount;
                SGrossAmount += item.GrossAmount;
                CashVatAmount += item.VatAmount;
            }

            strRen.AppendLine(eBigCharOff + "---------------");
            strRen.AppendLine(eCentre + $" Total     {CashAmount}");
            strRen.AppendLine(eBigCharOff + "---------------");

            strRen.AppendLine(eBigCharOff + "----------------");
            strRen.AppendLine(eCentre + eBigCharOn2 + " Card Payment");
            strRen.AppendLine(eBigCharOff + "-----------------");

            strRen.AppendLine(eBigCharOff + "--------------------");
            strRen.AppendLine(eCentre + " BillNo    Payment   Total");
            strRen.AppendLine(eBigCharOff + "--------------------");

            var CardsalesDetails = await this.GetSalesTransactionCard(1, 1);
            decimal CardAmount = 0;
            decimal CardDiscount = 0;
            decimal DGrossAmount = 0;
            decimal CardVatAmount = 0;

            if (CardsalesDetails == null)
            {
                return NotFound("Card Sales transaction  not found");
            }
            foreach (var item in CardsalesDetails)
            {
                strRen.AppendLine($"       {item.VoucherNo}     {item.PaymentMode}    {item.Amount}");
                CardAmount += item.Amount;
                CardDiscount += item.Discount;
                DGrossAmount += item.VatAmount;
                CardVatAmount += item.VatAmount;
            }

            strRen.AppendLine(eBigCharOff + "---------------");
            strRen.AppendLine(eCentre + $" Total     {CardAmount}");
            strRen.AppendLine(eBigCharOff + "---------------");


            strRen.AppendLine("------------------");
            strRen.AppendLine($" GrossAmount  : {SGrossAmount + DGrossAmount}");
            strRen.AppendLine($" VATAmount    : {CashVatAmount + CardVatAmount}");
            strRen.AppendLine($" Discount     : {CashDiscount + CardDiscount}");
            strRen.AppendLine($" Total        : {CashAmount + CardAmount}");
            strRen.AppendLine("-------- END");



            string path = "C:\\Users\\user\\Desktop\\New Text Document.txt";
            await System.IO.File.WriteAllTextAsync(path, strRen.ToString());



            // RawPrinterHelper.SendStringToPrinter(PrinterNameRen, StrRen.ToString);
            var response = new
            {
                Success = true,
                CashsalesDetails = CashsalesDetails,
                CardsalesDetails = CardsalesDetails,
            };
            return Ok(response);
        }
        [HttpGet]
        [Route("GetSalesTransactionCash")]
        public async Task<List<POS_SummaryDateWiseReport>> GetSalesTransactionCash(int workPeriodId, int stationId)
        {
            return await _posReportService.GetSalesTransactionCash(workPeriodId, stationId);

        }


        [HttpGet]
        [Route("GetSalesTransactionCard")]
        public async Task<List<POS_SummaryDateWiseReport>> GetSalesTransactionCard(int workPeriodId, int stationId)
        {
            return await _posReportService.GetSalesTransactionCard(workPeriodId, stationId);

        }

    }
}