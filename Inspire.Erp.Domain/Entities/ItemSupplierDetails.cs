using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspire.Erp.Domain.Entities
{
    public partial class ItemSupplierDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ItemSupplierDetailsItemSupplierDetailsId { get; set; }
        public long? ItemSupplierDetailsItemId { get; set; }
        public int? ItemSupplierDetailsSupplierId { get; set; }
        public decimal? ItemSupplierDetailsRate { get; set; }
        public int? ItemSupplierDetailsBatchId { get; set; }
        public bool? ItemSupplierDetailsDelStatus { get; set; }

        public virtual ItemMaster? ItemSupplierDetailsItem { get; set; }
        public virtual SuppliersMaster? ItemSupplierDetailsSupplier { get; set; }    
    }
}
