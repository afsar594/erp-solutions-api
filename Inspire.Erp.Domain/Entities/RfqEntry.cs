using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspire.Erp.Domain.Entities
{
    public partial class RfqEntry
    {
        public int RFQEntryID { get; set; }
        public string? RfqEntryRfqId { get; set; }
        public DateTime? RfqEntryRfqDate { get; set; }
        public int? RfqEntryRfqEnqId { get; set; }
        public int? RfqEntrySupplierId { get; set; }
        public string RfqEntrySupplierAddress { get; set; }
        public bool? RfqEntryStatus { get; set; }
        public int? RfqEntrySalesManId { get; set; }
        public bool? RfqEntryDeliveryStatus { get; set; }
        public bool? RfqEntryDelStatus { get; set; }
        public string RFQ_EntryvouchaseNo { get; set; }
        public string? Remarks { get; set; }
        [NotMapped]
        public List<RfqEntryDetails> RfqEntryDetails { get; set; }
    }
}
