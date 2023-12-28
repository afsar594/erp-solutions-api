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
                string itemQry = string.Empty;
                if (sFilter.LocationId > 0) qry += $" and StockRegisterLocationID = {sFilter.LocationId}";
                if (!string.IsNullOrWhiteSpace(sFilter.ItemGroupId)) itemQry += $" and ItemMasterAccountNo = {sFilter.ItemGroupId}";
                if (!string.IsNullOrWhiteSpace(sFilter.PartNo)) itemQry += $" and ItemMasterPartNo = {sFilter.PartNo}";
                if (sFilter.ItemId > 0) itemQry += $" and ItemMasterItemId = {sFilter.ItemId}";

                if (sFilter.IsDateChecked == true)
                {

                    var items = _iMster.GetAsQueryable().Where($"1==1 {itemQry}").ToList();

                    qry += $" and StockRegisterAssignedDate.Value.Date > DateTime({ sFilter.fromDate.Year},{ sFilter.fromDate.Month},{ sFilter.fromDate.Day})";
                    var openValues = _sRegister.GetAsQueryable().Where($"1==1 {qry}")
                                    .Select(x => new
                                    {
                                        ItemId = (long)x.StockRegisterMaterialID,
                                        OpenQty = x.StockRegisterSIN ?? 0 - x.StockRegisterSout ?? 0,
                                        OpenQtyAmt = x.StockRegisterSIN ?? 0 * x.StockRegisterRate ?? 0 - x.StockRegisterSout ?? 0 * x.StockRegisterRate ?? 0,

                                    }).ToList();

                    qry += $" and (StockRegisterAssignedDate.Value.Date >  DateTime({ sFilter.fromDate.Year},{ sFilter.fromDate.Month},{ sFilter.fromDate.Day}) and StockRegisterAssignedDate <=  DateTime({ sFilter.toDate.Year},{ sFilter.toDate.Month},{ sFilter.toDate.Day}))";
                    var otherrValues = _sRegister.GetAsQueryable().Where($"1==1 {qry}")
                                    .Select(x => new
                                    {
                                        ItemId = (long)x.StockRegisterMaterialID,
                                        TotRevd = x.StockRegisterTransType != "SR" ? x.StockRegisterSIN ?? 0 : 0,
                                        TotRevdAmt = x.StockRegisterTransType != "SR" ? x.StockRegisterSIN ?? 0 * x.StockRegisterRate ?? 0 : 0,

                                        TOTSaled = x.StockRegisterSout ?? 0 - (x.StockRegisterTransType == "SR" ? x.StockRegisterSIN ?? 0 : 0),
                                        TOTSaledAmt = x.StockRegisterSout ?? 0 * x.StockRegisterRate ?? 0 - (x.StockRegisterTransType == "SR" ? x.StockRegisterSIN ?? 0 * x.StockRegisterRate ?? 0 : 0),

                                        BalaTot = x.StockRegisterSIN ?? 0 - x.StockRegisterSout ?? 0,
                                        BalaTotamt = x.StockRegisterSIN ?? 0 * x.StockRegisterRate ?? 0 - x.StockRegisterSout ?? 0 * x.StockRegisterRate ?? 0,
                                        StockRate = x.StockRegisterRate

                                    }).ToList();

                    var result = from im in items
                                 join op in openValues on im.ItemMasterItemId equals op.ItemId
                                 join sr in otherrValues on im.ItemMasterItemId equals sr.ItemId
                                 group new
                                 {
                                     Mat_ID = sr.ItemId,
                                     PartNo = im.ItemMasterPartNo,
                                     ItemName = im.ItemMasterItemName,

                                     OpenQty = op.OpenQty,
                                     OpenQtyAmt = op.OpenQtyAmt,

                                     TotRevd = sr.TotRevd,
                                     TotRevdAmt = sr.TotRevdAmt,

                                     TOTSaled = sr.TOTSaled,
                                     TOTSaledAmt = sr.TOTSaledAmt,

                                     BalaTot = sr.BalaTot,
                                     BalaTotamt = sr.BalaTotamt,


                                     RelativeNo = im.ItemMasterAccountNo,
                                     StockRate = sr.StockRate
                                 } by im.ItemMasterItemId into g
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

                    var items = _iMster.GetAsQueryable().Where($"1==1 {itemQry}").ToList();

                    qry += $" and StockRegisterAssignedDate.Value.Date > DateTime({ minFDate.Value.Year},{ minFDate.Value.Month},{ minFDate.Value.Day})";
                    var openValues = _sRegister.GetAsQueryable().Where($"1==1 {qry}")
                                    .Select(x => new
                                    {
                                        ItemId = (long)x.StockRegisterMaterialID,
                                        OpenQty = x.StockRegisterSIN ?? 0 - x.StockRegisterSout ?? 0,
                                        OpenQtyAmt = x.StockRegisterSIN ?? 0 * x.StockRegisterRate ?? 0 - x.StockRegisterSout ?? 0 * x.StockRegisterRate ?? 0,

                                    }).ToList();


                    var otherrValues = _sRegister.GetAsQueryable().Where($"1==1 {qry}")
                                    .Select(x => new
                                    {
                                        ItemId = (long)x.StockRegisterMaterialID,
                                        TotRevd = x.StockRegisterTransType != "SR" ? x.StockRegisterSIN ?? 0 : 0,
                                        TotRevdAmt = x.StockRegisterTransType != "SR" ? x.StockRegisterSIN ?? 0 * x.StockRegisterRate ?? 0 : 0,

                                        TOTSaled = x.StockRegisterSout ?? 0 - (x.StockRegisterTransType == "SR" ? x.StockRegisterSIN ?? 0 : 0),
                                        TOTSaledAmt = x.StockRegisterSout ?? 0 * x.StockRegisterRate ?? 0 - (x.StockRegisterTransType == "SR" ? x.StockRegisterSIN ?? 0 * x.StockRegisterRate ?? 0 : 0),

                                        BalaTot = x.StockRegisterSIN ?? 0 - x.StockRegisterSout ?? 0,
                                        BalaTotamt = x.StockRegisterSIN ?? 0 * x.StockRegisterRate ?? 0 - x.StockRegisterSout ?? 0 * x.StockRegisterRate ?? 0,
                                        StockRate = x.StockRegisterRate

                                    }).ToList();

                    var result = from im in items
                                 join op in openValues on im.ItemMasterItemId equals op.ItemId
                                 join sr in otherrValues on im.ItemMasterItemId equals sr.ItemId
                                 group new
                                 {
                                     Mat_ID = sr.ItemId,
                                     PartNo = im.ItemMasterPartNo,
                                     ItemName = im.ItemMasterItemName,

                                     OpenQty = op.OpenQty,
                                     OpenQtyAmt = op.OpenQtyAmt,

                                     TotRevd = sr.TotRevd,
                                     TotRevdAmt = sr.TotRevdAmt,

                                     TOTSaled = sr.TOTSaled,
                                     TOTSaledAmt = sr.TOTSaledAmt,

                                     BalaTot = sr.BalaTot,
                                     BalaTotamt = sr.BalaTotamt,


                                     RelativeNo = im.ItemMasterAccountNo,
                                     StockRate = sr.StockRate
                                 } by im.ItemMasterItemId into g
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
                               select new
                               {
                                   ItemId = g.Key,
                                   TotalQty = g.Sum(x => x.PurchaseVoucherDetailsQuantity)
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
                MatId = (long)x.ItemId,
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
