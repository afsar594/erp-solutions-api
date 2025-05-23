using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.DTO.Job_Master
{
    public class JobMasterExpenseDto
    {
        public string accNo { get; set; }
        public string accName { get; set; }
        public DateTime? transDate { get; set; }
        public string VoucherNo { get; set; }
        public string VoucherType { get; set; }
        public decimal CrAmt { get; set; }
        public decimal DrAmt { get; set; }
        public string Description { get; set; }
        public DateTime? Chq_Date { get; set; }
        public string Chq_No { get; set; }
        public string CostName { get; set; }

        public decimal? RunningBalance { get; set; }

        public string? JobNo { get; set; }
        public string? JobValue { get; set; }
        public int? Material { get; set; }
        public string? Labour { get; set; }
        public string? OverHead { get; set; }
        public string? Miscellaneous { get; set; }
        public decimal? TotCost { get; set; }
        public decimal? Profit { get; set; }
        public decimal? InvoiceTot { get; set; }
    }
}
