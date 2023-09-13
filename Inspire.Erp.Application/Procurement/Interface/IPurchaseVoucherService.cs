using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspire.Erp.Domain.Entities;

namespace Inspire.Erp.Application.Master.Interfaces
{
    public interface IPurchaseVoucherService
    {
        public IEnumerable<PurchaseVoucher> GetPurchaseVoucher();
        public IEnumerable<PurchaseVoucher> InsertPurchaseVoucher(PurchaseVoucher DataObj);
        public IEnumerable<PurchaseVoucher> UpdatePurchaseVoucher(PurchaseVoucher DataObj);
        public IEnumerable<PurchaseVoucher> DeletePurchaseVoucher(PurchaseVoucher DataObj);
        public IEnumerable<PurchaseVoucherDetails> GetPurchaseVoucherDetails();
        public IQueryable GetPurchaseReturnReport();
    }
}