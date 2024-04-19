using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Master
{
    
    public class RateItemListrModel
    {
        public long? id { get; set; }
        public double? UnitPrice { get; set; }
        public string? UD_Barcode { get; set; }
        public string? Packing { get; set; }
        public string? UD_Discrption { get; set; }
    }
}