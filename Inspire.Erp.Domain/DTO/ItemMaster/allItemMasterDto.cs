using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Domain.DTO.ItemMaster
{
    public class allItemMasterDto
    {
     
            public long ItemId { get; set; }
            public string ItemName { get; set; }
            public int UnitId { get; set; }
            public string UnitShortName { get; set; }
            public decimal Rate { get; set; }
            public decimal ConversionType { get; set; }

        public string Barcode { get; set; }
            public string DisplayName { get; set; }
    }
}
