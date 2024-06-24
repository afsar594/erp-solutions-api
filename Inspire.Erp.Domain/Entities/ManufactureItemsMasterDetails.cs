using System;
using System.Collections.Generic;

namespace Inspire.Erp.Domain.Entities
{
    public partial class ManufactureItemsMasterDetails
    {
        public string MI_Details_VocherNo { get; set; }
        public int? MID_Id_N { get; set; }
        public int? MI_Id_N { get; set; }
        public int MID_ItemId_N { get; set; }
        public string MID_Description_V { get; set; }
        public int MID_UnitId_N { get; set; }
        public decimal MID_QTY_N { get; set; }
        public decimal MID_Price_N { get; set; }
        public decimal MID_DelQty_N { get; set; }
        public decimal MID_Amt_N { get; set; }
        public string MID_Remarks_V { get; set; }       
    }    
}
