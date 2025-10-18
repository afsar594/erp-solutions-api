using Inspire.Erp.Application.Master.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Inspire.Erp.Application.Master.Implementations
{
    public class OtherChargesMasterService : IOtherChargesMasterService
    {
        private IRepository<OtherChargesMaster> _repository;
        public OtherChargesMasterService(IRepository<OtherChargesMaster> repository)
        {
            _repository = repository;
        }
        public IEnumerable<OtherChargesMaster> GetAllOtherChargesMaster()
        {
            IEnumerable<OtherChargesMaster> otherChargesMaster;
            try
            {
                otherChargesMaster = _repository.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            otherChargesMaster = otherChargesMaster.OrderBy(sm => sm.ServiceName).ToList();
            return otherChargesMaster;
        }

        public IEnumerable<OtherChargesMaster> GetAllOtherChargesMasterById(int id)
        {
            IEnumerable<OtherChargesMaster> otherChargesMaster;
            try
            {
                otherChargesMaster = _repository.GetAsQueryable().Where(k => k.ServiceID == id).Select(k => k);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return otherChargesMaster;
        }

        public IEnumerable<OtherChargesMaster> InsertOtherChargesMaster(OtherChargesMaster otherChargesMaster)
        {
            try
            {
                _repository.Insert(otherChargesMaster);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return this.GetAllOtherChargesMaster();
        }

        public IEnumerable<OtherChargesMaster> UpdateOtherChargesMaster(OtherChargesMaster otherChargesMaster)
        {
            try
            {
                _repository.Update(otherChargesMaster);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return this.GetAllOtherChargesMaster();
        }
        public IEnumerable<OtherChargesMaster> DeleteOtherChargesMaster(OtherChargesMaster otherChargesMaster)
        {
            try
            {
                _repository.Delete(otherChargesMaster);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return this.GetAllOtherChargesMaster();
        }
    }

}
