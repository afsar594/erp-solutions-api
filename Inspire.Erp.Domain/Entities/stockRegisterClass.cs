using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspire.Erp.Domain.Entities
{
    public partial class stockRegisterClass
    {
        public stockRegisterClass()
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
