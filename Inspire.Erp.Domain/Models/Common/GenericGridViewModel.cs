using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Modals.Common
{
    public class GenericGridViewModel
    {
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 10000000;
        public string Field { get; set; }
        public string Dir { get; set; }
        public string Search { get; set; }
        public string Filter { get; set; }
        public int Total { get; set; } = 0;
        public string Extension { get; set; }
        public string  FormatType { get; set; }
    }

    public class ReportFilter
    {
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public string Formate { get; set; }
        public int TotalCount { get; set; }
        public string Query { get; set; }
        public string Serach { get; set; }
        public string CostCenter { get; set; }
        public bool HideZeroTransactions { get; set; }
        public string PrintGroupWise { get; set; }
        public string FinancialMindate { get; set; }
        public int SupplierId { get; set; }
        public int  JobId { get; set; }
        public string Status { get; set; }
        public int  LocationId { get; set; }
        public int ItemId { get; set; }
        public string PONO { get; set; }

    }
    public class DeliveryNoteReportFilter
    {
        public int LocationId { get; set; }
        public long CustomerId { get; set; }
        public string InvioceStatus { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public string Formate { get; set; }

    }
}
