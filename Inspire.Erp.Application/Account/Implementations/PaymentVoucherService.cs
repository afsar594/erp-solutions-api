using Inspire.Erp.Application.Account.Interfaces;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Infrastructure.Database;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;


namespace Inspire.Erp.Application.Account.Implementations
{
    public class PaymentVoucherService : IPaymentVoucherService
    {
        private IRepository<AccountsTransactions> _accountTransactionRepository;
        private IRepository<PaymentVoucher> _paymentVoucherRepository;
        private IRepository<PaymentVoucherDetails> _paymentVoucherDetailsRepository;
        private IRepository<ProgramSettings> _programsettingsRepository;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;
        private IRepository<UserTracking> UserTracking;
        private IRepository<UserFile> UserFile;
        private readonly ILogger<PaymentVoucherService> _logger;
        private readonly InspireErpDBContext context;
        private IRepository<AllocationVoucherDetails> _AllocationVoucherDetails;
        public PaymentVoucherService(IRepository<AccountsTransactions> accountTransactionRepository, IRepository<ProgramSettings> programsettingsRepository,
            IRepository<VouchersNumbers> voucherNumbers, ILogger<PaymentVoucherService> logger, IRepository<UserFile> _UserFile, IRepository<UserTracking> _UserTracking,
            IRepository<PaymentVoucher> paymentVoucherRepository, IRepository<PaymentVoucherDetails> paymentVoucherDetailsRepository,
            InspireErpDBContext _context, IRepository<AllocationVoucherDetails> AllocationVoucherDetail)
        {
            this._accountTransactionRepository = accountTransactionRepository;
            this._paymentVoucherRepository = paymentVoucherRepository;
            this._paymentVoucherDetailsRepository = paymentVoucherDetailsRepository;
            _programsettingsRepository = programsettingsRepository;
            _voucherNumbersRepository = voucherNumbers;
            _logger = logger;
            UserTracking = _UserTracking;
            UserFile = _UserFile;
            context = _context;
            _AllocationVoucherDetails = AllocationVoucherDetail;
        }

        public PaymentVoucher UpdatePaymentVoucher(PaymentVoucher paymentVoucher, List<AccountsTransactions> accountsTransactions,
                                                        List<PaymentVoucherDetails> paymentVoucherDetails)
        {
            try
            {
                _paymentVoucherRepository.BeginTransaction();

                _paymentVoucherRepository.Update(paymentVoucher);
                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    if (k.AccountsTransactionsTransSno == 0)
                    {
                        k.AccountsTransactionsVoucherType = VoucherType.Payment;
                        k.AccountsTransactionsStatus = AccountStatus.Approved;
                    }

                    return k;
                }).ToList();
                _accountTransactionRepository.UpdateList(accountsTransactions);
                _paymentVoucherDetailsRepository.UpdateList(paymentVoucherDetails);

