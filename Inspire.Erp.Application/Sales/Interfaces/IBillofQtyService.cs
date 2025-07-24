using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Modals.Sales;
using Inspire.Erp.Domain.Modals.Stock;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Sales.Interfaces
{
    public interface IBillofQtyService
    {        
        public Task<Response<BillofQtyMasterModel>> InsertBillofQty(BillofQtyMaster customerQuotation, List<BillOfQTyDetails> customerQuotationDetails, List<EstimationCostCenter> EstimationCostCenter);
        public Task<Response<BillofQtyMasterModel>> UpdateBillofQty(BillofQtyMaster customerQuotation, List<BillOfQTyDetails> customerQuotationDetails, List<EstimationCostCenter> EstimationCostCenter);
        public int DeleteBillofQty(decimal id);
        public Task<Response<BillofQtyMasterModel>> GetSavedBillofQty(long Id); 
        public IEnumerable<BillofQtyMaster> GetBillofQty();
        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate);
        public Response<List<BillofQtyTracking>> GetBillofQtyTracking(DateTime? fromDate, DateTime? toDate, string? jobNo);



    }
}
