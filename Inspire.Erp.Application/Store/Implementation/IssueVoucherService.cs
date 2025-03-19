using Inspire.Erp.Application.Account.Implementations;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Application.Store.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Extensions.Logging;
using Inspire.Erp.Application.MODULE;
using Microsoft.AspNetCore.Mvc;
namespace Inspire.Erp.Application.Store.Implementation
{
    public class IssueVoucherService : IIssueVoucherService
    {
        private IRepository<StockRegister> _stockRegisterRepository;
        private IRepository<AccountsTransactions> _accountTransactionRepository;
        private IRepository<IssueVoucher> _issueVoucherRepository;
        private IRepository<IssueVoucherDetails> _issueVoucherDetailsRepository;
        private IRepository<ProgramSettings> _programsettingsRepository;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;
        private readonly ILogger<PaymentVoucherService> _logger;
        private IRepository<CurrencyMaster> _currencyMasterRepository;
        private IRepository<ItemMaster> _itemMasterRepository;
        private IRepository<UnitMaster> _unitMasterRepository;
        private IRepository<UnitDetails> _UnitDetailsRepository;
        private IRepository<SuppliersMaster> _suppliersMasterRepository;
        private IRepository<LocationMaster> _locationMasterRepository;
        private IRepository<AccountSettings> _accountsSettingssvcs;
        private IRepository<JobMasterBudgetDetails> _jobbudgetRepository;
        private IRepository<JobMaster> _JobMastertRepository;


        public IssueVoucherService(
            IRepository<ItemMaster> itemMasterRepository, IRepository<UnitMaster> unitMasterRepository,
            IRepository<SuppliersMaster> suppliersMasterRepository, IRepository<LocationMaster> locationMasterRepository,
            IRepository<AccountsTransactions> accountTransactionRepository, IRepository<StockRegister> stockRegisterRepository, IRepository<ProgramSettings> programsettingsRepository,
             IRepository<VouchersNumbers> voucherNumbers, IRepository<UnitDetails> unitDetailsRepository, ILogger<PaymentVoucherService> logger, IRepository<CurrencyMaster> currencyMasterRepository,
            IRepository<IssueVoucher> issueVoucherRepository, IRepository<IssueVoucherDetails> issueVoucherDetailsRepository, IRepository<AccountSettings> accountsSettingssvcsRepository,
            IRepository<JobMasterBudgetDetails> jobbudgetRepository, IRepository<JobMaster> JobMastertRepository)
        {
            _UnitDetailsRepository = unitDetailsRepository;
            this._accountTransactionRepository = accountTransactionRepository;
            this._stockRegisterRepository = stockRegisterRepository;
            this._issueVoucherRepository = issueVoucherRepository;
            this._issueVoucherDetailsRepository = issueVoucherDetailsRepository;
            _programsettingsRepository = programsettingsRepository;
            _voucherNumbersRepository = voucherNumbers;
            _currencyMasterRepository = currencyMasterRepository;
            _itemMasterRepository = itemMasterRepository;
            _unitMasterRepository = unitMasterRepository;
            _suppliersMasterRepository = suppliersMasterRepository;
            _locationMasterRepository = locationMasterRepository;
            _accountsSettingssvcs = accountsSettingssvcsRepository;
            _jobbudgetRepository = jobbudgetRepository;
            _JobMastertRepository = JobMastertRepository;
        }


