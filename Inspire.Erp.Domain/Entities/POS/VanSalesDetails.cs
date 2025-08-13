using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization; // <-- Add this

namespace Inspire.Erp.Domain.Entities
{
    [Table("van_sales_details")]
    public class VanSalesDetails
    {
        [Key]
        [Column("DetailId")]
        public int DetailId { get; set; }

        [Column("SalesId")]
        public int SalesId { get; set; }

        [Column("SI_No")]
        public int SI_No { get; set; }

        [Column("Item_Name")]
        [MaxLength(200)]
        public string Item_Name { get; set; }

        [Column("Qty")]
        public decimal Qty { get; set; } 

        [Column("Rate")]
        public decimal Rate { get; set; }

        [Column("Amount")]
        public decimal Amount { get; set; }

        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; }

        // Navigation Property: Many details belong to one sale
        [ForeignKey(nameof(SalesId))]
        [JsonIgnore] // 👈 Prevent serialization loop
        public VanSales Sale { get; set; }
    }
}
