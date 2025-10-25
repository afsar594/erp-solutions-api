using System.ComponentModel.DataAnnotations;

namespace Inspire.Erp.Domain.Entities
{
    public class OtherChargesMaster
    {
        [Key]
        public int ServiceID { get; set; }
        public string ServiceName { get; set; }
        public string AccountNo { get; set; }
        public decimal VatPer { get; set; }
        public string ServiceType { get; set; }
    }
}
