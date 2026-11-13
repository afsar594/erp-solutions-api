using Inspire.Erp.Domain.DTO;
using Inspire.Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Complaint
{
    public interface IComplaint
    {
        IEnumerable<ComplaintMaster> GetAllComplaints();
        ComplaintMaster GetComplaintById(int Id);
        bool InsertComplaint(ComplainMasterDto dto);
        bool UpdateComplaintStatus(int Id, string NewStatus);
        void DeleteComplaint(int Id);
    }
}
