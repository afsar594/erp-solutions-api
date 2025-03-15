using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Inspire.Erp.Domain.Entities
{
    public class BillofQtyMaster
    {
        public BillofQtyMaster()
        {
            BillOfQTyDetails = new HashSet<BillOfQTyDetails>();
            EstimationCostCenter = new HashSet<EstimationCostCenter>();
        }

        public decimal Id { get; set; }
        public string BillofqtyNo { get; set; }
        public DateTime? Date { get; set; }
        public decimal? CustomerEnqId { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public decimal? BOQAmount { get; set; }
        public decimal? CostantsAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? ProftPercentage { get; set; }
        public decimal? ProfitAmount { get; set; }
        public decimal? QuoteAmount { get; set; }
        [NotMapped]
        public virtual ICollection<BillOfQTyDetails> BillOfQTyDetails { get; set; }
        public virtual ICollection<EstimationCostCenter> EstimationCostCenter { get; set; }
    }

    public class BillofQtyMasterModel
    {

        public decimal Id { get; set; }
        public string BillofqtyNo { get; set; }
        public DateTime? Date { get; set; }
        public decimal? CustomerEnqId { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public virtual List<BillOfQTyDetailsModel> BillOfQTyDetails { get; set; }
        public virtual List<EstimationCostCenterModel> EstimationCostCenter { get; set; }

    }
    public class BoqDetailDto
    {
        public decimal Id { get; set; }
        public decimal dId { get; set; }
        public string BillofqtyNo { get; set; }
        public DateTime? Date { get; set; }
        public decimal? CustomerEnqId { get; set; }
        public decimal? ItemCode { get; set; }
        public decimal? Qty { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }
        public decimal? UnitCode { get; set; }
    }

}
