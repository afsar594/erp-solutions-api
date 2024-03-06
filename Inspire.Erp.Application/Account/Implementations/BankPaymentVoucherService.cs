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
    public class BankPaymentVoucherService : IBankPaymentVoucherService
    {
        private IRepository<AccountsTransactions> _accountTransactionRepository;
        private IRepository<BankPaymentVoucher> _bankPaymentVoucherRepository;
        private IRepository<BankPaymentVoucherDetails> _bankPaymentVoucherDetailsRepository;
        private IRepository<ProgramSettings> _programsettingsRepository;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;
        private IRepository<PdcDetails> _pdcDetailsRepository;

        private readonly IRepository<UserTracking> _UserTracking;
        private readonly IRepository<UserFile> _UserFile;
        private readonly InspireErpDBContext context;
        private readonly IPaymentVoucherService _IPaymentVoucherService;
        public BankPaymentVoucherService(IRepository<AccountsTransactions> accountTransactionRepository, IRepository<ProgramSettings> programsettingsRepository,
            IRepository<VouchersNumbers> voucherNumbers,
            IRepository<BankPaymentVoucher> bankPaymentVoucherRepository, IRepository<BankPaymentVoucherDetails> bankPaymentVoucherDetailsRepository,
            IRepository<PdcDetails> pdcDetailsRepository, IRepository<UserTracking> UserTracking, IRepository<UserFile> UserFile, InspireErpDBContext _context,
            IPaymentVoucherService IPaymentVoucherService)
        {
            this._accountTransactionRepository = accountTransactionRepository;
            this._bankPaymentVoucherRepository = bankPaymentVoucherRepository;
            this._bankPaymentVoucherDetailsRepository = bankPaymentVoucherDetailsRepository;
            _programsettingsRepository = programsettingsRepository;
            _voucherNumbersRepository = voucherNumbers;
            _pdcDetailsRepository = pdcDetailsRepository;
            _UserTracking = UserTracking;
            _UserFile = UserFile;
            context = _context;
            _IPaymentVoucherService = IPaymentVoucherService;
        }
        public BankPaymentVoucher UpdateBankPaymentVoucher(BankPaymentVoucher bankPaymentVoucher, List<AccountsTransactions> accountsTransactions, List<BankPaymentVoucherDetails> bankPaymentVoucherDetails)
        {

            try
            {
                _bankPaymentVoucherRepository.BeginTransaction();

                _bankPaymentVoucherRepository.Update(bankPaymentVoucher);
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
                _bankPaymentVoucherDetailsRepository.UpdateList(bankPaymentVoucherDetails);

                UserTracking trackingData = new UserTracking();
                trackingData.UserTrackingUserUserId = 1;
                trackingData.UserTrackingUserVpAction = "Update";
                trackingData.UserTrackingUserVpNo = bankPaymentVoucher.BankPaymentVoucherVoucherNo;
                trackingData.UserTrackingUserChangeDt = DateTime.Now;
                trackingData.UserTrackingUserChangeTime = DateTime.Now;
                trackingData.UserTrackingUserVpType = "BANKPAYMENTVOUCHER";
                _UserTracking.Insert(trackingData);
                _bankPaymentVoucherRepository.TransactionCommit();

            }
            catch (Exception ex)
            {
                _bankPaymentVoucherRepository.TransactionRollback();
                throw ex;
            }

            return this.GetSavedBankPaymentDetails(bankPaymentVoucher.BankPaymentVoucherVoucherNo);
        }

        public int DeleteBankPaymentVoucher(BankPaymentVoucher bankPaymentVoucher, List<AccountsTransactions> accountsTransactions, List<BankPaymentVoucherDetails> bankPaymentVoucherDetails)
        {
            int i = 0;
            try
            {
                _bankPaymentVoucherRepository.BeginTransaction();

                bankPaymentVoucher.BankPaymentVoucherDelStatus = true;

                bankPaymentVoucherDetails = bankPaymentVoucherDetails.Select((k) =>
                {
                    k.BankPaymentVoucherDetailsDelStatus = true;
                    return k;
                }).ToList();

                _bankPaymentVoucherDetailsRepository.UpdateList(bankPaymentVoucherDetails);

                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    k.AccountstransactionsDelStatus = true;
                    return k;
                }).ToList();
                _accountTransactionRepository.UpdateList(accountsTransactions);

                _bankPaymentVoucherRepository.Update(bankPaymentVoucher);

                var vchrnumer = _voucherNumbersRepository.GetAsQueryable().Where(k => k.VouchersNumbersVNo == bankPaymentVoucher.BankPaymentVoucherVoucherNo).FirstOrDefault();

                _voucherNumbersRepository.Update(vchrnumer);
                UserTracking trackingData = new UserTracking();
                trackingData.UserTrackingUserUserId = 1;
                trackingData.UserTrackingUserVpAction = "Delete";
                trackingData.UserTrackingUserVpNo = bankPaymentVoucher.BankPaymentVoucherVoucherNo;
                trackingData.UserTrackingUserChangeDt = DateTime.Now;
                trackingData.UserTrackingUserChangeTime = DateTime.Now;
                trackingData.UserTrackingUserVpType = "BANKPAYMENTVOUCHER";
                _UserTracking.Insert(trackingData);
                _bankPaymentVoucherRepository.TransactionCommit();
                i = 1;
            }
            catch (Exception ex)
            {
                _bankPaymentVoucherRepository.TransactionRollback();
                i = 0;
                throw ex;
            }


            return i;

        }
        public BankPaymentVoucher InsertBankPaymentVoucher(BankPaymentVoucher bankPaymentVoucher, List<AccountsTransactions> accountsTransactions, List<BankPaymentVoucherDetails> bankPaymentVoucherDetails)
        {
            try
            {
                _bankPaymentVoucherRepository.BeginTransaction();
                string bankPaymentVoucherNumber = this.GenerateVoucherNo(bankPaymentVoucher.BankPaymentVoucherVDt.Value).VouchersNumbersVNo;
                //bankPaymentVoucher.BankPaymentVoucherVoucherNo = bankPaymentVoucher.BankPaymentVoucherVoucherNo;
                bankPaymentVoucher.BankPaymentVoucherVoucherNo = bankPaymentVoucherNumber;

        
                bankPaymentVoucherDetails = bankPaymentVoucherDetails.Select((x) =>
                {
                    x.BankPaymentVoucherDetailsSno = null;
                    x.BankPaymentVoucherDetailsVoucherNo = bankPaymentVoucher.BankPaymentVoucherVoucherNo;
                    return x;
                }).ToList();
                _bankPaymentVoucherDetailsRepository.InsertList(bankPaymentVoucherDetails);

                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    k.AccountsTransactionsVoucherNo = bankPaymentVoucher.BankPaymentVoucherVoucherNo;
                    k.AccountsTransactionsVoucherType = VoucherType.BankPayment;
                    k.AccountsTransactionsStatus = AccountStatus.Approved;
                    return k;
                }).ToList();

                List<PdcDetails> _pdcDetails = new List<PdcDetails>();
                foreach (var item in bankPaymentVoucherDetails)
                {
                    if (item.BankPaymentVoucherDetailsPdc == true)
                    {
                        _pdcDetails.Add(new PdcDetails()
                        {
                            PDC_Details_V_No = bankPaymentVoucher.BankPaymentVoucherVoucherNo,
                            PDC_Details_V_Type = "BANK PAYMENT",
                            PDC_Details_Trans_Type = "RECEIVED",
                            PDC_Details_Trans_Date = DateTime.Now,
                            PDC_Details_PDC_Date = DateTime.Now,
                            PDC_Details_Cheque_No = item.BankPaymentVoucherDetailsChequeNo == null ? "" : item.BankPaymentVoucherDetailsChequeNo.ToString(),
                            PDC_Details_Cheque_Amount = item.BankPaymentVoucherDetailsDrAmount,
                            PDC_Details_FC_Cheque_Amount = item.BankPaymentVoucherDetailsDrAmount,
                            PDC_Details_Bank_Account_No = item.BankPaymentVoucherDetailsAcNo,
                            PDC_Details_Cheque_Bank_Name = "",
                            PDC_Details_Cheque_Status = "NC",
                            PDC_Details_PDC_Voucher_ID = null,
                            PDC_Details_PDC_Voucher_Date = DateTime.Now,
                            PDC_Details_PDC_Voucher_Narration = item.BankPaymentVoucherDetailsNarration,
                            PDC_Details_FSNO = item.BankPaymentVoucherDetailsFsno,
                            PDC_Details_User_ID = bankPaymentVoucher.BankPaymentVoucherUserId,
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

                _accountTransactionRepository.InsertList(accountsTransactions);
                _bankPaymentVoucherRepository.Insert(bankPaymentVoucher);

                UserTracking trackingData = new UserTracking();
                trackingData.UserTrackingUserUserId = 1;
                trackingData.UserTrackingUserVpAction = "Insert";
                trackingData.UserTrackingUserVpNo = bankPaymentVoucher.BankPaymentVoucherVoucherNo;
                trackingData.UserTrackingUserChangeDt = DateTime.Now;
                trackingData.UserTrackingUserChangeTime = DateTime.Now;
                trackingData.UserTrackingUserVpType = "BANKPAYMENTVOUCHER";
                _UserTracking.Insert(trackingData);
                _bankPaymentVoucherRepository.TransactionCommit();
                return this.GetSavedBankPaymentDetails(bankPaymentVoucher.BankPaymentVoucherVoucherNo);

            }
            catch (Exception ex)
            {
                _bankPaymentVoucherRepository.TransactionRollback();
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
        public IEnumerable<BankPaymentVoucher> GetBankPaymentVouchers()
        {
            IEnumerable<BankPaymentVoucher> bankPaymentVouchers = _bankPaymentVoucherRepository.GetAll().Where(k => k.BankPaymentVoucherDelStatus == false || k.BankPaymentVoucherDelStatus == null);
            return bankPaymentVouchers;
        }
        public BankPaymentVoucher GetSavedBankPaymentDetails(string pvno)
        {
            BankPaymentVoucher voucherdata = new BankPaymentVoucher();
            var userTrackngData = (from hdr in context.UserTracking
                                   join dtl in context.UserFile on (long)hdr.UserTrackingUserUserId equals dtl.UserId
                                   where (hdr.UserTrackingUserVpType == "BANKPAYMENTVOUCHER" && hdr.UserTrackingUserVpNo == pvno)
                                   select new UserTrackingDisplay
                                   {
                                       //VPAction = hdr.UserTrackingUserVpAction,
                                       //UserId = dtl.UserId,
                                       //VPType = hdr.UserTrackingUserVpType,
                                       //ChangeDt = hdr.UserTrackingUserChangeDt,
                                       //ChangeTime = hdr.UserTrackingUserChangeTime,
                                       //VPNo = hdr.UserTrackingUserVpNo,

                                       Action = hdr.UserTrackingUserVpAction,
                                       Id = dtl.UserId.ToString(),
                                       VType = hdr.UserTrackingUserVpType,
                                       Date = hdr.UserTrackingUserChangeDt.Value.ToString("dd/MM/yyyy"),
                                       Time = hdr.UserTrackingUserChangeTime.Value.ToString("hh:mm t"),
                                       VNo = hdr.UserTrackingUserVpNo,

                                   }).ToList();
            voucherdata = _bankPaymentVoucherRepository.GetAsQueryable().Where(k => k.BankPaymentVoucherVoucherNo == pvno).SingleOrDefault();
            voucherdata.UserTrackingData = userTrackngData;
            voucherdata.AccountsTransactions = _accountTransactionRepository.GetAsQueryable().Where(c => c.AccountsTransactionsVoucherNo == pvno).ToList();
            voucherdata.BankPaymentVoucherDetails = _bankPaymentVoucherDetailsRepository.GetAsQueryable().Where(x => x.BankPaymentVoucherDetailsVoucherNo == pvno).ToList();
            voucherdata.AllocationDetails = this._IPaymentVoucherService.GetAllocationDetails(voucherdata.BankPaymentVoucherCrAcNo, voucherdata.BankPaymentVoucherVoucherNo);
            return voucherdata;
        }

        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate)
        {
            try
            {
                var prefix = this._programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.BPV_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(context.VouchersNumbers.AsNoTracking().ToList()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.BankPayment)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;
                VouchersNumbers vouchersNumbers = new VouchersNumbers
                {
                    VouchersNumbersVNo = "BPV" + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.BankPayment,
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
    }
}

