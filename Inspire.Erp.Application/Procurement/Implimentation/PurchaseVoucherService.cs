using Inspire.Erp.Application.Master.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Inspire.Erp.Infrastructure.Database;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Modals.Procurement;
using Inspire.Erp.Domain.Modals;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Master.Implementations
{
    public class PurchaseVoucherService : IPurchaseVoucherService
    {
        private IRepository<PurchaseVoucher> _PurchaseVoucher;
        private IRepository<PurchaseVoucherDetails> _PurchaseVoucherDetails;
        private IRepository<JobMaster> _JobMaster;
        private IRepository<Suppliers> _Suppliers;
        private IRepository<LocationMaster> _LocationMaster;
        private IRepository<ItemMaster> _ItemMaster;
        private IRepository<UnitMaster> _UnitMaster;
        private InspireErpDBContext _context;
        public PurchaseVoucherService(IRepository<PurchaseVoucher> PurchaseVoucher, IRepository<PurchaseVoucherDetails> PurchaseVoucherDetails,
        IRepository<JobMaster> JobMaster, IRepository<Suppliers> Suppliers, IRepository<LocationMaster> LocationMaster,
        IRepository<ItemMaster> ItemMaster, IRepository<UnitMaster> UnitMaster, InspireErpDBContext context)
        {
            _PurchaseVoucher = PurchaseVoucher;
            _PurchaseVoucherDetails = PurchaseVoucherDetails;
            _JobMaster = JobMaster;
            _Suppliers = Suppliers;
            _LocationMaster = LocationMaster;
            _ItemMaster = ItemMaster;
            _UnitMaster = UnitMaster;
            _context = context;
        }
        public IEnumerable<PurchaseVoucher> GetPurchaseVoucher()
        {
            try
            {
                IEnumerable<PurchaseVoucher> _PurchaseVoucherList = _PurchaseVoucher.GetAll().ToList();
                return _PurchaseVoucherList;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<PurchaseVoucherDetails> GetPurchaseVoucherDetails()
        {
            try
            {
                IEnumerable<PurchaseVoucherDetails> _PurchaseVoucherList = _PurchaseVoucherDetails.GetAll().ToList();
                return _PurchaseVoucherList;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<PurchaseVoucher> InsertPurchaseVoucher(PurchaseVoucher DataObj)
        {
            try
            {
                DataObj.PurchaseVoucherPurID = _PurchaseVoucher.GetAsQueryable().Select(x=> x.PurchaseVoucherPurID).Max() + 1;
                _PurchaseVoucher.Insert(DataObj);
                return this.GetPurchaseVoucher();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<PurchaseVoucher> UpdatePurchaseVoucher(PurchaseVoucher DataObj)
        {
            try
            {
                _PurchaseVoucher.Update(DataObj);
                return this.GetPurchaseVoucher();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<PurchaseVoucher> DeletePurchaseVoucher(PurchaseVoucher DataObj)
        {
            try
            {
                _PurchaseVoucher.Delete(DataObj);
                return this.GetPurchaseVoucher();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable GetPurchaseReturnReport()
        {
            try
            {
                var supplierList = _context.Suppliers.ToList();
                var detailData = (from dtl in _context.PurchaseVoucherDetails.ToList()
                                  join hdr in _context.PurchaseVoucher.ToList() on dtl.PurchaseVoucherDetailsVoucherNo equals hdr.PurchaseVoucherVoucherNo
                                  join itm in _context.ItemMaster.ToList() on (long)dtl.PurchaseVoucherDetailsMaterialId equals itm.ItemMasterItemId into itemMaster
                                  from leftItem in itemMaster.DefaultIfEmpty()
                                  join unit in _context.UnitMaster.ToList() on dtl.PurchaseVoucherDetailsUnitId equals unit.UnitMasterUnitId into unitMaster
                                  from lunit in unitMaster.DefaultIfEmpty()
                                  select new
                                  {
                                      PurchaseDetailID = dtl.PurchaseVoucherDetailsPurcahseDetailsId,
                                      PartNo = dtl.PurchaseVoucherDetailsBatchCode,
                                      ItemName = leftItem != null ? leftItem.ItemMasterItemName : "",
                                      UnitShortName = lunit != null ? lunit.UnitMasterUnitShortName : "",
                                      Quantity = dtl.PurchaseVoucherDetailsQuantity,
                                      Rate = dtl.PurchaseVoucherDetailsRate,
                                      Amount = dtl.PurchaseVoucherDetailsAmount,
                                      PurchaseVoucherDetailsVoucherNo = dtl.PurchaseVoucherDetailsVoucherNo,
                                  }).AsQueryable();

                var finalData = (from hdr in _context.PurchaseVoucher.ToList()
                                 join job in _context.JobMaster.ToList() on hdr.PurchaseVoucherJobId equals job.JobMasterJobId into leftjob
                                 from jobmaster in leftjob.DefaultIfEmpty()
                                 join location in _context.LocationMaster.ToList() on hdr.PurchaseVoucherLocationId equals location.LocationMasterLocationId into leftlocation
                                 from locationmaster in leftlocation.DefaultIfEmpty()
                                 select new
                                 {
                                     JobId = hdr.PurchaseVoucherJobId,
                                     LocationId = hdr.PurchaseVoucherLocationId,
                                     CurrencyId = hdr.PurchaseVoucherCurrencyId,
                                     SupplierId = supplierList.FirstOrDefault(o => o.SuppliersInsName == hdr.PurchaseVoucherCashSupplierName) == null ? 0 : supplierList.FirstOrDefault(o => o.SuppliersInsName == hdr.PurchaseVoucherCashSupplierName).SuppliersInsId,
                                     PurchaseID = hdr.PurchaseVoucherPurID,
                                     PurchaseType = hdr.PurchaseVoucherPurchaseType,
                                     SupplierName = hdr.PurchaseVoucherCashSupplierName,
                                     SupplierInvoice = hdr.PurchaseVoucherSupplyInvoiceNo,
                                     Gross = hdr.PurchaseVoucherActualAmount,
                                     Discount = hdr.PurchaseVoucherTotalDiscountAmt,
                                     VatAmount = hdr.PurchaseVoucherVatAmount,
                                     NetAmount = hdr.PurchaseVoucherNetAmount,
                                     LocationName = locationmaster != null ? locationmaster.LocationMasterLocationName : "",
                                     PurchaseDetails = detailData.Where(o => o.PurchaseVoucherDetailsVoucherNo == hdr.PurchaseVoucherVoucherNo).ToList()
                                 }).AsQueryable();
                return finalData;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }


        public async Task<Response<GridWrapperResponse<List<GetPurchaseVoucherResponse>>>> GetPurchaseVoucherReport(GenericGridViewModel model)
        {
            try
            {
                List<GetPurchaseVoucherResponse> response = new List<GetPurchaseVoucherResponse>();


                var purVouchers = (from pm in _context.PurchaseVoucher
                           join jm in _context.JobMaster on pm.PurchaseVoucherJobId equals jm.JobMasterJobId into jmpm
                           from y in jmpm.DefaultIfEmpty()
                           join lm in _context.LocationMaster on pm.PurchaseVoucherLocationId equals lm.LocationMasterLocationId into locs
                           from x in locs.DefaultIfEmpty()
                           select new GetPurchaseVoucherResponse{ PurchaseVoucherVoucherNo = pm.PurchaseVoucherVoucherNo,
                               PurchaseVoucherPurchaseDate = pm.PurchaseVoucherPurchaseDate,
                               PurchaseVoucherPurchaseType = pm.PurchaseVoucherPurchaseType,
                               PurchaseVoucherCashSupplierName = pm.PurchaseVoucherCashSupplierName,
                               PurchaseVoucherSupplyInvoiceNo = pm.PurchaseVoucherSupplyInvoiceNo,
                               PurchaseVoucherActualAmount = pm.PurchaseVoucherActualAmount,
                               PurchaseVoucherDisAmount = pm.PurchaseVoucherDisAmount,
                               PurchaseVoucherVatAmount = pm.PurchaseVoucherVatAmount,
                               PurchaseVoucherNetAmount = pm.PurchaseVoucherNetAmount
                           }).ToList();

               // response.AddRange(loj);


         
                var gridResponse = new GridWrapperResponse<List<GetPurchaseVoucherResponse>>();
                gridResponse.Data = purVouchers;
                gridResponse.Total = 0;
                return Response<GridWrapperResponse<List<GetPurchaseVoucherResponse>>>.Success(gridResponse, "Data found");
            }
            catch (Exception ex)
            {
                return Response<GridWrapperResponse<List<GetPurchaseVoucherResponse>>>.Fail(new GridWrapperResponse<List<GetPurchaseVoucherResponse>>(), ex.Message);
            }
        }
    }
}
