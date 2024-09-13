using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Entities
{
    public class AssetSubListHeader
    {
        public string VoucherNo { get; set; }      // VARCHAR(30) NULL
        public DateTime? VoucherDate { get; set; } // DATETIME NULL
        public string RefNo { get; set; }          // VARCHAR(100) NULL
        public string Description { get; set; }    // VARCHAR(2000) NULL
        public double? GrandTotal { get; set; }    // FLOAT NULL
    }

}
