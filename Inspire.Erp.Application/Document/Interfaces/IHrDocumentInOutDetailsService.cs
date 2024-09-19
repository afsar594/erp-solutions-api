using Inspire.Erp.Domain.Entities.documents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Document.Interfaces
{
    public interface IHrDocumentInOutDetailsService
    {
        public IEnumerable<HrDocumentInOutDetails> InsertHrDocumentInOutDetail(HrDocumentInOutDetails hrDocumentInOutDetails);
        public IEnumerable<HrDocumentInOutDetails> UpdateHrDocumentInOutDetail(HrDocumentInOutDetails hrDocumentInOutDetails);
        public IEnumerable<HrDocumentInOutDetails> DeleteHrDocumentInOutDetail(HrDocumentInOutDetails hrDocumentInOutDetails);
        public IEnumerable<HrDocumentInOutDetails> GetAllHrDocumentInOutDetails();
    }
}
