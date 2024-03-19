using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.Modals
{
    public class StatementOfAccountDetailResponse
    {
        public string AccountsTransactions_AccNo { get; set; }
        public DateTime AccountsTransactions_TransDate { get; set; }
        public string AccountsTransactions_Particulars { get; set; }
        public double AccountsTransactions_Debit { get; set; }
        public double AccountsTransactions_Credit { get; set; }
        public string AccountsTransactions_VoucherType { get; set; }
        public string AccountsTransactions_VoucherNo { get; set; }
        public string AccountsTransactions_Description { get; set; }
        public string AccountsTransactions_FSNo { get; set; }
        public string CostCenter { get; set; }
    }

    public class StatementOfAccountFCSummResponse
    {
        public string AccNo { get; set; }
        public string AccountName { get; set; }
        public double TotalDebit { get; set; }
        public double TotalCredit { get; set; }
        public double DebitBalance { get; set; }
        public double CreditBalance { get; set; }
    }
}
