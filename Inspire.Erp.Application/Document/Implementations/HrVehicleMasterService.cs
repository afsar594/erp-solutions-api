using Inspire.Erp.Application.Document.Interfaces;
using Inspire.Erp.Domain.Entities.documents;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Document.Implementations
{
    public class HrVehicleMasterService : IHrVehicleMasterService
    {
        private IRepository<HrVehicleMaster> hrVehicleMasterrepository;
        public HrVehicleMasterService(IRepository<HrVehicleMaster> _hrVehicleMasterrepository)
        {
            hrVehicleMasterrepository = _hrVehicleMasterrepository;
        }
        public IEnumerable<HrVehicleMaster> InsertHrVehicleMaster(HrVehicleMaster hrVehicleMaster)
        {
            bool valid = false;
            try
            {
                valid = true;
                hrVehicleMasterrepository.Insert(hrVehicleMaster);
            }
            catch (Exception ex)
            {
                valid = false;
                throw ex;

            }
            finally
            {
                //hrVehicleMasterrepository.Dispose();
            }
            return this.GetAllHrVehicleMaster();
        }
        public IEnumerable<HrVehicleMaster> UpdateHrVehicleMaster(HrVehicleMaster hrVehicleMaster)
        {
            bool valid = false;
            try
            {
                hrVehicleMasterrepository.Update(hrVehicleMaster);
                valid = true;

            }
            catch (Exception ex)
            {
                valid = false;
                throw ex;

            }
            finally
            {
                //hrVehicleMasterrepository.Dispose();
            }
            return this.GetAllHrVehicleMaster();
        }
        public IEnumerable<HrVehicleMaster> DeleteHrVehicleMaster(HrVehicleMaster hrVehicleMaster)
        {
            bool valid = false;
            try
            {
                hrVehicleMasterrepository.Delete(hrVehicleMaster);
                valid = true;

            }
            catch (Exception ex)
            {
                valid = false;
                throw ex;

            }
            finally
            {
                //hrVehicleMasterrepository.Dispose();
            }
            return this.GetAllHrVehicleMaster();
        }

        public IEnumerable<HrVehicleMaster> GetAllHrVehicleMaster()
        {
            IEnumerable<HrVehicleMaster> hrVehicleMasters;
            try
            {
                hrVehicleMasters = hrVehicleMasterrepository.GetAll();

            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                //hrVehicleMasterrepository.Dispose();
            }
            return hrVehicleMasters;
        }
    }
}
