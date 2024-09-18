using Inspire.Erp.Domain.Entities.documents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Document.Interfaces
{
    public interface IHrVehicleMasterService
    {
        public IEnumerable<HrVehicleMaster> InsertHrVehicleMaster(HrVehicleMaster hrVehicleMaster);
        public IEnumerable<HrVehicleMaster> UpdateHrVehicleMaster(HrVehicleMaster hrVehicleMaster);
        public IEnumerable<HrVehicleMaster> DeleteHrVehicleMaster(HrVehicleMaster hrVehicleMaster);
        public IEnumerable<HrVehicleMaster> GetAllHrVehicleMaster();
    }
}