        public IssueVoucherModel UpdateIssueVoucher(IssueVoucher issueVoucher, List<AccountsTransactions> accountsTransactions,
            List<IssueVoucherDetails> issueVoucherDetails
            , List<StockRegister> stockRegister
            )
        {

            try
            {
                _issueVoucherRepository.BeginTransaction();

                //=================================
                clsCommonFunctions.Delete_OldEntryOf_StockRegister(issueVoucher.IssueVoucherNo, VoucherType.IssueVoucher_TYPE, _stockRegisterRepository);
                clsCommonFunctions.Delete_OldEntry_AccountsTransactions(issueVoucher.IssueVoucherNo, VoucherType.IssueVoucher_TYPE, _accountTransactionRepository);
                ////=================================
                issueVoucher.IssueVoucherDetails.Clear();
                _issueVoucherRepository.Update(issueVoucher);

                foreach (var item in issueVoucherDetails)
                {
                    item.IssueVoucherId = issueVoucher.IssueVoucherId;
                    item.IssueVoucherDetailsNo = issueVoucher.IssueVoucherNo;

                    if (item.IssueVoucherDetailsId != 0)
                    {
                        _issueVoucherDetailsRepository.Update(item);

                    }
                    else
                    {
                        _issueVoucherDetailsRepository.Insert(item);
                    }
                }
                var accounts = _accountsSettingssvcs.GetAsQueryable().AsNoTracking().Where(x => x.AccountSettingsAccountDescription.Trim().Contains("Purchase Account") || x.AccountSettingsAccountDescription.Trim().Contains("Cost Of Finished Goods A/C")).ToList();
                //var credit = accounts.Where(a => a.AccountSettingsAccountDescription.Trim() == "Purchase Account").Select(x => x.AccountSettingsAccountTextValue).FirstOrDefault();
                //string[] parts = credit.Split(new string[] { "::" }, StringSplitOptions.None);
                //issueVoucher.IssueVoucherDebitAccNo = parts.Length > 1 ? parts[1].Trim() : string.Empty;

                //var debit = accounts.Where(a => a.AccountSettingsAccountDescription.Trim() == "Cost Of Finished Goods A/C").Select(x => x.AccountSettingsAccountTextValue).FirstOrDefault();
                //string[] parts1 = debit.Split(new string[] { "::" }, StringSplitOptions.None);
                //issueVoucher.IssueVoucherCreditAccNo = parts1.Length > 1 ? parts1[1].Trim() : string.Empty;

                var currencyRate = clsCommonFunctions.getConverionCurrencyRate(issueVoucher.IssueVoucherCurrencyId, _currencyMasterRepository);
                var rate = (decimal)currencyRate;

                accountsTransactions.Add(new AccountsTransactions
                {
                    AccountsTransactionsTransDate = issueVoucher.IssueVoucherDate,
                    AccountsTransactionsTstamp = DateTime.Now,
                    AccountsTransactionsVoucherNo = issueVoucher.IssueVoucherNo,
                    AccountsTransactionsVoucherType = VoucherType.IssueVoucher_TYPE,
                    AccountsTransactionsStatus = AccountStatus.Approved,
                    AccountsTransactionsAccNo = issueVoucher.IssueVoucherCreditAccNo,
                    AccountsTransactionsCredit = (decimal)issueVoucher.IssueVoucherTotalGrossAmount,
                    AccountsTransactionsFcCredit = (decimal)issueVoucher.IssueVoucherTotalGrossAmount * rate,
                    AccountsTransactionsAllocBalance = (decimal)issueVoucher.IssueVoucherTotalGrossAmount,
                    AccountsTransactionsFcAllocBalance = (decimal)issueVoucher.IssueVoucherTotalGrossAmount * rate,
                    AccountsTransactionsLocation = issueVoucher.IssueVoucherLocationId,
                    AccountsTransactionsJobNo = issueVoucher.IssueVoucherJobId,
                    AccountsTransactionsCostCenterId = (int)issueVoucher.IssueVoucherCostCenterId,
                    AccountsTransactionsCurrencyId = issueVoucher.IssueVoucherCurrencyId,
                    AccountsTransactionsUserId = 1,

                });
                accountsTransactions.Add(new AccountsTransactions
                {
                    AccountsTransactionsTransDate = issueVoucher.IssueVoucherDate,
                    AccountsTransactionsVoucherNo = issueVoucher.IssueVoucherNo,
                    AccountsTransactionsTstamp = DateTime.Now,
                    AccountsTransactionsVoucherType = VoucherType.IssueVoucher_TYPE,
                    AccountsTransactionsStatus = AccountStatus.Approved,
                    AccountsTransactionsAccNo = issueVoucher.IssueVoucherDebitAccNo,
                    AccountsTransactionsDebit = (decimal)issueVoucher.IssueVoucherTotalGrossAmount,
                    AccountsTransactionsFcDebit = (decimal)issueVoucher.IssueVoucherTotalGrossAmount * rate,
                    AccountsTransactionsAllocBalance = (decimal)issueVoucher.IssueVoucherTotalGrossAmount,
                    AccountsTransactionsFcAllocBalance = (decimal)issueVoucher.IssueVoucherTotalGrossAmount * rate,
                    AccountsTransactionsLocation = issueVoucher.IssueVoucherLocationId,
                    AccountsTransactionsJobNo = issueVoucher.IssueVoucherJobId,
                    AccountsTransactionsCostCenterId = (int)issueVoucher.IssueVoucherCostCenterId,
                    AccountsTransactionsCurrencyId = issueVoucher.IssueVoucherCurrencyId,
                    AccountsTransactionsUserId = 1,

                });
                _accountTransactionRepository.InsertList(accountsTransactions);

                foreach (var item in issueVoucherDetails)
                {

                    var avgrate = item.IssueVoucherDetailsSalesPrice ?? 1;

                    var converontype = this.getConverionTypebyUnitId(item.IssueVoucherDetailsMatId, item.IssueVoucherDetailsUnitId);
                    stockRegister.Add(new StockRegister
                    {
                        StockRegisterMaterialID = item.IssueVoucherDetailsMatId,
                        StockRegisterPurchaseID = issueVoucher.IssueVoucherNo,
                        StockRegisterRefVoucherNo = issueVoucher.IssueVoucherNo,
                        StockRegisterQuantity = item.IssueVoucherDetailsQuantity * (decimal)converontype ?? 0,
                        StockRegisterRate = avgrate,
                        StockRegisterSout = item.IssueVoucherDetailsQuantity * (decimal)converontype ?? 1,
                        StockRegisterAmount = item.IssueVoucherDetailsQuantity * (decimal)converontype * avgrate ?? 1,
                        StockRegisterFCAmount = item.IssueVoucherDetailsQuantity * (decimal)converontype * avgrate * rate ?? 1,
                        StockRegisterVoucherDate = issueVoucher.IssueVoucherDate,
                        StockRegisterTransType = VoucherType.IssueVoucher_TYPE,
                        StockRegisterStatus = AccountStatus.Approved,
                        StockRegisterAssignedDate = issueVoucher.IssueVoucherDate,
                        StockRegisterExpDate = null,
                        StockRegisterDepID = (int?)issueVoucher.IssueVoucherDepartmentId,
                        StockRegisterDelStatus = false,
                        StockRegisterUnitID = item.IssueVoucherDetailsUnitId,
                        StockRegisterLocationID = issueVoucher.IssueVoucherLocationId,
                        //ali ansari
                        //StockRegisterJobID = issueVoucher.IssueVoucherJobId,
                        StockRegisterJobID = item.IssueVoucherDetailsJobID,
                        //ali ansari
                    });
                }
                _stockRegisterRepository.InsertList(stockRegister);
                _issueVoucherRepository.TransactionCommit();

            }
            catch (Exception ex)
            {
                _issueVoucherRepository.TransactionRollback();
                throw ex;
            }

            return this.GetSavedIssueVoucherDetails((int)issueVoucher.IssueVoucherId);
        }

