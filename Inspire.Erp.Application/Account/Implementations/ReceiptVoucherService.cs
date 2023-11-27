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
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
        private IRepository<UserFile> UserFile;
        private readonly InspireErpDBContext context;
        public ReceiptVoucherService(IRepository<AccountsTransactions> accountTransactionRepository, IRepository<ProgramSettings> programsettingsRepository,
            IRepository<VouchersNumbers> voucherNumbers,
            IRepository<ReceiptVoucherMaster> receiptVoucherRepository, IRepository<ReceiptVoucherDetails> receiptVoucherDetailsRepository,
            IRepository<UserFile> _UserFile, IRepository<UserTracking> _UserTracking, InspireErpDBContext _context,
            IPaymentVoucherService IPaymentVoucherService)
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
        }
        public ReceiptVoucherMaster UpdateReceiptVoucher(ReceiptVoucherMaster receiptVoucher, List<AccountsTransactions> accountsTransactions, List<ReceiptVoucherDetails> receiptVoucherDetails)
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
                                                       List<ReceiptVoucherDetails> receiptVoucherDetails)
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
                    return k;
                }).ToList();
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(accountsTransactions));
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
                _receiptVoucherRepository.TransactionCommit();
                return receiptVoucher;
            }
            catch (Exception ex)
            {
                _receiptVoucherRepository.TransactionRollback();
                throw ex;
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
                voucherdata.AccountsTransactions = _accountTransactionRepository.GetAll().Where(o => o.AccountsTransactionsVoucherNo == Rvno && o.AccountstransactionsDelStatus == false).ToList();
                voucherdata.ReceiptVoucherDetails = _receiptVoucherDetailsRepository.GetAll().Where(o => o.ReceiptVoucherDetailsVoucherNo == Rvno && o.ReceiptVoucherDetailsDelStatus == false).ToList();
                voucherdata.TrackingData = userTrackngData;
                voucherdata.AllocationDetails = this._IPaymentVoucherService.GetAllocationDetails(voucherdata.ReceiptVoucherMasterDrAcNo, voucherdata.ReceiptVoucherMasterVoucherNo);
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
