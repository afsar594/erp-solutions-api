using Inspire.Erp.Application.Store.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inspire.Erp.Domain.Modals.Stock;
using Inspire.Erp.Application.MODULE;

namespace Inspire.Erp.Application.Store.Implementation
{
    public class StockAdjustmentService : IStockAdjustmentService
    {
        private readonly IRepository<StockAdjustmentVoucher> _stockAdjustmentVoucherRepo;
        private readonly IRepository<StockAdjustmentVoucherDetails> _stockAdjustmentVoucherDetailsRepo;
        private IRepository<ProgramSettings> _programsettingsRepository;
        private readonly IRepository<VouchersNumbers> _VoucherNumberRepo;
        private IRepository<ItemMaster> _itemMaster;
        private readonly IRepository<JobMaster> _jobMaster;
        private readonly IRepository<StockRegister> _stockRegisterRepo;
        private readonly IRepository<CurrencyMaster> _currencyMasterRepo;
        private IRepository<UnitDetails> _UnitDetailsRepository;


        public StockAdjustmentService(IRepository<StockAdjustmentVoucher> stockAdjustmentVoucherRepo,
            IRepository<VouchersNumbers> voucherNumberRepo, IRepository<StockAdjustmentVoucherDetails> stockAdjustmentVoucherDetailsRepo, 
            IRepository<ProgramSettings> programsettingsRepository,
            IRepository<JobMaster> jobMaster,
            IRepository<ItemMaster> itemMasterRepository,
            IRepository<StockRegister> stockRegisterRepo,
            IRepository<CurrencyMaster> currencyMasterRepo,
             IRepository<UnitDetails> unitDetailsRepos




            )

        {
            _stockAdjustmentVoucherRepo = stockAdjustmentVoucherRepo;
            _VoucherNumberRepo = voucherNumberRepo;
            _stockAdjustmentVoucherDetailsRepo = stockAdjustmentVoucherDetailsRepo;
            _programsettingsRepository = programsettingsRepository;
            _jobMaster = jobMaster;
            _itemMaster = itemMasterRepository;
            _stockRegisterRepo = stockRegisterRepo;
            _currencyMasterRepo = currencyMasterRepo;
            _UnitDetailsRepository = unitDetailsRepos;
        }
        public IEnumerable<StockAdjustmentVoucher> GetStockAdjustment()
        {
            IEnumerable<StockAdjustmentVoucher> stockAdjusment = _stockAdjustmentVoucherRepo.GetAsQueryable().Where(k => k.StockAdjustmentVoucherDelStatus == false || k.StockAdjustmentVoucherDelStatus == null);
            return stockAdjusment;
        }

