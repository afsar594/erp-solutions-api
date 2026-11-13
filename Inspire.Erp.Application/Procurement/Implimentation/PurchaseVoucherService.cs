using Inspire.Erp.Application.Common;
using Inspire.Erp.Application.Master.Interfaces;
using Inspire.Erp.Application.MODULE;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Modals.Procurement;
using Inspire.Erp.Infrastructure.Database;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
//using Microsoft.Data.SqlClient;

namespace Inspire.Erp.Application.Master.Implementations
{
    public class PurchaseVoucherService : IPurchaseVoucherService
    {
        private IRepository<StockRegister> _stockRegisterRepository;
        private IRepository<CurrencyMaster> _currencyMasterRepository;
        private IRepository<PurchaseVoucher> _PurchaseVoucher;
        private IRepository<PurchaseVoucherDetails> _PurchaseVoucherDetails;
        private IRepository<AccountsTransactions> _accountTransactionRepository;
        private IRepository<ProgramSettings> _programsettingsRepository;
        private IRepository<JobMaster> _JobMaster;
        private IRepository<Suppliers> _Suppliers;
        private IRepository<LocationMaster> _LocationMaster;
        private IRepository<ItemMaster> _ItemMaster;
        private IRepository<UnitMaster> _UnitMaster;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;
        private IRepository<PurchaseCharges> _purchaseChargesRepository;
        private InspireErpDBContext _context;
        private readonly IConfiguration _configuration;
        public PurchaseVoucherService(IRepository<PurchaseVoucher> PurchaseVoucher, IRepository<PurchaseVoucherDetails> PurchaseVoucherDetails,
        IRepository<JobMaster> JobMaster, IRepository<Suppliers> Suppliers, IRepository<LocationMaster> LocationMaster,
        IRepository<ItemMaster> ItemMaster, IRepository<UnitMaster> UnitMaster, InspireErpDBContext context, IRepository<VouchersNumbers> voucherNumbersRepository,
        IRepository<ProgramSettings> programsettingsRepository, IRepository<AccountsTransactions> accountTransactionRepository, IRepository<StockRegister> stockRegisterRepository, IRepository<CurrencyMaster> currencyMasterRepository, IRepository<PurchaseCharges> purchaseChargesRepository, IConfiguration configuration)
        {
            _PurchaseVoucher = PurchaseVoucher;
            _PurchaseVoucherDetails = PurchaseVoucherDetails;
            _JobMaster = JobMaster;
            _Suppliers = Suppliers;
            _LocationMaster = LocationMaster;
            _ItemMaster = ItemMaster;
            _UnitMaster = UnitMaster;
            _context = context;
            _voucherNumbersRepository = voucherNumbersRepository;
            _programsettingsRepository = programsettingsRepository;
            _accountTransactionRepository = accountTransactionRepository;
            _stockRegisterRepository = stockRegisterRepository;
            _currencyMasterRepository = currencyMasterRepository;
            _purchaseChargesRepository = purchaseChargesRepository;
            _configuration = configuration;
        }
        public IEnumerable<PurchaseVoucher> GetPurchaseVoucher()
        {

            IEnumerable<PurchaseVoucher> purchaseVouchers = _PurchaseVoucher.GetAll().Where(k => k.PurchaseVoucherDelStatus == false || k.PurchaseVoucherDelStatus == null);
            return purchaseVouchers;
        }
        public async Task<Response<GridWrapperResponse<List<PurchaseVoucher>>>> GetPurchaseVoucherList(GenericGridViewModel model)
        {

            try
            {
                var baseQuery = (from s in _PurchaseVoucher.GetAsQueryable()
                                     //join c in _PurchaseVoucher.GetAsQueryable() on s.PurchaseVoucherPartyID equals c.PurchaseVoucherSupplyInvoiceNo
                                     //join cr in _currencyMasterRepository.GetAsQueryable() on s.SalesVoucherCurrencyId equals cr.CurrencyMasterCurrencyId
                                 where s.PurchaseVoucherDelStatus == null || s.PurchaseVoucherDelStatus == false
                                 select s);

                // Date range
                if (model.FromDate.HasValue)
                    baseQuery = baseQuery.Where(x => x.PurchaseVoucherPurchaseDate >= model.FromDate.Value);

                if (model.ToDate.HasValue)
                    baseQuery = baseQuery.Where(x => x.PurchaseVoucherPurchaseDate <= model.ToDate.Value);

                // Filtering
                if (!string.IsNullOrEmpty(model.Filter))
                    baseQuery = baseQuery.Where(model.Filter);

                // Search
                if (!string.IsNullOrEmpty(model.Search))
                {
                    baseQuery = baseQuery.Where(x =>
                        x.PurchaseVoucherVoucherNo.Contains(model.Search) ||
                        x.PurchaseVoucherPartyName.Contains(model.Search) ||
                        x.PurchaseVoucherPurchaseRef.Contains(model.Search) ||
                        x.PurchaseVoucherPurchaseType.Contains(model.Search));
                }


                // Count AFTER filters
                var totalCount = await baseQuery.CountAsync();

                // Sorting
                IOrderedQueryable<PurchaseVoucher> orderedQuery;
                if (!string.IsNullOrEmpty(model.Field))
                {
                    string sortDir = model.Dir?.ToLower() == "desc" ? "descending" : "ascending";
                    orderedQuery = baseQuery.OrderBy($"{model.Field} {sortDir}");
                }
                else
                {
                    orderedQuery = baseQuery.OrderByDescending(x => x.PurchaseVoucherPurchaseDate);
                }


                // Pagination
                var result = await orderedQuery
                    .Skip(model.Skip)
                    .Take(model.Take)
                    .ToListAsync();
                var gridResponse = new GridWrapperResponse<List<PurchaseVoucher>>();
                gridResponse.Data = result;
                gridResponse.Total = totalCount;

                return Response<GridWrapperResponse<List<PurchaseVoucher>>>.Success(gridResponse, "Data retrieved successfully");

                //return Response<List<SalesVoucher>>.Success(result, $"Data retrieved successfully. Total records: {totalCount}");
            }
            catch (Exception ex)
            {
                var result = new List<PurchaseVoucher>();
                var gridResponse = new GridWrapperResponse<List<PurchaseVoucher>>();
                gridResponse.Data = result;
                gridResponse.Total = 0;
                return Response<GridWrapperResponse<List<PurchaseVoucher>>>.Fail(gridResponse, ex.Message);
            }

        }

