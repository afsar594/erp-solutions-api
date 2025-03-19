using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inspire.Erp.WebReport.Models
{
    public class ReportFilterModel
    {
        public string ReportName { get; set; }
        public string SelectionFormula { get; set; }
        public List<ReportParameters> Parameters { get; set; }
        public List<string> Queries { get; set; }
        public List<SPQueries> SP_Queries { get; set; }
    }

    public class ReportParameters
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class SPQueries
    {
        public string SP_Name { get; set; }
        public List<ReportParameters> Parameters { get; set; }
    }
}