using Inspire.Erp.Domain.Entities.documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Inspire.Erp.Application.Document.Interfaces;
using Inspire.Erp.Infrastructure.Database.Repositoy;

using System.Threading.Tasks;
namespace Inspire.Erp.Application.Document.Interfaces
{
    public interface IHrStaffMasterService
    {

        public IEnumerable<HrStaffMaster> InsertHrStaffMaster(HrStaffMaster hrStaffMaster);
        public IEnumerable<HrStaffMaster> UpdateHrStaffMaster(HrStaffMaster hrStaffMaster);
        public IEnumerable<HrStaffMaster> DeleteHrStaffMaster(HrStaffMaster hrStaffMaster);
        public IEnumerable<HrStaffMaster> GetAllHrStaffMaster();
        IQueryable GetDepartmentWiseStaff();
        IQueryable GetStaffReport(long? siteID, long? sponsorID, long? companyID, long? visaCompanyID, long? departmentID);
        IQueryable GetStaffsBithday(string monthName);
        List<HrStaffMaster> InsertStaffOTRate(List<HrStaffMaster> dataList);
        public IQueryable GetStaffOTRate();
        public List<HrStaffMaster> UpdateActiveStatus(List<HrStaffMaster> dataList);
        public IQueryable GetActiveStatusStaff();
        public IQueryable GetDesignationWiseStaff();
        public HrStaffMaster getStaffByID(long staffID);
    }
}
