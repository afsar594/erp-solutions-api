using Inspire.Erp.Application.Account.Interfaces;
using Inspire.Erp.Application.Common;
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
namespace Inspire.Erp.Application.Account.Implementations
{
    public class PurchaseJournalService : IPurchaseJournalService
    {
        private IRepository<AccountsTransactions> _accountTransactionRepository;
        private IRepository<PurchaseJournal> _purchaseJournalRepository;
        private IRepository<PurchaseJournalDetails> _purchaseJournalDetailsRepository;
        private IRepository<ProgramSettings> _programsettingsRepository;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;
        private readonly ILogger<PaymentVoucherService> _logger;

        public PurchaseJournalService(IRepository<AccountsTransactions> accountTransactionRepository, IRepository<ProgramSettings> programsettingsRepository,
            IRepository<VouchersNumbers> voucherNumbers, ILogger<PaymentVoucherService> logger,
            IRepository<PurchaseJournal> purchaseJournalRepository, IRepository<PurchaseJournalDetails> purchaseJournalDetailsRepository)
        {
            this._accountTransactionRepository = accountTransactionRepository;
            this._purchaseJournalRepository = purchaseJournalRepository;
            this._purchaseJournalDetailsRepository = purchaseJournalDetailsRepository;
            _programsettingsRepository = programsettingsRepository;
            _voucherNumbersRepository = voucherNumbers;

        }
        //public PurchaseJournal InsertPurchaseJournal(PurchaseJournal purchaseJournal,
        //  List<PurchaseJournalDetails> purchaseJournalDetails)
        //{
        //    List<AccountsTransactions> accountsTransactions = new List<AccountsTransactions>();
        //    try {
        //        _accountTransactionRepository.BeginTransaction();

        //        _purchaseJournalRepository.Insert(purchaseJournal);
        //        _purchaseJournalDetailsRepository.InsertList(purchaseJournalDetails.Select(k => 
        //        {
        //            k.PurchaseJournalDetailsVno = purchaseJournal.PurchaseJournalVno;
        //            return k;
        //        }).ToList()) ;
        //        //int acctTransMaxVal = Convert.ToInt32(_accountTransactionRepository.GetAsQueryable()
        //        //                                        .Where(x => x.AccountsTransactionsTransSno > 0)
        //        //                                        .DefaultIfEmpty()
        //        //                                        .Max(o => o == null ? 0 : o.AccountsTransactionsTransSno))  + 1;

        //        AccountsTransactions creditData = new AccountsTransactions();
        //        creditData.AccountsTransactionsAccNo = purchaseJournal.PurchaseJournalCrAcNo;
        //        creditData.AccountsTransactionsTransDate = (DateTime) purchaseJournal.PurchaseJournalDate.Value.Date;
        //        creditData.AccountsTransactionsParticulars = purchaseJournal.PurchaseJournalNarration;
        //        creditData.AccountsTransactionsCredit = Convert.ToDecimal(purchaseJournal.PurchaseJournalDbAmount);
        //        creditData.AccountsTransactionsFcCredit = Convert.ToDecimal(purchaseJournal.PurchaseJournalDbAmount);
        //        creditData.AccountsTransactionsVoucherType = VoucherType.PurchaseJournal_TYPE;
        //        creditData.AccountsTransactionsTstamp = DateTime.Now;
        //        creditData.AccountsTransactionsVoucherNo = purchaseJournal.PurchaseJournalVno;
        //        creditData.AccountsTransactionsDescription = purchaseJournal.PurchaseJournalNarration;
        //        creditData.AccountsTransactionsStatus = AccountStatus.Approved;
        //        accountsTransactions.Add(creditData);
        //        //int transactionIntialCount = Convert.ToInt32(creditData.AccountsTransactionsTransSno + 1);
        //        foreach (var debit in purchaseJournalDetails)
        //        {
        //            //transactionIntialCount = transactionIntialCount + 1;
        //            AccountsTransactions debitData = new AccountsTransactions();
        //            debitData.AccountsTransactionsAccNo = debit.PurchaseJournalDetailsAcNo;
        //            debitData.AccountsTransactionsTransDate = (DateTime) creditData.AccountsTransactionsTransDate;
        //            debitData.AccountsTransactionsParticulars = debit.PurchaseJournalDetailsNarration;
        //            debitData.AccountsTransactionsDebit = Convert.ToDecimal(debit.PurchaseJournalDetailsDbAmount);
        //            debitData.AccountsTransactionsFcDebit = Convert.ToDecimal(debit.PurchaseJournalDetailsDbAmount);
        //            debitData.AccountsTransactionsVoucherType = VoucherType.PurchaseJournal_TYPE;
        //            debitData.AccountsTransactionsTstamp = DateTime.Now;
        //            debitData.AccountsTransactionsVoucherNo = purchaseJournal.PurchaseJournalVno;
        //            debitData.AccountsTransactionsDescription = debit.PurchaseJournalDetailsNarration;
        //            debitData.AccountsTransactionsStatus = AccountStatus.Approved;
        //            accountsTransactions.Add(debitData);

