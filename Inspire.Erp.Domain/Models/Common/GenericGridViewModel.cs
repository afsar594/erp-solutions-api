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

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
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

    public class StockAdjustmentReportFilter
    {
        public int? JobId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class CustomerEnquirySearchFilter
    {
        public string Enquiry_No { get; set; }
        public long CustomerId { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
    }
    public class PurChaseRequisitionStatusFilterReport
    {
        public int JobId { get; set; }
        public int ItemId { get; set; }
        public string POStatus { get; set; }
        public string PRNO { get; set; }
        public bool ByDate { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }       
    }

    public class AccountTransactionsFilterReport
    {
       public List<string> accountNos { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
    public class PurChaseRequisitionStatus
    {
        public int issueQty { get; set; }
        public int BalToIssue { get; set; }
        public int stock { get; set; }
        public string ItemName { get; set; }
        public string Unit { get; set; }
        public string PartNo { get; set; }
        public int DelQty { get; set; }
        public string PoStatus { get; set; }
        public DateTime RequisitionDate { get; set; }
    }
    public class OutStandingReprortFilter
    {
        public string accountNo { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public bool isDateRange { get; set; }
    }

    public class JobCostReportSearchFilter
    {
        public long JobNo { get; set; }
        public long CostCenterId { get; set; }
        public bool IsdateSelected { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
    }

    public class ItemBarCodeSearchFilter
    {
        public bool DisableVat { get; set; }
        public string BarCode { get; set; }
    }
}
