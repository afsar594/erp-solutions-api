using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Models.Procurement
{
   public class PurchaseRequisitionStatus
    {
        public string PurchaseRequisitonNo { get; set; }
        public DateTime PurchaseRequisitonDate { get; set; }
        public string PartNo { get; set; }
        public string MaterialName { get; set; }
        public string MaterailUnit { get; set; }
        public decimal? PurchaseRequisitionQty{ get; set; }
        public decimal? StockQty { get; set; }
        public decimal? PurchaseOrderQty { get; set; }
        public decimal BalanceToOrderQty { get; set; }
        public decimal IssueQty { get; set; }
        public decimal BalanceToIssueQty { get; set; }
    }
}
