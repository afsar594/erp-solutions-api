using Inspire.Erp.Application.Document.Interfaces;
using Inspire.Erp.Domain.Entities.documents;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Document.Implementations
{
    public class HrDocumentStatusMasterService : IHrDocumentStatusMasterService
    {
        private IRepository<HrDocumentStatusMaster> repository;

        public HrDocumentStatusMasterService(IRepository<HrDocumentStatusMaster> repository)
        {
            this.repository = repository;
        }

        public IEnumerable<HrDocumentStatusMaster> DeleteHrDocumentStatusMaster(HrDocumentStatusMaster hrDocumentStatusMaster)
        {
            try
            {
                repository.Delete(hrDocumentStatusMaster);
                return repository.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<HrDocumentStatusMaster> GetAllHrDocumentStatusMaster()
        {
            try
            {
                return repository.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<HrDocumentStatusMaster> InsertHrDocumentStatusMaster(HrDocumentStatusMaster hrDocumentStatusMaster)
        {
            try
            {
                repository.Insert(hrDocumentStatusMaster);
                return repository.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<HrDocumentStatusMaster> UpdateHrDocumentStatusMaster(HrDocumentStatusMaster hrDocumentStatusMaster)
        {
            try
            {
                repository.Update(hrDocumentStatusMaster);
                return repository.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
