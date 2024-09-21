using Inspire.Erp.Application.Document.Implementations;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Entities.documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Document.Interfaces
{
    public interface IHrDocumentMasterService
    {
      
        public DocumentMaster InsertHrDocumentMaster(DocumentMaster hrDocumentMaster);

        public DocumentMaster UpdateHrDocumentMaster(DocumentMaster hrDocumentMaster);

        public IEnumerable<DocumentMaster> DeleteDocumentMaster(long Id);

        public IEnumerable<DocumentMaster> GetAllHrDocumentMasters();

        public DocumentMaster GetDocumentMasterById(int? id);




        //List<HrDocumentMaster> GetAllStaffDocumentMaster();

        //List<HrDocumentMaster> GetAllVeicleDocumentMaster();
        //List<HrDocumentMaster> GetDocumentMasterByVehicleID(long vehicleId);
        //List<HrDocumentMaster> GetDocumentMasterByStaffID(long staffId);
        //IQueryable GetStaffDocumentReport(List<long?> staffIds, List<long?> documentTypeIds, long? documentStatudId);
        //IQueryable GetVehicleDocumentReport(List<long?> vehicleIds, List<long?> documentTypeIds, long? documentStatudId);
        //IQueryable GetCompanyDocumentReport(List<long?> companyIds, List<long?> documentTypeIds, long? documentStatudId);
        //public IQueryable GetAllDocumentsStaff();
        //public IQueryable GetAllDocumentsVehicle();
        //public List<HrDocumentImages> getDocumentImagesByMasterId(long Id);
        //IQueryable GetDocumentTypeReport(HrDocumentTypeFilter docFilter);
    }
}
