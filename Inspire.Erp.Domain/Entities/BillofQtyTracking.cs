using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Entities
{
    public class BillofQtyTracking
    {
        public int BillMasterId { get; set; }
        public string BillofqtyNo { get; set; }
        public DateTime BOQDate { get; set; }
        public string JobMasterNo { get; set; }
        public string JobMasterName { get; set; }
        public decimal BillDetailId { get; set; }
        public decimal ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UnitShortName { get; set; }
        public decimal BOQQty { get; set; }
        public int PrQty { get; set; }
        public int BalPrQty { get; set; }
        public int POQty { get; set; }
        public int BalPOQty { get; set; }
    }

    public class BoqToCustomerQuotationDto
    {
        public int BillofQtyId { get; set; }
        public string BillofQtyNo { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public int Qty { get; set; }
        public decimal Rate { get; set; }
        public decimal CurrencyRate { get; set; }
        public decimal FreightCharges { get; set; }
        public decimal PackingCharges { get; set; }
        public decimal ClearanceCharges { get; set; }
        public decimal Duty { get; set; }
        public decimal Mofa { get; set; }
        public decimal Others { get; set; }
        public decimal FinalAmount { get; set; }

    }
}
