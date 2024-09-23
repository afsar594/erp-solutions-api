using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Entities.documents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Document.Interfaces
{
    public interface IHrDocumentTypeService
    {
        public HrDocumentType InsertDocumentType(HrDocumentType hrDocumentType);

        public HrDocumentType UpdateDocumentType(HrDocumentType hrDocumentType);
       
        public IEnumerable<HrDocumentType> DeleteDocumentType(long Id);

        public IEnumerable<HrDocumentType> GetAllHrDocumentType();
        public HrDocumentType GetDocumentTypeById(int? id);

        public IEnumerable<StationMasterList> GetAllCompanies();

        public IEnumerable<HrDocumentTypeList> GetAlldocumentType();
    }
}
