using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Modals.Dashboard
{
  public  class DashboardFilterModel
    {
        public string Type { get; set; }
        public string PeriodType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

    public class DashboardSalesPurchaseModel
    {
        public string Choice { get; set; }
        public string PeriodType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
