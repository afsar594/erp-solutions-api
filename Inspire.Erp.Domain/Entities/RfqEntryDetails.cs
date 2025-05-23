using System;
using System.Collections.Generic;

namespace Inspire.Erp.Domain.Entities
{
    public partial class RfqEntryDetails
    {
        public int RFQEntryDetailsDetID { get; set; }
        public int? RFQEntryDetailsItemID { get; set; }
        public string RFQEntryDetailsDescription { get; set; }
        public int? RFQEntryDetailsUnitID { get; set; }
        public double? RFQEntryDetailsQuantity { get; set; }
        public double? RFQEntryDetailsPrice { get; set; }
        public double? RFQEntryDetailsDelqty { get; set; }
        public double? RFQEntryDetailsRFQAmt { get; set; }
        public string? RFQEntryDetailsRemarks { get; set; }
        public bool? RFQEntryDetailsDelStatus { get; set; }
        public string RFQ_EntryvouchaseNo { get; set; }

        public int RFQ_Entry_ID { get; set; }

    }
}
