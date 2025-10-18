using System.ComponentModel.DataAnnotations;

namespace Inspire.Erp.Domain.Entities
{
    public class PurchaseCharges
    {
        [Key]
        public long ChargesId { get; set; }
        public string Voucherno { get; set; }
        public int ServiceID { get; set; }
        public decimal Amount { get; set; }
        public decimal TaxIncAmount { get; set; }
        public string Remarks { get; set; }
    }
}
