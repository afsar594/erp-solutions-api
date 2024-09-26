using Inspire.Erp.Application.Document.Implementations;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Entities.documents;
using Inspire.Erp.Domain.Modals.Stock;
using Inspire.Erp.Domain.Modals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inspire.Erp.Domain.DTO.WorkGropPermission;

namespace Inspire.Erp.Application.Document.Interfaces
{
    public interface IHrDocumentMasterService   
    {
      
        public DocumentMaster InsertHrDocumentMaster(DocumentMaster hrDocumentMaster);

        public DocumentMaster UpdateHrDocumentMaster(DocumentMaster hrDocumentMaster);

        public IEnumerable<DocumentMaster> DeleteDocumentMaster(long Id);

        public IEnumerable<DocumentMaster> GetAllHrDocumentMasters();

        public DocumentMaster GetDocumentMasterById(int? id);

        Response<List<DocumentMasterlist>> GetDocumentReport(Documentfilter model);

    }
}