        public IEnumerable<PurchaseVoucherDetails> GetPurchaseVoucherDetails()
        {
            try
            {
                IEnumerable<PurchaseVoucherDetails> _PurchaseVoucherList = _PurchaseVoucherDetails.GetAll().Where(a => a.PurchaseVoucherDetailsDelStatus != true).ToList();
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
                var grnList = _context.PurchaseVoucher.Where(o => o.PurchaseVoucherDelStatus != true && o.PurchaseVoucherVoucherNo.Contains("GRN")).AsQueryable();
                return grnList;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public PurchaseVoucherModel InsertPurchaseVoucher(PurchaseVoucher purchaseVoucher, List<AccountsTransactions> accountsTransactions,
              List<PurchaseVoucherDetails> purchaseVoucherDetails
              , List<StockRegister> stockRegister
              )
        {
            try
            {
                _PurchaseVoucher.BeginTransaction();
                // string puchaseVoucherNumber = this.GenerateVoucherNo(purchaseVoucher.PurchaseVoucherPurchaseDate.Date).VouchersNumbersVNo;
                string puchaseVoucherNumber = clsCommonFunctions.GenrateVoucherNo(VoucherType.PurchaseVoucher_TYPE, Prefix.PurchaseVoucher_Prefix, _programsettingsRepository, _voucherNumbersRepository);
                purchaseVoucher.PurchaseVoucherVoucherNo = puchaseVoucherNumber;


                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    _PurchaseVoucher.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.PurchaseVoucherPurID) + 1);

                purchaseVoucher.PurchaseVoucherPurID = maxcount;

                //purchaseVoucherDetails = purchaseVoucherDetails.Select((x) =>
                //{
                //    x.PurchaseVoucherDetailsPrId = maxcount;
                //    x.PurchaseVoucherDetailsVoucherNo = puchaseVoucherNumber;
                //    return x;
                //}).ToList();



                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    //k.AccountsTransactionsTransDate = salesVoucher.SalesVoucherDate;
                    k.AccountsTransactionsVoucherNo = puchaseVoucherNumber;
                    k.AccountsTransactionsVoucherType = VoucherType.PurchaseVoucher_TYPE;
                    k.AccountsTransactionsStatus = AccountStatus.Approved;
                    return k;
                }).OrderBy(x => x.AccountsTransactionsVoucherNo).AsEnumerable().ToList();
                _accountTransactionRepository.InsertList(accountsTransactions);

                decimal stockRegCount = 0;
                stockRegCount = Convert.ToInt32(
                    _stockRegisterRepository.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.StockRegisterStoreID) + 1);


                List<StockRegister> stockRegisters = new List<StockRegister>();

                var currencyRate = clsCommonFunctions.getConverionCurrencyRate(purchaseVoucher.PurchaseVoucherCurrencyID, _currencyMasterRepository);
                var rate = (decimal)currencyRate;

