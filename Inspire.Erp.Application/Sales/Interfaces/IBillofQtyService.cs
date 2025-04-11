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
    public interface IBillofQtyService
    {        
        public BillofQtyMaster InsertBillofQty(BillofQtyMaster customerQuotation, List<BillOfQTyDetails> customerQuotationDetails, List<EstimationCostCenter> EstimationCostCenter);
        public BillofQtyMaster UpdateBillofQty(BillofQtyMaster customerQuotation, List<BillOfQTyDetails> customerQuotationDetails, List<EstimationCostCenter> EstimationCostCenter);
        public int DeleteBillofQty(decimal id);
        public BillofQtyMaster GetSavedBillofQty(decimal id); 
        public IEnumerable<BillofQtyMaster> GetBillofQty();
        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate);
        public Response<List<BillofQtyTracking>> GetBillofQtyTracking(DateTime? BoqDate, string? jobNo);



    }
}