        public async Task<StockAdjustmentVoucher> InsertStockAdjustmentVoucher(StockAdjustmentVoucher stockAdjustmentVoucher, List<StockAdjustmentVoucherDetails> stockAdjustmentVoucherDetails)
        {
            try
            {
                _stockAdjustmentVoucherRepo.BeginTransaction();
                string stockVoucherNumber = clsCommonFunctions.GenrateVoucherNo(VoucherType.StockAdjustmentVoucher_TYPE, Prefix.StockAdjustment_Prefix, _programsettingsRepository, _VoucherNumberRepo);
                stockAdjustmentVoucher.StockAdjustmentVoucherSaNo = stockVoucherNumber;


                decimal SSIN = 0;
                decimal Sout = 0;
                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    await _stockAdjustmentVoucherRepo.GetAsQueryable()
                    .DefaultIfEmpty().MaxAsync(o => o == null ? 0 : o.StockAdjustmentVoucherSaId) + 1);

                stockAdjustmentVoucher.StockAdjustmentVoucherSaId = maxcount;

                
                stockAdjustmentVoucherDetails = stockAdjustmentVoucherDetails.Select((x) =>
                {
                    x.StockAdjustmentVoucherDetailsSaNo = stockVoucherNumber;
                    x.StockAdjustmentVoucherDetailsSaId = maxcount;
                    x.StockAdjustmentVoucherDetailsLocationId = stockAdjustmentVoucher.StockAdjustmentVoucherLocationId;
                    return x;
                }).OrderBy(x => x.StockAdjustmentVoucherDetailsSaNo).AsEnumerable().ToList();

                _stockAdjustmentVoucherRepo.Insert(stockAdjustmentVoucher);
                _stockAdjustmentVoucherDetailsRepo.InsertList(stockAdjustmentVoucherDetails);

                //stock register
                //

                var stockReg = stockAdjustmentVoucherDetails.Select(item =>
                {
                    var avgrate = item.StockAdjustmentVoucherDetailsCosePrice ?? 1;
                    var converontype = this.getConverionTypebyUnitId(item.StockAdjustmentVoucherDetailsMaterialId, item.StockAdjustmentVoucherDetailsUnitId);

                    // Calculate SSIN and Sout using ternary operators
                    var SSIN = item.StockAdjustmentVoucherDetailsAdjQty > 0
                        ? (decimal)item.StockAdjustmentVoucherDetailsAdjQty * (decimal)(converontype)
                        : 0;

                    var Sout = item.StockAdjustmentVoucherDetailsAdjQty <= 0
                        ? (decimal)item.StockAdjustmentVoucherDetailsAdjQty * (decimal)(converontype)
                        : 0;

                    return new StockRegister
                    {
                        StockRegisterPurchaseID = stockAdjustmentVoucher.StockAdjustmentVoucherSaNo,
                        StockRegisterRefVoucherNo = stockAdjustmentVoucher.StockAdjustmentVoucherSaNo,
                        StockRegisterVoucherDate = stockAdjustmentVoucher.StockAdjustmentVoucherSaDate,
                        StockRegisterMaterialID = item.StockAdjustmentVoucherDetailsMaterialId,
                        StockRegisterQuantity = Convert.ToDecimal(item.StockAdjustmentVoucherDetailsAdjQty??0) * (decimal)(converontype),
                        StockRegisterSIN = SSIN,
                        StockRegisterSout = Sout,
                        StockRegisterRate = Convert.ToDecimal(avgrate),
                        StockRegisterAmount = Convert.ToDecimal(item.StockAdjustmentVoucherDetailsAdjQty??0) * (decimal)(converontype),
                        StockRegisterFCAmount = Convert.ToDecimal(item.StockAdjustmentVoucherDetailsAdjQty??0) * (decimal)(converontype),
                        StockRegisterAssignedDate = DateTime.Now.Date,
                        StockRegisterStatus = "A",
                        StockRegisterTransType = VoucherType.StockAdjustmentVoucher_TYPE,
                        StockRegisterUnitID = item.StockAdjustmentVoucherDetailsUnitId,
                        StockRegisterLocationID = stockAdjustmentVoucher.StockAdjustmentVoucherLocationId,
                        StockRegisterDelStatus = false,
                        StockRegisterStoreID=0
                    };
                }).ToList();

                _stockRegisterRepo.InsertList(stockReg);

                
                //

                _stockAdjustmentVoucherRepo.TransactionCommit();
                return this.GetSavedStockAdjustmentferDetails((int)stockAdjustmentVoucher.StockAdjustmentVoucherSaId);

            }
            catch (Exception ex)
            {
                _stockAdjustmentVoucherRepo.TransactionRollback();
                throw;
            }
        }
        private decimal getConverionTypebyUnitId(int? itemid, int? unitDetailsid)
        {
            try
            {
                var convT = _UnitDetailsRepository.GetAsQueryable().FirstOrDefault(x => x.UnitDetailsItemId == itemid && x.UnitDetailsUnitId == unitDetailsid);
                if( convT == null )
                {
                    return 0;
                }
                return (decimal)convT.UnitDetailsConversionType;
            }
            catch
            {
                return 1;
            }
        }

        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate)
        {
            try
            {


                //var prefix = this._programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.StockTransfer_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(_VoucherNumberRepo.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.StockAdjustmentVoucher_TYPE)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;



                VouchersNumbers vouchersNumbers = new VouchersNumbers
                {
                    VouchersNumbersVNo = "SA" + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.StockAdjustmentVoucher_TYPE,
                    VouchersNumbersFsno = 1,
                    VouchersNumbersStatus = AccountStatus.Pending,
                    VouchersNumbersVoucherDate = VoucherGenDate

                };
                _VoucherNumberRepo.Insert(vouchersNumbers);
                return vouchersNumbers;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public StockAdjustmentVoucher GetSavedStockAdjustmentferDetails(int sano)
        {
            StockAdjustmentVoucher stockAdjustmentVoucher = new StockAdjustmentVoucher();
            stockAdjustmentVoucher = _stockAdjustmentVoucherRepo.GetAsQueryable().Where(k => k.StockAdjustmentVoucherSaId == sano).SingleOrDefault();
            stockAdjustmentVoucher.stockAdjustmentVoucherDetails = _stockAdjustmentVoucherDetailsRepo.GetAsQueryable().Where(x => x.StockAdjustmentVoucherDetailsSaNo == stockAdjustmentVoucher.StockAdjustmentVoucherSaNo).ToList();
            return stockAdjustmentVoucher;
        }

        public async Task<StockAdjustmentVoucher> UpdateStockAdjustmentVoucher(StockAdjustmentVoucher stockAdjustmentVoucher, List<StockAdjustmentVoucherDetails> stockAdjustmentVoucherDetails)
        {
            try
            {
                _stockAdjustmentVoucherRepo.BeginTransaction();
                _stockAdjustmentVoucherRepo.Update(stockAdjustmentVoucher);
                _stockAdjustmentVoucherDetailsRepo.UpdateList(stockAdjustmentVoucherDetails);

                _stockAdjustmentVoucherRepo.TransactionCommit();

                return this.GetSavedStockAdjustmentferDetails((int)stockAdjustmentVoucher.StockAdjustmentVoucherSaId);
            }
            catch (Exception ex)
            {
                _stockAdjustmentVoucherRepo.TransactionRollback();
                throw;
            }
        }

        public int DeleteStockAdjustmentVoucher(StockAdjustmentVoucher stockAdjustmentVoucher, List<StockAdjustmentVoucherDetails> stockAdjustmentVoucherDetails)
        {
            int i = 0;
            try
            {
                _stockAdjustmentVoucherRepo.BeginTransaction();
                stockAdjustmentVoucher.StockAdjustmentVoucherDelStatus = true;
                stockAdjustmentVoucherDetails = stockAdjustmentVoucherDetails.Select((k) =>
                {
                    k.StockAdjustmentVoucherDetailsDelStatus = true;
                    return k;
                }).OrderBy(x => x.StockAdjustmentVoucherDetailsSaNo).AsEnumerable().ToList();
            
          
                _stockAdjustmentVoucherRepo.Update(stockAdjustmentVoucher);
                _stockAdjustmentVoucherDetailsRepo.UpdateList(stockAdjustmentVoucherDetails);

                _stockAdjustmentVoucherRepo.TransactionCommit();

                i = 1;
            }
            catch (Exception ex)
            {
                _stockAdjustmentVoucherRepo.TransactionRollback();
                i = 0;
                throw ex;
            }
            return i;
        }

        public async Task<Response<List<StockAdjustmentReportResponse>>> StockAdjustmentReport(StockAdjustmentReportFilter model)
        {
            try
            {


                var assets = await (from sav in _stockAdjustmentVoucherRepo.GetAsQueryable()
                            
                             .Where(x => (model.FromDate == null || x.StockAdjustmentVoucherSaDate >= model.FromDate)
                                               && (model.ToDate == null || x.StockAdjustmentVoucherSaDate <= model.ToDate)
                                              )
                              join savd in _stockAdjustmentVoucherDetailsRepo.GetAsQueryable()
                              on sav.StockAdjustmentVoucherSaId equals savd.StockAdjustmentVoucherDetailsSaId into g
                              from savdg in g.DefaultIfEmpty()
                              join jb in _jobMaster.GetAsQueryable()
                              on savdg.StockAdjustmentVoucherDetailsJobId equals jb.JobMasterJobId into sbg
                              from sbgj in sbg.DefaultIfEmpty()
                              join im in _itemMaster.GetAsQueryable()
                              on (long)savdg.StockAdjustmentVoucherDetailsMaterialId equals im.ItemMasterItemId into img
                              from imsvg in img.DefaultIfEmpty()
                                  //group savdg by new
                                  //{
                                  //    savdg.StockAdjustmentVoucherDetailsJobId,

                                  //} into gp
                                    where (model.JobId == null || sbgj.JobMasterJobId==model.JobId)
                            //  where gp.Sum(x => x.AccountsTransactionsCredit) > 0 || gp.Sum(x => x.AccountsTransactionsDebit) > 0
                              select new StockAdjustmentReportResponse
                              {
                                  SADate =sav.StockAdjustmentVoucherSaDate,
                                  SANumber = sav.StockAdjustmentVoucherSaNo,
                                  JobName= sbgj.JobMasterJobName,
                                  ItemName=imsvg.ItemMasterItemName,
                                  ManualQuantity =(double)savdg.StockAdjustmentVoucherDetailsManualQty,
                                  CostPrice = (double)savdg.StockAdjustmentVoucherDetailsCosePrice,
                                  AdjustedQuantity = (double) savdg.StockAdjustmentVoucherDetailsAdjQty,
                                  AdjustedValue= (double)(savdg.StockAdjustmentVoucherDetailsCosePrice * savdg.StockAdjustmentVoucherDetailsAdjQty??0),
                                  ExistingQuantity= (double)(savdg.StockAdjustmentVoucherDetailsManualQty-savdg.StockAdjustmentVoucherDetailsAdjQty??0)
                              })

                     .OrderBy(t => t.JobName).ToListAsync();

                return Response<List<StockAdjustmentReportResponse>>.Success(assets, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<StockAdjustmentReportResponse>>.Fail(new List<StockAdjustmentReportResponse>(), ex.Message);
            }
        }


       public ResponseInfo GetStockByLocation(int id)
        {
            //var res = new ResponseInfo();
            //var data = _stockRegisterRepo.GetAsQueryable().AsNoTracking().Where(i=> i.StockRegisterLocationID == id && i.StockRegisterDelStatus != true &&i.StockRegisterUnitID != null&& i.StockRegisterUnitID != null).Select(s=> new
            //{
            //    s.StockRegisterMaterialID,
            //    s.StockRegisterUnitID,
            //    s.StockRegisterJobID,
            //    s.StockRegisterRate,
            //    s.StockRegisterQuantity,

            //}).ToList();
            //res.ResultSet = data;
            //res.IsSuccess = true;
            //return res;
            var res = new ResponseInfo();
            var data = _stockRegisterRepo.GetAsQueryable()
                .AsNoTracking()
                .Where(i => i.StockRegisterLocationID == id
                            && i.StockRegisterDelStatus != true
                            && i.StockRegisterUnitID != null)
                .AsEnumerable() // Switch to LINQ-to-Objects for client-side processing
                .GroupBy(s => s.StockRegisterMaterialID) // Group by StockRegisterMaterialID
                .Select(g => new
                {
                    StockRegisterMaterialID = g.Key, // Group key
                    StockRegisterQuantity = g.Sum(x => x.StockRegisterQuantity), // Aggregate quantity
                    StockRegisterRate = g.Average(x => x.StockRegisterRate), // Calculate average rate
                    StockRegisterUnitID = g.Select(x => x.StockRegisterUnitID).Distinct().ToList(), // List of distinct unit IDs
                    StockRegisterJobID = g.Select(x => x.StockRegisterJobID).Distinct().ToList() // List of distinct job IDs
                })
                .ToList();

            res.ResultSet = data;
            res.IsSuccess = true;
            return res;
        }
    }
}