                foreach (var item in purchaseVoucherDetails)
                {
                    var converontype = this.getConverionTypebyUnitId(item.PurchaseVoucherDetailsMaterialId, item.PurchaseVoucherDetailsUnitId);

                    item.PurchaseVoucherDetailsVoucherNo = puchaseVoucherNumber;
                    item.PurchaseVoucherDetailsPrId = maxcount;
                    stockRegisters.Add(new StockRegister
                    {
                        StockRegisterMaterialID = item.PurchaseVoucherDetailsMaterialId,
                        StockRegisterRefVoucherNo = puchaseVoucherNumber,
                        StockRegisterPurchaseID = puchaseVoucherNumber,
                        StockRegisterVoucherDate = purchaseVoucher.PurchaseVoucherPurchaseDate,
                        StockRegisterQuantity = item.PurchaseVoucherDetailsQuantity * (converontype) ?? 1,
                        StockRegisterSIN = item.PurchaseVoucherDetailsQuantity * (converontype) ?? 1,
                        StockRegisterRate = item.PurchaseVoucherDetailsRate / converontype ?? 1,
                        StockRegisterAmount = item.PurchaseVoucherDetailsQuantity * item.PurchaseVoucherDetailsRate,
                        StockRegisterFCAmount = item.PurchaseVoucherDetailsQuantity * item.PurchaseVoucherDetailsRate * rate,
                        StockRegisterTransType = VoucherType.PurchaseVoucher_TYPE,
                        StockRegisterStatus = AccountStatus.Approved,
                        StockRegisterUnitID = item.PurchaseVoucherDetailsUnitId,
                        StockRegisterLocationID = purchaseVoucher.PurchaseVoucherLocationID,
                        StockRegisterJobID = purchaseVoucher.PurchaseVoucherJobID,
                        StockRegisterDelStatus = false
                    });
                }
                _stockRegisterRepository.InsertList(stockRegisters);
                //stockRegister = stockRegister.Select((k) =>
                //{
                //    k.StockRegisterVoucherDate = purchaseVoucher.PurchaseVoucherPurchaseDate;
                //    k.StockRegisterRefVoucherNo = puchaseVoucherNumber;
                //    k.StockRegisterTransType = VoucherType.PurchaseVoucher_TYPE;
                //    k.StockRegisterStatus = AccountStatus.Approved;
                //    //k.StockRegisterSout= purchaseVoucher.
                //    return k;
                //}).ToList();
                //_stockRegisterRepository.InsertList(stockRegister);

