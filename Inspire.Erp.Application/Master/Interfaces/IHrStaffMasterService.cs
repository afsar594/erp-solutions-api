using Inspire.Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Master.Interfaces
{
    public interface IHrStaffMasterService
    {
        public List<HrStaffMaster> GetAllHrStaff(); 
    }
}