        //        }
        //        _accountTransactionRepository.InsertList(accountsTransactions);
        //        _accountTransactionRepository.TransactionCommit();
        //    }
        //    catch(Exception ex) {
        //        _accountTransactionRepository.TransactionRollback();
        //        throw ex;
        //    }

        //    return this.GetSavedPurchaseJournalDetails(purchaseJournal.PurchaseJournalVno);

        //}

        public PurchaseJournalModel UpdatePurchaseJournal(PurchaseJournal purchaseJournal, List<AccountsTransactions> accountsTransactions, List<PurchaseJournalDetails> purchaseJournalDetails)
        {

            try
            {
                _purchaseJournalRepository.BeginTransaction();


                purchaseJournal.PurchaseJournalDetails = purchaseJournalDetails.Select((k) =>
                {
                    //if (k.PurchaseJournalDetailsId == 0)
                    //{
                    k.PurchaseJournalId = purchaseJournal.PurchaseJournalId;
                    k.PurchaseJournalDetailsVno = purchaseJournal.PurchaseJournalVno;
                    //k.PurchaseJournalDetailsId = 0;
                    //}

                    return k;
                }).ToList();

                _purchaseJournalRepository.Update(purchaseJournal);





                //_purchaseJournalRepository.Update(purchaseJournal);
                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    if (k.AccountsTransactionsTransSno == 0)
                    {
                        k.AccountsTransactionsVoucherType = VoucherType.PurchaseJournal_TYPE;
                        k.AccountsTransactionsStatus = AccountStatus.Approved;
                    }

                    return k;
                }).ToList();
                _accountTransactionRepository.UpdateList(accountsTransactions);


                //purchaseJournalDetails = purchaseJournalDetails.Select((k) =>
                //{
                //    if (k.PurchaseJournalId == 0)
                //    {
                //        k.PurchaseJournalId = purchaseJournal.PurchaseJournalId;
                //        k.PurchaseJournalDetailsVno = purchaseJournal.PurchaseJournalVno;
                //    }

                //    return k;
                //}).ToList();


                //_purchaseJournalDetailsRepository.UpdateList(purchaseJournalDetails);
                _purchaseJournalRepository.TransactionCommit();

            }
            catch (Exception ex)
            {
                _purchaseJournalRepository.TransactionRollback();
                throw ex;
            }

