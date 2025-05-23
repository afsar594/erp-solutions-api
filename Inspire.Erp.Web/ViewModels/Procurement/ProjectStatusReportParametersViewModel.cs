using System;
using System.Collections.Generic;
using Inspire.Erp.Web.ViewModels.Store;
using System.Linq;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.ViewModels.Procurement
{
    public class ProjectStatusReportParametersViewModel
    {

        public long? A { get; set; }
        public long? CustomerMasterCustomerId { get; set; }
        public long? JobMasterJobId { get; set; }
        public string JobMasterJobNo { get; set; }
        public string CustomerMasterCustomerName { get; set; }
        public string JobMasterJobName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? JobMasterJobValue { get; set; }
        public decimal? JobMasterJobVariationAmount { get; set; }
        public decimal? RevisedValue { get; set; }
        public decimal? CumValue { get; set; }
        public decimal? DirectCost { get; set; }
        public decimal? GrossProfit { get; set; }
        public decimal? RetAmount { get; set; }
        public decimal? AdvInvoice { get; set; }
        public decimal? AdvRecovery { get; set; }
        public decimal? Receivable { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? Received { get; set; }
        public decimal? Balance { get; set; }

    }
}
