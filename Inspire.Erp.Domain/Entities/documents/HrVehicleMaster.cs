using IInspire.Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Entities.documents
{
    public partial class HrVehicleMaster : BaseEntity
    {
        public long Id { get; set; }
        public string VehicleTye { get; set; }
        public string VehicleNo { get; set; }
        public string EngineNo { get; set; }
        public string ChasisNo { get; set; }
        public bool? Sold { get; set; }
        public long? StaffId { get; set; }
        public long? LocationId { get; set; }
        public long? CompanyId { get; set; }
    }
}
