using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Models.Procurement.PurchaseOrderTracking
{
   public class POTrackingDetails : POTrackingCommon
    {
        public int? PODetailsId { get; set; }
        public string ItemId { get; set; }
        public string PartNO { get; set; }
        public string ItemName { get; set; }
        public decimal? POQuantity { get; set; }
        public decimal? DeliveredQty { get; set; }
        public decimal? BalanceQty{ get; set; }
        public decimal? IssuedQty{ get; set; }
        public decimal? ItemRate { get; set; }
        public decimal? CurrencyRate { get; set; }

    }
}
