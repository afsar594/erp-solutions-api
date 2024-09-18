using Inspire.Erp.Application.Document.Interfaces;
using Inspire.Erp.Domain.Entities.documents;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Document.Implementations
{
    public class HrDocumentTypeService : IHrDocumentTypeService
    {
        private IRepository<HrDocumentType> _HrDocumentTypeRepo;

        public HrDocumentTypeService(IRepository<HrDocumentType> HrDocumentTypeRepo)
        {
            _HrDocumentTypeRepo = HrDocumentTypeRepo;
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
    }
}
