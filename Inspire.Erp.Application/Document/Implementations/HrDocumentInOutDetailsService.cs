using Inspire.Erp.Application.Document.Interfaces;
using Inspire.Erp.Domain.Entities.documents;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Document.Implementations
{
    public class HrDocumentInOutDetailsService : IHrDocumentInOutDetailsService
    {
        private IRepository<HrDocumentInOutDetails> repository;
        public HrDocumentInOutDetailsService(IRepository<HrDocumentInOutDetails> repository)
        {
            this.repository = repository;
        }

        public IEnumerable<HrDocumentInOutDetails> DeleteHrDocumentInOutDetail(HrDocumentInOutDetails hrDocumentInOutDetails)
        {
            try
            {
                repository.Delete(hrDocumentInOutDetails);
                return repository.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<HrDocumentInOutDetails> GetAllHrDocumentInOutDetails()
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

        public IEnumerable<HrDocumentInOutDetails> InsertHrDocumentInOutDetail(HrDocumentInOutDetails hrDocumentInOutDetails)
        {
            try
            {
                repository.Insert(hrDocumentInOutDetails);
                return repository.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<HrDocumentInOutDetails> UpdateHrDocumentInOutDetail(HrDocumentInOutDetails hrDocumentInOutDetails)
        {
            try
            {
                repository.Update(hrDocumentInOutDetails);
                return repository.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
