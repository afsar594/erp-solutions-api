using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Domain.DTO
{
    public class SalesVoucherDto
    {
        public int Id { get; set; }
        public string VoucherNo { get; set; }
        public DateTime VoucherDate { get; set; }
        public string CustomerName { get; set; }

        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal Quantity { get; set; }

        public int UnitId { get; set; }
        public string Unit { get; set; }

        public string LocationName { get; set; }
    }


}
