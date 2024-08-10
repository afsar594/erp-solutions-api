using Inspire.Erp.Application.Account.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Modals;
using Microsoft.EntityFrameworkCore;

namespace Inspire.Erp.Application.Account.Implementations
{
    public class BankReconcilation : IBankReconcilation
    {
        public IRepository<MasterAccountsTable> _MasterAccountsTableRepo;
        public IRepository<AccountsTransactions> _AccountsTransactionsRepo;
        public IRepository<BankReceiptVoucherDetails> _BankReceiptVoucherDetailsRepo;
        public IRepository<ReconciliationVoucherDetails> _ReconciliationVoucherDetailsRepo;
        public IRepository<BankPaymentVoucherDetails> _BankPaymentVoucherDetailsRepo;
        public IRepository<BankLedger> _BankLedgerRepo;
        public BankReconcilation(
            IRepository<MasterAccountsTable> MasterAccountsTable,
            IRepository<AccountsTransactions> AccountsTransactionsRepo,
            IRepository<BankReceiptVoucherDetails> BankReceiptVoucherDetailsRepo,
            IRepository<ReconciliationVoucherDetails> ReconciliationVoucherDetailsRepo,
            IRepository<BankPaymentVoucherDetails> BankPaymentVoucherDetailsRepo,
            IRepository<BankLedger> bank
            )
        {
            _BankLedgerRepo = bank;
            _MasterAccountsTableRepo = MasterAccountsTable;
            _AccountsTransactionsRepo = AccountsTransactionsRepo;
            _BankReceiptVoucherDetailsRepo = BankReceiptVoucherDetailsRepo;
            _ReconciliationVoucherDetailsRepo = ReconciliationVoucherDetailsRepo;
            _BankPaymentVoucherDetailsRepo = BankPaymentVoucherDetailsRepo;

        }

        public async Task<IEnumerable<BankLedger>> BankLedger(string bankAccNo)
        {
            try
            {
               
                var excludedVouchers = _ReconciliationVoucherDetailsRepo
                                        .GetAsQueryable()
                                        .Where(rv => rv.ReconciliationVoucherDetailsVno != null && rv.ReconciliationVoucherDetailsMatched == "Y")
                                        .Select(rv => rv.ReconciliationVoucherDetailsVno);

                var list = await (from at in _AccountsTransactionsRepo.GetAsQueryable()
                                  join bpvd in _BankPaymentVoucherDetailsRepo.GetAsQueryable()
                                  on at.AccountsTransactionsVoucherNo equals bpvd.BankPaymentVoucherDetailsVoucherNo into bpvdGroup
                                  from bpvd in bpvdGroup.DefaultIfEmpty()
                                  join rvd in _BankReceiptVoucherDetailsRepo.GetAsQueryable()
                                  on at.AccountsTransactionsVoucherNo equals rvd.BankReceiptVoucherDetailsVoucherNo into rvdGroup
                                  from rvd in rvdGroup.DefaultIfEmpty()
                                  where at.AccountsTransactionsAccNo == bankAccNo
                                  where bpvd != null || rvd != null
                                  where !excludedVouchers.Contains(at.AccountsTransactionsVoucherNo)
                                  orderby at.AccountsTransactionsTransDate
                                  select new BankLedger
                                  {
                                      AccountsTransactions_VoucherNo = at.AccountsTransactionsVoucherNo,
                                      AccountsTransactions_Debit = at.AccountsTransactionsDebit,
                                      AccountsTransactions_Credit = at.AccountsTransactionsCredit,
                                      AccountsTransactions_TransSno = at.AccountsTransactionsTransSno,
                                      checkNo = at.AccountsTransactionsCheqNo != null ? (int?)Convert.ToInt32(at.AccountsTransactionsCheqNo) : null,
                                      AccountsTransactions_Description = at.AccountsTransactionsDescription,
                                      AccountsTransactions_TransDate = at.AccountsTransactionsTransDate
                                  }).ToListAsync();


                return list;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<List<MasterAccountsTable>> GetAccountListForDropdown()
        {
            var list = (from mat in _MasterAccountsTableRepo.GetAsQueryable()
                        where (mat.MaStatus ?? "") == "R" && (mat.MaRelativeNo ?? "") == "01001002"
                        select new MasterAccountsTable
                        {
                            MaSno = mat.MaSno,
                            MaRelativeNo = mat.MaRelativeNo,
                            MaAccName = mat.MaAccName,
                            MaAccType = mat.MaAccType,
                            MaMainHead = mat.MaMainHead,
                            MaSubHead = mat.MaSubHead,
                            MaImageKey = mat.MaImageKey,
                            MaSystemAcc = mat.MaSystemAcc,
                            MaFsno = mat.MaFsno,
                            MaStatus = mat.MaStatus,
                            MaUserId = mat.MaUserId,
                            MaDateCreated = mat.MaDateCreated,
                            MaCurrencyId = mat.MaCurrencyId,
                            MaGpAcc = mat.MaGpAcc,
                            MaAcAcc = mat.MaAcAcc,
                            MaEdAcc = mat.MaEdAcc,
                            MaOpenBalance = mat.MaOpenBalance,
                            MaTotalDebit = mat.MaTotalDebit,
                            MaTotalCredit = mat.MaTotalCredit,
                            MaManualCode = mat.MaManualCode,
                            MaIsAirAcc = mat.MaIsAirAcc,
                            MaIsSeaAcc = mat.MaIsSeaAcc,
                            MaCostCenterId = mat.MaCostCenterId,
                            MaShowSuminTb = mat.MaShowSuminTb,
                            MaCostCenterSub = mat.MaCostCenterSub,
                            MaSortNo = mat.MaSortNo,
                            MaAssetValue = mat.MaAssetValue,
                            MaAssetDepValue = mat.MaAssetDepValue,
                            MaAssetQty = mat.MaAssetQty,
                            MaLifeInYrs = mat.MaLifeInYrs,
                            MaAssetDepMode = mat.MaAssetDepMode,
                            MaAssetDate = mat.MaAssetDate,
                            MaIsAsset = mat.MaIsAsset,
                            MaDelStatus = mat.MaDelStatus

                        }).ToList();
            return list;
        }
    }
}
