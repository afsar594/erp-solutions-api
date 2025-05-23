using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Models.Master
{
    public class AssetRegisterModel
    {
            public string searchText { get; set; }
            public DateTime? fromDate { get; set; }
            public DateTime? toDate { get; set; }
        public DateTime depDate { get; set; }

    }
}
