using Inspire.Erp.Application.Master.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Inspire.Erp.Infrastructure.Database;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Application.MODULE;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Modals.Procurement;
using Microsoft.AspNetCore.Server.IIS.Core;
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
        private InspireErpDBContext _context;
        private readonly IConfiguration _configuration;
        public PurchaseVoucherService(IRepository<PurchaseVoucher> PurchaseVoucher, IRepository<PurchaseVoucherDetails> PurchaseVoucherDetails,
        IRepository<JobMaster> JobMaster, IRepository<Suppliers> Suppliers, IRepository<LocationMaster> LocationMaster,
        IRepository<ItemMaster> ItemMaster, IRepository<UnitMaster> UnitMaster, InspireErpDBContext context, IRepository<VouchersNumbers> voucherNumbersRepository, IRepository<ProgramSettings> programsettingsRepository, IRepository<AccountsTransactions> accountTransactionRepository, IRepository<StockRegister> stockRegisterRepository, IRepository<CurrencyMaster> currencyMasterRepository, IConfiguration configuration)
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
            _configuration = configuration;
        }
        public IEnumerable<PurchaseVoucher> GetPurchaseVoucher()
        {
            //try
            //{
            //    IEnumerable<PurchaseVoucher> _PurchaseVoucherList = _PurchaseVoucher.GetAll().Where(c => c.PurchaseVoucherDelStatus == false || c.PurchaseVoucherDelStatus == null).ToList();
            //    IEnumerable<CurrencyMaster> _currencyMaster = _currencyMasterRepository.GetAll().ToList();


            //    foreach (var purchaseVoucher in _PurchaseVoucherList)
            //    {
            //        purchaseVoucher.CurrencyMasterCurrencyName = _currencyMaster
            //                                                     .Where(c => c.CurrencyMasterCurrencyId == purchaseVoucher.PurchaseVoucherCurrencyId)
            //                                                     .FirstOrDefault().CurrencyMasterCurrencyName;
            //    }
            //    return _PurchaseVoucherList;
            //}
            //catch (System.Exception ex)
            //{
            //    throw ex;
            //}

            IEnumerable<PurchaseVoucher> purchaseVouchers = _PurchaseVoucher.GetAll().Where(k => k.PurchaseVoucherDelStatus == false || k.PurchaseVoucherDelStatus == null);
            return purchaseVouchers;

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

        public PurchaseVoucherModel InsertPurchaseVoucher(PurchaseVoucher purchaseVoucher, List<AccountsTransactions> accountsTransactions,
              List<PurchaseVoucherDetails> purchaseVoucherDetails
              , List<StockRegister> stockRegister
              )
        {
            try
            {
                _PurchaseVoucher.BeginTransaction();
                string puchaseVoucherNumber = this.GenerateVoucherNo(purchaseVoucher.PurchaseVoucherPurchaseDate.Date).VouchersNumbersVNo;
                purchaseVoucher.PurchaseVoucherVoucherNo = puchaseVoucherNumber;


                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    _PurchaseVoucher.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.PurchaseVoucherPurID) + 1);

               // purchaseVoucher.PurchaseVoucherPurID = maxcount;

                purchaseVoucherDetails = purchaseVoucherDetails.Select((x) =>
                {
                    x.PurchaseVoucherDetailsPrId = maxcount;
                    x.PurchaseVoucherDetailsVoucherNo = puchaseVoucherNumber;
                    return x;
                }).ToList();
                
              

                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    //k.AccountsTransactionsTransDate = salesVoucher.SalesVoucherDate;
                    k.AccountsTransactionsVoucherNo = puchaseVoucherNumber;
                    k.AccountsTransactionsVoucherType = VoucherType.PurchaseVoucher_TYPE;
                    k.AccountsTransactionsStatus = AccountStatus.Approved;
                    return k;
                }).ToList();
                _accountTransactionRepository.InsertList(accountsTransactions);

                decimal stockRegCount = 0;
                stockRegCount = Convert.ToInt32(
                    _stockRegisterRepository.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.StockRegisterStoreID) + 1);

                

                stockRegister = stockRegister.Select((k) =>
                {
                    k.StockRegisterVoucherDate = purchaseVoucher.PurchaseVoucherPurchaseDate;
                    k.StockRegisterRefVoucherNo = puchaseVoucherNumber;
                    k.StockRegisterTransType = VoucherType.PurchaseVoucher_TYPE;
                    k.StockRegisterStatus = AccountStatus.Approved;
                    return k;
                }).ToList();
                _stockRegisterRepository.InsertList(stockRegister);





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

        //public IEnumerable<PurchaseVoucher> UpdatePurchaseVoucher(PurchaseVoucher DataObj)
        //{
        //    try
        //    {
        //        _PurchaseVoucher.Update(DataObj);
        //        return this.GetPurchaseVoucher();
        //    }
        //    catch (System.Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public IEnumerable<PurchaseVoucher> DeletePurchaseVoucher(PurchaseVoucher DataObj)
        //{
        //    try
        //    {
        //        _PurchaseVoucher.Delete(DataObj);
        //        return this.GetPurchaseVoucher();
        //    }
        //    catch (System.Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

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


            voucherModel.accountsTransactions = _accountTransactionRepository.GetAsQueryable().Where(c => c.AccountsTransactionsVoucherNo == pvno && c.AccountsTransactionsVoucherType == VoucherType.PurchaseVoucher_TYPE && (c.AccountstransactionsDelStatus == false || c.AccountstransactionsDelStatus == null)).ToList();
            voucherModel.purchaseVoucherDetails = _PurchaseVoucherDetails.GetAsQueryable().Where(x => x.PurchaseVoucherDetailsVoucherNo == pvno && (x.PurchaseVoucherDetailsDelStatus == false || x.PurchaseVoucherDetailsDelStatus == null)).ToList();
            return voucherModel;

           // throw new NotImplementedException();
        }

        public PurchaseVoucher GetSavedPurchaseVoucherDetailsV2(string pvno)
        {
            PurchaseVoucher voucherModel = new PurchaseVoucher();
            voucherModel = _PurchaseVoucher.GetAsQueryable().Where(k => k.PurchaseVoucherVoucherNo == pvno).SingleOrDefault();
            voucherModel.AccountsTransactions = _accountTransactionRepository.GetAsQueryable().Where(c => c.AccountsTransactionsVoucherNo == pvno && c.AccountsTransactionsVoucherType == VoucherType.PurchaseVoucher_TYPE && (c.AccountstransactionsDelStatus == false || c.AccountstransactionsDelStatus == null)).ToList();
            voucherModel.PurchaseVoucherDetails = _PurchaseVoucherDetails.GetAsQueryable().Where(x => x.PurchaseVoucherDetailsVoucherNo == pvno && (x.PurchaseVoucherDetailsDelStatus == false || x.PurchaseVoucherDetailsDelStatus == null)).ToList();
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
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.SalesVoucher_TYPE)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;


                //var prefix = "CN";
                //int vnoMaxVal = 1;


                VouchersNumbers vouchersNumbers = new VouchersNumbers
                {
                    VouchersNumbersVNo = prefix + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.SalesVoucher_TYPE,
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
                
                 string connstring = _configuration["ApplicationSettings:ConnectionString"];

               
              
                _PurchaseVoucher.BeginTransaction();

                //=================================
                clsCommonFunctions.Delete_OldEntry_StockRegister(purchaseVoucher.PurchaseVoucherVoucherNo, VoucherType.PurchaseVoucher_TYPE, _stockRegisterRepository);
                clsCommonFunctions.Delete_OldEntry_AccountsTransactions(purchaseVoucher.PurchaseVoucherVoucherNo, VoucherType.PurchaseVoucher_TYPE, _accountTransactionRepository);
                //=================================
                purchaseVoucher.PurchaseVoucherDetails = purchaseVoucherDetails.Select((k) =>
                {
                    //if (k.SalesVoucherDetailsId == 0)
                    //{
                    k.PurchaseVoucherDetailsPurcahseDetailsId = _PurchaseVoucherDetails.GetAsQueryable().Where(c => c.PurchaseVoucherDetailsVoucherNo == purchaseVoucher.PurchaseVoucherVoucherNo).FirstOrDefault().PurchaseVoucherDetailsPurcahseDetailsId; ;
                    k.PurchaseVoucherDetailsVoucherNo = purchaseVoucher.PurchaseVoucherVoucherNo;
                    //k.SalesVoucherDetailsId = 0;
                    //}

                    return k;
                }).ToList();
              
                foreach (var purchaseVoucherDetail in purchaseVoucherDetails)
                {
                    if (purchaseVoucherDetail != null && purchaseVoucher != null)
                    {
                        //purchaseVoucherDetail.PurchaseVoucherDetailsPurcahseDetailsId = _PurchaseVoucherDetails.GetAsQueryable().Where(c => c.PurchaseVoucherDetailsVoucherNo == purchaseVoucher.PurchaseVoucherVoucherNo).FirstOrDefault().PurchaseVoucherDetailsPurcahseDetailsId;

                        using (SqlConnection connection = new SqlConnection(connstring))
                        {
                            string sql = $@"Delete from [dbo].[Purchase_Voucher_Details] where [Purchase_Voucher_Details_VoucherNo] = '{purchaseVoucher.PurchaseVoucherVoucherNo}'";
                            using (SqlCommand command = new SqlCommand(sql, connection))
                            {
                                connection.Open();
                                command.ExecuteNonQuery();
                                connection.Close();
                            }
                        }
                    }
              
                     
                    
                   
                }
                
                //purchaseVoucher.PurchaseVoucherDetails = purchaseVoucherDetails;

                // _PurchaseVoucherDetails.DeleteList(purchaseVoucherDetails);

                purchaseVoucher.PurchaseVoucherDetails = purchaseVoucher.PurchaseVoucherDetails.Select((x) =>
                {
                    x.PurchaseVoucherDetailsPrId = Convert.ToInt32(purchaseVoucher.PurchaseVoucherPurID);
                    x.PurchaseVoucherDetailsVoucherNo = purchaseVoucher.PurchaseVoucherVoucherNo;
                    x.PurchaseVoucherDetailsPurcahseDetailsId = 0;
                    return x;
                }).ToList();
                //purchaseVoucher.PurchaseVoucherDetails = null;
                _PurchaseVoucher.Update(purchaseVoucher);
                _PurchaseVoucherDetails.InsertList(purchaseVoucher.PurchaseVoucherDetails.ToList());

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
                }).ToList();
                _accountTransactionRepository.UpdateList(accountsTransactions);


                stockRegister = stockRegister.Select((k) =>
                {
                    if (k.StockRegisterStoreID == 0)
                    {
                        k.StockRegisterVoucherDate = purchaseVoucher.PurchaseVoucherPurchaseDate;
                        k.StockRegisterRefVoucherNo = purchaseVoucher.PurchaseVoucherVoucherNo;
                        k.StockRegisterTransType = VoucherType.PurchaseVoucher_TYPE;

                        k.StockRegisterStatus = AccountStatus.Approved;
                    }

                    return k;
                }).ToList();
                _stockRegisterRepository.UpdateList(stockRegister);
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
                clsCommonFunctions.Delete_OldEntry_AccountsTransactions(purchaseVoucher.PurchaseVoucherVoucherNo, VoucherType.SalesVoucher_TYPE, _accountTransactionRepository);
                //=================================


                purchaseVoucher.PurchaseVoucherDelStatus = true;

                purchaseVoucherDetails = purchaseVoucherDetails.Select((k) =>
                {
                    k.PurchaseVoucherDetailsDelStatus = true;
                    return k;
                }).ToList();

                //_salesVoucherDetailsRepository.UpdateList(salesVoucherDetails);

                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    k.AccountstransactionsDelStatus = true;
                    return k;
                }).ToList();
                _accountTransactionRepository.UpdateList(accountsTransactions);

                stockRegister = stockRegister.Select((k) =>
                {
                    k.StockRegisterDelStatus = true;
                    return k;
                }).ToList();
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
        public PurchaseVoucher InsertPurchaseVoucher(PurchaseVoucher purchaseVoucher, List<AccountsTransactions> accountsTransactions, List<PurchaseVoucherDetails> purchaseVoucherDetails)
        {
            try
            {

                _accountTransactionRepository.BeginTransaction();
                
                string vnumber = this.GenerateVoucherNo(purchaseVoucher.PurchaseVoucherPurchaseDate).VouchersNumbersVNo;
                purchaseVoucher.PurchaseVoucherVoucherNo = vnumber;

                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    _PurchaseVoucher.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.PurchaseVoucherPurID) + 1);
                _PurchaseVoucher.Insert(purchaseVoucher);
                purchaseVoucherDetails = purchaseVoucherDetails.Select((x) =>
                {
                    x.PurchaseVoucherDetailsPrId = maxcount;
                    x.PurchaseVoucherDetailsVoucherNo = vnumber;
                    return x;
                }).ToList();

                _PurchaseVoucherDetails.InsertList(purchaseVoucherDetails);

                //accountsTransactions = accountsTransactions.Select((k) =>
                //{
                //    k.AccountsTransactionsVoucherNo = vnumber;
                //    k.AccountsTransactionsVoucherType = VoucherType.PurchaseVoucher_TYPE;
                //    k.AccountsTransactionsStatus = AccountStatus.Approved;
                //    return k;
                //}).ToList();
                //_accountTransactionRepository.InsertList(accountsTransactions);

           
                List<StockRegister> stockRegisters = new List<StockRegister>();
                foreach (var item in purchaseVoucherDetails)
                {
                    stockRegisters.Add(new StockRegister
                    {
                        StockRegisterMaterialID = item.PurchaseVoucherDetailsMaterialId,
                        StockRegisterRefVoucherNo = vnumber,
                        StockRegisterVoucherDate = purchaseVoucher.PurchaseVoucherPurchaseDate, 
                        StockRegisterQuantity  = item.PurchaseVoucherDetailsQuantity,
                        StockRegisterSIN = item.PurchaseVoucherDetailsQuantity,
                        StockRegisterRate = item.PurchaseVoucherDetailsRate,
                        StockRegisterAmount = item.PurchaseVoucherDetailsAmount,
                    });
                }
                _stockRegisterRepository.InsertList(stockRegisters);
                
                _accountTransactionRepository.Commit();

                return this.GetSavedPurchaseVoucherDetailsV2(purchaseVoucher.PurchaseVoucherVoucherNo);
            }
            catch (Exception ex)
            {
                _accountTransactionRepository.TransactionRollback();
                throw;
            }
        }

        public PurchaseVoucher UpdatePurchaseVoucher(PurchaseVoucher purchaseVoucher, List<AccountsTransactions> accountsTransactions, List<PurchaseVoucherDetails> purchaseVoucherDetails)
        {
            try
            {

                _accountTransactionRepository.BeginTransaction();

                //accountsTransactions = accountsTransactions.Select((k) =>
                //{
                //    if (k.AccountsTransactionsTransSno == 0)
                //    {
                //        k.AccountsTransactionsTransDate = purchaseVoucher.PurchaseVoucherPurchaseDate;
                //        k.AccountsTransactionsVoucherNo = purchaseVoucher.PurchaseVoucherVoucherNo;
                //        k.AccountsTransactionsVoucherType = VoucherType.PurchaseVoucher_TYPE;
                //        k.AccountsTransactionsStatus = AccountStatus.Approved;
                //    }

                //    return k;
                //}).ToList();
                //_accountTransactionRepository.UpdateList(accountsTransactions);

                _PurchaseVoucher.Update(purchaseVoucher);

                _PurchaseVoucherDetails.UpdateList(purchaseVoucherDetails);
             
                _accountTransactionRepository.Commit();
                return this.GetSavedPurchaseVoucherDetailsV2(purchaseVoucher.PurchaseVoucherVoucherNo);

            }
            catch (Exception ex)
            {
                _accountTransactionRepository.TransactionRollback();
                throw;
            }
        }
    }
}
