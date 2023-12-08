using Inspire.Erp.Application.Procurement.Interface;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Models.Procurement.PurchaseOrderTracking;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Transactions;

namespace Inspire.Erp.Application.Procurement.Implimentation
{
    public class PurchaseOrdertracking : IPurchaseOrdertracking
    {
        private IRepository<PurchaseOrder> _purchaseOrder;
        private IRepository<PurchaseOrderDetails> _poDetails;
        private IRepository<Suppliers> _supliers;
        private IRepository<JobMaster> _jobMaster;
        private IRepository<ItemMaster> _itemMaster;
        private IRepository<GrnPoDetails> _grnPODetails;
        private IRepository<CurrencyMaster> _curreny;
        private IRepository<StockRegister> _stock;
        private IRepository<PurOrderRegister> _poRegister;

        public PurchaseOrdertracking(IRepository<PurchaseOrder> purchaseOrder, IRepository<PurchaseOrderDetails> poDetails, IRepository<Suppliers> supliers, IRepository<JobMaster> jobMaster, IRepository<ItemMaster> itemMaster, IRepository<GrnPoDetails> grnPODetails, IRepository<CurrencyMaster> curreny, IRepository<StockRegister> stock, IRepository<PurOrderRegister> poRegister)
        {
            _purchaseOrder = purchaseOrder;
            _poDetails = poDetails;
            _supliers = supliers;
            _jobMaster = jobMaster;
            _itemMaster = itemMaster;
            _grnPODetails = grnPODetails;
            _curreny = curreny;
            _stock = stock;
            _poRegister = poRegister;
        }

        public async Task<Response<List<POTrackingDetails>>> GetPurchaseOrderTrackingDetails(ReportFilter model)
        {
            List<POTrackingDetails> data = new List<POTrackingDetails>();
            try
            {
                string filteredValue = string.Empty;
                if (!string.IsNullOrEmpty(model.fromDate) && !string.IsNullOrEmpty(model.toDate))
                {
                    filteredValue += $" and (po.PurchaseOrderDate >= {model.fromDate} and  po.PurchaseOrderDate <= {model.toDate} )";
                }
                if (model.SupplierId > 0)
                {
                    filteredValue += $" and po.PurchaseOrderSupInvNo = {model.SupplierId} ";
                }
                if (model.JobId > 0)
                {
                    filteredValue += $" and po.PurchaseOrderJobId = {model.JobId} ";
                }
                if (model.LocationId > 0)
                {
                    filteredValue += $" and po.PurchaseOrderLocationId = {model.LocationId} ";
                }
                if (model.ItemId > 0)
                {
                    filteredValue += $" and pod.PurchaseOrderDetailsMatId = {model.ItemId} ";
                }
                if (!string.IsNullOrEmpty(model.PONO) && model.PONO.ToUpper() != "ALL".ToUpper())
                {
                    filteredValue += $" and po.PurchaseOrderPono = {model.ItemId} ";
                }


                var result = await (from po in _purchaseOrder.GetAsQueryable().Where($"1 == 1 {filteredValue}")
                                    join sp in _supliers.GetAsQueryable() on po.PurchaseOrderSupInvNo equals sp.SuppliersInsId.ToString() into spGroup
                                    from sp in spGroup.DefaultIfEmpty()
                                    join pod in _poDetails.GetAsQueryable() on po.PurchaseOrderId equals pod.PurchaseOrderId into podGroup
                                    from pod in podGroup.DefaultIfEmpty()
                                    join i in _itemMaster.GetAsQueryable() on pod.PurchaseOrderDetailsMatId equals i.ItemMasterItemId into iGroup
                                    from i in iGroup.DefaultIfEmpty()
                                    join cm in _curreny.GetAsQueryable() on po.PurchaseOrderCurrencyId equals cm.CurrencyMasterCurrencyId into cmGroup
                                    from cm in cmGroup.DefaultIfEmpty()
                                    join jm in _jobMaster.GetAsQueryable() on po.PurchaseOrderJobId equals jm.JobMasterJobId into jmGroup
                                    from jm in jmGroup.DefaultIfEmpty()
                                    select new POTrackingDetails()
                                    {
                                        PONO = po.PurchaseOrderPono,
                                        PODate = po.PurchaseOrderDate,
                                        Supplier = sp.SuppliersInsName,
                                        POQuantity = pod.PurchaseOrderDetailsQuantity,
                                        PartNO = (i != null) ? i.ItemMasterPartNo : "",
                                        ItemName = pod.PurchaseOrderDetailsItemName,
                                        ItemRate = pod.PurchaseOrderDetailsRate,
                                        PODetailsId = pod.PurchaseOrderDetailsPodId,
                                        ItemId = pod.PurchaseOrderDetailsMatId.ToString(),
                                        POStatus = po.PurchaseOrderStatus,
                                        CurrencyRate = (cm != null) ? (decimal?)cm.CurrencyMasterCurrencyRate : 1,
                                        DiscountPerc = po.PurchaseOrderTotalDisPer,
                                        VATPerc = po.PurchaseOrderVatPer,
                                        Amount = po.PurchaseOrderNetAmount,
                                        DiscountAmount = po.PurchaseOrderTotalDisAmount,
                                        IssuedQty = _poRegister.GetAsQueryable()
                                                      .Where(pr => pr.PurOrderRegisterOrderNo == po.PurchaseOrderPono && pr.PurOrderRegisterDetailId == pod.PurchaseOrderDetailsPodId && pr.PurOrderRegisterMaterialId == pod.PurchaseOrderDetailsMatId)
                                                      .Sum(pr => (int?)pr.PurOrderRegisterQtyIssued) ?? 0,
                                        BalanceQty = pod.PurchaseOrderDetailsQuantity - (_poRegister.GetAsQueryable().Where(pr => pr.PurOrderRegisterOrderNo == po.PurchaseOrderPono && pr.PurOrderRegisterDetailId == pod.PurchaseOrderDetailsPodId && pr.PurOrderRegisterMaterialId == pod.PurchaseOrderDetailsMatId)
                                                                       .Sum(pr => (int?)pr.PurOrderRegisterQtyIssued) ?? 0),
                                        Currency = (cm != null) ? cm.CurrencyMasterCurrencyName : "",
                                        JobName = (jm != null) ? jm.JobMasterJobName : "",
                                        JobNo = (jm != null) ? jm.JobMasterJobNo : ""
                                    }).OrderBy(x => x.PODate).ThenBy(x => x.PONO).ToDynamicListAsync();
                if (result.Count > 0)
                {
                    foreach (var item in result)
                    {
                        data.Add(item);
                    }
                }
                return new Response<List<POTrackingDetails>>()
                {
                    Valid = true,
                    Message = "Data Found",
                    Result = data
                };
            }
            catch (Exception ex)
            {
                return Response<List<POTrackingDetails>>.Fail(new List<POTrackingDetails>(), ex.Message);
            }
        }

        public async Task<Response<List<POTrackingSummary>>> GetPurchaseOrderTrackingSummary(ReportFilter filter)
        {
            try
            {
                List<POTrackingSummary> data = new List<POTrackingSummary>();

                    return new Response<List<POTrackingSummary>>()
                    {
                        Valid = true,
                        Message = "Data Found",
                        Result = data
                    };
            }
            catch(Exception ex)
            {
                return Response<List<POTrackingSummary>>.Fail(new List<POTrackingSummary>(), ex.Message);
            }
        }
    }
}
