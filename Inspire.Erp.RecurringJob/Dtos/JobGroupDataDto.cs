using Spire.Pdf.Exporting.XPS.Schema;
using System.Collections.Generic;

namespace Inspire.Erp.RecurringJob.Dtos
{
    public class JobGroupDataDto
    {
        public JobGroupDataDto()
        {
            Items = new List<JobGroupItem>();
        }
        public int JobId { get; set; }
        public string JobNo { get; set; }
        public string JobName { get; set; }
        public string ProjectName { get; set; }

        public List<JobGroupItem> Items { get; set; }
    }
    public class JobGroupItem
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
    }
}
