using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Modals.Sales;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Modals.Stock;

namespace Inspire.Erp.Application.Sales.Interfaces
{
    public interface IFinishedProduct
    {
        public FinishedProductMaster InsertFinishProduct(FinishedProductMaster customerQuotation, List<FinishedProductDetails> customerQuotationDetails );
        //public BillofQtyMaster UpdateBillofQty(BillofQtyMaster customerQuotation, List<FinishedProductDetails> customerQuotationDetails);
        //public int DeleteBillofQty(decimal id);
        //public BillofQtyMaster GetSavedBillofQty(decimal id); 
        public IEnumerable<FinishedProductMaster> GetIFinishedProduct();
        //public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate);




    }
}
