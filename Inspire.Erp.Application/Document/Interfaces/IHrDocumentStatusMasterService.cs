using Inspire.Erp.Domain.Entities.documents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Document.Interfaces
{
    public interface IHrDocumentStatusMasterService
    {
        public IEnumerable<HrDocumentStatusMaster> InsertHrDocumentStatusMaster(HrDocumentStatusMaster hrDocumentStatusMaster);
        public IEnumerable<HrDocumentStatusMaster> UpdateHrDocumentStatusMaster(HrDocumentStatusMaster hrDocumentStatusMaster);
        public IEnumerable<HrDocumentStatusMaster> DeleteHrDocumentStatusMaster(HrDocumentStatusMaster hrDocumentStatusMaster);
        public IEnumerable<HrDocumentStatusMaster> GetAllHrDocumentStatusMaster();
    }
}
