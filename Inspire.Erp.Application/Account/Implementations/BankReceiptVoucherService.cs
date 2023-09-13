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
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Inspire.Erp.Application.Account.Implementations
{
    public class BankReceiptVoucherService : IBankReceiptVoucherService
    {
        private IRepository<AccountsTransactions> _accountTransactionRepository;
        private IRepository<BankReceiptVoucher> _bankReceiptVoucherRepository;
        private IRepository<BankReceiptVoucherDetails> _bankReceiptVoucherDetailsRepository;
        private IRepository<ProgramSettings> _programsettingsRepository;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;
        private IRepository<PdcDetails> _pdcDetailsRepository;
        private readonly IRepository<UserTracking> _UserTracking;
        private readonly IRepository<UserFile> _UserFile;
        private readonly InspireErpDBContext context;
        private readonly IPaymentVoucherService _IPaymentVoucherService;
        public BankReceiptVoucherService(IRepository<AccountsTransactions> accountTransactionRepository, IRepository<ProgramSettings> programsettingsRepository,
            IRepository<VouchersNumbers> voucherNumbers,
            IRepository<BankReceiptVoucher> bankReceiptVoucherRepository, IRepository<BankReceiptVoucherDetails> bankReceiptVoucherDetailsRepository,
             IRepository<PdcDetails> pdcDetailsRepository
             , IRepository<UserTracking> UserTracking, IRepository<UserFile> UserFile, InspireErpDBContext _context,
              IPaymentVoucherService IPaymentVoucherService)
        {
            this._accountTransactionRepository = accountTransactionRepository;
            this._bankReceiptVoucherRepository = bankReceiptVoucherRepository;
            this._bankReceiptVoucherDetailsRepository = bankReceiptVoucherDetailsRepository;
            _programsettingsRepository = programsettingsRepository;
            _voucherNumbersRepository = voucherNumbers;
            _pdcDetailsRepository = pdcDetailsRepository;
            _UserTracking = UserTracking;
            _UserFile = UserFile;
            context = _context;
            _IPaymentVoucherService = IPaymentVoucherService;
        }

        public BankReceiptVoucher UpdateBankReceiptVoucher(BankReceiptVoucher bankReceiptVoucher, List<AccountsTransactions> accountsTransactions, List<BankReceiptVoucherDetails> bankReceiptVoucherDetails)
        {

            try
            {
                _bankReceiptVoucherRepository.BeginTransaction();
                _bankReceiptVoucherRepository.Update(bankReceiptVoucher);
                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    if (k.AccountsTransactionsTransSno == 0)
                    {
                        k.AccountsTransactionsVoucherType = VoucherType.BankPayment;
                        k.AccountsTransactionsStatus = AccountStatus.Approved;
                    }

                    return k;
                }).ToList();
                _accountTransactionRepository.UpdateList(accountsTransactions);
                _bankReceiptVoucherDetailsRepository.UpdateList(bankReceiptVoucherDetails);
                UserTracking trackingData = new UserTracking();
                trackingData.UserTrackingUserUserId = 1;
                trackingData.UserTrackingUserVpAction = "Update";
                trackingData.UserTrackingUserVpNo = bankReceiptVoucher.BankReceiptVoucherVoucherNo;
                trackingData.UserTrackingUserChangeDt = DateTime.Now;
                trackingData.UserTrackingUserChangeTime = DateTime.Now;
                trackingData.UserTrackingUserVpType = "BANKRECEIPTVOUCHER";
                _UserTracking.Insert(trackingData);
                _bankReceiptVoucherRepository.TransactionCommit();

            }
            catch (Exception ex)
            {
                _bankReceiptVoucherRepository.TransactionRollback();
                throw ex;
            }
            return this.GetSavedBankReceiptDetails(bankReceiptVoucher.BankReceiptVoucherVoucherNo);
        }

        public int DeleteBankReceiptVoucher(BankReceiptVoucher bankReceiptVoucher, List<AccountsTransactions> accountsTransactions, List<BankReceiptVoucherDetails> bankReceiptVoucherDetails)
        {
            int i = 0;
            try
            {
                _bankReceiptVoucherRepository.BeginTransaction();
                bankReceiptVoucher.BankReceiptVoucherDelStatus = true;
                bankReceiptVoucherDetails = bankReceiptVoucherDetails.Select((k) =>
                {
                    k.BankReceiptVoucherDetailsDelStatus = true;
                    return k;
                }).ToList();
                _bankReceiptVoucherDetailsRepository.UpdateList(bankReceiptVoucherDetails);
                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    k.AccountstransactionsDelStatus = true;
                    return k;
                }).ToList();
                _accountTransactionRepository.UpdateList(accountsTransactions);
                _bankReceiptVoucherRepository.Update(bankReceiptVoucher);
                var vchrnumer = _voucherNumbersRepository.GetAsQueryable().Where(k => k.VouchersNumbersVNo == bankReceiptVoucher.BankReceiptVoucherVoucherNo).FirstOrDefault();
                _voucherNumbersRepository.Update(vchrnumer);
                UserTracking trackingData = new UserTracking();
                trackingData.UserTrackingUserUserId = 1;
                trackingData.UserTrackingUserVpAction = "Delete";
                trackingData.UserTrackingUserVpNo = bankReceiptVoucher.BankReceiptVoucherVoucherNo;
                trackingData.UserTrackingUserChangeDt = DateTime.Now;
                trackingData.UserTrackingUserChangeTime = DateTime.Now;
                trackingData.UserTrackingUserVpType = "BANKRECEIPTVOUCHER";
                _UserTracking.Insert(trackingData);
                _bankReceiptVoucherRepository.TransactionCommit();
                i = 1;
            }
            catch (Exception ex)
            {
                _bankReceiptVoucherRepository.TransactionRollback();
                i = 0;
                throw ex;
            }
            return i;
        }

        public BankReceiptVoucher InsertBankReceiptVoucher(BankReceiptVoucher bankReceiptVoucher, List<AccountsTransactions> accountsTransactions, List<BankReceiptVoucherDetails> bankReceiptVoucherDetails)
        {
            try
            {
                _bankReceiptVoucherRepository.BeginTransaction();
                bankReceiptVoucher.BankReceiptVoucherVoucherNo = bankReceiptVoucher.BankReceiptVoucherVoucherNo;
                bankReceiptVoucherDetails = bankReceiptVoucherDetails.Select((x) =>
                {
                    x.BankReceiptVoucherDetailsSno = null;
                    x.BankReceiptVoucherDetailsVoucherNo = bankReceiptVoucher.BankReceiptVoucherVoucherNo;
                    return x;
                }).ToList();
                _bankReceiptVoucherDetailsRepository.UpdateList(bankReceiptVoucherDetails);

                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    k.AccountsTransactionsVoucherNo = bankReceiptVoucher.BankReceiptVoucherVoucherNo;
                    k.AccountsTransactionsVoucherType = VoucherType.BankPayment;
                    k.AccountsTransactionsStatus = AccountStatus.Approved;
                    return k;
                }).ToList();
                List<PdcDetails> _pdcDetails = new List<PdcDetails>();
                foreach (var item in bankReceiptVoucherDetails)
                {
                    if (item.BankReceiptVoucherDetailsPdc == true)
                    {
                        _pdcDetails.Add(new PdcDetails()
                        {
                            PDC_Details_V_No = bankReceiptVoucher.BankReceiptVoucherVoucherNo,
                            PDC_Details_V_Type = "BANK RECEIPT",
                            PDC_Details_Trans_Type = "RECEIVED",
                            PDC_Details_Trans_Date = DateTime.Now,
                            PDC_Details_PDC_Date = DateTime.Now,
                            PDC_Details_Cheque_No = item.BankReceiptVoucherDetailsChequeNo == null ? "" : item.BankReceiptVoucherDetailsChequeNo.ToString(),
                            PDC_Details_Cheque_Amount = 0,
                            PDC_Details_FC_Cheque_Amount = 0,
                            PDC_Details_Bank_Account_No = item.BankReceiptVoucherDetailsAcNo,
                            PDC_Details_Cheque_Bank_Name = "",
                            PDC_Details_Cheque_Status = "NC",
                            PDC_Details_PDC_Voucher_ID = null,
                            PDC_Details_PDC_Voucher_Date = null,
                            PDC_Details_PDC_Voucher_Narration = item.BankReceiptVoucherDetailsNarration,
                            PDC_Details_FSNO = item.BankReceiptVoucherDetailsFsno,
                            PDC_Details_User_ID = bankReceiptVoucher.BankReceiptVoucherUserId,
                            PDC_Details_Flat_ID = null,
                            PDC_Details_Building_ID = null,
                            PDC_Details_Contract = null,
                            PDC_Details_Old_Cheque_Status = null,
                            PDC_Details_PartyAccNo = null,
                            PDC_Details_DelStatus = false,
                        });
                    }
                }
                if (_pdcDetails != null && _pdcDetails.Count() > 0)
                {
                    _pdcDetailsRepository.InsertList(_pdcDetails);
                }
                _accountTransactionRepository.UpdateList(accountsTransactions);
                _bankReceiptVoucherRepository.Insert(bankReceiptVoucher);
                UserTracking trackingData = new UserTracking();
                trackingData.UserTrackingUserUserId = 1;
                trackingData.UserTrackingUserVpAction = "Insert";
                trackingData.UserTrackingUserVpNo = bankReceiptVoucher.BankReceiptVoucherVoucherNo;
                trackingData.UserTrackingUserChangeDt = DateTime.Now;
                trackingData.UserTrackingUserChangeTime = DateTime.Now;
                trackingData.UserTrackingUserVpType = "BANKRECEIPTVOUCHER";
                _UserTracking.Insert(trackingData);
                _bankReceiptVoucherRepository.TransactionCommit();
                return this.GetSavedBankReceiptDetails(bankReceiptVoucher.BankReceiptVoucherVoucherNo);

            }
            catch (Exception ex)
            {
                _bankReceiptVoucherRepository.TransactionRollback();
                throw ex;
            }
        }

        public void InsertPDCDetails(List<PdcDetails> pdcDetails)
        {
            _pdcDetailsRepository.InsertList(pdcDetails);
        }
        public IEnumerable<AccountsTransactions> GetAllTransaction()
        {
            return _accountTransactionRepository.GetAll();
        }
        public IEnumerable<BankReceiptVoucher> GetBankReceiptVouchers()
        {
            IEnumerable<BankReceiptVoucher> bankReceiptVouchers = _bankReceiptVoucherRepository.GetAll().Where(k => k.BankReceiptVoucherDelStatus == false || k.BankReceiptVoucherDelStatus == null);
            return bankReceiptVouchers;
        }
        public BankReceiptVoucher GetSavedBankReceiptDetails(string pvno)
        {
            BankReceiptVoucher voucherdata = new BankReceiptVoucher();
            var userTrackngData = (from hdr in context.UserTracking
                                   join dtl in context.UserFile on (long)hdr.UserTrackingUserUserId equals dtl.UserId
                                   where (hdr.UserTrackingUserVpType == "BANKRECEIPTVOUCHER" && hdr.UserTrackingUserVpNo == pvno)
                                   select new
                                   {
                                       VPAction = hdr.UserTrackingUserVpAction,
                                       UserId = dtl.UserId,
                                       VPType = hdr.UserTrackingUserVpType,
                                       ChangeDt = hdr.UserTrackingUserChangeDt,
                                       ChangeTime = hdr.UserTrackingUserChangeTime,
                                       VPNo = hdr.UserTrackingUserVpNo,
                                   }).AsQueryable();
            voucherdata = _bankReceiptVoucherRepository.GetAsQueryable().Where(k => k.BankReceiptVoucherVoucherNo == pvno).SingleOrDefault();
            voucherdata.UserTrackingData = userTrackngData;
            voucherdata.AccountsTransactions = _accountTransactionRepository.GetAsQueryable().Where(c => c.AccountsTransactionsVoucherNo == pvno).ToList();
            voucherdata.BankReceiptVoucherDetails = _bankReceiptVoucherDetailsRepository.GetAsQueryable().Where(x => x.BankReceiptVoucherDetailsVoucherNo == pvno).ToList();
            voucherdata.AllocationDetails = this._IPaymentVoucherService.GetAllocationDetails(voucherdata.BankReceiptVoucherDrAcNo, voucherdata.BankReceiptVoucherVoucherNo);
            return voucherdata;
        }

        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate)
        {
            try
            {
                var prefix = this._programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.BRV_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(_voucherNumbersRepository.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.BankReceipt)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;
                var voucherNo = "BRV" + vnoMaxVal;
                var voucherNumber = _voucherNumbersRepository.GetAll().FirstOrDefault(o => o.VouchersNumbersVNo == voucherNo);
                if (voucherNo != null)
                {

                    var num = _voucherNumbersRepository.GetAll().Where(o => o.VouchersNumbersVNo.Contains("BRV")).OrderByDescending(o => o.VouchersNumbersVNoNu).FirstOrDefault().VouchersNumbersVNoNu + 1;
                    VouchersNumbers vouchersNumbers = new VouchersNumbers
                    {
                        VouchersNumbersVNo = "BRV" + num,
                        VouchersNumbersVNoNu = (decimal)num,
                        VouchersNumbersVType = VoucherType.BankReceipt,
                        VouchersNumbersFsno = 1,
                        VouchersNumbersStatus = AccountStatus.Pending,
                        VouchersNumbersVoucherDate = DateTime.Now
                    };
                    _voucherNumbersRepository.Insert(vouchersNumbers);
                    return vouchersNumbers;
                }
                else
                {
                    VouchersNumbers vouchersNumbers = new VouchersNumbers
                    {
                        VouchersNumbersVNo = "BRV" + vnoMaxVal,
                        VouchersNumbersVNoNu = vnoMaxVal,
                        VouchersNumbersVType = VoucherType.BankReceipt,
                        VouchersNumbersFsno = 1,
                        VouchersNumbersStatus = AccountStatus.Pending,
                        VouchersNumbersVoucherDate = VoucherGenDate
                    };
                    _voucherNumbersRepository.Insert(vouchersNumbers);
                    return vouchersNumbers;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
