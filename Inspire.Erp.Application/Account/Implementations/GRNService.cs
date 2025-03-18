using AutoMapper.Configuration.Annotations;
using Inspire.Erp.Application.Account.Implementations;
using Inspire.Erp.Application.Account.Interfaces;
using Inspire.Erp.Application.MODULE;
using Inspire.Erp.Application.StoreWareHouse.Interface;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Infrastructure.Database;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Spire.Pdf.Exporting.XPS.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.StoreWareHouse.Implementation
{
    public class GRNService : IGRNService
    {
        private IRepository<GRNResponse> _grnRepository;
        private IRepository<StockRegister> _stockRegisterRepository;
        private IRepository<AccountsTransactions> _accountTransactionRepository;
        private IRepository<PurchaseVoucher> _purchaseVoucherRepository;
        private IRepository<PurchaseVoucherDetails> _purchaseVoucherDetailsRepository;
        private IRepository<VouchersNumbers> _voucherNumbersRepository; private IRepository<CurrencyMaster> _currencyMasterRepository;
        IRepository<ProgramSettings> _programsettingsRepository;
        private readonly ILogger<PaymentVoucherService> _logger;
        private readonly InspireErpDBContext _context;
        public static string prefix;
        private IRepository<AccountSettings> _accountsSettingssvcs;
      
        private IRepository<IssueVoucher> _issueVoucherRepository;
        private IRepository<IssueVoucherDetails> _issueVoucherDetailsRepository;
        private IAccountsSettingsService _accountssettingSvc;
        public GRNService(
            ILogger<PaymentVoucherService> logger,
            IRepository<GRNResponse> grnRepository,
            IRepository<AccountsTransactions> accountTransactionRepository,
            IRepository<StockRegister> stockRegisterRepository,
            IRepository<ProgramSettings> programsettingsRepository,
            IRepository<IssueVoucher> issueVoucherRepository, IRepository<IssueVoucherDetails> issueVoucherDetailsRepository,
            IRepository<VouchersNumbers> voucherNumbers, IRepository<CurrencyMaster> currencyMasterRepository,
            IRepository<PurchaseVoucher> purchaseVoucherRepository,
            IRepository<PurchaseVoucherDetails> purchaseVoucherDetailsRepository,
            IAccountsSettingsService accountssettingSvc,
            IRepository<AccountSettings> accountsSettingssvcsRepository,
            InspireErpDBContext context)
        {
            _accountTransactionRepository = accountTransactionRepository;
            _stockRegisterRepository = stockRegisterRepository;
            _purchaseVoucherRepository = purchaseVoucherRepository;
            _purchaseVoucherDetailsRepository = purchaseVoucherDetailsRepository;
            _voucherNumbersRepository = voucherNumbers;
            _programsettingsRepository = programsettingsRepository;
            _grnRepository = grnRepository; _currencyMasterRepository = currencyMasterRepository;
            _logger = logger;
            _context = context;
            this._issueVoucherRepository = issueVoucherRepository;
            this._issueVoucherDetailsRepository = issueVoucherDetailsRepository;
            _accountssettingSvc = accountssettingSvc;
            _accountsSettingssvcs = accountsSettingssvcsRepository;
            prefix = programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.PurchaseVoucher_Prefix).FirstOrDefault().ProgramSettingsTextValue;
        }


        public IQueryable GetAllGRN()
        {
            try
            {
                var grnList = _context.PurchaseVoucher.Where(o => o.PurchaseVoucherVoucherNo.Contains("GRN") && o.PurchaseVoucherDelStatus != true).AsQueryable();
                return grnList;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public PurchaseVoucher GetByID(int id)
        {
            try
            {
                PurchaseVoucher pv = new PurchaseVoucher();
             

                pv = _purchaseVoucherRepository.GetAll().FirstOrDefault(o => o.PurchaseVoucherPurID == Convert.ToInt64(id));
                if (pv != null)
                {
                    var userTrackngData = (from hdr in _context.UserTracking.Where(o => o.UserTrackingUserVpType == "GRN" && o.UserTrackingUserVpNo == pv.PurchaseVoucherVoucherNo)
                                           join dtl in _context.UserFile on (long)hdr.UserTrackingUserUserId equals dtl.UserId
                                           select new UserTrackingDisplay()
                                           {
                                               Action = hdr.UserTrackingUserVpAction,
                                               Id = dtl.UserId.ToString(),
                                               Name = dtl.UserName,
                                               VType = hdr.UserTrackingUserVpType,
                                               Date = Convert.ToDateTime(hdr.UserTrackingUserChangeDt).ToString("dd/MMM/yyyy"),
                                               Time = Convert.ToDateTime(hdr.UserTrackingUserChangeTime).ToShortTimeString(),
                                               VNo = hdr.UserTrackingUserVpNo,
                                           }).OrderBy(x => x.VType).AsEnumerable().ToList();


                    pv.PurchaseVoucherDetails = _purchaseVoucherDetailsRepository.GetAll().Where(o => o.PurchaseVoucherDetailsVoucherNo == pv.PurchaseVoucherVoucherNo).ToList();
                    pv.AccountsTransactions = _accountTransactionRepository.GetAll().Where(o => o.AccountsTransactionsVoucherNo == pv.PurchaseVoucherVoucherNo && o.AccountstransactionsDelStatus != true).ToList();
                    pv.UserTrackingData = userTrackngData;
                }
                return pv;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public PurchaseVoucher updateGRN(PurchaseVoucher grn, List<AccountsTransactions> accountsTransactions)
        {
            try
            {
                clsCommonFunctions.Delete_OldEntry_AccountsTransactions(grn.PurchaseVoucherVoucherNo, VoucherType.PurchaseVoucher_TYPE, _accountTransactionRepository);
                clsCommonFunctions.Delete_OldEntryOf_StockRegister(grn.PurchaseVoucherVoucherNo, VoucherType.PurchaseVoucher_TYPE, _stockRegisterRepository);

                foreach (var k in accountsTransactions)
                {
                    k.AccountsTransactionsVoucherNo = grn.PurchaseVoucherVoucherNo;
                    k.AccountsTransactionsVoucherType = VoucherType.PurchaseVoucher_TYPE;
                    k.AccountsTransactionsStatus = AccountStatus.Approved;
                    k.AccountsTransactionsUserId = 1;
                }
                _context.AccountsTransactions.AddRange(accountsTransactions);
                _context.SaveChanges();

                _purchaseVoucherRepository.Update(grn);
                var existingGrnList = _purchaseVoucherDetailsRepository.GetAll().Where(o => o.PurchaseVoucherDetailsVoucherNo == grn.PurchaseVoucherVoucherNo).ToList();
                _purchaseVoucherDetailsRepository.DeleteList(existingGrnList);

                List<StockRegister> stockRegisters = new List<StockRegister>();

                var currencyRate = clsCommonFunctions.getConverionCurrencyRate(grn.PurchaseVoucherCurrencyID, _currencyMasterRepository);
                var rate = (decimal)currencyRate;

                foreach (var item in grn.PurchaseVoucherDetails)
                {
                    item.PurchaseVoucherDetailsVoucherNo = grn.PurchaseVoucherVoucherNo;
                    if (item.PurchaseVoucherDetailsPurcahseDetailsId != 0)
                    {
                        _purchaseVoucherDetailsRepository.Update(item);

                    }
                    else
                    {
                        _purchaseVoucherDetailsRepository.Insert(item);
                    }

                    var converontype = this.getConverionTypebyUnitId(item.PurchaseVoucherDetailsMaterialId, item.PurchaseVoucherDetailsUnitId);
                    stockRegisters.Add(new StockRegister
                    {
                        StockRegisterMaterialID = item.PurchaseVoucherDetailsMaterialId,
                        StockRegisterRefVoucherNo = grn.PurchaseVoucherVoucherNo,
                        StockRegisterPurchaseID = grn.PurchaseVoucherVoucherNo,
                        StockRegisterVoucherDate = grn.PurchaseVoucherPurchaseDate,
                        StockRegisterQuantity = item.PurchaseVoucherDetailsQuantity * (converontype) ?? 1,
                        StockRegisterSIN = item.PurchaseVoucherDetailsQuantity * (converontype) ?? 1,
                        StockRegisterRate = item.PurchaseVoucherDetailsRate / converontype ?? 1,
                        StockRegisterAmount = item.PurchaseVoucherDetailsQuantity * item.PurchaseVoucherDetailsRate,
                        StockRegisterFCAmount = item.PurchaseVoucherDetailsQuantity * item.PurchaseVoucherDetailsRate * rate,
                        StockRegisterTransType = VoucherType.PurchaseVoucher_TYPE,
                        StockRegisterStatus = AccountStatus.Approved,
                        StockRegisterUnitID = item.PurchaseVoucherDetailsUnitId,
                        StockRegisterLocationID = grn.PurchaseVoucherLocationID,
                        StockRegisterJobID = grn.PurchaseVoucherJobID,
                        StockRegisterDelStatus = false
                    });

                }
                _stockRegisterRepository.InsertList(stockRegisters);
                //_purchaseVoucherDetailsRepository.UpdateList(grn.PurchaseVoucherDetails.ToList());

                UserTracking trackingData = new UserTracking();
                trackingData.UserTrackingUserUserId = 1;
                trackingData.UserTrackingUserVpAction = "Update";
                trackingData.UserTrackingUserVpNo = grn.PurchaseVoucherVoucherNo;
                trackingData.UserTrackingUserChangeDt = DateTime.Now;
                trackingData.UserTrackingUserChangeTime = DateTime.Now;
                trackingData.UserTrackingUserVpType = "GRN";
                _context.UserTracking.Add(trackingData);
                _context.SaveChanges();


                if ((bool)grn.PurchaseVoucherJobDirectPurchase == true)
                {
                    var data = _issueVoucherRepository.GetAsQueryable().Where(i => i.IssueVoucherPurchaseVoucherID == grn.PurchaseVoucherPurID).FirstOrDefault();
                    var detaildata = _issueVoucherDetailsRepository.GetAll().Where(i => i.IssueVoucherId == data.IssueVoucherId).ToList();
                    clsCommonFunctions.Delete_OldEntry_AccountsTransactions(data.IssueVoucherNo, VoucherType.IssueVoucher_TYPE, _accountTransactionRepository);
                    clsCommonFunctions.Delete_OldEntryOf_StockRegister(data.IssueVoucherNo, VoucherType.IssueVoucher_TYPE, _stockRegisterRepository);
                    //data.IssueVoucherDetails.Clear();
                    //var existingissue = _issueVoucherRepository.GetAsQueryable().Where(o => o.IssueVoucherId == data.IssueVoucherId).FirstOrDefault();
                    _issueVoucherRepository.Delete(data);
                    _issueVoucherDetailsRepository.DeleteList(detaildata);
                    var issueObj = new IssueVoucher();
                    
                    issueObj.IssueVoucherId = data.IssueVoucherId;
                    issueObj.IssueVoucherNo = data.IssueVoucherNo;
                    issueObj.IssueVoucherCreditAccNo = data.IssueVoucherCreditAccNo;
                    issueObj.IssueVoucherDebitAccNo = data.IssueVoucherDebitAccNo;
                    issueObj.IssueVoucherPurchaseVoucherID = (int)grn.PurchaseVoucherPurID;
                    issueObj.IssueVoucherDate = grn.PurchaseVoucherPurchaseDate;
                    issueObj.IssueVoucherCurrencyId = grn.PurchaseVoucherCurrencyID;
                    issueObj.IssueVoucherDescription = grn.PurchaseVoucherDescription;
                    issueObj.IssueVoucherGrdate = grn.PurchaseVoucherGRDate;
                    issueObj.IssueVoucherGrno = grn.PurchaseVoucherGRNo;
                    issueObj.IssueVoucherJobId = grn.PurchaseVoucherJobID;
                    issueObj.IssueVoucherLocationId = grn.PurchaseVoucherJobID;
                    issueObj.IssueVoucherLpono = grn.PurchaseVoucherLPONo;
                    issueObj.IssueVoucherLpodate = grn.PurchaseVoucherLPODate;
                    issueObj.IssueVoucherNetAmount = grn.PurchaseVoucherNetAmount;
                    issueObj.IssueVoucherTotalGrossAmount = grn.PurchaseVoucherTotalGrossAmt;
                    issueObj.IssueVoucherTotalActualAmount = grn.PurchaseVoucherActualAmount;
                    issueObj.IssueVoucherNetDisAmount = grn.PurchaseVoucherDisAmount;
                    issueObj.IssueVoucherTotalDisPer = grn.PurchaseVoucherDiscountPercentage;
                    issueObj.IssueVoucherVatAmt = grn.PurchaseVoucherVATAmount;
                    issueObj.IssueVoucherVatPer = grn.PurchaseVoucherVATPercentage;
                    issueObj.IssueVoucherVatRoundSign = grn.PurchaseVoucherVATRoundSign;
                    issueObj.IssueVoucherVatRountAmt = grn.PurchaseVoucherVATRoundAmount;

                    

                    issueObj.IssueVoucherDetails = new List<IssueVoucherDetails>();
                    foreach (var item in grn.PurchaseVoucherDetails)
                    {
                        item.PurchaseVoucherDetailsVoucherNo = grn.PurchaseVoucherVoucherNo;

                        var converontype = this.getConverionTypebyUnitId(item.PurchaseVoucherDetailsMaterialId, item.PurchaseVoucherDetailsUnitId);
                        issueObj.IssueVoucherDetails.Add(new IssueVoucherDetails
                        {

                            IssueVoucherDetailsId = 0,
                            IssueVoucherId = data.IssueVoucherId,
                            IssueVoucherDetailsNo = data.IssueVoucherNo,
                            IssueVoucherDetailsMatId = item.PurchaseVoucherDetailsMaterialId,
                            IssueVoucherDetailsQuantity = item.PurchaseVoucherDetailsQuantity,
                            IssueVoucherDetailsRate = item.PurchaseVoucherDetailsRate,
                            IssueVoucherDetailsNetAmt = item.PurchaseVoucherDetailsNetAmount,
                            IssueVoucherDetailsGrossAmount = item.PurchaseVoucherDetailsQuantity * item.PurchaseVoucherDetailsRate,
                            //IssueVoucherDetailsGrossAmount = item.PurchaseVoucherDetailsGrossAmount,
                            IssueVoucherDetailsSno = 0,
                            //IssueVoucherDetailsSalesPrice=item.sa,
                            IssueVoucherDetailsUnitId = item.PurchaseVoucherDetailsUnitId,
                            IssueVoucherDetailsDelStatus = false,
                            IssueVoucherDetailsJobID = item.PurchaseVoucherDetailsJobID,
                        });
                      

                    }
                    List<StockRegister> stockRegister = new List<StockRegister>();

                    foreach (var item in issueObj.IssueVoucherDetails)
                    {
                        var avgrate = item.IssueVoucherDetailsSalesPrice ?? 1;

                        var converontype = this.getConverionTypebyUnitId(item.IssueVoucherDetailsMatId, item.IssueVoucherDetailsUnitId);
                        stockRegister.Add(new StockRegister
                        {
                            StockRegisterMaterialID = item.IssueVoucherDetailsMatId,
                            StockRegisterPurchaseID = data.IssueVoucherNo,
                            StockRegisterRefVoucherNo = data.IssueVoucherNo,
                            StockRegisterQuantity = item.IssueVoucherDetailsQuantity * (decimal)converontype ?? 0,
                            StockRegisterRate = avgrate,
                            StockRegisterSout = item.IssueVoucherDetailsQuantity * (decimal)converontype ?? 0,
                            StockRegisterAmount = item.IssueVoucherDetailsQuantity * (decimal)converontype * avgrate ?? 0,
                            StockRegisterFCAmount = item.IssueVoucherDetailsQuantity * (decimal)converontype * avgrate * rate ?? 0,
                            StockRegisterVoucherDate = issueObj.IssueVoucherDate,
                            StockRegisterTransType = VoucherType.IssueVoucher_TYPE,
                            StockRegisterStatus = AccountStatus.Approved,
                            StockRegisterAssignedDate = issueObj.IssueVoucherDate,
                            StockRegisterExpDate = null,
                            //StockRegisterDepID = (int?)issueObj.IssueVoucherDepartmentId,
                            StockRegisterDelStatus = false,
                            StockRegisterUnitID = item.IssueVoucherDetailsUnitId,
                            StockRegisterLocationID = issueObj.IssueVoucherLocationId,
                            StockRegisterJobID = item.IssueVoucherDetailsJobID,

                        });
                    }
                    _stockRegisterRepository.InsertList(stockRegister);

                    List<AccountsTransactions> accountsTransactionslist = new List<AccountsTransactions>();
                    accountsTransactionslist.Add(new AccountsTransactions
                    {
                        AccountsTransactionsTransDate = issueObj.IssueVoucherDate,
                        AccountsTransactionsTstamp = DateTime.Now,
                        AccountsTransactionsVoucherNo = data.IssueVoucherNo,
                        AccountsTransactionsVoucherType = VoucherType.IssueVoucher_TYPE,
                        AccountsTransactionsStatus = AccountStatus.Approved,
                        AccountsTransactionsAccNo = issueObj.IssueVoucherCreditAccNo,
                        AccountsTransactionsCredit = (decimal)issueObj.IssueVoucherTotalGrossAmount,
                        AccountsTransactionsFcCredit = (decimal)issueObj.IssueVoucherTotalGrossAmount * rate,
                        AccountsTransactionsAllocBalance = (decimal)issueObj.IssueVoucherTotalGrossAmount,
                        AccountsTransactionsFcAllocBalance = (decimal)issueObj.IssueVoucherTotalGrossAmount * rate,
                        AccountsTransactionsLocation = issueObj.IssueVoucherLocationId,
                        AccountsTransactionsJobNo = issueObj.IssueVoucherJobId,
                        //AccountsTransactionsCostCenterId = (int)issueObj.IssueVoucherCostCenterId,
                        AccountsTransactionsCurrencyId = issueObj.IssueVoucherCurrencyId,
                        AccountsTransactionsUserId = 1,
                        AccountstransactionsDelStatus = false,

                    });
                    accountsTransactionslist.Add(new AccountsTransactions
                    {
                        AccountsTransactionsTransDate = issueObj.IssueVoucherDate,
                        AccountsTransactionsVoucherNo = data.IssueVoucherNo,
                        AccountsTransactionsTstamp = DateTime.Now,
                        AccountsTransactionsVoucherType = VoucherType.IssueVoucher_TYPE,
                        AccountsTransactionsStatus = AccountStatus.Approved,
                        AccountsTransactionsAccNo = issueObj.IssueVoucherDebitAccNo,
                        AccountsTransactionsDebit = (decimal)issueObj.IssueVoucherTotalGrossAmount,
                        AccountsTransactionsFcDebit = (decimal)issueObj.IssueVoucherTotalGrossAmount * rate,
                        AccountsTransactionsAllocBalance = (decimal)issueObj.IssueVoucherTotalGrossAmount,
                        AccountsTransactionsFcAllocBalance = (decimal)issueObj.IssueVoucherTotalGrossAmount * rate,
                        AccountsTransactionsLocation = issueObj.IssueVoucherLocationId,
                        AccountsTransactionsJobNo = issueObj.IssueVoucherJobId,
                        //AccountsTransactionsCostCenterId = (int)issueObj.IssueVoucherCostCenterId,
                        AccountsTransactionsCurrencyId = issueObj.IssueVoucherCurrencyId,
                        AccountsTransactionsUserId = 1,
                        AccountstransactionsDelStatus = false,

                    });
                    _accountTransactionRepository.InsertList(accountsTransactionslist);
                    _issueVoucherRepository.Insert(issueObj);

                }



                return grn;
            }
            catch (Exception ex)
            {
                throw ex;
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
        public PurchaseVoucher saveGRN(PurchaseVoucher grn)
        {
            try
            {
                grn.PurchaseVoucherDelStatus = false;

                string contraVoucherNumber = clsCommonFunctions.GenrateVoucherNo(VoucherType.PurchaseVoucher_TYPE, Prefix.PurchaseVoucher_Prefix, _programsettingsRepository, _voucherNumbersRepository);
                grn.PurchaseVoucherVoucherNo = contraVoucherNumber;

                foreach (var k in grn.AccountsTransactions)
                {
                    k.AccountsTransactionsVoucherNo = grn.PurchaseVoucherVoucherNo;
                    k.AccountsTransactionsVoucherType = VoucherType.PurchaseVoucher_TYPE;
                    k.AccountsTransactionsUserId = 1;
                    k.AccountsTransactionsStatus = AccountStatus.Approved;
                }
                _context.AccountsTransactions.AddRange(grn.AccountsTransactions);
                _context.SaveChanges();

                _purchaseVoucherRepository.Insert(grn);


                List<StockRegister> stockRegisters = new List<StockRegister>();

                var currencyRate = clsCommonFunctions.getConverionCurrencyRate(grn.PurchaseVoucherCurrencyID, _currencyMasterRepository);
                var rate = (decimal)currencyRate;

                foreach (var item in grn.PurchaseVoucherDetails)
                {
                    item.PurchaseVoucherDetailsVoucherNo = grn.PurchaseVoucherVoucherNo;

                    var converontype = this.getConverionTypebyUnitId(item.PurchaseVoucherDetailsMaterialId, item.PurchaseVoucherDetailsUnitId);
                    stockRegisters.Add(new StockRegister
                    {
                        StockRegisterMaterialID = item.PurchaseVoucherDetailsMaterialId,
                        StockRegisterRefVoucherNo = grn.PurchaseVoucherVoucherNo,
                        StockRegisterPurchaseID = grn.PurchaseVoucherVoucherNo,
                        StockRegisterVoucherDate = grn.PurchaseVoucherPurchaseDate,
                        StockRegisterQuantity = item.PurchaseVoucherDetailsQuantity * (converontype) ?? 1,
                        StockRegisterSIN = item.PurchaseVoucherDetailsQuantity * (converontype) ?? 1,
                        StockRegisterRate = item.PurchaseVoucherDetailsRate / converontype ?? 1,
                        StockRegisterAmount = item.PurchaseVoucherDetailsQuantity * item.PurchaseVoucherDetailsRate,
                        StockRegisterFCAmount = item.PurchaseVoucherDetailsQuantity * item.PurchaseVoucherDetailsRate * rate,
                        StockRegisterTransType = VoucherType.PurchaseVoucher_TYPE,
                        StockRegisterStatus = AccountStatus.Approved,
                        StockRegisterUnitID = item.PurchaseVoucherDetailsUnitId,
                        StockRegisterLocationID = grn.PurchaseVoucherLocationID,
                        StockRegisterJobID = grn.PurchaseVoucherJobID,
                        StockRegisterDelStatus = false
                    });
                }
                _stockRegisterRepository.InsertList(stockRegisters);


                _purchaseVoucherDetailsRepository.InsertList(grn.PurchaseVoucherDetails.ToList());

                UserTracking trackingData = new UserTracking();
                trackingData.UserTrackingUserUserId = 1;
                trackingData.UserTrackingUserVpAction = "Insert";
                trackingData.UserTrackingUserVpNo = grn.PurchaseVoucherVoucherNo;
                trackingData.UserTrackingUserChangeDt = DateTime.Now;
                trackingData.UserTrackingUserChangeTime = DateTime.Now;
                trackingData.UserTrackingUserVpType = "GRN";
                _context.UserTracking.Add(trackingData);
                _context.SaveChanges();
                //var autoCreateIssue = _programsettingsRepository.GetAsQueryable().AsNoTracking().Where(x => x.ProgramSettingsDescription.Trim().Contains("Automatically Create Issue Voucher on Purchasae")).FirstOrDefault().ProgramSettingsBoolValue ;
                if ((bool)grn.PurchaseVoucherJobDirectPurchase == true)
                {
                    var issueObj = new IssueVoucher();
                    string issueVoucherNumber = clsCommonFunctions.GenrateVoucherNo(VoucherType.IssueVoucher_TYPE, Prefix.IssueVoucher_Prefix, _programsettingsRepository, _voucherNumbersRepository);


                    var accounts = _accountsSettingssvcs.GetAsQueryable().AsNoTracking().Where(x => x.AccountSettingsAccountDescription.Trim().Contains("Stock Account") || x.AccountSettingsAccountDescription.Trim().Contains("Cost Of Finished Goods A/C")).OrderBy(x => x.AccountSettingsAccountTableName).AsEnumerable().ToList();

                    var credit = accounts.Where(a => a.AccountSettingsAccountDescription.Trim() == "Stock Account").Select(x => x.AccountSettingsAccountTextValue).FirstOrDefault();
                    string[] parts = credit.Split(new string[] { "::" }, StringSplitOptions.None);
                    issueObj.IssueVoucherDebitAccNo = parts.Length > 1 ? parts[1].Trim() : string.Empty;

                    var debit = accounts.Where(a => a.AccountSettingsAccountDescription.Trim() == "Cost Of Finished Goods A/C").Select(x => x.AccountSettingsAccountTextValue).FirstOrDefault();
                    string[] parts1 = debit.Split(new string[] { "::" }, StringSplitOptions.None);
                    issueObj.IssueVoucherCreditAccNo = parts1.Length > 1 ? parts1[1].Trim() : string.Empty;

                    int maxcount = 0;
                    maxcount = Convert.ToInt32(
                        _issueVoucherRepository.GetAsQueryable()
                        .DefaultIfEmpty().Max(o => o == null ? 0 : o.IssueVoucherId) + 1);
                    issueObj.IssueVoucherId = maxcount;
                    issueObj.IssueVoucherNo = issueVoucherNumber;
                    issueObj.IssueVoucherPurchaseVoucherID = (int)grn.PurchaseVoucherPurID;
                    issueObj.IssueVoucherDate = grn.PurchaseVoucherPurchaseDate;
                    issueObj.IssueVoucherCurrencyId = grn.PurchaseVoucherCurrencyID;
                    //issueObj.IssueVoucherDepartmentId= grn.d
                    issueObj.IssueVoucherDescription = grn.PurchaseVoucherDescription;
                    issueObj.IssueVoucherGrdate = grn.PurchaseVoucherGRDate;
                    issueObj.IssueVoucherGrno = grn.PurchaseVoucherGRNo;
                    //issueObj.IssueVoucherIvDateForRet=grn.PurchaseVoucherPurchaseRef
                    //issueObj.IssueVoucherIvNoForRet
                    issueObj.IssueVoucherJobId = grn.PurchaseVoucherJobID;
                    issueObj.IssueVoucherLocationId = grn.PurchaseVoucherJobID;
                    issueObj.IssueVoucherLpono = grn.PurchaseVoucherLPONo;
                    issueObj.IssueVoucherLpodate = grn.PurchaseVoucherLPODate;
                    issueObj.IssueVoucherNetAmount = grn.PurchaseVoucherNetAmount;
                    //issueObj.IssueVoucherQtnNo
                    //issueObj.IssueVoucherRefNo = grn.PurchaseVoucherPurchaseRef;
                    //issueObj.IssueVoucherReqDate= grn.
                    issueObj.IssueVoucherTotalGrossAmount = grn.PurchaseVoucherTotalGrossAmt;
                    issueObj.IssueVoucherTotalActualAmount = grn.PurchaseVoucherActualAmount;
                    issueObj.IssueVoucherNetDisAmount = grn.PurchaseVoucherDisAmount;
                    issueObj.IssueVoucherTotalDisPer = grn.PurchaseVoucherDiscountPercentage;
                    issueObj.IssueVoucherVatAmt = grn.PurchaseVoucherVATAmount;
                    issueObj.IssueVoucherVatPer = grn.PurchaseVoucherVATPercentage;
                    issueObj.IssueVoucherVatRoundSign = grn.PurchaseVoucherVATRoundSign;
                    issueObj.IssueVoucherVatRountAmt = grn.PurchaseVoucherVATRoundAmount;

                    issueObj.IssueVoucherDetails = new List<IssueVoucherDetails>();
                    foreach (var item in grn.PurchaseVoucherDetails)
                    {
                        item.PurchaseVoucherDetailsVoucherNo = grn.PurchaseVoucherVoucherNo;

                        var converontype = this.getConverionTypebyUnitId(item.PurchaseVoucherDetailsMaterialId, item.PurchaseVoucherDetailsUnitId);
                        issueObj.IssueVoucherDetails.Add(new IssueVoucherDetails
                        {

                            IssueVoucherDetailsId = 0,
                            IssueVoucherId = maxcount,
                            IssueVoucherDetailsNo = issueVoucherNumber,
                            IssueVoucherDetailsMatId = item.PurchaseVoucherDetailsMaterialId,
                            IssueVoucherDetailsQuantity = item.PurchaseVoucherDetailsQuantity,
                            IssueVoucherDetailsRate = item.PurchaseVoucherDetailsRate,
                            IssueVoucherDetailsNetAmt = item.PurchaseVoucherDetailsNetAmount,
                            IssueVoucherDetailsGrossAmount = item.PurchaseVoucherDetailsQuantity * item.PurchaseVoucherDetailsRate,
                            //IssueVoucherDetailsGrossAmount = item.PurchaseVoucherDetailsGrossAmount,
                            IssueVoucherDetailsSno = 0,
                            //IssueVoucherDetailsSalesPrice=item.sa,
                            IssueVoucherDetailsUnitId = item.PurchaseVoucherDetailsUnitId,
                            IssueVoucherDetailsDelStatus = false,
                            IssueVoucherDetailsJobID = item.PurchaseVoucherDetailsJobID,
                        });


                    }
                    List<StockRegister> stockRegister = new List<StockRegister>();

                    foreach (var item in issueObj.IssueVoucherDetails)
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
                            StockRegisterVoucherDate = issueObj.IssueVoucherDate,
                            StockRegisterTransType = VoucherType.IssueVoucher_TYPE,
                            StockRegisterStatus = AccountStatus.Approved,
                            StockRegisterAssignedDate = issueObj.IssueVoucherDate,
                            StockRegisterExpDate = null,
                            //StockRegisterDepID = (int?)issueObj.IssueVoucherDepartmentId,
                            StockRegisterDelStatus = false,
                            StockRegisterUnitID = item.IssueVoucherDetailsUnitId,
                            StockRegisterLocationID = issueObj.IssueVoucherLocationId,
                            StockRegisterJobID = item.IssueVoucherDetailsJobID,

                        });
                    }
                    _stockRegisterRepository.InsertList(stockRegister);

                    List<AccountsTransactions> accountsTransactionslist = new List<AccountsTransactions>();
                    accountsTransactionslist.Add(new AccountsTransactions
                    {
                        AccountsTransactionsTransDate = issueObj.IssueVoucherDate,
                        AccountsTransactionsTstamp = DateTime.Now,
                        AccountsTransactionsVoucherNo = issueVoucherNumber,
                        AccountsTransactionsVoucherType = VoucherType.IssueVoucher_TYPE,
                        AccountsTransactionsStatus = AccountStatus.Approved,
                        AccountsTransactionsAccNo = issueObj.IssueVoucherCreditAccNo,
                        AccountsTransactionsCredit = (decimal)issueObj.IssueVoucherTotalGrossAmount,
                        AccountsTransactionsFcCredit = (decimal)issueObj.IssueVoucherTotalGrossAmount * rate,
                        AccountsTransactionsAllocBalance = (decimal)issueObj.IssueVoucherTotalGrossAmount,
                        AccountsTransactionsFcAllocBalance = (decimal)issueObj.IssueVoucherTotalGrossAmount * rate,
                        AccountsTransactionsLocation = issueObj.IssueVoucherLocationId,
                        AccountsTransactionsJobNo = issueObj.IssueVoucherJobId,
                        //AccountsTransactionsCostCenterId = (int)issueObj.IssueVoucherCostCenterId,
                        AccountsTransactionsCurrencyId = issueObj.IssueVoucherCurrencyId,
                        AccountsTransactionsUserId = 1,
                        AccountstransactionsDelStatus = false,

                    });
                    accountsTransactionslist.Add(new AccountsTransactions
                    {
                        AccountsTransactionsTransDate = issueObj.IssueVoucherDate,
                        AccountsTransactionsVoucherNo = issueVoucherNumber,
                        AccountsTransactionsTstamp = DateTime.Now,
                        AccountsTransactionsVoucherType = VoucherType.IssueVoucher_TYPE,
                        AccountsTransactionsStatus = AccountStatus.Approved,
                        AccountsTransactionsAccNo = issueObj.IssueVoucherDebitAccNo,
                        AccountsTransactionsDebit = (decimal)issueObj.IssueVoucherTotalGrossAmount,
                        AccountsTransactionsFcDebit = (decimal)issueObj.IssueVoucherTotalGrossAmount * rate,
                        AccountsTransactionsAllocBalance = (decimal)issueObj.IssueVoucherTotalGrossAmount,
                        AccountsTransactionsFcAllocBalance = (decimal)issueObj.IssueVoucherTotalGrossAmount * rate,
                        AccountsTransactionsLocation = issueObj.IssueVoucherLocationId,
                        AccountsTransactionsJobNo = issueObj.IssueVoucherJobId,
                        //AccountsTransactionsCostCenterId = (int)issueObj.IssueVoucherCostCenterId,
                        AccountsTransactionsCurrencyId = issueObj.IssueVoucherCurrencyId,
                        AccountsTransactionsUserId = 1,
                        AccountstransactionsDelStatus = false,

                    });
                    _accountTransactionRepository.InsertList(accountsTransactionslist);

                    _issueVoucherRepository.Insert(issueObj);
                }
                return grn;
            }
            catch (Exception ex)
            {
                var recd = _voucherNumbersRepository.GetAsQueryable().Where(a => a.VouchersNumbersVNo == grn.PurchaseVoucherVoucherNo).FirstOrDefault();
                if (recd != null)
                {
                    recd.VouhersNumbersDelStatus = true;
                    _voucherNumbersRepository.Update(recd);
                }
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

        public IQueryable DeleteGRN(PurchaseVoucher grn)
        {
            try
            {
                VouchersNumbers vs = _voucherNumbersRepository.GetAll().FirstOrDefault(k => k.VouchersNumbersVNo == grn.PurchaseVoucherVoucherNo);
                if (vs != null)
                {
                    vs.VouhersNumbersDelStatus = true;
                    _voucherNumbersRepository.Update(vs);
                }

                PurchaseVoucher pv = _purchaseVoucherRepository.GetAll().FirstOrDefault(k => k.PurchaseVoucherVoucherNo == grn.PurchaseVoucherVoucherNo);
                if (pv != null)
                {
                    pv.PurchaseVoucherDelStatus = true;
                    _purchaseVoucherRepository.Update(pv);
                }

                List<PurchaseVoucherDetails> pvd = _purchaseVoucherDetailsRepository.GetAll().Where(k => k.PurchaseVoucherDetailsVoucherNo == grn.PurchaseVoucherVoucherNo).ToList();
                foreach (var item in pvd)
                {
                    item.PurchaseVoucherDetailsDelStatus = true;
                }
                _purchaseVoucherDetailsRepository.UpdateList(pvd);
                clsCommonFunctions.Delete_OldEntry_AccountsTransactions(grn.PurchaseVoucherVoucherNo, VoucherType.PurchaseVoucher_TYPE, _accountTransactionRepository);
                clsCommonFunctions.Delete_OldEntryOf_StockRegister(grn.PurchaseVoucherVoucherNo, VoucherType.PurchaseVoucher_TYPE, _stockRegisterRepository);
                UserTracking trackingData = new UserTracking();
                trackingData.UserTrackingUserUserId = 1;
                trackingData.UserTrackingUserVpAction = "Delete";
                trackingData.UserTrackingUserChangeDt = DateTime.Now;
                trackingData.UserTrackingUserChangeTime = DateTime.Now;
                trackingData.UserTrackingUserVpType = "GRN";
                _context.UserTracking.Add(trackingData);
                _context.SaveChanges();
                return this.GetAllGRN();
            }
            catch (Exception)
            {
                throw;
            }
        }


        public IEnumerable<GRNResponse> loadPo(GRNRequest obj)
        {
            try
            {
                var x = _grnRepository.GetBySP($"EXEC LoadPoList").Where(x => x.Suppliers_INS_Name.Equals(obj.PurchaseID)).ToList();
                return x;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }


    }
}
