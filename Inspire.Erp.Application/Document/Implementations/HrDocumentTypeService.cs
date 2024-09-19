using Inspire.Erp.Application.Document.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Entities.documents;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Document.Implementations
{
    public class HrDocumentTypeService : IHrDocumentTypeService
    {
        private IRepository<HrDocumentType> _HrDocumentTypeRepo;
        private IRepository<StationMaster> _StationMasterRepo;

        public HrDocumentTypeService(IRepository<HrDocumentType> HrDocumentTypeRepo, IRepository<StationMaster> stationMasterRepo)
        {
            _HrDocumentTypeRepo = HrDocumentTypeRepo;
            _StationMasterRepo = stationMasterRepo;
        }
        public IEnumerable<HrDocumentType> InsertDocumentType(HrDocumentType HrDocumentType)
        {
            bool valid = false;
            try
            {
                valid = true;
                _HrDocumentTypeRepo.Insert(HrDocumentType);
            }
            catch (Exception ex)
            {
                valid = false;
                throw ex;
            }
            finally
            {
                //hrCompanyMastersrepository.Dispose();
            }
            return this.GetAllHrDocumentType();
        }
        public IEnumerable<HrDocumentType> UpdateDocumentType(HrDocumentType HrDocumentType)
        {
            bool valid = false;
            try
            {
                _HrDocumentTypeRepo.Update(HrDocumentType);
                valid = true;
            }
            catch (Exception ex)
            {
                valid = false;
                throw ex;
            }
            finally
            {
                //hrCompanyMastersrepository.Dispose();
            }
            return this.GetAllHrDocumentType();
        }
        public IEnumerable<HrDocumentType> DeleteDocumentType(HrDocumentType hrCompanyMaster)
        {
            bool valid = false;
            try
            {
                _HrDocumentTypeRepo.Delete(hrCompanyMaster);
                valid = true;
            }
            catch (Exception ex)
            {
                valid = false;
                throw ex;
            }
            finally
            {
                //hrCompanyMastersrepository.Dispose();
            }
            return this.GetAllHrDocumentType();
        }

        public IEnumerable<HrDocumentType> GetAllHrDocumentType()
        {
            IEnumerable<HrDocumentType> HrDocumentTypes;
            try
            {
                HrDocumentTypes = _HrDocumentTypeRepo.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //hrShiftsrepository.Dispose();
            }
            return HrDocumentTypes;
        }
        public IEnumerable<StationMasterList> GetAllCompanies()
        {
            IEnumerable<StationMasterList> StationMaster;
            try
            {
              
                return _StationMasterRepo.GetAll().Where(a => a.StationMasterDelStatus != true)
                    .Select(c => new StationMasterList { StationMasterCode = c.StationMasterCode, StationMasterStationName = c.StationMasterStationName });

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //_StationMasterRepo.Dispose();
            }
            //return StationMaster;
        }
    }
}
