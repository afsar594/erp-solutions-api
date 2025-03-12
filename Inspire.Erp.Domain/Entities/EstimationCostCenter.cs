using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Entities
{
    public class EstimationCostCenter
    {
            public int Id { get; set; }
            public string Remarks { get; set; }
            public decimal? Amount { get; set; }
            public int BillQtyId { get; set;}           
           public int costcenter { get; set; }
    }
}
