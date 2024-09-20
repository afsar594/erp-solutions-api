using Inspire.Erp.Domain.Entities.documents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Document.Interfaces
{
    public interface IHrDocumentRequestService
    {
        public IEnumerable<HrDocumentRequest> InsertHrDocumentRequest(HrDocumentRequest hrDocumentRequest);
        public IEnumerable<HrDocumentRequest> UpdateHrDocumentRequest(HrDocumentRequest hrDocumentRequest);
        public IEnumerable<HrDocumentRequest> DeleteHrDocumentRequest(HrDocumentRequest hrDocumentRequest);
        public IEnumerable<HrDocumentRequest> GetAllHrDocumentRequest();
    }
}
