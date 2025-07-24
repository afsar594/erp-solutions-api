using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Domain.DTO
{
    public class BillofQtyMasterDto
    {
        public decimal Id { get; set; }
        public string BillofqtyNo { get; set; }
        public DateTime? Date { get; set; }
        public decimal? CustomerEnqId { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public decimal? BOQAmount { get; set; }
        public decimal? CostantsAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? ProftPercentage { get; set; }
        public decimal? ProfitAmount { get; set; }
        public decimal? QuoteAmount { get; set; }
    }
}
