using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Modals
{
    public class WorkPeriodResponse
    {
        public int Id { get; set; }
        public string WorkPeriodName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? Enddate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; }
        public int? StationId { get; set; }
        public string StationName { get; set; }
        public int? UserId { get; set; }
        public int? UserEnd { get; set; }
        public decimal? OpeningCash { get; set; }
        public decimal? ClosingCash { get; set; }
        public string LoginSystemIP { get; set; }
        public string LoginComputerName { get; set; }
        public decimal? ClosingCashUser { get; set; }
        public int? UserClosebal { get; set; }
        public decimal? DifferenceAmount { get; set; }
        public decimal? OpeningCashUser { get; set; }
        public int? UserOpeningBal { get; set; }
    }
   
}
