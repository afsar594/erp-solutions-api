using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspire.Erp.Domain.Entities
{
    public partial class POS_SalesTransactionDetails
    {
        public long Id { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public int? WorkPeriodId { get; set; }
        public string PaymentMode { get; set; }
        public decimal? Amount { get; set; }
        public string CounterName { get; set; }
        public string Status { get; set; }
        public int? UserId { get; set; }
        public string Description { get; set; }
        public string BarCode { get; set; }
        public string CreditAccountNo { get; set; }

    }
    public partial class POS_SummaryDateWiseReport
    {

        public string VoucherNo { get; set; }
        public string PaymentMode { get; set; }
        public decimal Discount { get; set; }
        public decimal Amount { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal VatAmount { get; set; }

    }
    public partial class POS_todaySalesByDate
    {

        public string VoucherNo { get; set; }
        public string CustomerName { get; set; }
        public string PaymentMode { get; set; }
        public decimal Discount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal VatAmount { get; set; }
        public  DateTime? VoucherDate { get; set; }

    }

}

