using Inspire.Erp.Application.Master.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
namespace Inspire.Erp.Application.Master.Implementation
{
    public class HrDesignationService : IHrDesignationService
    {
        private IRepository<HrDesignation> _hrRepo;

        public HrDesignationService(IRepository<HrDesignation> hrRepo)
        {
            _hrRepo = hrRepo;
        }
        public async Task<dynamic> GetAllDesignation() 
        {
            try
            {
                return _hrRepo.GetAll();
            }
            catch (Exception ex)
            {
                return ex.Message;
                throw;
            }
        }
        public HrDesignation GetDHrDesignationById(int? id)
        {
            try
            {
                var HrDesignation = new HrDesignation();
                HrDesignation = _hrRepo.GetAsQueryable().FirstOrDefault(x => x.DesignationId == Convert.ToInt32(id));
                return HrDesignation;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<dynamic> InsertDesignation(HrDesignation obj)
        {
            bool valid = false;
            try
            {
                int id = 1;
                int? DId = _hrRepo.GetAsQueryable().Max(a =>(int)a.DesignationId);
                id = (DId ?? 0) + 1;

                obj.DesignationId = id;
                _hrRepo.Insert(obj);
                return this.GetAllDesignation();
            }
            catch (Exception ex)
            {
                valid = false;
                throw ex;

            }
        }
        public async Task<dynamic> UpdateDesignation(HrDesignation obj)
        {
            bool valid = false;
            try
            {
                _hrRepo.Update(obj);
                return this.GetAllDesignation();
            }
            catch (Exception ex)
            {
                valid = false;
                throw ex;

            }
        }
        //public async Task<dynamic> DeleteDesignation(HrDesignation obj)
        //{
        //    bool valid=false;
        //    try
        //    {
        //        _hrRepo.Delete(obj);
        //        return this.GetAllDesignation();
        //    }
        //    catch (Exception ex)
        //    {
        //        valid = false;
        //        throw ex;

        //    }
          
        //}

        public IEnumerable<HrDesignation> DeleteDesignation(long Id)
        {
            try
            {
                var dataObj = _hrRepo.GetAsQueryable().Where(o => o.DesignationId == Id).FirstOrDefault();

                _hrRepo.Delete(dataObj);
                return _hrRepo.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }   
}
