using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Entities.documents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Document.Interfaces
{
    public interface IHrDocumentTypeService
    {
        public IEnumerable<HrDocumentType> InsertDocumentType(HrDocumentType hrDocumentType);
        public IEnumerable<HrDocumentType> UpdateDocumentType(HrDocumentType hrDocumentType);
        public IEnumerable<HrDocumentType> DeleteDocumentType(HrDocumentType hrDocumentType);
        public IEnumerable<HrDocumentType> GetAllHrDocumentType();
        public IEnumerable<StationMasterList> GetAllCompanies();
    }
}
