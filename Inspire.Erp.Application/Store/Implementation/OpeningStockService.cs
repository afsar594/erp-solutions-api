////using System;
////using System.Collections.Generic;
////using System.Text;

////namespace Inspire.Erp.Application.Store.Implementation
////{
////    class OpeningStockService
////    {
////    }
////}
///
using Inspire.Erp.Application.Store.Interfaces;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Inspire.Erp.Application.Store.Implementation
{
    public class OpeningStockService : IOpeningStockService
    {
        private IRepository<AccountsTransactions> _accountTransactionRepository;
        private IRepository<OpeningStock> _openingStockRepository;
        private IRepository<StockRegister> _stockRegisterRepository;
        private IRepository<ProgramSettings> _programsettingsRepository;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;

        private readonly ILogger<OpeningStockService> _logger;

        public OpeningStockService(IRepository<AccountsTransactions> accountTransactionRepository, IRepository<ProgramSettings> programsettingsRepository,
            IRepository<VouchersNumbers> voucherNumbers, ILogger<OpeningStockService> logger,
            IRepository<OpeningStock> openingStockRepository, IRepository<StockRegister> stockRegisterRepository)
        {
            this._accountTransactionRepository = accountTransactionRepository;
            this._openingStockRepository = openingStockRepository;
            this._stockRegisterRepository = stockRegisterRepository;
            _programsettingsRepository = programsettingsRepository;
            _voucherNumbersRepository = voucherNumbers;
            _logger = logger;

        }
       
        public OpeningStockModel UpdateOpeningStock(OpeningStock openingStock, List<AccountsTransactions> accountsTransactions,
                                                        List<StockRegister> stockregister)
        {

            try
            {
                _openingStockRepository.BeginTransaction();

                _openingStockRepository.Update(openingStock);
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
                _stockRegisterRepository.UpdateList(stockregister);
                _openingStockRepository.TransactionCommit();

            }
            catch (Exception ex)
            {
                _openingStockRepository.TransactionRollback();
                throw ex;
            }

            return this.GetSavedOPStockDetails(openingStock.OpeningStockPurchaseId);
        }

        public int DeleteOpeningStock(OpeningStock openingStock, List<AccountsTransactions> accountsTransactions, List<StockRegister> stockRegister)
        {
            int i = 0;
            try
            {
                _openingStockRepository.BeginTransaction();

                openingStock.OpeningStockDelStatus = true;

                stockRegister = stockRegister.Select((k) => {
                    k.StockRegisterDelStatus = true;
                    return k;
                }).ToList();

                _stockRegisterRepository.UpdateList(stockRegister);

                accountsTransactions = accountsTransactions.Select((k) => {
                    k.AccountstransactionsDelStatus = true;
                    return k;
                }).ToList();
                _accountTransactionRepository.UpdateList(accountsTransactions);

                _openingStockRepository.Update(openingStock);

                var vchrnumer = _voucherNumbersRepository.GetAsQueryable().Where(k => k.VouchersNumbersVNo == openingStock.OpeningStockPurchaseId).FirstOrDefault();

                _voucherNumbersRepository.Update(vchrnumer);

                _openingStockRepository.TransactionCommit();
                i = 1;
            }
            catch (Exception ex)
            {
                _openingStockRepository.TransactionRollback();
                i = 0;
                throw ex;
            }


            return i;

        }
        public OpeningStockModel InsertOpeningStock(OpeningStock openingStock, List<AccountsTransactions> accountsTransactions,
                                                        List<StockRegister> stockRegister)
        {
            try
            {
                _openingStockRepository.BeginTransaction();
                string openingSNumber = (openingStock.OpeningStockPurchaseId == null || openingStock.OpeningStockPurchaseId.Trim() == "") ?
                                              this.GenerateVoucherNo(openingStock.OpeningStockExpDate.Value).VouchersNumbersVNo : openingStock.OpeningStockPurchaseId;
                openingStock.OpeningStockPurchaseId= openingSNumber;

                StockRegister stockRegster = new StockRegister
                {

                    StockRegisterPurchaseID = openingSNumber,
                    StockRegisterRefVoucherNo = openingSNumber,
                    StockRegisterVoucherDate = DateTime.Now,
                    StockRegisterSno = 0,
                    StockRegisterBatchCode = openingStock.OpeningStockBatchCode,
                    StockRegisterMaterialID = openingStock.OpeningStockMaterialId,
                    StockRegisterQuantity = Convert.ToDecimal(openingStock.OpeningStockQty),
                    StockRegisterSIN = Convert.ToDecimal(openingStock.OpeningStockQty),
                    StockRegisterSout = 0,
                    StockRegisterRate = Convert.ToDecimal(openingStock.OpeningStockUnitRate),
                    StockRegisterAmount = Convert.ToDecimal(openingStock.OpeningStockAmount),
                    StockRegisterFCAmount = Convert.ToDecimal(openingStock.OpeningStockFcAmount), 
                    StockRegisterRemarks = openingStock.OpeningStockRemakrs,
                    StockRegisterUnitID = openingStock.OpeningStockUnitId,
                    StockRegisterLocationID = openingStock.OpeningStockLocationId,
                    StockRegisterJobID = openingStock.OpeningStockJobId,
                    StockRegisterFSNO = openingStock.OpeningStockFsno

                };

                _stockRegisterRepository.InsertList(stockRegister);

                accountsTransactions = accountsTransactions.Select((k) => {
                    k.AccountsTransactionsVoucherNo = openingSNumber;
                    k.AccountsTransactionsVoucherType = VoucherType.Payment;
                    k.AccountsTransactionsStatus = AccountStatus.Approved;
                    return k;
                }).ToList();
                _accountTransactionRepository.InsertList(accountsTransactions);
                _openingStockRepository.Insert(openingStock);
                _openingStockRepository.TransactionCommit();
                return this.GetSavedOPStockDetails(openingStock.OpeningStockPurchaseId);

            }
            catch (Exception ex)
            {
                _openingStockRepository.TransactionRollback();
                throw ex;
            }

        }
        public IEnumerable<AccountsTransactions> GetAllTransaction()
        {
            return _accountTransactionRepository.GetAll();
        }
        public IEnumerable<OpeningStock> GetOpeningStocks()
        {
            IEnumerable<OpeningStock> openingStocks = _openingStockRepository.GetAll().Where(k => k.OpeningStockDelStatus == false || k.OpeningStockDelStatus == null);
            return openingStocks;
        }
        public OpeningStockModel GetSavedOPStockDetails(string pvno)
        {
            if (_voucherNumbersRepository.GetAsQueryable().Any(x => x.VouchersNumbersVNo == pvno))
            {
                OpeningStockModel voucherdata = _voucherNumbersRepository.GetAsQueryable().Where(x => x.VouchersNumbersVNo == pvno)
                                                .Include(k => k.AccountsTransactions)
                                                .Include(k => k.PaymentVoucherDetails).AsEnumerable()
                                                .Select((k) => new OpeningStockModel
                                                {
                                                    AccountsTransactions = k.AccountsTransactions.Where(x => x.AccountstransactionsDelStatus == false).ToList(),
                                                   // stockRegister = k.StockRegisters.Where(x => x.StockRegisterDelStatus == false).ToList()
                                                })
                                                .SingleOrDefault();
                voucherdata.openingStock = _openingStockRepository.GetAsQueryable().Where(k => k.OpeningStockPurchaseId == pvno).FirstOrDefault();
                return voucherdata;

            }
            return null;

        }

        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate)
        {
            try
            {
                ////var vno=  this._paymentVoucherRepository.GetAsQueryable().Where(k => k.PaymentVoucherVoucherNo == null).FirstOrDefault();

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
    }
}
