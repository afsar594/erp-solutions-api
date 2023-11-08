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
using Microsoft.EntityFrameworkCore;
using Inspire.Erp.Domain.Enums;

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

        public IQueryable GetAllGRN()
        {
            try
            {
                var grnList = _context.PurchaseVoucher.Where(o => o.PurchaseVoucherVoucherNo.Contains("GRN")).AsQueryable();
                return grnList;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        //public PurchaseVoucher GetByID(string voucherNo)
        //{
        //    try
        //    {
        //        PurchaseVoucher pv = new PurchaseVoucher();
        //        var userTrackngData = (from hdr in _context.UserTracking.Where(o => o.UserTrackingUserVpType == "GRN" && o.UserTrackingUserVpNo == voucherNo)
        //                               join dtl in _context.UserFile on (long)hdr.UserTrackingUserUserId equals dtl.UserId
        //                               select new UserTrackingDisplay()
        //                               {
        //                                   VPAction = hdr.UserTrackingUserVpAction,
        //                                   UserId = dtl.UserId,
        //                                   UserName = dtl.UserName,
        //                                   VPType = hdr.UserTrackingUserVpType,
        //                                   ChangeDt = hdr.UserTrackingUserChangeDt,
        //                                   ChangeTime = hdr.UserTrackingUserChangeTime,
        //                                   VPNo = hdr.UserTrackingUserVpNo,
        //                               }).ToList();
        //        pv = _context.PurchaseVoucher.FirstOrDefault(o => o.PurchaseVoucherVoucherNo == voucherNo);
        //        pv.PurchaseVoucherDetails = _context.PurchaseVoucherDetails.Where(o => o.PurchaseVoucherDetailsVoucherNo == voucherNo).ToList();
        //        pv.AccountsTransactions = _context.AccountsTransactions.Where(o => o.AccountsTransactionsVoucherNo == voucherNo).ToList();
        //        pv.UserTrackingData = userTrackngData;
        //        return pv;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public IEnumerable<PurchaseVoucher> InsertPurchaseVoucher(PurchaseVoucher DataObj)
        {
            try
            {
                //DataObj.PurchaseVoucherPurID = _PurchaseVoucher.GetAsQueryable().Select(x=> x.PurchaseVoucherPurID).Max() + 1;
                //_PurchaseVoucher.Insert(DataObj);
                DataObj.PurchaseVoucherVoucherNo = DataObj.PurchaseVoucherVoucherNo;
                _context.PurchaseVoucher.Add(DataObj);
                _context.SaveChanges();
                DataObj.PurchaseVoucherPurID = DataObj.PurchaseVoucherPurID;

                DataObj.PurchaseVoucherDetails = DataObj.PurchaseVoucherDetails.Select((x) =>
                {
                    x.PurchaseVoucherDetailsSno = (int)DataObj.PurchaseVoucherPurID;
                    x.PurchaseVoucherDetailsVoucherNo = DataObj.PurchaseVoucherVoucherNo;
                    return x;
                }).ToList();
                _context.PurchaseVoucherDetails.AddRange(DataObj.PurchaseVoucherDetails);
                //_context.SaveChanges();

                DataObj.AccountsTransactions = DataObj.AccountsTransactions.Select((k) =>
                {
                    k.AccountsTransactionsVoucherNo = DataObj.PurchaseVoucherVoucherNo;
                    k.AccountsTransactionsVoucherType = VoucherType.PurchaseVoucher_TYPE;
                    k.AccountsTransactionsStatus = AccountStatus.Approved;
                    return k;
                }).ToList();
                _context.AccountsTransactions.AddRange(DataObj.AccountsTransactions);
                //_context.SaveChanges();

                List<StockRegister> sr = new List<StockRegister>();

                foreach (var item in DataObj.PurchaseVoucherDetails)
                {
                    sr.Add(new StockRegister()
                    {
                        StockRegisterRefVoucherNo = DataObj.PurchaseVoucherVoucherNo,
                        StockRegisterAssignedDate = DateTime.Now,
                        StockRegisterTransType = VoucherType.PurchaseVoucher_TYPE,
                        StockRegisterFCAmount = DataObj.PurchaseVoucherActualAmount,
                        StockRegisterFcRate = DataObj.PurchaseVoucherFCRate,
                        StockRegisterStatus = "A",
                        StockRegisterJobID = DataObj.PurchaseVoucherJobID,
                        StockRegisterLocationID = DataObj.PurchaseVoucherLocationID,
                        StockRegisterFSNO = null,
                        StockRegisterVoucherDate = DataObj.PurchaseVoucherGRDate,
                        StockRegisterSIN = item.PurchaseVoucherDetailsQuantity,
                        StockRegisterMaterialID = item.PurchaseVoucherDetailsMaterialId,
                    });
                }
                _context.StockRegister.AddRange(sr);

                UserTracking trackingData = new UserTracking();
                trackingData.UserTrackingUserUserId = 1;
                trackingData.UserTrackingUserVpAction = "Insert";
                trackingData.UserTrackingUserVpNo = DataObj.PurchaseVoucherVoucherNo;
                trackingData.UserTrackingUserChangeDt = DateTime.Now;
                trackingData.UserTrackingUserChangeTime = DateTime.Now;
                trackingData.UserTrackingUserVpType = "GRN";
                _context.UserTracking.Add(trackingData);
                _context.SaveChanges();

                _context.SaveChanges();

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
                _context.PurchaseVoucher.Update(DataObj);

                foreach (var k in DataObj.AccountsTransactions)
                {
                    k.AccountsTransactionsVoucherNo = DataObj.PurchaseVoucherVoucherNo;
                    k.AccountsTransactionsVoucherType = VoucherType.PurchaseVoucher_TYPE;
                    k.AccountsTransactionsStatus = AccountStatus.Approved;
                }
                _context.AccountsTransactions.UpdateRange(DataObj.AccountsTransactions);
             
                foreach (var item in DataObj.PurchaseVoucherDetails)
                {
                    item.PurchaseVoucherDetailsVoucherNo = DataObj.PurchaseVoucherVoucherNo;
                }
                _context.PurchaseVoucherDetails.UpdateRange(DataObj.PurchaseVoucherDetails);

                UserTracking trackingData = new UserTracking();
                trackingData.UserTrackingUserUserId = 1;
                trackingData.UserTrackingUserVpAction = "Update";
                trackingData.UserTrackingUserVpNo = DataObj.PurchaseVoucherVoucherNo;
                trackingData.UserTrackingUserChangeDt = DateTime.Now;
                trackingData.UserTrackingUserChangeTime = DateTime.Now;
                trackingData.UserTrackingUserVpType = "GRN";
                _context.UserTracking.Add(trackingData);

                _context.SaveChanges();
                return this.GetPurchaseVoucher();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        //public IEnumerable<PurchaseVoucher> DeleteGRN(GRNRequest grn)
        //{
        //    try
        //    {
        //        VouchersNumbers vs = _context.VouchersNumbers.Where(k => k.VouchersNumbersVNoNu == int.Parse(grn.V_NO)).SingleOrDefault();
        //        if (vs != null)
        //        {
        //            vs.VouhersNumbersDelStatus = true;
        //            _context.VouchersNumbers.Update(vs);
        //        }
            
        //        PurchaseVoucher pv = _context.PurchaseVoucher.Where(k => k.PurchaseVoucherPoNo == int.Parse(grn.V_NO)).SingleOrDefault();
        //        if (pv!= null)
        //        {
        //            pv.PurchaseVoucherDelStatus = true;
        //            _context.PurchaseVoucher.Update(pv);
        //        }
           
        //        PurchaseVoucherDetails pvd = _context.PurchaseVoucherDetails.Where(k => k.PurchaseVoucherDetailsGrnNo == grn.V_NO).SingleOrDefault();
        //        if (pvd != null)
        //        {
        //            pvd.PurchaseVoucherDetailsDelStatus = true;
        //            _context.PurchaseVoucherDetails.Update(pvd);
        //        }
        
        //        StockRegister sr = _context.StockRegister.Where(k => k.StockRegisterRefVoucherNo == grn.V_NO).SingleOrDefault();
        //        if (vs == null)
        //        {
        //            return false;
        //        }
        //        sr.StockRegisterDelStatus = true;
        //        _context.StockRegister.Update(sr);
        //        AccountsTransactions at = _context.AccountsTransactions.Where(k => k.AccountsTransactionsVoucherNo == grn.V_NO).SingleOrDefault();
        //        if (vs == null)
        //        {
        //            return false;
        //        }
        //        at.AccountstransactionsDelStatus = true;
        //        _context.AccountsTransactions.Update(at);

        //        UserTracking trackingData = new UserTracking();
        //        trackingData.UserTrackingUserUserId = 1;
        //        trackingData.UserTrackingUserVpAction = "Delete";
        //        // trackingData.UserTrackingUserVpNo = grn.PurchaseVoucherVoucherNo;
        //        trackingData.UserTrackingUserChangeDt = DateTime.Now;
        //        trackingData.UserTrackingUserChangeTime = DateTime.Now;
        //        trackingData.UserTrackingUserVpType = "GRN";
        //        _context.UserTracking.Add(trackingData);
        //        _context.SaveChanges();
        //        return this.GetPurchaseVoucher();
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}


        //public IEnumerable<GRNResponse> loadPo(GRNRequest obj)
        //{
        //    try
        //    {
        //        var x = _grnRepository.GetBySP($"EXEC LoadPoList").Where(x => x.Suppliers_INS_Name.Equals(obj.PurchaseID)).ToList();
        //        return x;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        throw;
        //    }
        //}

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
                                 join job in _context.JobMaster.ToList() on hdr.PurchaseVoucherJobID equals job.JobMasterJobId into leftjob
                                 from jobmaster in leftjob.DefaultIfEmpty()
                                 join location in _context.LocationMaster.ToList() on hdr.PurchaseVoucherLocationID equals location.LocationMasterLocationId into leftlocation
                                 from locationmaster in leftlocation.DefaultIfEmpty()
                                 select new
                                 {
                                     JobId = hdr.PurchaseVoucherJobID,
                                     LocationId = hdr.PurchaseVoucherLocationID,
                                     CurrencyId = hdr.PurchaseVoucherCurrencyID,
                                     SupplierId = supplierList.FirstOrDefault(o => o.SuppliersInsName == hdr.PurchaseVoucherCashSupplierName) == null ? 0 : supplierList.FirstOrDefault(o => o.SuppliersInsName == hdr.PurchaseVoucherCashSupplierName).SuppliersInsId,
                                     PurchaseID = hdr.PurchaseVoucherPurID,
                                     PurchaseType = hdr.PurchaseVoucherPurchaseType,
                                     SupplierName = hdr.PurchaseVoucherCashSupplierName,
                                     SupplierInvoice = hdr.PurchaseVoucherSupplyInvoiceNo,
                                     Gross = hdr.PurchaseVoucherActualAmount,
                                     Discount = hdr.PurchaseVoucherTotalDiscountAmt,
                                     VatAmount = hdr.PurchaseVoucherVATAmount,
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
                           join jm in _context.JobMaster on pm.PurchaseVoucherJobID equals jm.JobMasterJobId into jmpm
                           from y in jmpm.DefaultIfEmpty()
                           join lm in _context.LocationMaster on pm.PurchaseVoucherLocationID equals lm.LocationMasterLocationId into locs
                           from x in locs.DefaultIfEmpty()
                           select new GetPurchaseVoucherResponse{ PurchaseVoucherVoucherNo = pm.PurchaseVoucherVoucherNo,
                               PurchaseVoucherPurchaseDate = pm.PurchaseVoucherPurchaseDate,
                               PurchaseVoucherPurchaseType = pm.PurchaseVoucherPurchaseType,
                               PurchaseVoucherCashSupplierName = pm.PurchaseVoucherCashSupplierName,
                               PurchaseVoucherSupplyInvoiceNo = pm.PurchaseVoucherSupplyInvoiceNo,
                               PurchaseVoucherActualAmount = pm.PurchaseVoucherActualAmount,
                               PurchaseVoucherDisAmount = pm.PurchaseVoucherDisAmount,
                               PurchaseVoucherVatAmount = pm.PurchaseVoucherVATAmount,
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