        public int DeleteIssueVoucher(IssueVoucher issueVoucher, List<AccountsTransactions> accountsTransactions,
            List<IssueVoucherDetails> issueVoucherDetails
            , List<StockRegister> stockRegister
            )
        {
            int i = 0;
            try
            {
                _issueVoucherRepository.BeginTransaction();

                //=================================
                clsCommonFunctions.Delete_OldEntry_StockRegister(issueVoucher.IssueVoucherNo, VoucherType.IssueVoucher_TYPE, _stockRegisterRepository);
                clsCommonFunctions.Delete_OldEntry_AccountsTransactions(issueVoucher.IssueVoucherNo, VoucherType.IssueVoucher_TYPE, _accountTransactionRepository);
                //=================================

                issueVoucher.IssueVoucherDelStatus = true;

                issueVoucherDetails = issueVoucherDetails.Select((k) =>
                {
                    k.IssueVoucherDetailsDelStatus = true;
                    return k;
                }).OrderBy(x => x.IssueVoucherDetailsItemName).AsEnumerable().ToList();

                //_issueVoucherDetailsRepository.UpdateList(issueVoucherDetails);

                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    k.AccountstransactionsDelStatus = true;
                    return k;
                }).OrderBy(x => x.AccountsTransactionsVoucherNo).AsEnumerable().ToList();
                _accountTransactionRepository.UpdateList(accountsTransactions);

