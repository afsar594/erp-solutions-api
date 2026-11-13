using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Entities
{
    public class AssetConsumptionHeader
    {
        public string? VoucherNo { get; set; }
        public DateTime? VoucherDate { get; set; }
        public long? JobID { get; set; }
        public string? RefNo { get; set; }
        public string? Description { get; set; }
        public float? GrandTotal { get; set; }
    }

}
