using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Modals.Procurement;
using Inspire.Erp.Domain.Modals;

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

        Task<Response<GridWrapperResponse<List<GetPurchaseVoucherResponse>>>> GetPurchaseVoucherReport(GenericGridViewModel model);
    }
}