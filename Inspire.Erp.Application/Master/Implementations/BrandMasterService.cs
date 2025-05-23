using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inspire.Erp.Application.Master
{
    public class BrandMasterService : IBrandMasterService
    {
        private IRepository<VendorMaster> Brandrepository;
        public BrandMasterService(IRepository<VendorMaster> _Brandrepository)
        {
            Brandrepository = _Brandrepository;
        }
        public IEnumerable<VendorMaster> InsertBrand(VendorMaster VendorMaster)
        {
            bool valid = false;
            try
            {
                valid = true;
                long mxc = 0;
                  mxc = Convert.ToInt32(
                   Brandrepository.GetAsQueryable()
                   .DefaultIfEmpty().Max(o => o == null ? 0 : o.VendorMasterVendorId) + 1);              

                VendorMaster.VendorMasterVendorId = mxc;

                Brandrepository.Insert(VendorMaster);
            }
             catch (Exception ex)
            {
                valid = false;
                throw ex;

            }
            finally
            {
                //cityrepository.Dispose();
            }
            return this.GetAllBrand();
        }
        public IEnumerable<VendorMaster> UpdateBrand(VendorMaster VendorMaster)
        {
            bool valid = false;
            try
            {
                Brandrepository.Update(VendorMaster);
                valid = true;

            }
            catch (Exception ex)
            {
                valid = false;
                throw ex;

            }
            finally
            {
                //cityrepository.Dispose();
            }
            return this.GetAllBrand();
        }
        public IEnumerable<VendorMaster> DeleteBrand(VendorMaster VendorMaster)
        {
            bool valid = false;
            try
            {
                Brandrepository.Delete(VendorMaster);
                valid = true;

            }
            catch (Exception ex)
            {
                valid = false;
                throw ex;

            }
            finally
            {
                //cityrepository.Dispose();
            }
            return this.GetAllBrand();
        }

        public IEnumerable<VendorMaster> GetAllBrand()
        {
            IEnumerable<VendorMaster> VendorMasters;
            try
            {
                VendorMasters = Brandrepository.GetAll();

            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                //cityrepository.Dispose();
            }
            VendorMasters = VendorMasters.OrderBy(sm => sm.VendorMasterVendorName).ToList();
            return VendorMasters;
        }

        public IEnumerable<VendorMaster> GetAllBrandById(int id)
        {
            IEnumerable<VendorMaster> VendorMasters;
            try
            {
                VendorMasters = Brandrepository.GetAsQueryable().Where(k => k.VendorMasterVendorId == id).Select(k => k);

            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                //cityrepository.Dispose();
            }
            return VendorMasters;

        }

    }
}
