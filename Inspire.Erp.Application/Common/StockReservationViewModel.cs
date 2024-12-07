using Inspire.Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Common
{
    public partial class StockReservationViewModel  
    {
      
            public int StockReservationId { get; set; }
            public DateTime? StockReservationRDate { get; set; }
            public int? StockReservationJobId { get; set; }
            public string StockReservationRemarks { get; set; }
            public bool? StockReservationRelease { get; set; }
            public bool? StockReservationDelStatus { get; set; }

           public List<StockReservationDetails> StockReservationDetails { get; set; }
    }
}
