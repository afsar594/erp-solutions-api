using Inspire.Erp.Domain.Entities;
using System.Collections.Generic;

namespace Inspire.Erp.Application.Master.Interfaces
{
    public interface IOtherChargesMasterService
    {
        public IEnumerable<OtherChargesMaster> InsertOtherChargesMaster(OtherChargesMaster otherChargesMaster);
        public IEnumerable<OtherChargesMaster> UpdateOtherChargesMaster(OtherChargesMaster otherChargesMaster);
        public IEnumerable<OtherChargesMaster> DeleteOtherChargesMaster(OtherChargesMaster otherChargesMaster);
        public IEnumerable<OtherChargesMaster> GetAllOtherChargesMasterByType(string type);
        public IEnumerable<OtherChargesMaster> GetAllOtherChargesMaster();
        public IEnumerable<OtherChargesMaster> GetAllOtherChargesMasterById(int id);
    }
}
