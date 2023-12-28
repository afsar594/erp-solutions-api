using Inspire.Erp.Application.Store.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.Stock;
using Inspire.Erp.Domain.Models;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Store.Implementation
{
    public class StockLedger : IStockLedger
    {
        private readonly IRepository<StockRegister> _sRegister;
        private readonly IRepository<ItemMaster> _iMster;
        private readonly IRepository<PurchaseVoucher> _pv;
        private readonly IRepository<PurchaseVoucherDetails> _pvDetail;
        private readonly IRepository<FinancialPeriods> _fP;

        public StockLedger(IRepository<StockRegister> sRegister, IRepository<ItemMaster> iMster, IRepository<PurchaseVoucherDetails> pvDetail, IRepository<PurchaseVoucher> pv, IRepository<FinancialPeriods> fP)
        {
            this._sRegister = sRegister;
            this._iMster = iMster;
            _pvDetail = pvDetail;
            _pv = pv;
            _fP = fP;
        }

        public async Task<Response<List<StockLedgerResponse>>> GetStockLedgerReport(StockFilterModel sFilter)
        {
            try
            {
                await Task.Delay(500);
                string qry = string.Empty;
                if (sFilter.LocationId > 0) qry += $" and sr.StockRegisterLocationID = {sFilter.LocationId}";
                if (!string.IsNullOrWhiteSpace(sFilter.ItemGroupId)) qry += $" and im.ItemMasterAccountNo = {sFilter.ItemGroupId}";
                if (!string.IsNullOrWhiteSpace(sFilter.PartNo)) qry += $" and im.ItemMasterPartNo = {sFilter.PartNo}";
                if (sFilter.ItemId > 0) qry += $" and im.ItemMasterItemId = {sFilter.ItemId}";

                if (sFilter.IsDateChecked == true)
                {
                    var result = from sr in _sRegister.GetAsQueryable().Where($"1==1 {qry}")
                                 join im in _iMster.GetAsQueryable() on sr.StockRegisterMaterialID equals (int)im.ItemMasterItemId into imGroup
                                 from im in imGroup.DefaultIfEmpty()
                                 //join gsv in GetStockValue(0, 0) on sr.StockRegisterMaterialID equals (int)gsv.MatId into gsvGroup
                                 //from gsv in gsvGroup.DefaultIfEmpty()
                                 where im.ItemMasterServices == false &&
                                       sr.StockRegisterAssignedDate <= sFilter.toDate &&
                                       !(new[] { "STOCKTRANS" }).Contains(sr.StockRegisterTransType)
                                 group new
                                 {
                                     Mat_ID = sr.StockRegisterMaterialID,
                                     PartNo = im.ItemMasterPartNo,
                                     ItemName = im.ItemMasterItemName,
                                     OpenQty = sr.StockRegisterAssignedDate < sFilter.fromDate ? (sr.StockRegisterSIN ?? 0 - sr.StockRegisterSout ?? 0) : 0,
                                     OpenQtyAmt = sr.StockRegisterAssignedDate < sFilter.fromDate ? (sr.StockRegisterSIN ?? 0 * sr.StockRegisterRate ?? 0 - sr.StockRegisterSout ?? 0 * sr.StockRegisterRate ?? 0) : 0,

                                     TotRevd = sr.StockRegisterAssignedDate >= sFilter.fromDate && sr.StockRegisterAssignedDate <= sFilter.toDate && sr.StockRegisterTransType != "SR" ? sr.StockRegisterSIN ?? 0 : 0,
                                     TotRevdAmt = sr.StockRegisterAssignedDate >= sFilter.fromDate && sr.StockRegisterAssignedDate <= sFilter.toDate && sr.StockRegisterTransType != "SR" ? sr.StockRegisterSIN ?? 0 * sr.StockRegisterRate ?? 0 : 0,
                                     TOTSaled = sr.StockRegisterAssignedDate >= sFilter.fromDate && sr.StockRegisterAssignedDate <= sFilter.toDate ? sr.StockRegisterSout ?? 0 - (sr.StockRegisterAssignedDate > sFilter.fromDate && sr.StockRegisterTransType == "SR" ? sr.StockRegisterSIN ?? 0 : 0) : 0,
                                     TOTSaledAmt = sr.StockRegisterAssignedDate >= sFilter.fromDate && sr.StockRegisterAssignedDate <= sFilter.toDate ? sr.StockRegisterSout ?? 0 * sr.StockRegisterRate ?? 0 - (sr.StockRegisterAssignedDate > sFilter.fromDate && sr.StockRegisterTransType == "SR" ? sr.StockRegisterSIN ?? 0 * sr.StockRegisterRate ?? 0 : 0) : 0,
                                     BalaTot = sr.StockRegisterAssignedDate <= sFilter.fromDate ? sr.StockRegisterSIN ?? 0 - sr.StockRegisterSout ?? 0 : 0,
                                     BalaTotamt = sr.StockRegisterAssignedDate <= sFilter.fromDate ? sr.StockRegisterSIN ?? 0 * sr.StockRegisterRate ?? 0 - sr.StockRegisterSout ?? 0 * sr.StockRegisterRate ?? 0 : 0,


                                     RelativeNo = im.ItemMasterAccountNo,
                                     StockRate = sr.StockRegisterRate
                                 } by sr.StockRegisterMaterialID into g
                                 select new StockLedgerResponse()
                                 {
                                     ItemId = g.Key.ToString(),
                                     OpenQuantity = g.Sum(x => x.OpenQty),
                                     OpenValue = g.Sum(x => x.OpenQtyAmt),
                                     ReceivedQuantity = g.Sum(x => x.TotRevd),
                                     ReceivedValue = g.Sum(x => x.TotRevdAmt),
                                     SaledQuantity = g.Sum(x => x.TOTSaled),
                                     SaledValue = g.Sum(x => x.TOTSaledAmt),
                                     ClosingQuantity = g.Sum(x => x.BalaTot),
                                     ClosingValue = g.Sum(x => x.BalaTotamt),
                                     RelativeNo = g.Max(x => x.RelativeNo).ToString(),
                                     PartNo = g.Max(x => x.PartNo),
                                     ItemName = g.Max(x => x.ItemName),
                                     StockRate = (decimal)g.Max(x => x.StockRate)
                                 };

                    if (sFilter.ExcludeZeroBalance == true)
                    {
                        var result2 = result.ToList().Where(x => x.ClosingValue > 0).Select(x => x).ToList();
                        return Response<List<StockLedgerResponse>>.Success(result2, "Data Found");
                    }
                    if (sFilter.ExcludeZeroOpeningBalance == true)
                    {
                        var result2 = result.ToList().Where(x => x.OpenValue > 0).Select(x => x).ToList();
                        return Response<List<StockLedgerResponse>>.Success(result2, "Data Found");
                    }
                    else
                    {
                        return Response<List<StockLedgerResponse>>.Success(result.ToList(), "Data Found");
                    }

                }
                else
                {
                    var minFDate = _fP.GetAsQueryable().Select(x => x.FinancialPeriodsStartDate).SingleOrDefault();

                    var items = _iMster.GetAsQueryable().ToList();
                    var result = from sr in _sRegister.GetAsQueryable().Where($"1==1 {qry}")
                                 join im in _iMster.GetAsQueryable() on sr.StockRegisterMaterialID equals (int)im.ItemMasterItemId into imGroup
                                 from im in imGroup.DefaultIfEmpty()
                                
                                 where im.ItemMasterServices == false &&
                                       sr.StockRegisterAssignedDate <= minFDate &&
                                       !(new[] { "STOCKTRANS" }).Contains(sr.StockRegisterTransType)
                                 group new
                                 {
                                     Mat_ID = sr.StockRegisterMaterialID,
                                     PartNo = im.ItemMasterPartNo,
                                     ItemName = im.ItemMasterItemName,

                                     OpenQty = sr.StockRegisterAssignedDate < minFDate ? (sr.StockRegisterSIN ?? 0 - sr.StockRegisterSout ?? 0) : 0,
                                     OpenQtyAmt = sr.StockRegisterAssignedDate < minFDate ? (sr.StockRegisterSIN ?? 0 * sr.StockRegisterRate ?? 0 - sr.StockRegisterSout ?? 0 * sr.StockRegisterRate ?? 0) : 0,

                                     TotRevd = sr.StockRegisterAssignedDate > minFDate && sr.StockRegisterTransType != "SR" ? sr.StockRegisterSIN ?? 0 : 0,
                                     TotRevdAmt = sr.StockRegisterAssignedDate > minFDate && sr.StockRegisterTransType != "SR" ? sr.StockRegisterSIN ?? 0 * sr.StockRegisterRate ?? 0 : 0,

                                     TOTSaled = sr.StockRegisterAssignedDate > minFDate ? sr.StockRegisterSout ?? 0 - (sr.StockRegisterAssignedDate > minFDate && sr.StockRegisterTransType == "SR" ? sr.StockRegisterSIN ?? 0 : 0) : 0,
                                     TOTSaledAmt = sr.StockRegisterAssignedDate > minFDate ? sr.StockRegisterSout ?? 0 * sr.StockRegisterRate ?? 0 - (sr.StockRegisterAssignedDate > minFDate && sr.StockRegisterTransType == "SR" ? sr.StockRegisterSIN ?? 0 * sr.StockRegisterRate ?? 0 : 0) : 0,

                                     BalaTot = sr.StockRegisterAssignedDate > minFDate ? sr.StockRegisterSIN ?? 0 - sr.StockRegisterSout ?? 0 : 0,
                                     BalaTotamt = sr.StockRegisterAssignedDate > minFDate ? sr.StockRegisterSIN ?? 0 * sr.StockRegisterRate ?? 0 - sr.StockRegisterSout ?? 0 * sr.StockRegisterRate ?? 0 : 0,


                                     RelativeNo = im.ItemMasterAccountNo,
                                     StockRate = sr.StockRegisterRate
                                 } by sr.StockRegisterMaterialID into g
                                 select new StockLedgerResponse()
                                 {
                                     ItemId = g.Key.ToString(),
                                     OpenQuantity = g.Sum(x => x.OpenQty),
                                     OpenValue = g.Sum(x => x.OpenQtyAmt),
                                     ReceivedQuantity = g.Sum(x => x.TotRevd),
                                     ReceivedValue = g.Sum(x => x.TotRevdAmt),
                                     SaledQuantity = g.Sum(x => x.TOTSaled),
                                     SaledValue = g.Sum(x => x.TOTSaledAmt),
                                     ClosingQuantity = g.Sum(x => x.BalaTot),
                                     ClosingValue = g.Sum(x => x.BalaTotamt),
                                     RelativeNo = g.Max(x => x.RelativeNo).ToString(),
                                     PartNo = g.Max(x => x.PartNo),
                                     ItemName = g.Max(x => x.ItemName),
                                     StockRate = (decimal)g.Max(x => x.StockRate)
                                 };

                    if (sFilter.ExcludeZeroBalance == true)
                    {
                        var result2 = result.ToList().Where(x => x.ClosingValue > 0).Select(x => x).ToList();
                        return Response<List<StockLedgerResponse>>.Success(result2, "Data Found");
                    }
                    if (sFilter.ExcludeZeroOpeningBalance == true)
                    {
                        var result2 = result.ToList().Where(x => x.OpenValue > 0).Select(x => x).ToList();
                        return Response<List<StockLedgerResponse>>.Success(result2, "Data Found");
                    }
                    else
                    {
                        var a = result.ToList();
                        return Response<List<StockLedgerResponse>>.Success(result.ToList(), "Data Found");
                    }
                }



            }
            catch (Exception ex)
            {
                return Response<List<StockLedgerResponse>>.Fail(new List<StockLedgerResponse>(), ex.Message);
            }
        }


        public List<StockValueResult> GetStockValue(long itemId, long itemGroupId)
        {
            var query = _iMster.GetAsQueryable();

            if (itemId > 0)
            {
                query = query.Where(im => im.ItemMasterItemId == itemId);
            }
            else if (itemGroupId > 0)
            {
                query = query.Where(im => im.ItemMasterAccountNo == itemGroupId && im.ItemMasterItemType == "A")
                             .OrderBy(im => im.ItemMasterItemId);
            }
            else
            {
                query = query.Where(im => im.ItemMasterItemType == "A")
                             .OrderBy(im => im.ItemMasterItemId);
            }

            
            var vDetailItems = (from d in _pvDetail.GetAsQueryable()
                     join p in _pv.GetAsQueryable() on d.PurchaseVoucherDetailsVoucherNo equals p.PurchaseVoucherVoucherNo
                     where p.PurchaseVoucherStatus == "P"
                     select d).ToList();

            var itemDetails = (from ss in query.ToList()
                      join a in vDetailItems on ss.ItemMasterItemId equals (long)a.PurchaseVoucherDetailsMaterialId
                      group a by a.PurchaseVoucherDetailsMaterialId into g
                      select new { 
                      ItemId = g.Key,
                      TotalQty = g.Sum(x=>x.PurchaseVoucherDetailsQuantity)
                      }).ToList();



            //var res = (from im in query.ToList()
            //           join d in _pvDetail.GetAsQueryable() on (int?)im.ItemMasterItemId equals d.PurchaseVoucherDetailsMaterialId
            //           join p in _pv.GetAsQueryable() on d.PurchaseVoucherDetailsVoucherNo equals p.PurchaseVoucherVoucherNo
            //           where p.PurchaseVoucherStatus == "P"
            //           group new { d.PurchaseVoucherDetailsQuantity, im.ItemMasterItemId } by im.ItemMasterItemId into g
            //           select new
            //           {
            //               ItemMasterItemId = g.Key,
            //               TotalQuantity = g.Sum(x => x.PurchaseVoucherDetailsQuantity),
            //           }).ToList();

            var result = itemDetails.Select((x) => new StockValueResult
            {
                MatId =(long) x.ItemId,
                StockRate = (decimal)x.TotalQty > 0 ? getQuantityValue((long)x.ItemId) / (decimal)x.TotalQty : 0,
                StockValue = (decimal)x.TotalQty * ((decimal)x.TotalQty > 0 ? getQuantityValue((long)x.ItemId) / (decimal)x.TotalQty : 0)
            }).ToList();

            return result;
        }

        private decimal getQuantityValue(long itemId)
        {
            var res = (from d in _pvDetail.GetAsQueryable()
                       join p in _pv.GetAsQueryable() on d.PurchaseVoucherDetailsVoucherNo equals p.PurchaseVoucherVoucherNo
                       where p.PurchaseVoucherStatus == "P" && (int)d.PurchaseVoucherDetailsMaterialId == itemId
                       select new
                       {
                           Rete = (decimal)d.PurchaseVoucherDetailsRate,
                           Quantity = (decimal)d.PurchaseVoucherDetailsQuantity
                       }).ToList().Sum(x => x.Rete * x.Quantity);
            return res;
        }

    }

    public class StockValueResult
    {
        public long MatId { get; set; }
        public decimal StockRate { get; set; }
        public decimal StockValue { get; set; }
    }
}
