using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Inspire.Erp.Domain.Entities
{
    public class BillOfQTyDetails
    {
        public decimal Id { get; set; }
        public decimal? BillQtyId { get; set; }
        public decimal? ItemCode { get; set; }
        public decimal? UnitCode { get; set; }
        public decimal? Qty { get; set; }
        public string Remarks { get; set; }
        public bool? IsEdit { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Freight_Charges { get; set; }
        public decimal? Packing_Charges { get; set; }
        public decimal? Clearance_Charges { get; set; }
        public decimal? Duty { get; set; }
        public decimal? Mofa { get; set; }
        public decimal? Others { get; set; }
        public int? Currency_TypeId { get; set; }
        public decimal? FinalAmount { get; set; }
        public decimal? CurrencyRate { get; set; }
        public int? EnquiryDetailId { get; set; }

    }
    public class EstimationCostCenterModel
    {
        public int Id { get; set; }
        public string Remarks { get; set; }
        public decimal? Amount { get; set; }
        public int BillQtyId { get; set; }
        public int costcenter { get; set; }
        public string CostCenterName { get; set; }
    }

    public class BillOfQTyDetailsModel
    {
        public decimal Id { get; set; }
        public decimal? BillQtyId { get; set; }
        public decimal? ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal? UnitCode { get; set; }
        public string UnitName { get; set; }
        public decimal? Qty { get; set; }

        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }
        public string Remarks { get; set; }
        public bool? IsEdit { get; set; }
        public decimal? Freight_Charges { get; set; }
        public decimal? Packing_Charges { get; set; }
        public decimal? Clearance_Charges { get; set; }
        public decimal? Duty { get; set; }
        public decimal? Mofa { get; set; }
        public decimal? Others { get; set; }
        public int? Currency_TypeId { get; set; }
        public decimal? FinalAmount { get; set; }
        public decimal? CurrencyRate { get; set; }

    }
}
