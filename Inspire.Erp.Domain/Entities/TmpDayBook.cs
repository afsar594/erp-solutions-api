using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Domain.Entities
{
    public class TmpDayBook
    {
     [Key]
     public int Id { get; set; }
     public string AccountsTransactionsVoucherNo { get; set; }
     public string AccountsTransactionsTransDateString { get; set; }
     public string RefNo{ get; set; }
     public string AccountsTransactionsDescription { get; set; }
     public string AccountsTransactionsParticulars { get; set; }
     public decimal AccountsTransactionsDebit { get; set; }
     public decimal AccountsTransactionsCredit { get; set; }
     public string AccountsTransactionsVoucherType { get; set; }
     public string? JobName { get; set; }
     public string? CostCenterName { get; set; }
    }
}
