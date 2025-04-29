using Inspire.Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Master.Interfaces
{
    public interface IHrDesignationService
    {
        public Task<dynamic> GetAllDesignation();
        public HrDesignation GetDHrDesignationById(int? id);

        Task<dynamic> InsertDesignation(HrDesignation obj);
        public Task<dynamic> UpdateDesignation(HrDesignation obj);
        public IEnumerable<HrDesignation> DeleteDesignation(long Id);
    }
}
