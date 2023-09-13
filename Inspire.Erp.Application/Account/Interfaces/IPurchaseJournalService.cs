using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Account.Interfaces
{
    public interface IPurchaseJournalService
    {
        //public PurchaseJournal InsertPurchaseJournal(PurchaseJournal purchaseJournal,
        //  List<PurchaseJournalDetails> purchaseJournalDetails);

        public PurchaseJournalModel InsertPurchaseJournal(PurchaseJournal purchaseJournal, List<AccountsTransactions> accountsTransactions, List<PurchaseJournalDetails> purchaseJournalDetails);
        public PurchaseJournalModel UpdatePurchaseJournal(PurchaseJournal purchaseJournal, List<AccountsTransactions> accountsTransactions, List<PurchaseJournalDetails> purchaseJournalDetails);
        public int DeletePurchaseJournal(PurchaseJournal purchaseJournal, List<AccountsTransactions> accountsTransactions, List<PurchaseJournalDetails> purchaseJournalDetails);
        public IEnumerable<AccountsTransactions> GetAllTransaction();
        public PurchaseJournalModel GetSavedPurchaseJournalDetails(string pvno);
        public IEnumerable<PurchaseJournal> GetPurchaseJournal();
        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate);

        public VouchersNumbers GetVouchersNumbers(string pvno);

    }
}
