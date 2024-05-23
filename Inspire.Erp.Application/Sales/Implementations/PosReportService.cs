using Inspire.Erp.Application.Sales.Interfaces;
using Inspire.Erp.Application.Account.Implementations;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Inspire.Erp.Application.MODULE;
using Inspire.Erp.Domain.Modals.Sales;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Infrastructure.Database;
using SendGrid.Helpers.Mail;
using Inspire.Erp.Domain.Entities.POS;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace Inspire.Erp.Application.Sales.Implementations
{
    public class PosReportService : IPosReportService
    {
        private readonly InspireErpDBContext _dbContext;

        public PosReportService(InspireErpDBContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<List<POS_SummaryDateWiseReport>> GetSalesTransactionCash(int wpId, int stationId)
        {
            try
            {
                var ab = await (from c in _dbContext.POS_Sales_Voucher
                                join d in _dbContext.POS_SalesTransactionDetails on c.VoucherNo equals d.InvoiceNo
                                where d.Amount != 0 && d.PaymentMode == "CAS" && c.WorkPeriodID == wpId
                                && c.StationID == stationId //&& c.VoucherDate == DateTime.Now
                                orderby d.Id
                                select new POS_SummaryDateWiseReport
                                {
                                    VoucherNo = c.VoucherNo,
                                    PaymentMode = c.Voucher_Type,
                                    Discount = c.Discount ?? 0m,
                                    Amount = d.Amount ?? 0m,
                                    GrossAmount = c.GrossAmount ?? 0m,
                                    VatAmount = c.VatAmount ?? 0m
                                }).ToListAsync();

                var ac = await (from c in _dbContext.POS_Sales_Voucher
                                join d in _dbContext.POS_SalesTransactionDetails on c.VoucherNo equals d.InvoiceNo
                                where d.PaymentMode == "CAS" && d.Amount == 0
                                && c.WorkPeriodID == wpId && c.StationID == stationId //&& c.VoucherDate == DateTime.Now
                                orderby d.Id
                                select new POS_SummaryDateWiseReport
                                {
                                    VoucherNo = c.VoucherNo,
                                    PaymentMode = c.Voucher_Type,
                                    Discount = c.Discount ?? 0m,
                                    Amount = d.Amount ?? 0m,
                                    GrossAmount = c.GrossAmount ?? 0m,
                                    VatAmount = c.VatAmount ?? 0m
                                }).ToListAsync();

                var result = ab.Union(ac).OrderBy(a => a.VoucherNo).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<POS_SummaryDateWiseReport>> GetSalesTransactionCard(int wpId, int stationId)
        {
            try
            {
                var ab = await (from c in _dbContext.POS_Sales_Voucher
                                join d in _dbContext.POS_SalesTransactionDetails on c.VoucherNo equals d.InvoiceNo
                                where d.Amount != 0 && d.PaymentMode == "CARD" && c.WorkPeriodID == wpId
                                && c.StationID == stationId //&& c.VoucherDate == DateTime.Now
                                select new POS_SummaryDateWiseReport
                                {
                                    VoucherNo = c.VoucherNo,
                                    PaymentMode = c.Voucher_Type,
                                    Discount = c.Discount ?? 0m,
                                    Amount = d.Amount ?? 0m,
                                    GrossAmount = c.GrossAmount ?? 0m,
                                    VatAmount = c.VatAmount ?? 0m
                                }).ToListAsync();

                var ac = await (from c in _dbContext.POS_Sales_Voucher
                                join d in _dbContext.POS_SalesTransactionDetails on c.VoucherNo equals d.InvoiceNo
                                where d.PaymentMode == "CARD" && d.Amount == 0 && c.WorkPeriodID == wpId
                                && c.StationID == stationId //&& c.VoucherDate == DateTime.Now
                                orderby d.Id
                                select new POS_SummaryDateWiseReport
                                {
                                    VoucherNo = c.VoucherNo,
                                    PaymentMode = c.Voucher_Type,
                                    Discount = c.Discount ?? 0m,
                                    Amount = d.Amount ?? 0m,
                                    GrossAmount = c.GrossAmount ?? 0m,
                                    VatAmount = c.VatAmount ?? 0m
                                }).ToListAsync();

                var result = ab.Union(ac).OrderBy(a => a.VoucherNo).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
