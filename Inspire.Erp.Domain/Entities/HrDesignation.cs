using System;
using System.Collections.Generic;
using IInspire.Erp.Domain.Entities;

//   :BaseEntity
namespace Inspire.Erp.Domain.Entities
{
    public partial class HrDesignation : BaseEntity
    {
        //public HrDesignation()
        //{
        //    HrStaffMaster = new HashSet<HrStaffMaster>();
        //}

        public long DesignationId { get; set; }
        public string Designation { get; set; }
        public string ShortName { get; set; }

        //public virtual ICollection<HrStaffMaster> HrStaffMaster { get; set; }
    }
}
