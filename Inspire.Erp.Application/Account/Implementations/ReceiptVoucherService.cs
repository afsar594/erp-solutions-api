using Inspire.Erp.Application.Account.Interfaces;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Infrastructure.Database;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;

namespace Inspire.Erp.Application.Account.Implementations
{
    public class ReceiptVoucherService : IReceiptVoucherService
    {
        private IRepository<AccountsTransactions> _accountTransactionRepository;
        private IRepository<ReceiptVoucherMaster> _receiptVoucherRepository;
        private IRepository<ReceiptVoucherDetails> _receiptVoucherDetailsRepository;
        private IRepository<ProgramSettings> _programsettingsRepository;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;
        private readonly IPaymentVoucherService _IPaymentVoucherService;
        private IRepository<UserTracking> UserTracking;

        private readonly IRepository<CustomerMaster> _customerMaster;
        private readonly IRepository<SuppliersMaster> _supplierMaster;
        private readonly IRepository<MasterAccountsTable> _masterAccountTable;
        private readonly IRepository<AllocationVoucher> _voucherAllocation;
        private readonly IRepository<AllocationVoucherDetails> _voucherAllocationDetails;
        private readonly IUtilityService _utilityService;
        private readonly IRepository<CurrencyMaster> _currencyMaster;


        private IRepository<UserFile> UserFile;
        private readonly InspireErpDBContext context;
        public ReceiptVoucherService(IRepository<AccountsTransactions> accountTransactionRepository, IRepository<ProgramSettings> programsettingsRepository,
            IRepository<VouchersNumbers> voucherNumbers,
            IRepository<ReceiptVoucherMaster> receiptVoucherRepository, IRepository<ReceiptVoucherDetails> receiptVoucherDetailsRepository,
            IRepository<UserFile> _UserFile, IRepository<UserTracking> _UserTracking, InspireErpDBContext _context,
            IPaymentVoucherService IPaymentVoucherService, IRepository<AllocationVoucherDetails> allocationVDlts, IRepository<CustomerMaster> customerMaster, IRepository<SuppliersMaster> supplierMaster, IRepository<MasterAccountsTable> masterAccountTable, IRepository<AllocationVoucher> voucherAllocation, IRepository<AllocationVoucherDetails> voucherAllocationDetails, IUtilityService utilityService, IRepository<CurrencyMaster> currencyMaster)
        {
            this._accountTransactionRepository = accountTransactionRepository;
            this._receiptVoucherRepository = receiptVoucherRepository;
            this._receiptVoucherDetailsRepository = receiptVoucherDetailsRepository;
            _programsettingsRepository = programsettingsRepository;
            _voucherNumbersRepository = voucherNumbers;
            UserTracking = _UserTracking;
            UserFile = _UserFile;
            context = _context;
            _IPaymentVoucherService = IPaymentVoucherService;
            _customerMaster = customerMaster;
            _supplierMaster = supplierMaster;
            _masterAccountTable = masterAccountTable;
            _voucherAllocation = voucherAllocation;
            _voucherAllocationDetails = voucherAllocationDetails;
            _utilityService = utilityService;
            _currencyMaster = currencyMaster;
        }
        public ReceiptVoucherMaster UpdateReceiptVoucher(ReceiptVoucherMaster receiptVoucher, List<AccountsTransactions> accountsTransactions, List<ReceiptVoucherDetails> receiptVoucherDetails, List<Domain.Models.AllocationDetails> alloDetails)
        {

            try
            {
                _receiptVoucherRepository.BeginTransaction();
                _receiptVoucherRepository.Update(receiptVoucher);
                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    if (k.AccountsTransactionsTransSno == 0)
                    {
                        k.AccountsTransactionsVoucherType = VoucherType.Receipt;
                        k.AccountsTransactionsStatus = AccountStatus.Approved;
                    }
                    return k;
                }).ToList();
                _accountTransactionRepository.UpdateList(accountsTransactions);
                _receiptVoucherDetailsRepository.UpdateList(receiptVoucherDetails);
                UserTracking trackingData = new UserTracking();
                trackingData.UserTrackingUserUserId = 1;
                trackingData.UserTrackingUserVpAction = "Update";
                trackingData.UserTrackingUserVpNo = receiptVoucher.ReceiptVoucherMasterVoucherNo;
                trackingData.UserTrackingUserChangeDt = DateTime.Now;
                trackingData.UserTrackingUserChangeTime = DateTime.Now;
                trackingData.UserTrackingUserVpType = "RECEIPTVOUCHER";
                UserTracking.Insert(trackingData);