                stockRegister = stockRegister.Select((k) =>
                {
                    k.StockRegisterDelStatus = true;
                    return k;
                }).OrderBy(x => x.StockRegisterRefVoucherNo).AsEnumerable().ToList();
                _stockRegisterRepository.UpdateList(stockRegister);

                issueVoucher.IssueVoucherDetails = issueVoucherDetails;
                _issueVoucherRepository.Update(issueVoucher);
                var vchrnumer = _voucherNumbersRepository.GetAsQueryable().Where(k => k.VouchersNumbersVNo == issueVoucher.IssueVoucherNo).FirstOrDefault();
                _voucherNumbersRepository.Update(vchrnumer);
                _issueVoucherRepository.TransactionCommit();
                i = 1;
            }
            catch (Exception ex)
            {
                _issueVoucherRepository.TransactionRollback();
                i = 0;
                throw ex;
            }

            return i;

        }

        public IssueVoucherModel InsertIssueVoucher(IssueVoucher issueVoucher, List<AccountsTransactions> accountsTransactions,
            List<IssueVoucherDetails> issueVoucherDetails
            , List<StockRegister> stockRegister
            )
        {
            try
            {
                _issueVoucherRepository.BeginTransaction();
                string issueVoucherNumber = clsCommonFunctions.GenrateVoucherNo(VoucherType.IssueVoucher_TYPE, Prefix.IssueVoucher_Prefix, _programsettingsRepository, _voucherNumbersRepository);

                //string issueVoucherNumber = this.GenerateVoucherNo(issueVoucher.IssueVoucherDate.Date).VouchersNumbersVNo;
                issueVoucher.IssueVoucherNo = issueVoucherNumber;
                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    _issueVoucherRepository.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.IssueVoucherId) + 1);
                issueVoucher.IssueVoucherId = maxcount;
                issueVoucherDetails = issueVoucherDetails.Select((x) =>
                {
                    x.IssueVoucherId = maxcount;
                    x.IssueVoucherDetailsNo = issueVoucherNumber;
                    return x;
                }).OrderBy(x => x.IssueVoucherDetailsItemName).AsEnumerable().ToList();
                _issueVoucherRepository.Insert(issueVoucher);
                var accounts = _accountsSettingssvcs.GetAsQueryable().AsNoTracking().Where(x => x.AccountSettingsAccountDescription.Trim().Contains("Purchase Account") || x.AccountSettingsAccountDescription.Trim().Contains("Cost Of Finished Goods A/C")).OrderBy(x => x.AccountSettingsAccountTableName).AsEnumerable().ToList();

                //var credit = accounts.Where(a => a.AccountSettingsAccountDescription.Trim() == "Purchase Account").Select(x => x.AccountSettingsAccountTextValue).FirstOrDefault();
                //string[] parts = credit.Split(new string[] { "::" }, StringSplitOptions.None);
                //issueVoucher.IssueVoucherDebitAccNo = parts.Length > 1 ? parts[1].Trim() : string.Empty;

                //var debit = accounts.Where(a => a.AccountSettingsAccountDescription.Trim() == "Cost Of Finished Goods A/C").Select(x => x.AccountSettingsAccountTextValue).FirstOrDefault();
                //string[] parts1 = debit.Split(new string[] { "::" }, StringSplitOptions.None);
                //issueVoucher.IssueVoucherCreditAccNo = parts1.Length > 1 ? parts1[1].Trim() : string.Empty;

                var currencyRate = clsCommonFunctions.getConverionCurrencyRate(issueVoucher.IssueVoucherCurrencyId, _currencyMasterRepository);
                var rate = (decimal)currencyRate;

                accountsTransactions.Add(new AccountsTransactions
                {
                    AccountsTransactionsTransDate = issueVoucher.IssueVoucherDate,
                    AccountsTransactionsTstamp = DateTime.Now,
                    AccountsTransactionsVoucherNo = issueVoucherNumber,
                    AccountsTransactionsVoucherType = VoucherType.IssueVoucher_TYPE,
                    AccountsTransactionsStatus = AccountStatus.Approved,
                    AccountsTransactionsAccNo = issueVoucher.IssueVoucherCreditAccNo,
                    AccountsTransactionsCredit = (decimal)issueVoucher.IssueVoucherTotalGrossAmount,
                    AccountsTransactionsFcCredit = (decimal)issueVoucher.IssueVoucherTotalGrossAmount * rate,
                    AccountsTransactionsAllocBalance = (decimal)issueVoucher.IssueVoucherTotalGrossAmount,
                    AccountsTransactionsFcAllocBalance = (decimal)issueVoucher.IssueVoucherTotalGrossAmount * rate,
                    AccountsTransactionsLocation = issueVoucher.IssueVoucherLocationId,
                    AccountsTransactionsJobNo = issueVoucher.IssueVoucherJobId,
                    AccountsTransactionsCostCenterId = (int)issueVoucher.IssueVoucherCostCenterId,
                    AccountsTransactionsCurrencyId = issueVoucher.IssueVoucherCurrencyId,
                    AccountsTransactionsUserId = 1,
                    AccountstransactionsDelStatus = false,

                });
                accountsTransactions.Add(new AccountsTransactions
                {
                    AccountsTransactionsTransDate = issueVoucher.IssueVoucherDate,
                    AccountsTransactionsVoucherNo = issueVoucherNumber,
                    AccountsTransactionsTstamp = DateTime.Now,
                    AccountsTransactionsVoucherType = VoucherType.IssueVoucher_TYPE,
                    AccountsTransactionsStatus = AccountStatus.Approved,
                    AccountsTransactionsAccNo = issueVoucher.IssueVoucherDebitAccNo,
                    AccountsTransactionsDebit = (decimal)issueVoucher.IssueVoucherTotalGrossAmount,
                    AccountsTransactionsFcDebit = (decimal)issueVoucher.IssueVoucherTotalGrossAmount * rate,
                    AccountsTransactionsAllocBalance = (decimal)issueVoucher.IssueVoucherTotalGrossAmount,
                    AccountsTransactionsFcAllocBalance = (decimal)issueVoucher.IssueVoucherTotalGrossAmount * rate,
                    AccountsTransactionsLocation = issueVoucher.IssueVoucherLocationId,
                    AccountsTransactionsJobNo = issueVoucher.IssueVoucherJobId,
                    AccountsTransactionsCostCenterId = (int)issueVoucher.IssueVoucherCostCenterId,
                    AccountsTransactionsCurrencyId = issueVoucher.IssueVoucherCurrencyId,
                    AccountsTransactionsUserId = 1,
                    AccountstransactionsDelStatus = false,

                });
                _accountTransactionRepository.InsertList(accountsTransactions);

                foreach (var item in issueVoucherDetails)
                {
                    var avgrate = item.IssueVoucherDetailsSalesPrice ?? 1;

                    var converontype = this.getConverionTypebyUnitId(item.IssueVoucherDetailsMatId, item.IssueVoucherDetailsUnitId);
                    stockRegister.Add(new StockRegister
                    {
                        StockRegisterMaterialID = item.IssueVoucherDetailsMatId,
                        StockRegisterPurchaseID = issueVoucherNumber,
                        StockRegisterRefVoucherNo = issueVoucherNumber,
                        StockRegisterQuantity = item.IssueVoucherDetailsQuantity * (decimal)converontype ?? 0,
                        StockRegisterRate = avgrate,
                        StockRegisterSout = item.IssueVoucherDetailsQuantity * (decimal)converontype ?? 0,
                        StockRegisterAmount = item.IssueVoucherDetailsQuantity * (decimal)converontype * avgrate ?? 0,
                        StockRegisterFCAmount = item.IssueVoucherDetailsQuantity * (decimal)converontype * avgrate * rate ?? 0,
                        StockRegisterVoucherDate = issueVoucher.IssueVoucherDate,
                        StockRegisterTransType = VoucherType.IssueVoucher_TYPE,
                        StockRegisterStatus = AccountStatus.Approved,
                        StockRegisterAssignedDate = issueVoucher.IssueVoucherDate,
                        StockRegisterExpDate = null,
                        StockRegisterDepID = (int?)issueVoucher.IssueVoucherDepartmentId,
                        StockRegisterDelStatus = false,
                        StockRegisterUnitID = item.IssueVoucherDetailsUnitId,
                        StockRegisterLocationID = issueVoucher.IssueVoucherLocationId,
                        //ali ansari
                        //StockRegisterJobID = issueVoucher.IssueVoucherJobId,
                        StockRegisterJobID = item.IssueVoucherDetailsJobID,
                        //ali ansari
                        
                    });
                }
                _stockRegisterRepository.InsertList(stockRegister);
                if (issueVoucher.IssueVoucherJobId > 0)
                {
                    var jobbuget = _jobbudgetRepository.GetAsQueryable().Where(s => s.JobMasterBudgetDetailsJobId == issueVoucher.IssueVoucherJobId).FirstOrDefault();
                    var jobNo = _JobMastertRepository.GetAsQueryable().Where(s => s.JobMasterJobId == issueVoucher.IssueVoucherJobId).FirstOrDefault()?.JobMasterJobNo;
                    if(jobbuget.JobMasterNo != "" || jobbuget.JobMasterNo != null)
                    {
                        var newBudgetDetail = new JobMasterBudgetDetails
                        {
                            JobMasterNo = jobbuget.JobMasterNo,
                            //JobMasterBudgetDetailsBudAmount = 0,
                            JobMasterBudgetDetailsJobId = issueVoucher.IssueVoucherJobId,
                            JobMasterBudgetDetailsBudId = (int)issueVoucher.IssueVoucherCostCenterId,
                            JobMasterBudgetDetailsActual = (decimal)issueVoucher.IssueVoucherTotalGrossAmount,
                            JobMasterBudgetDetailsVariance = -(decimal)issueVoucher.IssueVoucherTotalGrossAmount,
                            JobMasterBudgetDetailsDelStatus = false,

                        };
                        _jobbudgetRepository.Update(newBudgetDetail);
                    }
                    else
                    {
                        var newBudgetDetail = new JobMasterBudgetDetails
                        {
                            JobMasterNo = jobNo,
                            JobMasterBudgetDetailsBudAmount = 0,
                            JobMasterBudgetDetailsJobId = issueVoucher.IssueVoucherJobId,
                            JobMasterBudgetDetailsBudId = (int)issueVoucher.IssueVoucherCostCenterId,
                            JobMasterBudgetDetailsActual = (decimal)issueVoucher.IssueVoucherTotalGrossAmount,
                            JobMasterBudgetDetailsVariance = -(decimal)issueVoucher.IssueVoucherTotalGrossAmount,
                            JobMasterBudgetDetailsDelStatus = false,

                        };
                        _jobbudgetRepository.Insert(newBudgetDetail);
                    }
                 }
                _issueVoucherRepository.TransactionCommit();
                return this.GetSavedIssueVoucherDetails((int)issueVoucher.IssueVoucherId);

            }
            catch (Exception ex)
            {
                _issueVoucherRepository.TransactionRollback();
                throw ex;
            }
        }
        public IEnumerable<AccountsTransactions> GetAllTransaction()
        {
            return _accountTransactionRepository.GetAll();
        }
        public IEnumerable<IssueVoucher> GetIssueVoucher()
        {
            IEnumerable<IssueVoucher> issueVoucher_ALL = _issueVoucherRepository.GetAll().Where(k => k.IssueVoucherDelStatus == false || k.IssueVoucherDelStatus == null);
            return issueVoucher_ALL;
        }
        public IssueVoucherModel GetSavedIssueVoucherDetails(int pvno)
        {
            IssueVoucherModel issueVoucherModel = new IssueVoucherModel();
            //issueVoucherModel.issueVoucher = _issueVoucherRepository.GetAsQueryable().Where(k => k.IssueVoucherNo == pvno && k.IssueVoucherDelStatus == false).SingleOrDefault();

            issueVoucherModel.issueVoucher = _issueVoucherRepository.GetAsQueryable().Where(k => k.IssueVoucherId == pvno).SingleOrDefault();

            issueVoucherModel.accountsTransactions = _accountTransactionRepository.GetAsQueryable().Where(c => c.AccountsTransactionsVoucherNo == issueVoucherModel.issueVoucher.IssueVoucherNo && c.AccountsTransactionsVoucherType == VoucherType.IssueVoucher_TYPE && (c.AccountstransactionsDelStatus == false || c.AccountstransactionsDelStatus == null)).OrderBy(x => x.AccountsTransactionsVoucherNo).AsEnumerable().ToList();
            issueVoucherModel.issueVoucherDetails = _issueVoucherDetailsRepository.GetAsQueryable().Where(x => x.IssueVoucherDetailsNo == issueVoucherModel.issueVoucher.IssueVoucherNo && (x.IssueVoucherDetailsDelStatus == false || x.IssueVoucherDetailsDelStatus == null)).OrderBy(x => x.IssueVoucherDetailsItemName).AsEnumerable().ToList();
            return issueVoucherModel;
        }

        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate)
        {
            try
            {


                var prefix = this._programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.IssueVoucher_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(_voucherNumbersRepository.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.IssueVoucher_TYPE)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;


                //var prefix = "CN";
                //int vnoMaxVal = 1;


                VouchersNumbers vouchersNumbers = new VouchersNumbers
                {
                    VouchersNumbersVNo = prefix + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.IssueVoucher_TYPE,
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
        public VouchersNumbers GetVouchersNumbers(string pvno)
        {
            try
            {
                VouchersNumbers vouchersNumbers = _voucherNumbersRepository.GetAsQueryable().Where(k => k.VouchersNumbersVNo == pvno).SingleOrDefault();
                return vouchersNumbers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }

        }
        private decimal getConverionTypebyUnitId(int? itemid, int? unitDetailsid)
        {
            try
            {
                return (decimal)_UnitDetailsRepository.GetAsQueryable().FirstOrDefault(x => x.UnitDetailsItemId == itemid && x.UnitDetailsUnitId == unitDetailsid).UnitDetailsConversionType;
            }
            catch
            {
                return 1;
            }
        }

    }
}
