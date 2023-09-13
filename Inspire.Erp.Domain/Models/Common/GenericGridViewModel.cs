using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Modals.Common
{
   public class GenericGridViewModel
    {
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 10000000;
        public string Field { get; set; }
        public string Dir { get; set; }
        public string Search { get; set; }
        public string Filter { get; set; }
        public int Total { get; set; } = 0;
        public string Extension { get; set; }
        public string  FormatType { get; set; }
    }
}
