using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Inspire.Erp.Domain.Entities.StoreWareHouse;

namespace Inspire.Erp.Domain.Entities
{
    public partial class stocktransferRegister
    {
        public stocktransferRegister()
        {

            UnitDetails = new List<UnitDetails>();
        }
        public int StockRegisterMaterialID { get; set; }
        public int StockRegisterQuantity { get; set; }
        public int StockRegisterRate { get; set; }
        public int StockRegisterUnitID { get; set; }
        public virtual ICollection<UnitDetails> UnitDetails { get; set; }

    }  
}
