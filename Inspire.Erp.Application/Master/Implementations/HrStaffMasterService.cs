using Inspire.Erp.Application.Master.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inspire.Erp.Application.Master.Implementations
{
    public class HrStaffMasterService : IHrStaffMasterService
    {
        private IRepository<HrStaffMaster> staffRepository;
        public HrStaffMasterService(IRepository<HrStaffMaster> _staffRepository)
        {
            staffRepository = _staffRepository;
        }

        public List<HrStaffMaster> GetAllHrStaff()
        {
            var data =staffRepository.GetAsQueryable().ToList();
            if (data != null)
            {
                return data; 
            }
            else
            {
                return null;
            }
        }
    }
}
