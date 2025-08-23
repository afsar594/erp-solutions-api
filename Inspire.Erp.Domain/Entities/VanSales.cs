using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspire.Erp.Domain.Entities
{
    [Table("van_sales")]
    public  class VanSales
    {
        [Key]
        [Column("SalesId")]
        public int SalesId { get; set; }

        [Column("Mode")]
        [MaxLength(20)]
        public string Mode { get; set; }

        [Column("Product")]
        [MaxLength(100)]
        public string Product { get; set; }

        [Column("Barcode")]
        [MaxLength(50)]
        public string? Barcode { get; set; }

        [Column("MRP")]
        public decimal? MRP { get; set; }

        [Column("CutOffRate")]
        public decimal? CutOffRate { get; set; }

        [Column("DiscountPercent")]
        public decimal? DiscountPercent { get; set; }

        [Column("UnitExclV")]
        public decimal? UnitExclV { get; set; }

        [Column("FOC")]
        public decimal? FOC { get; set; }

        [Column("DiscountAmount")]
        public decimal? DiscountAmount { get; set; }

        [Column("UnitInclV")]
        public decimal? UnitInclV { get; set; }

        [Column("Quantity")]
        public decimal? Quantity { get; set; }

        [Column("VatPercent")]
        public decimal? VatPercent { get; set; }

        [Column("Total")]
        public decimal? Total { get; set; }

        [Column("SummaryDiscountPercent")]
        public decimal? SummaryDiscountPercent { get; set; }

        [Column("VatExcl")]
        public decimal? VatExcl { get; set; }

        [Column("TenderedAmount")]
        public decimal? TenderedAmount { get; set; }

        [Column("SummaryDiscountAmount")]
        public decimal? SummaryDiscountAmount { get; set; }

        [Column("Vat")]
        public decimal? Vat { get; set; }

        [Column("CardReceived")]
        public decimal? CardReceived { get; set; }

        [Column("Balance")]
        public decimal? Balance { get; set; }

        [Column("SummaryTotal")]
        public decimal? SummaryTotal { get; set; }

        [Column("AddedToCredit")]
        public decimal? AddedToCredit { get; set; }

        [Column("Remark")]
        public string Remark { get; set; }

        [Column("PrintInvoice")]
        public bool? PrintInvoice { get; set; }

        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; }
        [Column("CustomerId")]
        public int? CustomerId { get; set; }
        [Column("CustomerName")]
        public string? CustomerName { get; set; }

        [Column("LocationId")]

        public int? LocationId { get; set; }

        [Column("CurrencyId")]

        public int? CurrencyId { get; set; }

        public ICollection<VanSalesDetails> Details { get; set; } = new List<VanSalesDetails>();

    }
}
