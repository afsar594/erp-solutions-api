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
                if (sFilter.ItemGroupId > 0) itemQry += $" and ItemMasterAccountNo = {sFilter.ItemGroupId}";
                if (!string.IsNullOrEmpty(sFilter.PartNo)) itemQry += $" and ItemMasterPartNo = {sFilter.PartNo}";
                if (sFilter.ItemId > 0) itemQry += $" and ItemMasterItemId = {sFilter.ItemId}";

                if (sFilter.IsDateChecked == true)
                {

                    var items = _iMster.GetAsQueryable().Where(i => i.ItemMasterItemType == "A" && i.ItemMasterDelStatus == null).Where($"1==1 {itemQry}").ToList();

                    //qry += $" and StockRegisterAssignedDate.Value.Date > DateTime({ sFilter.fromDate.Year},{ sFilter.fromDate.Month},{ sFilter.fromDate.Day})";
                    //var openValues = _sRegister.GetAsQueryable().Where($"1==1 {qry}")
                    //                .Select(x => new
                    //                {
                    //                    ItemId = (long)x.StockRegisterMaterialID,
                    //                    OpenQty = x.StockRegisterSIN  - x.StockRegisterSout ,
                    //                    OpenQtyAmt = x.StockRegisterSIN  * x.StockRegisterRate  - x.StockRegisterSout  * x.StockRegisterRate ,

                    //                }).ToList();

                    qry += $" and (StockRegisterAssignedDate.Value.Date >  DateTime({sFilter.fromDate.Year},{sFilter.fromDate.Month},{sFilter.fromDate.Day}) and StockRegisterAssignedDate <=  DateTime({sFilter.toDate.Year},{sFilter.toDate.Month},{sFilter.toDate.Day}))";
                    var otherValues = (from s in _sRegister.GetAsQueryable().Where($"1==1 {itemQry}")
                                       join im in _iMster.GetAsQueryable() on s.StockRegisterMaterialID equals (int)im.ItemMasterItemId
                                       group s by s.StockRegisterMaterialID into g
                                       select new
                                       {
                                           ItemId = (long)g.Key,
                                           TotRevd = g.Sum(x => x.StockRegisterTransType != "SR" ? x.StockRegisterSIN : 0),
                                           TotRevdAmt = g.Sum(x => x.StockRegisterTransType != "SR" ? x.StockRegisterSIN * x.StockRegisterRate : 0),

                                           TOTSaled = g.Sum(x => x.StockRegisterSout - (x.StockRegisterTransType == "SR" ? x.StockRegisterSIN : 0)),
                                           TOTSaledAmt = g.Sum(x => x.StockRegisterSout * x.StockRegisterRate - (x.StockRegisterTransType == "SR" ? x.StockRegisterSIN * x.StockRegisterRate : 0)),


                                           TolSin = g.Sum(x => x.StockRegisterSIN),
                                           TolSOut = g.Sum(x => x.StockRegisterSout),

                                           StockRate = g.Max(x => x.StockRegisterRate)
                                       }

                                ).ToList();


                    var result = (from im in items
                                  join sr in otherValues on im.ItemMasterItemId equals sr.ItemId
                                  group new
                                  {
                                      Mat_ID = sr.ItemId,
                                      PartNo = im.ItemMasterPartNo,
                                      ItemName = im.ItemMasterItemName,

                                      OpenQty = 0,
                                      OpenQtyAmt = 0,

                                      TotRevd = sr.TotRevd,
                                      TotRevdAmt = sr.TotRevdAmt,

                                      TOTSaled = sr.TOTSaled,
                                      TOTSaledAmt = sr.TOTSaledAmt,

                                      BalaTot = sr.TolSin - sr.TolSOut,
                                      BalaTotamt = (sr.TolSin - sr.TolSOut) * sr.StockRate,

                                      RelativeNo = im.ItemMasterAccountNo,
                                      StockRate = sr.StockRate
                                  } by im.ItemMasterItemId into g
                                  select new StockLedgerResponse()
                                  {
                                      ItemId = g.Key.ToString(),
                                      OpenQuantity = (decimal)g.Sum(x => x.OpenQty),
                                      OpenValue = (decimal)g.Sum(x => x.OpenQtyAmt),
                                      ReceivedQuantity = (decimal)g.Sum(x => x.TotRevd),
                                      ReceivedValue = (decimal)g.Sum(x => x.TotRevdAmt),
                                      SaledQuantity = (decimal)g.Sum(x => x.TOTSaled),
                                      SaledValue = (decimal)g.Sum(x => x.TOTSaledAmt),
                                      ClosingQuantity = (decimal)g.Sum(x => x.BalaTot),
                                      ClosingValue = (decimal)g.Sum(x => x.BalaTotamt),
                                      RelativeNo = g.Max(x => x.RelativeNo).ToString(),
                                      PartNo = g.Max(x => x.PartNo),
                                      ItemName = g.Max(x => x.ItemName),
                                      StockRate = (decimal)g.Max(x => x.StockRate)
                                  }).ToList();
                    var openValues = getOpenValues(qry);
                    if (openValues.Count > 0)
                    {
                        for (int i = 0; i < result.Count - 1; i++)
                        {
                            try
                            {
                                if (result[i].ItemId == openValues[i].ItemId.ToString())
                                {
                                    result[i].OpenQuantity = (decimal)openValues[i].OpenQty;
                                    result[i].OpenValue = (decimal)openValues[i].OpenValue;
                                }
                                else
                                {
                                    result[i].OpenQuantity = 0;
                                    result[i].OpenValue = 0;
                                }

                            }
                            catch
                            {
                                result[i].OpenQuantity = 0;
                                result[i].OpenValue = 0;
                            }
                        }
                    }
                    if (sFilter.ExcludeZeroBalance == true)
                    {
                        result = result.Where(x => x.ClosingValue > 0).Select(x => x).ToList();
                    }
                    if (sFilter.ExcludeZeroOpeningBalance == true)
                    {
                        result = result.Where(x => x.OpenValue > 0).Select(x => x).ToList();
                    }
                    return Response<List<StockLedgerResponse>>.Success(result, "Data Found");

                }
                else
                {
                    var minFDate = _fP.GetAsQueryable().Select(x => x.FinancialPeriodsStartDate).SingleOrDefault();

                    var items = _iMster.GetAsQueryable().Where(i => i.ItemMasterItemType == "A" && i.ItemMasterDelStatus == null).Where($"1==1 {itemQry}").ToList();

                    qry += $" and StockRegisterAssignedDate.Value.Date > DateTime({minFDate.Value.Year},{minFDate.Value.Month},{minFDate.Value.Day})";


                    var otherValues = (from s in _sRegister.GetAsQueryable().Where($"1==1 {itemQry}")
                                       join im in _iMster.GetAsQueryable() on s.StockRegisterMaterialID equals (int?)im.ItemMasterItemId
                                       group s by s.StockRegisterMaterialID into g
                                       select new
                                       {
                                           ItemId = (long)g.Key,
                                           TotRevd = g.Sum(x => x.StockRegisterTransType != "SR" ? x.StockRegisterSIN : 0),
                                           TotRevdAmt = g.Sum(x => x.StockRegisterTransType != "SR" ? x.StockRegisterSIN * x.StockRegisterRate : 0),

                                           TOTSaled = g.Sum(x => x.StockRegisterSout - (x.StockRegisterTransType == "SR" ? x.StockRegisterSIN : 0)),
                                           TOTSaledAmt = g.Sum(x => x.StockRegisterSout * x.StockRegisterRate - (x.StockRegisterTransType == "SR" ? x.StockRegisterSIN * x.StockRegisterRate : 0)),

                                           TolSin = g.Sum(x => x.StockRegisterSIN),
                                           TolSOut = g.Sum(x => x.StockRegisterSout),

                                           StockRate = g.Max(x => x.StockRegisterRate)
                                       }

                                ).ToList();



                    var result = (from im in items
                                  join sr in otherValues on im.ItemMasterItemId equals sr.ItemId
                                  group new
                                  {
                                      Mat_ID = sr.ItemId,
                                      PartNo = im.ItemMasterPartNo,
                                      ItemName = im.ItemMasterItemName,

                                      OpenQty = 0,
                                      OpenQtyAmt = 0,

                                      TotRevd = sr.TotRevd,
                                      TotRevdAmt = sr.TotRevdAmt,

                                      TOTSaled = sr.TOTSaled,
                                      TOTSaledAmt = sr.TOTSaledAmt,

                                      BalaTot = sr.TolSin - sr.TolSOut,
                                      BalaTotamt = (sr.TolSin - sr.TolSOut) * sr.StockRate,


                                      RelativeNo = im.ItemMasterAccountNo,
                                      StockRate = sr.StockRate
                                  } by im.ItemMasterItemId into g
                                  select new StockLedgerResponse()
                                  {
                                      ItemId = g.Key.ToString(),
                                      OpenQuantity = (decimal)g.Sum(x => x.OpenQty),
                                      OpenValue = (decimal)g.Sum(x => x.OpenQtyAmt),
                                      ReceivedQuantity = (decimal)g.Sum(x => x.TotRevd),
                                      ReceivedValue = (decimal)g.Sum(x => x.TotRevdAmt),
                                      SaledQuantity = (decimal)g.Sum(x => x.TOTSaled),
                                      SaledValue = (decimal)g.Sum(x => x.TOTSaledAmt),
                                      ClosingQuantity = (decimal)g.Sum(x => x.BalaTot),
                                      ClosingValue = (decimal)g.Sum(x => x.BalaTotamt),
                                      RelativeNo = g.Max(x => x.RelativeNo).ToString(),
                                      PartNo = g.Max(x => x.PartNo),
                                      ItemName = g.Max(x => x.ItemName),
                                      StockRate = (decimal)g.Max(x => x.StockRate)
                                  }).ToList();
                    var openValues = getOpenValues(qry);
                    if (openValues.Count > 0)
                    {
                        for (int i = 0; i < result.Count - 1; i++)
                        {
                            try
                            {
                                if (result[i].ItemId == openValues[i].ItemId.ToString())
                                {
                                    result[i].OpenQuantity = (decimal)openValues[i].OpenQty;
                                    result[i].OpenValue = (decimal)openValues[i].OpenValue;
                                }
                                else
                                {
                                    result[i].OpenQuantity = 0;
                                    result[i].OpenValue = 0;
                                }

                            }
                            catch
                            {
                                result[i].OpenQuantity = 0;
                                result[i].OpenValue = 0;
                            }
                        }
                    }
                    if (sFilter.ExcludeZeroBalance == true)
                    {
                        result = result.Where(x => x.ClosingValue > 0).Select(x => x).ToList();
                    }
                    if (sFilter.ExcludeZeroOpeningBalance == true)
                    {
                        result = result.Where(x => x.OpenValue > 0).Select(x => x).ToList();
                    }
                    return Response<List<StockLedgerResponse>>.Success(result, "Data Found");
                }
            }
            catch (Exception ex)
            {
                return Response<List<StockLedgerResponse>>.Fail(new List<StockLedgerResponse>(), ex.Message);
            }
        }

        private List<OpenValues> getOpenValues(string qry)
        {
            return _sRegister.GetAsQueryable().Where($"1==1 {qry}")
                            .Select(x => new OpenValues()
                            {
                                ItemId = x.StockRegisterMaterialID,
                                OpenQty = x.StockRegisterSIN - x.StockRegisterSout,
                                OpenValue = x.StockRegisterSIN * x.StockRegisterRate - x.StockRegisterSout * x.StockRegisterRate,

                            }).ToList();

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

    public class OpenValues
    {
        public int? ItemId { get; set; }
        public decimal? OpenQty { get; set; }
        public decimal? OpenValue { get; set; }
    }
}