                insertAllocations(receiptVoucher, alloDetails);
                _receiptVoucherRepository.TransactionCommit();
            }
            catch (Exception ex)
            {
                _receiptVoucherRepository.TransactionRollback();
                throw ex;
            }
            return this.GetSavedReceiptDetails(receiptVoucher.ReceiptVoucherMasterVoucherNo);
        }

        public int DeleteReceiptVoucher(ReceiptVoucherMaster receiptVoucher, List<AccountsTransactions> accountsTransactions, List<ReceiptVoucherDetails> receiptVoucherDetails)
        {
            int i = 0;
            try
            {
                _receiptVoucherRepository.BeginTransaction();
                receiptVoucher.ReceiptVoucherMasterDelStatus = true;
                receiptVoucherDetails = receiptVoucherDetails.Select((k) =>
                {
                    k.ReceiptVoucherDetailsDelStatus = true;
                    return k;
                }).ToList();
                _receiptVoucherDetailsRepository.UpdateList(receiptVoucherDetails);
                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    k.AccountstransactionsDelStatus = true;
                    return k;
                }).ToList();
                _accountTransactionRepository.UpdateList(accountsTransactions);
                _receiptVoucherRepository.Update(receiptVoucher);
                var vchrnumer = _voucherNumbersRepository.GetAsQueryable().Where(k => k.VouchersNumbersVNo == receiptVoucher.ReceiptVoucherMasterVoucherNo).FirstOrDefault();
                _voucherNumbersRepository.Update(vchrnumer);
                UserTracking trackingData = new UserTracking();
                trackingData.UserTrackingUserUserId = 1;
                trackingData.UserTrackingUserVpAction = "Delete";
                trackingData.UserTrackingUserVpNo = receiptVoucher.ReceiptVoucherMasterVoucherNo;
                trackingData.UserTrackingUserChangeDt = DateTime.Now;
                trackingData.UserTrackingUserChangeTime = DateTime.Now;
                trackingData.UserTrackingUserVpType = "RECEIPTVOUCHER";
                UserTracking.Insert(trackingData);
                _receiptVoucherRepository.TransactionCommit();
                i = 1;
            }
            catch (Exception ex)
            {
                _receiptVoucherRepository.TransactionRollback();
                i = 0;
                throw ex;
            }
            return i;
        }

        public ReceiptVoucherMaster InsertReceiptVoucher(ReceiptVoucherMaster receiptVoucher, List<AccountsTransactions> accountsTransactions,
                                                       List<ReceiptVoucherDetails> receiptVoucherDetails, List<Domain.Models.AllocationDetails> alloDetails)
        {
            try
            {
                _receiptVoucherRepository.BeginTransaction();
                //receiptVoucher.ReceiptVoucherMasterVoucherNo = receiptVoucher.ReceiptVoucherMasterVoucherNo;
                string receiptVoucherNumber = this.GenerateVoucherNo(receiptVoucher.ReceiptVoucherMasterVoucherDate).VouchersNumbersVNo;
                receiptVoucher.ReceiptVoucherMasterVoucherNo = receiptVoucherNumber;

                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    _receiptVoucherRepository.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.ReceiptVoucherMasterSno) + 1);
                receiptVoucherDetails = receiptVoucherDetails.Select((x) =>
                {
                    x.ReceiptVoucherDetailsSlNo = maxcount;
                    x.ReceiptVoucherDetailsVoucherNo = receiptVoucher.ReceiptVoucherMasterVoucherNo;
                    return x;
                }).ToList();
                _receiptVoucherDetailsRepository.InsertList(receiptVoucherDetails);
                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    k.AccountsTransactionsVoucherNo = receiptVoucher.ReceiptVoucherMasterVoucherNo;
                    k.AccountsTransactionsVoucherType = VoucherType.Receipt;
                    k.AccountsTransactionsStatus = AccountStatus.Approved;
                    k.AccountstransactionsDelStatus = false;
                    k.AccountsTransactionsAllocBalance = k.AccountsTransactionsDebit == 0 ? k.AccountsTransactionsCredit : k.AccountsTransactionsDebit;
                    k.AccountsTransactionsFcAllocDebit = k.AccountsTransactionsDebit;
                    k.AccountsTransactionsFcAllocCredit = k.AccountsTransactionsCredit;
                    k.AccountsTransactionsFcAllocBalance = k.AccountsTransactionsDebit == 0 ? k.AccountsTransactionsCredit : k.AccountsTransactionsDebit;
                    return k;
                }).ToList();
                // Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(accountsTransactions));
                _accountTransactionRepository.InsertList(accountsTransactions);
                _receiptVoucherRepository.Insert(receiptVoucher);
                UserTracking trackingData = new UserTracking();
                trackingData.UserTrackingUserUserId = 1;
                trackingData.UserTrackingUserVpAction = "Insert";
                trackingData.UserTrackingUserVpNo = receiptVoucher.ReceiptVoucherMasterVoucherNo;
                trackingData.UserTrackingUserChangeDt = DateTime.Now;
                trackingData.UserTrackingUserChangeTime = DateTime.Now;
                trackingData.UserTrackingUserVpType = "RECEIPTVOUCHER";
                UserTracking.Insert(trackingData);


                //insertAllocations(receiptVoucher, alloDetails);



                _receiptVoucherRepository.TransactionCommit();

                return this.GetSavedReceiptDetails(receiptVoucher.ReceiptVoucherMasterVoucherNo);
            }
            catch (Exception ex)
            {
                _receiptVoucherRepository.TransactionRollback();
                throw ex;
            }
        }

        private void insertAllocations(ReceiptVoucherMaster rcptVoucher, List<Domain.Models.AllocationDetails> alloDetails)
        {
            try
            {
                
                var alloModel = new AllocationVoucher()
                {
                    AllocationVoucherId = 0,
                    AllocationVoucherType = 1,
                    AllocationVoucherVoucherNo = $"AV{1}",
                    AllocationVoucherVoucherDate = DateTime.Now.Date,
                    AllocationVoucherDescription = rcptVoucher.ReceiptVoucherMasterNarration,
                    AllocationVoucherVoucherAccNo = rcptVoucher.ReceiptVoucherMasterDrAcNo,
                    AllocationVoucherVoucherAccType = "ALLOCATION",
                    AllocationVoucherVoucherFsno = rcptVoucher.ReceiptVoucherMasterFsno,
                    AllocationVoucherStatus = "A",
                    AllocationVoucherUserId = rcptVoucher.ReceiptVoucherMasterUserId,
                    AllocationVoucherAvStatus = "B",
                    AllocationVoucherRefVno = rcptVoucher.ReceiptVoucherMasterVoucherNo,
                    AllocationVoucherRefVtype = 1,//"RECEIPT",
                    AllocationVoucherLocationId = 1,
                    AllocationVoucherVcreation = "AccForm",
                    AllocationVoucherDelStatus = false,

                };
                alloModel.VoucherAllocationDetails = new List<AllocationVoucherDetails>();
                foreach (var item in alloDetails)
                {
                    alloModel.VoucherAllocationDetails.Add(new AllocationVoucherDetails()
                    {
                        //AllocationVoucherDetailsId =
                        AllocationVoucherDetailsVno = "",
                        AllocationVoucherDetailsVtype = 1,
                        AllocationVoucherDetailsVFsno = rcptVoucher.ReceiptVoucherMasterFsno,
                        AllocationVoucherDetailsVLocationId = 1,
                        AllocationVoucherDetailsTransSno = (int)item.TransNo,
                        AllocationVoucherDetailsAccNo = rcptVoucher.ReceiptVoucherMasterDrAcNo,
                        AllocationVoucherDetailsAllocDebit = (double)item.Debit,
                        AllocationVoucherDetailsAllocCredit = (double)item.Credit,
                        AllocationVoucherDetailsFcAllocDebit = (double)item.Debit,
                        AllocationVoucherDetailsFcAllocCredit = (double)item.Credit,
                        AllocationVoucherDetailsRefVno = rcptVoucher.ReceiptVoucherMasterVoucherNo,
                        AllocationVoucherDetailsRefVtype = "ALLOCATION",
                        AllocationVoucherDetailsRefLocationId = 1,
                        AllocationVoucherDetailsRefFsno = 1,
                        AllocationVoucherDetailsDelStatus = false

                    });
                }
                AddEditVoucherAllocation(alloModel);
            }
            catch (Exception ex)
            {

            }
        }
        public void AddEditVoucherAllocation(AllocationVoucher model)
        {
            try
            {
                if (model.AllocationVoucherId == null || model.AllocationVoucherId == 0)
                {

                    AllocationVoucher order = new AllocationVoucher()
                    {

                        AllocationVoucherType = model.AllocationVoucherType,
                        AllocationVoucherVoucherAccType = model.AllocationVoucherVoucherAccType,
                        AllocationVoucherDelStatus = false,
                        AllocationVoucherVoucherDate = model.AllocationVoucherVoucherDate,
                        AllocationVoucherVcreation = model.AllocationVoucherVcreation,
                        AllocationVoucherVoucherNo = model.AllocationVoucherVoucherNo,
                        AllocationVoucherVoucherAccNo = model.AllocationVoucherVoucherAccNo,
                        AllocationVoucherId = getMaxVoucherAllocation()

                    };
                    _voucherAllocation.Insert(order);

                    if (model.VoucherAllocationDetails != null && model.VoucherAllocationDetails.Count() > 0)
                    {
                        var vouchetAlloc = _voucherAllocation.GetAll().LastOrDefault();
                        Console.WriteLine("************************************************** = " + vouchetAlloc.AllocationVoucherId);
                        List<AllocationVoucherDetails> orderDetails = new List<AllocationVoucherDetails>();

                        foreach (var x in model.VoucherAllocationDetails)
                        {
                            orderDetails.Add(
                            new AllocationVoucherDetails
                            {
                                AllocationVoucherDetailsVno = x.AllocationVoucherDetailsVno,
                                AllocationVoucherDetailsAllocDebit = x.AllocationVoucherDetailsAllocDebit > 0 ? x.AllocationVoucherDetailsNetAllocation : 0.0,
                                AllocationVoucherDetailsAllocCredit = x.AllocationVoucherDetailsAllocCredit > 0 ? x.AllocationVoucherDetailsNetAllocation : 0.0,
                                AllocationVoucherDetailsVtype = 1,
                                AllocationVoucherDetailsDelStatus = false,
                                AllocationVoucherDetailsSno = Convert.ToInt32(vouchetAlloc.AllocationVoucherId),
                                AllocationVoucherDetailsVFsno = model.AllocationVoucherVoucherFsno,
                                AllocationVoucherDetailsVLocationId = model.AllocationVoucherLocationId,
                                AllocationVoucherDetailsTransSno = (int)x.AccountsTransactionsTransSno,
                                AllocationVoucherDetailsAccNo = model.AllocationVoucherVoucherAccNo,
                                AllocationVoucherDetailsFcAllocDebit = x.AllocationVoucherDetailsAllocDebit > 0 ? x.AllocationVoucherDetailsNetAllocation * 1 : 0.0, //==>currency rate
                                    AllocationVoucherDetailsFcAllocCredit = x.AllocationVoucherDetailsAllocCredit > 0 ? x.AllocationVoucherDetailsNetAllocation * 1 : 0.0,
                                AllocationVoucherDetailsRefVno = model.AllocationVoucherRefVno,
                                AllocationVoucherDetailsRefVtype = x.AllocationVoucherDetailsRefVtype,
                                AllocationVoucherDetailsRefLocationId = x.AllocationVoucherDetailsRefLocationId,
                                AllocationVoucherDetailsRefFsno = x.AllocationVoucherDetailsRefFsno,
                                    //AllocationVoucherDetailsId = getMaxVoucherAllocationDetails()
                                });
                        }

                        _voucherAllocationDetails.InsertList(orderDetails);
                        _voucherAllocationDetails.SaveChangesAsync();

                        //here need to update the accounts transaction table for allocation
                        List<AccountsTransactions> accountsTransactions = new List<AccountsTransactions>();
                        foreach (var item in model.VoucherAllocationDetails)
                        {
                            AccountsTransactions acc = _accountTransactionRepository.GetAsQueryable().Where(x => x.AccountsTransactionsTransSno == item.AllocationVoucherDetailsTransSno).FirstOrDefault();
                            var currency = _currencyMaster.GetAsQueryable().Where(c => c.CurrencyMasterCurrencyId == acc.AccountsTransactionsCurrencyId).Distinct().FirstOrDefault();
                            if (acc != null)
                            {
                                if (item.AllocationVoucherDetailsAllocDebit > 0)
                                {

                                    acc.AccountsTransactionsAllocDebit = (decimal)(item.AccountsTransactionsAllocBalance != null ? item.AllocationVoucherDetailsNetAllocation : 0); //NetAlloc;
                                    acc.AccountsTransactionsFcAllocDebit = (decimal)(item.AllocationVoucherDetailsAllocDebit != null ? item.AllocationVoucherDetailsAllocDebit : 1) * (currency != null ? (decimal)(currency.CurrencyMasterCurrencyRate != null ? currency.CurrencyMasterCurrencyRate : 1) : 1);
                                    acc.AccountsTransactionsFcAllocBalance = acc.AccountsTransactionsDebit - (acc.AccountsTransactionsFcAllocDebit != null ? acc.AccountsTransactionsFcAllocDebit : 0 + (decimal)(item.AllocationVoucherDetailsAllocDebit != null ? item.AllocationVoucherDetailsAllocDebit : 0));
                                    acc.AccountsTransactionsAllocBalance = acc.AccountsTransactionsFcDebit != null ? acc.AccountsTransactionsFcDebit : 0 - (acc.AccountsTransactionsAllocDebit != null ? acc.AccountsTransactionsAllocDebit : 0 + ((decimal)(item.AllocationVoucherDetailsAllocDebit != null ? item.AllocationVoucherDetailsAllocDebit : 1) * (currency != null ? (decimal)(currency.CurrencyMasterCurrencyRate != null ? currency.CurrencyMasterCurrencyRate : 1) : 1)));
                                }
                                else if (item.AllocationVoucherDetailsAllocCredit > 0)
                                {
                                    acc.AccountsTransactionsAllocCredit = (decimal)(item.AccountsTransactionsAllocBalance != null ? item.AllocationVoucherDetailsNetAllocation : 0); //NetAlloc;;
                                    acc.AccountsTransactionsFcAllocCredit = (decimal)(item.AllocationVoucherDetailsAllocCredit != null ? item.AllocationVoucherDetailsAllocCredit : 1) * (currency != null ? (decimal)(currency.CurrencyMasterCurrencyRate != null ? currency.CurrencyMasterCurrencyRate : 1) : 1);
                                    acc.AccountsTransactionsFcAllocBalance = acc.AccountsTransactionsCredit - (acc.AccountsTransactionsFcAllocCredit != null ? acc.AccountsTransactionsFcAllocCredit : 0 + (decimal)(item.AllocationVoucherDetailsAllocCredit != null ? item.AllocationVoucherDetailsAllocCredit : 0));
                                    acc.AccountsTransactionsAllocBalance = acc.AccountsTransactionsFcCredit != null ? acc.AccountsTransactionsFcCredit : 0 - (acc.AccountsTransactionsAllocCredit != null ? acc.AccountsTransactionsAllocCredit : 0 + ((decimal)(item.AllocationVoucherDetailsAllocCredit != null ? item.AllocationVoucherDetailsAllocCredit : 1) * (currency != null ? (decimal)(currency.CurrencyMasterCurrencyRate != null ? currency.CurrencyMasterCurrencyRate : 1) : 1)));
                                }

                                accountsTransactions.Add(acc);
                            }
                        }
                        _accountTransactionRepository.UpdateList(accountsTransactions);
                    }

                }
                else
                {

                    var orders = _voucherAllocation.GetAsQueryable().FirstOrDefault(x => x.AllocationVoucherId == model.AllocationVoucherId);
                    orders.AllocationVoucherType = model.AllocationVoucherType;
                    orders.AllocationVoucherDelStatus = false;
                    orders.AllocationVoucherVcreation = model.AllocationVoucherVcreation;
                    orders.AllocationVoucherVoucherNo = model.AllocationVoucherVoucherNo;
                    orders.AllocationVoucherVoucherAccType = model.AllocationVoucherVoucherAccType;
                    orders.AllocationVoucherVoucherAccNo = model.AllocationVoucherVoucherAccNo;
                    orders.AllocationVoucherVoucherDate = model.AllocationVoucherVoucherDate;
                    _voucherAllocation.Update(orders);

                    _voucherAllocation.SaveChangesAsync();

                    var listOrdersDetails = _voucherAllocationDetails.GetAsQueryable().Where(x => x.AllocationVoucherDetailsSno == model.AllocationVoucherId).ToList();
                    _voucherAllocationDetails.DeleteList(listOrdersDetails);

                    _voucherAllocationDetails.SaveChangesAsync();

                    List<AllocationVoucherDetails> orderDetails = new List<AllocationVoucherDetails>();
                    orderDetails.AddRange(model.VoucherAllocationDetails.Select(x => new AllocationVoucherDetails
                    {
                        AllocationVoucherDetailsVno = x.AllocationVoucherDetailsVno,
                        AllocationVoucherDetailsAllocDebit = x.AllocationVoucherDetailsAllocDebit,
                        AllocationVoucherDetailsAllocCredit = x.AllocationVoucherDetailsAllocCredit,
                        AllocationVoucherDetailsId = x.AllocationVoucherDetailsId,
                        AllocationVoucherDetailsVtype = x.AllocationVoucherDetailsVtype,
                        AllocationVoucherDetailsDelStatus = false,
                        AllocationVoucherDetailsSno = Convert.ToInt32(model.AllocationVoucherId),
                    }));
                    _voucherAllocationDetails.InsertList(orderDetails);
                    _voucherAllocationDetails.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
            }
        }
        private int getMaxVoucherAllocation()
        {
            try
            {
                int? maxValue = _voucherAllocation.GetAll().Max(x => x.AllocationVoucherId);
                int? incrementedValue = maxValue.HasValue ? maxValue + 1 : 1;
                return (int)incrementedValue;
            }
            catch
            {
                return 1;
            }
        }
        public IEnumerable<AccountsTransactions> GetAllTransaction()
        {
            return _accountTransactionRepository.GetAll();
        }

        public IEnumerable<ReceiptVoucherMaster> GetReceiptVouchers()
        {
            IEnumerable<ReceiptVoucherMaster> receiptVouchers = _receiptVoucherRepository.GetAll().Where(k => k.ReceiptVoucherMasterDelStatus == false || k.ReceiptVoucherMasterDelStatus == null);
            return receiptVouchers;
        }

        public ReceiptVoucherMaster GetSavedReceiptDetails(string Rvno)
        {
            if (_voucherNumbersRepository.GetAsQueryable().Any(x => x.VouchersNumbersVNo == Rvno))
            {
                var userTrackngData = (from hdr in context.UserTracking.Where(o => o.UserTrackingUserVpType == "RECEIPTVOUCHER" && o.UserTrackingUserVpNo == Rvno)
                                       join dtl in context.UserFile on (long)hdr.UserTrackingUserUserId equals dtl.UserId
                                       select new Domain.Models.TrackingData()
                                       {
                                           VPAction = hdr.UserTrackingUserVpAction,
                                           UserId = dtl.UserId,
                                           UserName = dtl.UserName,
                                           VPType = hdr.UserTrackingUserVpType,
                                           ChangeDt = hdr.UserTrackingUserChangeDt,
                                           ChangeTime = hdr.UserTrackingUserChangeTime,
                                           VPNo = hdr.UserTrackingUserVpNo,
                                       }).ToList();
                ReceiptVoucherMaster voucherdata = _voucherNumbersRepository.GetAsQueryable().Where(x => x.VouchersNumbersVNo == Rvno)
                                                .Include(k => k.AccountsTransactions)
                                                .Include(k => k.ReceiptVoucherDetails).AsEnumerable()
                                                .Select((k) => new ReceiptVoucherMaster
                                                {
                                                    AccountsTransactions = k.AccountsTransactions.Where(x => x.AccountstransactionsDelStatus == false).ToList(),
                                                    ReceiptVoucherDetails = k.ReceiptVoucherDetails.Where(x => x.ReceiptVoucherDetailsDelStatus == false).ToList()
                                                })
                                                .SingleOrDefault();
                voucherdata = _receiptVoucherRepository.GetAsQueryable().Where(k => k.ReceiptVoucherMasterVoucherNo == Rvno).FirstOrDefault();
                voucherdata.AccountsTransactions = _accountTransactionRepository.GetAsQueryable().Where(o => o.AccountsTransactionsVoucherNo == Rvno && o.AccountstransactionsDelStatus == false).ToList();
                voucherdata.ReceiptVoucherDetails = _receiptVoucherDetailsRepository.GetAsQueryable().Where(o => o.ReceiptVoucherDetailsVoucherNo == Rvno && o.ReceiptVoucherDetailsDelStatus == false).ToList();
                voucherdata.TrackingData = userTrackngData;
                voucherdata.rvAllocationDetails = this._IPaymentVoucherService.GetAllocationDetails(voucherdata.ReceiptVoucherMasterDrAcNo, voucherdata.ReceiptVoucherMasterVoucherNo);
                return voucherdata;
            }
            return null;
        }

        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate)
        {
            try
            {
                var prefix = this._programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.RV_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(_voucherNumbersRepository.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.Receipt)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;
                VouchersNumbers vouchersNumbers = new VouchersNumbers
                {
                    VouchersNumbersVNo = prefix + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.Receipt,
                    VouchersNumbersFsno = 1,
                    VouchersNumbersStatus = AccountStatus.Pending,
                    VouchersNumbersVoucherDate = DateTime.Now
                };
                _voucherNumbersRepository.Insert(vouchersNumbers);
                return vouchersNumbers;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public VouchersNumbers GetVouchersNumbers(string Rvno)
        {
            VouchersNumbers vouchersNumbers = _voucherNumbersRepository.GetAsQueryable().Where(k => k.VouchersNumbersVNo == Rvno).SingleOrDefault();
            return vouchersNumbers;
        }


    }
}
