using Inspire.Erp.Application.Document.Interfaces;
using Inspire.Erp.Domain.Entities.documents;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Document.Implementations
{
    public class HrDocumentRequestService : IHrDocumentRequestService
    {
        private IRepository<HrDocumentRequest> _repository;
        public HrDocumentRequestService(IRepository<HrDocumentRequest> repository)
        {
            this._repository = repository;
        }

        public IEnumerable<HrDocumentRequest> DeleteHrDocumentRequest(HrDocumentRequest hrDocumentRequest)
        {
            try
            {
                _repository.Delete(hrDocumentRequest);
                return _repository.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<HrDocumentRequest> GetAllHrDocumentRequest()
        {
            try
            {
                return _repository.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<HrDocumentRequest> InsertHrDocumentRequest(HrDocumentRequest hrDocumentRequest)
        {
            try
            {
                _repository.Insert(hrDocumentRequest);
                return _repository.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<HrDocumentRequest> UpdateHrDocumentRequest(HrDocumentRequest hrDocumentRequest)
        {
            try
            {
                _repository.Update(hrDocumentRequest);
                return _repository.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