            return this.GetSavedPurchaseJournalDetails(purchaseJournal.PurchaseJournalVno);
        }

        public int DeletePurchaseJournal(PurchaseJournal purchaseJournal, List<AccountsTransactions> accountsTransactions, List<PurchaseJournalDetails> purchaseJournalDetails)
        {
            int i = 0;
            try
            {
                _purchaseJournalRepository.BeginTransaction();

                purchaseJournal.PurchaseJournalDelStatus = true;

                purchaseJournalDetails = purchaseJournalDetails.Select((k) =>
                {
                    k.PurchaseJournalDetailsDelStatus = true;
                    return k;
                }).ToList();

                _purchaseJournalDetailsRepository.UpdateList(purchaseJournalDetails);

                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    k.AccountstransactionsDelStatus = true;
                    return k;
                }).ToList();
                _accountTransactionRepository.UpdateList(accountsTransactions);

                _purchaseJournalRepository.Update(purchaseJournal);

                var vchrnumer = _voucherNumbersRepository.GetAsQueryable().Where(k => k.VouchersNumbersVNo == purchaseJournal.PurchaseJournalVno).FirstOrDefault();

                _voucherNumbersRepository.Update(vchrnumer);

                _purchaseJournalRepository.TransactionCommit();
                i = 1;
            }
            catch (Exception ex)
            {
                _purchaseJournalRepository.TransactionRollback();
                i = 0;
                throw ex;
            }


            return i;

        }
        public PurchaseJournalModel InsertPurchaseJournal(PurchaseJournal purchaseJournal, List<AccountsTransactions> accountsTransactions, List<PurchaseJournalDetails> purchaseJournalDetails)
        {
            try
            {
                _purchaseJournalRepository.BeginTransaction();
                string purchaseJournalNumber = this.GenerateVoucherNo(purchaseJournal.PurchaseJournalDate.Value).VouchersNumbersVNo;
                purchaseJournal.PurchaseJournalVno = purchaseJournalNumber;


                long maxcount = 0;
                maxcount = Convert.ToInt32(
                    _purchaseJournalRepository.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.PurchaseJournalId) + 1);
                purchaseJournal.PurchaseJournalId = maxcount;

                purchaseJournal.PurchaseJournalDetails = purchaseJournal.PurchaseJournalDetails.AsEnumerable().Where(k => k != null).Select(x =>
                {
                    x.PurchaseJournalDetailsVno = purchaseJournalNumber;
                    return x;
                }).ToList();

                _purchaseJournalRepository.Insert(purchaseJournal);



                ////purchaseJournalDetails = purchaseJournalDetails.Select((x) =>
                ////{
                ////    x.PurchaseJournalId = maxcount;
                ////    x.PurchaseJournalDetailsVno = purchaseJournalNumber;
                ////    return x;
                ////}).ToList();
                ////_purchaseJournalDetailsRepository.InsertList(purchaseJournalDetails);

                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    k.AccountsTransactionsVoucherNo = purchaseJournalNumber;
                    k.AccountsTransactionsVoucherType = VoucherType.PurchaseJournal_TYPE;
                    k.AccountsTransactionsStatus = AccountStatus.Approved;
                    return k;
                }).ToList();
                _accountTransactionRepository.InsertList(accountsTransactions);
               

                _purchaseJournalRepository.TransactionCommit();
                return this.GetSavedPurchaseJournalDetails(purchaseJournal.PurchaseJournalVno);

            }
            catch (Exception ex)
            {
                _purchaseJournalRepository.TransactionRollback();
                throw ex;
            }



        }
        public IEnumerable<AccountsTransactions> GetAllTransaction()
        {
            return _accountTransactionRepository.GetAll();
        }
        public IEnumerable<PurchaseJournal> GetPurchaseJournal()
        {
            IEnumerable<PurchaseJournal> purchaseJournal_ALL = _purchaseJournalRepository.GetAll().Where(k => k.PurchaseJournalDelStatus == false || k.PurchaseJournalDelStatus == null);
            return purchaseJournal_ALL;
        }
        public PurchaseJournalModel GetSavedPurchaseJournalDetails(string pvno)
        {
            PurchaseJournalModel purchaseJournalModel = new PurchaseJournalModel();
            //purchaseJournalModel.purchaseJournal = _purchaseJournalRepository.GetAsQueryable().Where(k => k.PurchaseJournalVno == pvno && k.PurchaseJournalDelStatus == false).SingleOrDefault();

            purchaseJournalModel.purchaseJournal = _purchaseJournalRepository.GetAsQueryable().Where(k => k.PurchaseJournalVno == pvno).SingleOrDefault();



            purchaseJournalModel.accountsTransactions = _accountTransactionRepository.GetAsQueryable().Where(c => c.AccountsTransactionsVoucherNo == pvno && c.AccountsTransactionsVoucherType == VoucherType.PurchaseJournal_TYPE && (c.AccountstransactionsDelStatus == false || c.AccountstransactionsDelStatus == null)).ToList();
            purchaseJournalModel.purchaseJournalDetails = _purchaseJournalDetailsRepository.GetAsQueryable().Where(x => x.PurchaseJournalDetailsVno == pvno && (x.PurchaseJournalDetailsDelStatus == false || x.PurchaseJournalDetailsDelStatus == null)).ToList();
            return purchaseJournalModel;


        }

        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate)
        {
            try
            {






                var prefix = this._programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.PurchaseJournal_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(_voucherNumbersRepository.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.PurchaseJournal_TYPE)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;


                //var prefix = "CN";
                //int vnoMaxVal = 1;


                VouchersNumbers vouchersNumbers = new VouchersNumbers
                {
                    VouchersNumbersVNo = prefix + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.PurchaseJournal_TYPE,
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




        //using Microsoft.Extensions.Logging;
        //private readonly ILogger<PaymentVoucherService> _logger;
        //IRepository<VouchersNumbers> voucherNumbers, ILogger<PaymentVoucherService> logger,
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



    }
}