                _PurchaseVoucher.Insert(purchaseVoucher);
                _PurchaseVoucherDetails.InsertList(purchaseVoucherDetails);
                _PurchaseVoucher.TransactionCommit();
                return this.GetSavedPurchaseVoucherDetails(purchaseVoucher.PurchaseVoucherVoucherNo);

            }
            catch (Exception ex)
            {
                _PurchaseVoucher.TransactionRollback();
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

        public PurchaseVoucherModel GetSavedPurchaseVoucherDetails(string pvno)
        {
            PurchaseVoucherModel voucherModel = new PurchaseVoucherModel();
            voucherModel.purchaseVoucher = _PurchaseVoucher.GetAsQueryable().Where(k => k.PurchaseVoucherVoucherNo == pvno).SingleOrDefault();

            if (voucherModel.purchaseVoucher != null)
            {
                voucherModel.accountsTransactions = _accountTransactionRepository.GetAsQueryable().Where(c => c.AccountsTransactionsVoucherNo == pvno && c.AccountsTransactionsVoucherType == VoucherType.PurchaseVoucher_TYPE && (c.AccountstransactionsDelStatus == false || c.AccountstransactionsDelStatus == null)).ToList();
                voucherModel.purchaseVoucherDetails = _PurchaseVoucherDetails.GetAsQueryable().Where(x => x.PurchaseVoucherDetailsVoucherNo == pvno && (x.PurchaseVoucherDetailsDelStatus == false || x.PurchaseVoucherDetailsDelStatus == null)).ToList();

                if (voucherModel.purchaseVoucherDetails.Count > 0)
                {
                    foreach (var item in voucherModel.purchaseVoucherDetails)
                    {
                        decimal sumReturnedQty = 0;
                        var stockQuery = _stockRegisterRepository.GetAsQueryable()
                        .Where(a => a.StockRegisterMaterialID == item.PurchaseVoucherDetailsMaterialId &&
                                    a.StockRegisterTransType == "PurchaseReturn" &&
                                    a.StockRegisterRefVoucherNo == item.PurchaseVoucherDetailsVoucherNo);

                        sumReturnedQty = stockQuery.Any()
                                           ? stockQuery.Sum(s => (decimal?)s.StockRegisterSout) ?? 0
                                           : 0;

                        if (sumReturnedQty != null)
                        {
                            item.PurchaseReturnQtyTill = sumReturnedQty;
                        }

                    }
                }
            }
            return voucherModel;

            // throw new NotImplementedException();
        }

        public PurchaseVoucher GetSavedPurchaseVoucherDetailsV2(long pvno)
        {
            PurchaseVoucher voucherModel = new PurchaseVoucher();
            voucherModel = _PurchaseVoucher.GetAsQueryable().Where(k => k.PurchaseVoucherPurID == Convert.ToInt64(pvno)).SingleOrDefault();
            if (voucherModel != null)
            {


                voucherModel.AccountsTransactions = _accountTransactionRepository.GetAsQueryable().Where(c => c.AccountsTransactionsVoucherNo == voucherModel.PurchaseVoucherVoucherNo && c.AccountsTransactionsVoucherType == VoucherType.PurchaseVoucher_TYPE && (c.AccountstransactionsDelStatus == false || c.AccountstransactionsDelStatus == null)).ToList();
                voucherModel.PurchaseVoucherDetails = _PurchaseVoucherDetails.GetAsQueryable().Where(x => x.PurchaseVoucherDetailsVoucherNo == voucherModel.PurchaseVoucherVoucherNo && (x.PurchaseVoucherDetailsDelStatus == false || x.PurchaseVoucherDetailsDelStatus == null)).ToList();

                if (voucherModel.PurchaseVoucherDetails.Count > 0)
                {
                    foreach (var item in voucherModel.PurchaseVoucherDetails)
                    {
                        decimal sumReturnedQty = 0;
                        var stockQuery = _stockRegisterRepository.GetAsQueryable()
                        .Where(a => a.StockRegisterMaterialID == item.PurchaseVoucherDetailsMaterialId &&
                                    a.StockRegisterTransType == "PurchaseReturn" &&
                                    a.StockRegisterRefVoucherNo == item.PurchaseVoucherDetailsVoucherNo);

                        sumReturnedQty = stockQuery.Any()
                                           ? stockQuery.Sum(s => (decimal?)s.StockRegisterSout) ?? 0
                                           : 0;

                        if (sumReturnedQty != null)
                        {
                            item.PurchaseReturnQtyTill = sumReturnedQty;
                        }

                    }
                }
            }
            return voucherModel;
        }
        public PurchaseVoucher GetSavedPurchaseVoucherDetailsByPurchaseVoucher(string pvno)
        {
            PurchaseVoucher voucherModel = new PurchaseVoucher();
            voucherModel = _PurchaseVoucher.GetAsQueryable().Where(k => k.PurchaseVoucherVoucherNo == pvno).SingleOrDefault();
            if (voucherModel != null)
            {


                voucherModel.AccountsTransactions = _accountTransactionRepository.GetAsQueryable().Where(c => c.AccountsTransactionsVoucherNo == voucherModel.PurchaseVoucherVoucherNo && c.AccountsTransactionsVoucherType == VoucherType.PurchaseVoucher_TYPE && (c.AccountstransactionsDelStatus == false || c.AccountstransactionsDelStatus == null)).ToList();
                voucherModel.PurchaseVoucherDetails = _PurchaseVoucherDetails.GetAsQueryable().Where(x => x.PurchaseVoucherDetailsVoucherNo == voucherModel.PurchaseVoucherVoucherNo && (x.PurchaseVoucherDetailsDelStatus == false || x.PurchaseVoucherDetailsDelStatus == null)).ToList();

                if (voucherModel.PurchaseVoucherDetails.Count > 0)
                {
                    foreach (var item in voucherModel.PurchaseVoucherDetails)
                    {
                        decimal sumReturnedQty = 0;
                        var stockQuery = _stockRegisterRepository.GetAsQueryable()
                        .Where(a => a.StockRegisterMaterialID == item.PurchaseVoucherDetailsMaterialId &&
                                    a.StockRegisterTransType == "PurchaseReturn" &&
                                    a.StockRegisterRefVoucherNo == item.PurchaseVoucherDetailsVoucherNo);

                        sumReturnedQty = stockQuery.Any()
                                           ? stockQuery.Sum(s => (decimal?)s.StockRegisterSout) ?? 0
                                           : 0;

                        if (sumReturnedQty != null)
                        {
                            item.PurchaseReturnQtyTill = sumReturnedQty;
                        }

                    }
                }
            }
            return voucherModel;
        }

        public VouchersNumbers GetVouchersNumbers(string pvno)
        {
            try
            {
                VouchersNumbers vouchersNumbers = _voucherNumbersRepository.GetAsQueryable().Where(k => k.VouchersNumbersVNo == pvno).SingleOrDefault();
                return vouchersNumbers;
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate)
        {
            try
            {

                var prefix = this._programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.PurchaseVoucher_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(_voucherNumbersRepository.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.PurchaseVoucher_TYPE)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;


                //var prefix = "CN";
                //int vnoMaxVal = 1;


                VouchersNumbers vouchersNumbers = new VouchersNumbers
                {
                    VouchersNumbersVNo = prefix + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.PurchaseVoucher_TYPE,
                    VouchersNumbersFsno = 1,
                    VouchersNumbersStatus = AccountStatus.Pending,
                    VouchersNumbersVoucherDate = VoucherGenDate

                };
                _voucherNumbersRepository.Insert(vouchersNumbers);
                return vouchersNumbers;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public PurchaseVoucherModel UpdatePurchaseVoucher(PurchaseVoucher purchaseVoucher, List<AccountsTransactions> accountsTransactions, List<PurchaseVoucherDetails> purchaseVoucherDetails, List<StockRegister> stockRegister)
        {
            try
            {

                _PurchaseVoucher.BeginTransaction();

                //=================================
                clsCommonFunctions.Delete_OldEntryOf_StockRegister(purchaseVoucher.PurchaseVoucherVoucherNo, VoucherType.PurchaseVoucher_TYPE, _stockRegisterRepository);
                clsCommonFunctions.Delete_OldEntry_AccountsTransactions(purchaseVoucher.PurchaseVoucherVoucherNo, purchaseVoucher.PurchaseVoucherPurchaseType, _accountTransactionRepository);
             
                _PurchaseVoucher.Update(purchaseVoucher);

                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    if (k.AccountsTransactionsTransSno == 0)
                    {
                        k.AccountsTransactionsTransDate = purchaseVoucher.PurchaseVoucherPurchaseDate;
                        k.AccountsTransactionsVoucherNo = purchaseVoucher.PurchaseVoucherVoucherNo;
                        k.AccountsTransactionsVoucherType = VoucherType.PurchaseVoucher_TYPE;
                        k.AccountsTransactionsStatus = AccountStatus.Approved;
                    }
                    return k;
                }).OrderBy(x => x.AccountsTransactionsVoucherNo).AsEnumerable().ToList();
                _accountTransactionRepository.InsertList(accountsTransactions);

                List<StockRegister> stockRegisters = new List<StockRegister>();

                var currencyRate = clsCommonFunctions.getConverionCurrencyRate(purchaseVoucher.PurchaseVoucherCurrencyID, _currencyMasterRepository);
                var rate = (decimal)currencyRate;

                foreach (var item in purchaseVoucherDetails)
                {
                    var converontype = this.getConverionTypebyUnitId(item.PurchaseVoucherDetailsMaterialId, item.PurchaseVoucherDetailsUnitId);

                    item.PurchaseVoucherDetailsVoucherNo = purchaseVoucher.PurchaseVoucherVoucherNo;
                    if (item.PurchaseVoucherDetailsPurcahseDetailsId != 0)
                    {
                        _PurchaseVoucherDetails.Update(item);

                    }
                    else
                    {
                        _PurchaseVoucherDetails.Insert(item);
                    }

                    stockRegisters.Add(new StockRegister
                    {
                        StockRegisterMaterialID = item.PurchaseVoucherDetailsMaterialId,
                        StockRegisterRefVoucherNo = purchaseVoucher.PurchaseVoucherVoucherNo,
                        StockRegisterPurchaseID = purchaseVoucher.PurchaseVoucherVoucherNo,
                        StockRegisterVoucherDate = purchaseVoucher.PurchaseVoucherPurchaseDate,
                        StockRegisterQuantity = item.PurchaseVoucherDetailsQuantity * (converontype) ?? 1,
                        StockRegisterSIN = item.PurchaseVoucherDetailsQuantity * (converontype) ?? 1,
                        StockRegisterRate = item.PurchaseVoucherDetailsRate / converontype ?? 1,
                        StockRegisterAmount = item.PurchaseVoucherDetailsQuantity * item.PurchaseVoucherDetailsRate,
                        StockRegisterFCAmount = item.PurchaseVoucherDetailsQuantity * item.PurchaseVoucherDetailsRate * rate,
                        StockRegisterTransType = VoucherType.PurchaseVoucher_TYPE,
                        StockRegisterStatus = AccountStatus.Approved,
                        StockRegisterUnitID = item.PurchaseVoucherDetailsUnitId,
                        StockRegisterLocationID = purchaseVoucher.PurchaseVoucherLocationID,
                        StockRegisterJobID = purchaseVoucher.PurchaseVoucherJobID,
                        StockRegisterDelStatus = false
                    });
                }
                _stockRegisterRepository.InsertList(stockRegisters);

                _PurchaseVoucher.TransactionCommit();

            }
            catch (Exception ex)
            {
                _PurchaseVoucher.TransactionRollback();
                throw ex;
            }

            return this.GetSavedPurchaseVoucherDetails(purchaseVoucher.PurchaseVoucherVoucherNo);
        }

        public int DeletePurchaseVoucher(PurchaseVoucher purchaseVoucher, List<AccountsTransactions> accountsTransactions, List<PurchaseVoucherDetails> purchaseVoucherDetails, List<StockRegister> stockRegister)
        {
            int i = 0;
            try
            {
                _PurchaseVoucher.BeginTransaction();

                //=================================
                clsCommonFunctions.Delete_OldEntry_StockRegister(purchaseVoucher.PurchaseVoucherVoucherNo, VoucherType.PurchaseVoucher_TYPE, _stockRegisterRepository);
                clsCommonFunctions.Delete_OldEntry_AccountsTransactions(purchaseVoucher.PurchaseVoucherVoucherNo, VoucherType.PurchaseVoucher_TYPE, _accountTransactionRepository);
                //clsCommonFunctions.Delete_OldEntry_vouchernumber(purchaseVoucher.PurchaseVoucherVoucherNo, VoucherType.PurchaseVoucher_TYPE, _voucherNumbersRepository);

                //=================================


                purchaseVoucher.PurchaseVoucherDelStatus = true;

                purchaseVoucherDetails = purchaseVoucherDetails.Select((k) =>
                {
                    k.PurchaseVoucherDetailsDelStatus = true;
                    return k;
                }).OrderBy(x => x.PurchaseVoucherDetailsItemName).AsEnumerable().ToList();

                //_salesVoucherDetailsRepository.UpdateList(salesVoucherDetails);

                accountsTransactions = accountsTransactions.Select((k) => { k.AccountstransactionsDelStatus = true; return k; }).OrderBy(x => x.AccountsTransactionsVoucherNo).ToList();
                _accountTransactionRepository.UpdateList(accountsTransactions);

                stockRegister = stockRegister.Select((k) =>
                {
                    k.StockRegisterDelStatus = true;
                    return k;
                }).OrderBy(x => x.StockRegisterRefVoucherNo).AsEnumerable().ToList();
                _stockRegisterRepository.UpdateList(stockRegister);




                purchaseVoucher.PurchaseVoucherDetails = purchaseVoucherDetails;

                _PurchaseVoucher.Update(purchaseVoucher);

                var vchrnumer = _voucherNumbersRepository.GetAsQueryable().Where(k => k.VouchersNumbersVNo == purchaseVoucher.PurchaseVoucherVoucherNo).FirstOrDefault();

                _voucherNumbersRepository.Update(vchrnumer);

                _PurchaseVoucher.TransactionCommit();
                i = 1;
            }
            catch (Exception ex)
            {
                _PurchaseVoucher.TransactionRollback();
                i = 0;
                throw ex;
            }

            return i;
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
                                   select new GetPurchaseVoucherResponse
                                   {
                                       PurchaseVoucherVoucherNo = pm.PurchaseVoucherVoucherNo,
                                       PurchaseVoucherPurchaseDate = pm.PurchaseVoucherPurchaseDate,
                                       PurchaseVoucherPurchaseType = pm.PurchaseVoucherPurchaseType,
                                       PurchaseVoucherCashSupplierName = pm.PurchaseVoucherCashSupplierName,
                                       PurchaseVoucherSupplyInvoiceNo = pm.PurchaseVoucherSupplyInvoiceNo,
                                       PurchaseVoucherActualAmount = pm.PurchaseVoucherActualAmount,
                                       PurchaseVoucherDisAmount = pm.PurchaseVoucherDisAmount,
                                       PurchaseVoucherVatAmount = pm.PurchaseVoucherVATAmount,
                                       PurchaseVoucherNetAmount = pm.PurchaseVoucherNetAmount
                                   }).OrderBy(x => x.PurchaseVoucherVoucherNo).AsEnumerable().ToList();

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

        private decimal getConverionTypebyUnitId(int? itemid, int? unitDetailsid)
        {
            try
            {
                return (decimal)_context.UnitDetails.FirstOrDefault(x => x.UnitDetailsItemId == itemid && x.UnitDetailsUnitId == unitDetailsid).UnitDetailsConversionType;
            }
            catch
            {
                return 1;
            }
        }

        public PurchaseVoucher InsertPurchaseVoucher(PurchaseVoucher purchaseVoucher, List<AccountsTransactions> accountsTransactions, List<PurchaseVoucherDetails> purchaseVoucherDetails, List<PurchaseCharges> purchaseChargesList)
        {
            try
            {

                _accountTransactionRepository.BeginTransaction();

                //string vnumber = this.GenerateVoucherNo(purchaseVoucher.PurchaseVoucherPurchaseDate).VouchersNumbersVNo;
                string vnumber = clsCommonFunctions.GenrateVoucherNo(VoucherType.PurchaseVoucher_TYPE, Prefix.PurchaseVoucher_Prefix, _programsettingsRepository, _voucherNumbersRepository);
                purchaseVoucher.PurchaseVoucherVoucherNo = vnumber;

                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    _PurchaseVoucher.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.PurchaseVoucherPurID) + 1);
                if (purchaseChargesList != null && purchaseChargesList.Any())
                {
                    purchaseChargesList.ForEach(x => x.Voucherno = purchaseVoucher.PurchaseVoucherVoucherNo);

                    _purchaseChargesRepository.InsertListAsync(purchaseChargesList);

                    decimal totalServiceCharges = purchaseChargesList.Sum(x => x.TaxIncAmount);
                    purchaseVoucher.ServiceCharges = totalServiceCharges;
                }

                _PurchaseVoucher.Insert(purchaseVoucher);
                purchaseVoucherDetails = purchaseVoucherDetails.Select((x) =>
                {
                    x.PurchaseVoucherDetailsPrId = maxcount;
                    x.PurchaseVoucherDetailsVoucherNo = vnumber;
                    return x;
                }).OrderBy(x => x.PurchaseVoucherDetailsVoucherNo).AsEnumerable().ToList();
                _PurchaseVoucherDetails.InsertList(purchaseVoucherDetails);

                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    k.AccountsTransactionsAccNo = k.AccountsTransactionsAccNo ?? "";
                    k.AccountsTransactionsVoucherNo = vnumber;
                    k.AccountsTransactionsVoucherType = VoucherType.PurchaseVoucher_TYPE;
                    k.AccountsTransactionsStatus = AccountStatus.Approved;
                    return k;
                }).OrderBy(x => x.AccountsTransactionsVoucherNo).AsEnumerable().ToList();
                _accountTransactionRepository.InsertList(accountsTransactions);
                List<StockRegister> stockRegisters = new List<StockRegister>();

                var currencyRate = clsCommonFunctions.getConverionCurrencyRate(purchaseVoucher.PurchaseVoucherCurrencyID, _currencyMasterRepository);
                var rate = (decimal)currencyRate;

                foreach (var item in purchaseVoucherDetails)
                {
                    var converontype = this.getConverionTypebyUnitId(item.PurchaseVoucherDetailsMaterialId, item.PurchaseVoucherDetailsUnitId);
                    stockRegisters.Add(new StockRegister
                    {
                        StockRegisterMaterialID = item.PurchaseVoucherDetailsMaterialId,
                        StockRegisterRefVoucherNo = purchaseVoucher.PurchaseVoucherVoucherNo,
                        StockRegisterPurchaseID = purchaseVoucher.PurchaseVoucherVoucherNo,
                        StockRegisterVoucherDate = purchaseVoucher.PurchaseVoucherPurchaseDate,
                        StockRegisterQuantity = item.PurchaseVoucherDetailsQuantity * (converontype) ?? 1,
                        StockRegisterSIN = item.PurchaseVoucherDetailsQuantity * (converontype) ?? 1,
                        StockRegisterRate = item.PurchaseVoucherDetailsRate / converontype ?? 1,
                        StockRegisterAmount = item.PurchaseVoucherDetailsQuantity * item.PurchaseVoucherDetailsRate,
                        StockRegisterFCAmount = item.PurchaseVoucherDetailsQuantity * item.PurchaseVoucherDetailsRate * rate,
                        StockRegisterTransType = VoucherType.PurchaseVoucher_TYPE,
                        StockRegisterStatus = AccountStatus.Approved,
                        StockRegisterUnitID = item.PurchaseVoucherDetailsUnitId,
                        StockRegisterLocationID = purchaseVoucher.PurchaseVoucherLocationID,
                        StockRegisterJobID = purchaseVoucher.PurchaseVoucherJobID,
                        StockRegisterDelStatus = false
                    });
                }
                _stockRegisterRepository.InsertList(stockRegisters);

                _accountTransactionRepository.TransactionCommit();

                return this.GetSavedPurchaseVoucherDetailsV2(purchaseVoucher.PurchaseVoucherPurID);
            }
            catch (Exception ex)
            {
                _accountTransactionRepository.TransactionRollback();
                throw;
            }
        }

        public PurchaseVoucher UpdatePurchaseVoucher(PurchaseVoucher purchaseVoucher, List<AccountsTransactions> accountsTransactions, List<PurchaseVoucherDetails> purchaseVoucherDetails, List<PurchaseCharges> purchaseChargesList)
        {
            try
            {

                _accountTransactionRepository.BeginTransaction();
                clsCommonFunctions.Delete_OldEntryOf_StockRegister(purchaseVoucher.PurchaseVoucherVoucherNo, VoucherType.PurchaseVoucher_TYPE, _stockRegisterRepository);
                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    if (k.AccountsTransactionsTransSno == 0)
                    {
                        k.AccountsTransactionsTransDate = purchaseVoucher.PurchaseVoucherPurchaseDate;
                        k.AccountsTransactionsVoucherNo = purchaseVoucher.PurchaseVoucherVoucherNo;
                        k.AccountsTransactionsVoucherType = VoucherType.PurchaseVoucher_TYPE;
                        k.AccountsTransactionsStatus = AccountStatus.Approved;
                    }
                    return k;
                }).OrderBy(x => x.AccountsTransactionsVoucherNo).AsEnumerable().ToList();
                _accountTransactionRepository.UpdateList(accountsTransactions);

                //delete old record
                var existingPurchaseList = _PurchaseVoucherDetails.GetAll().Where(o => o.PurchaseVoucherDetailsVoucherNo == purchaseVoucher.PurchaseVoucherVoucherNo).ToList();
                _PurchaseVoucherDetails.DeleteList(existingPurchaseList);

                foreach (var xx in purchaseVoucher.PurchaseVoucherDetails)
                {
                    xx.PurchaseVoucherDetailsVoucherNo = purchaseVoucher.PurchaseVoucherVoucherNo;
                };
                _PurchaseVoucherDetails.InsertList(purchaseVoucherDetails);
                if (purchaseChargesList != null && purchaseChargesList.Any())
                {
                    var existingCharges = _purchaseChargesRepository.GetAsQueryable()
                        .Where(i => i.Voucherno == purchaseVoucher.PurchaseVoucherVoucherNo).ToList();
                    _purchaseChargesRepository.DeleteList(existingCharges);

                    purchaseChargesList.ForEach(x => x.Voucherno = purchaseVoucher.PurchaseVoucherVoucherNo);

                     _purchaseChargesRepository.InsertListAsync(purchaseChargesList);

                    decimal totalServiceCharges = purchaseChargesList.Sum(x => x.TaxIncAmount);
                    purchaseVoucher.ServiceCharges = totalServiceCharges;
                }
                _PurchaseVoucher.Update(purchaseVoucher);

                List<StockRegister> stockRegisters = new List<StockRegister>();

                var currencyRate = clsCommonFunctions.getConverionCurrencyRate(purchaseVoucher.PurchaseVoucherCurrencyID, _currencyMasterRepository);
                var rate = (decimal)currencyRate;

                foreach (var item in purchaseVoucherDetails)
                {
                    var converontype = this.getConverionTypebyUnitId(item.PurchaseVoucherDetailsMaterialId, item.PurchaseVoucherDetailsUnitId);
                    stockRegisters.Add(new StockRegister
                    {
                        StockRegisterMaterialID = item.PurchaseVoucherDetailsMaterialId,
                        StockRegisterRefVoucherNo = purchaseVoucher.PurchaseVoucherVoucherNo,
                        StockRegisterPurchaseID = purchaseVoucher.PurchaseVoucherVoucherNo,
                        StockRegisterVoucherDate = purchaseVoucher.PurchaseVoucherPurchaseDate,
                        StockRegisterQuantity = item.PurchaseVoucherDetailsQuantity * (converontype) ?? 1,
                        StockRegisterSIN = item.PurchaseVoucherDetailsQuantity * (converontype) ?? 1,
                        StockRegisterRate = item.PurchaseVoucherDetailsRate / converontype ?? 1,
                        StockRegisterAmount = item.PurchaseVoucherDetailsQuantity * item.PurchaseVoucherDetailsRate,
                        StockRegisterFCAmount = item.PurchaseVoucherDetailsQuantity * item.PurchaseVoucherDetailsRate * rate,
                        StockRegisterTransType = VoucherType.PurchaseVoucher_TYPE,
                        StockRegisterStatus = AccountStatus.Approved,
                        StockRegisterUnitID = item.PurchaseVoucherDetailsUnitId,
                        StockRegisterLocationID = purchaseVoucher.PurchaseVoucherLocationID,
                        StockRegisterJobID = purchaseVoucher.PurchaseVoucherJobID,
                        StockRegisterDelStatus = false
                    });
                }
                _stockRegisterRepository.InsertList(stockRegisters);


                _accountTransactionRepository.TransactionCommit();
                return this.GetSavedPurchaseVoucherDetailsV2(purchaseVoucher.PurchaseVoucherPurID);

            }
            catch (Exception ex)
            {
                _accountTransactionRepository.TransactionRollback();
                throw;
            }
        }
    }
}