                UserTracking trackingData = new UserTracking();
                trackingData.UserTrackingUserUserId = 1;
                trackingData.UserTrackingUserVpAction = "Update";
                trackingData.UserTrackingUserVpNo = paymentVoucher.PaymentVoucherVoucherNo;
                trackingData.UserTrackingUserChangeDt = DateTime.Now;
                trackingData.UserTrackingUserChangeTime = DateTime.Now;
                trackingData.UserTrackingUserVpType = "PAYMENTVOUCHER";
                UserTracking.Insert(trackingData);
                _paymentVoucherRepository.TransactionCommit();
            }
            catch (Exception ex)
            {
                _paymentVoucherRepository.TransactionRollback();
                throw ex;
            }
            return this.GetSavedPaymentDetails(paymentVoucher.PaymentVoucherVoucherNo);
        }

        public int DeletePaymentVoucher(PaymentVoucher paymentVoucher, List<AccountsTransactions> accountsTransactions, List<PaymentVoucherDetails> paymentVoucherDetails)
        {
            int i = 0;
            try
            {
                _paymentVoucherRepository.BeginTransaction();
                paymentVoucher.PaymentVoucherDelStatus = true;
                paymentVoucherDetails = paymentVoucherDetails.Select((k) =>
                {
                    k.PaymentVocherDetailsDelStatus = true;
                    return k;
                }).ToList();
                _paymentVoucherDetailsRepository.UpdateList(paymentVoucherDetails);
                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    k.AccountstransactionsDelStatus = true;
                    return k;
                }).ToList();
                _accountTransactionRepository.UpdateList(accountsTransactions);
                _paymentVoucherRepository.Update(paymentVoucher);
                var vchrnumer = _voucherNumbersRepository.GetAsQueryable().Where(k => k.VouchersNumbersVNo == paymentVoucher.PaymentVoucherVoucherNo).FirstOrDefault();
                _voucherNumbersRepository.Update(vchrnumer);
                UserTracking trackingData = new UserTracking();
                trackingData.UserTrackingUserUserId = 1;
                trackingData.UserTrackingUserVpAction = "Delete";
                trackingData.UserTrackingUserVpNo = paymentVoucher.PaymentVoucherVoucherNo;
                trackingData.UserTrackingUserChangeDt = DateTime.Now;
                trackingData.UserTrackingUserChangeTime = DateTime.Now;
                trackingData.UserTrackingUserVpType = "PAYMENTVOUCHER";
                UserTracking.Insert(trackingData);

                _paymentVoucherRepository.TransactionCommit();
                i = 1;
            }
            catch (Exception ex)
            {
                _paymentVoucherRepository.TransactionRollback();
                i = 0;
                throw ex;
            }
            return i;
        }
        public PaymentVoucher InsertPaymentVoucher(PaymentVoucher paymentVoucher, List<AccountsTransactions> accountsTransactions,
                                                        List<PaymentVoucherDetails> paymentVoucherDetails)
        {
            try
            {
                _paymentVoucherRepository.BeginTransaction();
                paymentVoucher.PaymentVoucherVoucherNo = paymentVoucher.PaymentVoucherVoucherNo;
                paymentVoucherDetails = paymentVoucherDetails.Select((x) =>
                {
                    x.PaymentVoucherDetailsSno = null;
                    x.PaymentVoucherDetailsVoucherNo = paymentVoucher.PaymentVoucherVoucherNo;
                    return x;
                }).ToList();
                _paymentVoucherDetailsRepository.InsertList(paymentVoucherDetails);
                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    k.AccountsTransactionsVoucherNo = paymentVoucher.PaymentVoucherVoucherNo;
                    k.AccountsTransactionsVoucherType = VoucherType.Payment;
                    k.AccountsTransactionsStatus = AccountStatus.Approved;
                    return k;
                }).ToList();
                _accountTransactionRepository.InsertList(accountsTransactions);
                _paymentVoucherRepository.Insert(paymentVoucher);
                UserTracking trackingData = new UserTracking();
                trackingData.UserTrackingUserUserId = 1;
                trackingData.UserTrackingUserVpAction = "Insert";
                trackingData.UserTrackingUserVpNo = paymentVoucher.PaymentVoucherVoucherNo;
                trackingData.UserTrackingUserChangeDt = DateTime.Now;
                trackingData.UserTrackingUserChangeTime = DateTime.Now;
                trackingData.UserTrackingUserVpType = "PAYMENTVOUCHER";
                //UserTracking.Insert(trackingData);
                _paymentVoucherRepository.TransactionCommit();
                return this.GetSavedPaymentDetails(paymentVoucher.PaymentVoucherVoucherNo);
            }
            catch (Exception ex)
            {
                _paymentVoucherRepository.TransactionRollback();
                throw ex;
            }
        }
        public IEnumerable<AccountsTransactions> GetAllTransaction()
        {
            return _accountTransactionRepository.GetAll();
        }
        public IEnumerable<PaymentVoucher> GetPaymentVouchers()
        {
            IEnumerable<PaymentVoucher> paymentVouchers = _paymentVoucherRepository.GetAll().Where(k => k.PaymentVoucherDelStatus == false || k.PaymentVoucherDelStatus == null).ToList();
            return paymentVouchers;
        }
        public PaymentVoucher GetSavedPaymentDetails(string pvno)
        {
            if (_voucherNumbersRepository.GetAsQueryable().Any(x => x.VouchersNumbersVNo == pvno))
            {
                PaymentVoucher voucherdata = new PaymentVoucher();
                // _voucherNumbersRepository.GetAsQueryable().Where(x => x.VouchersNumbersVNo == pvno)
                //                                 .Include(k => k.AccountsTransactions)
                //                                 .Include(k => k.PaymentVoucherDetails).AsEnumerable()
                //                                 .Select((k) => new PaymentVoucher
                //                                 {
                //                                     AccountsTransactions = k.AccountsTransactions.Where(x => x.AccountstransactionsDelStatus == false).ToList(),
                //                                     PaymentVoucherDetails = k.PaymentVoucherDetails.Where(x => x.PaymentVocherDetailsDelStatus == false).ToList(),
                //                                 })
                //                                 .SingleOrDefault();
                voucherdata = _paymentVoucherRepository.GetAsQueryable().Where(k => k.PaymentVoucherVoucherNo == pvno).FirstOrDefault();
                voucherdata.AccountsTransactions = _accountTransactionRepository.GetAll().Where(x => x.AccountsTransactionsVoucherNo == pvno && x.AccountstransactionsDelStatus == false).ToList();
                voucherdata.PaymentVoucherDetails = _paymentVoucherDetailsRepository.GetAll().Where(x => x.PaymentVoucherDetailsVoucherNo == pvno && x.PaymentVocherDetailsDelStatus == false).ToList();
                voucherdata.AllocationDetails = this.GetAllocationDetails(voucherdata.PaymentVoucherCrAcNo, voucherdata.PaymentVoucherVoucherNo);
                return voucherdata;
            }
            return null;
        }

        public IQueryable GetUserTracking(string voucherNo)
        {
            try
            {
                var userTrackngData = (from hdr in context.UserTracking.Where(o => o.UserTrackingUserVpType == "PAYMENTVOUCHER" && o.UserTrackingUserVpNo == voucherNo)
                                       join dtl in context.UserFile on (long)hdr.UserTrackingUserUserId equals dtl.UserId
                                       select new
                                       {
                                           VPAction = hdr.UserTrackingUserVpAction,
                                           UserId = dtl.UserId,
                                           UserName = dtl.UserName,
                                           VPType = hdr.UserTrackingUserVpType,
                                           ChangeDt = hdr.UserTrackingUserChangeDt,
                                           ChangeTime = hdr.UserTrackingUserChangeTime,
                                           VPNo = hdr.UserTrackingUserVpNo,
                                       }).AsQueryable();
                return userTrackngData;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public VouchersNumbers GenerateVoucherNo()
        {
            try
            {
                var prefix = this._programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.PV_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(_voucherNumbersRepository.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.Payment)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;
                VouchersNumbers vouchersNumbers = new VouchersNumbers
                {
                    VouchersNumbersVNo = prefix + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.Payment,
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

        public List<Domain.Models.AllocationDetails> GetAllocationDetails(string accountNo, string voucherNo)
        {
            try
            {
                var data = (from hdr in context.AllocationVoucherDetails
                            join dtl in context.AccountsTransactions on (long)hdr.AllocationVoucherDetailsTransSno equals dtl.AccountsTransactionsTransSno
                            where (dtl.RefNo == voucherNo &&
                            dtl.AccountsTransactionsAccNo == accountNo
                            )
                            select new Domain.Models.AllocationDetails()
                            {
                                TransDate = dtl.AccountsTransactionsTransDate,
                                VoucherNo = dtl.AccountsTransactionsVoucherNo,
                                Type = dtl.AccountsTransactionsVoucherType,
                                Debit = dtl.AccountsTransactionsAllocDebit,
                                Credit = dtl.AccountsTransactionsAllocCredit,
                                Balance = dtl.AccountsTransactionsAllocBalance,
                                AllocAmount = dtl.AccountsTransactionsFcRate,
                                NetAllocation = dtl.AccountsTransactionsVatableAmount,
                            }).ToList();
                return data;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}
