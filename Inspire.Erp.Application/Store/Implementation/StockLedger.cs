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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Inspire.Erp.Infrastructure.Database;

namespace Inspire.Erp.Application.Store.Implementation
{
    public class StockLedger : IStockLedger
    {
        private InspireErpDBContext _context;
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
                if (sFilter.LocationId == -1)
                {
                    sFilter.LocationId = null;
                }
                if (sFilter.IsDateChecked == true)
                {
                    //var items = _iMster.GetAsQueryable().Where(i => i.ItemMasterItemType == "A" && i.ItemMasterDelStatus == null
                    //&& (sFilter.ItemGroupId == null || i.ItemMasterAccountNo == sFilter.ItemGroupId)
                    //&& (sFilter.PartNo == null || i.ItemMasterPartNo == sFilter.PartNo)
                    //).ToList();

                    var query = _iMster.GetAsQueryable();

                    if (sFilter.ItemId > 0)
                    {
                        query = query.Where(im => im.ItemMasterItemId == sFilter.ItemId && im.ItemMasterDelStatus != true);
                    }
                    if (sFilter.ItemGroupId > 0)
                    {
                        query = query.Where(im => im.ItemMasterAccountNo == sFilter.ItemGroupId && im.ItemMasterItemType == "A" && im.ItemMasterDelStatus != true)
                                     .OrderBy(im => im.ItemMasterItemId);
                    }
                    else
                    {
                        query = query.Where(im => im.ItemMasterItemType == "A" && im.ItemMasterDelStatus != true)
                                     .OrderBy(im => im.ItemMasterItemId);
                    }

                    var stock = _sRegister.GetAsQueryable();

                    //var otherValues = (from s in _sRegister.GetAsQueryable()
                    //                   join im in _iMster.GetAsQueryable() on s.StockRegisterMaterialID equals (int)im.ItemMasterItemId
                    //                   where im.ItemMasterItemType == "A" && (sFilter.ItemId == null || im.ItemMasterItemId == sFilter.ItemId)
                    //                   //&& (sFilter.LocationId == null || s.StockRegisterLocationID == sFilter.LocationId)
                    //                   //&& (sFilter.PartNo == null || im.ItemMasterPartNo == sFilter.PartNo)
                    //                   //&& (sFilter.ItemGroupId == null || im.ItemMasterAccountNo == sFilter.ItemGroupId)
                    //                   //&& (s.StockRegisterAssignedDate >= sFilter.fromDate && s.StockRegisterAssignedDate <= sFilter.toDate)
                    //                   group s by s.StockRegisterMaterialID into g
                    //                   select new
                    //                   {
                    //                       ItemId = (long)g.Key,
                    //                       TotRevd = g.Sum(x => x.StockRegisterTransType != "SR" ? x.StockRegisterSIN : 0),
                    //                       TotRevdAmt = g.Sum(x => x.StockRegisterTransType != "SR" ? x.StockRegisterSIN * x.StockRegisterRate : 0),

                    //                       TOTSaled = g.Sum(x => x.StockRegisterSout - (x.StockRegisterTransType == "SR" ? x.StockRegisterSIN : 0)),
                    //                       TOTSaledAmt = g.Sum(x => x.StockRegisterSout * x.StockRegisterRate - (x.StockRegisterTransType == "SR" ? x.StockRegisterSIN * x.StockRegisterRate : 0)),


                    //                       TolSin = g.Sum(x => x.StockRegisterSIN),
                    //                       TolSOut = g.Sum(x => x.StockRegisterSout),

                    //                       StockRate = g.Max(x => x.StockRegisterRate)
                    //                   }

                    //            ).ToList();


                    //var result = (from im in items
                    //              join sr in otherValues on im.ItemMasterItemId equals sr.ItemId
                    //              group new
                    //              {
                    //                  Mat_ID = sr.ItemId,
                    //                  PartNo = im.ItemMasterPartNo,
                    //                  ItemName = im.ItemMasterItemName,

                    //                  OpenQty = 0,
                    //                  OpenQtyAmt = 0,

                    //                  TotRevd = sr.TotRevd,
                    //                  TotRevdAmt = sr.TotRevdAmt,

                    //                  TOTSaled = sr.TOTSaled,
                    //                  TOTSaledAmt = sr.TOTSaledAmt,

                    //                  BalaTot = sr.TolSin - sr.TolSOut,
                    //                  BalaTotamt = (sr.TolSin - sr.TolSOut) * sr.StockRate,

                    //                  RelativeNo = im.ItemMasterAccountNo,
                    //                  StockRate = sr.StockRate
                    //              } by im.ItemMasterItemId into g
                    //              select new StockLedgerResponse()
                    //              {
                    //                  ItemId = g.Key.ToString(),
                    //                  OpenQuantity = (decimal)g.Sum(x => x.OpenQty),
                    //                  OpenValue = (decimal)g.Sum(x => x.OpenQtyAmt),
                    //                  ReceivedQuantity = (decimal)g.Sum(x => x.TotRevd),
                    //                  ReceivedValue = (decimal)g.Sum(x => x.TotRevdAmt),
                    //                  SaledQuantity = (decimal)g.Sum(x => x.TOTSaled),
                    //                  SaledValue = (decimal)g.Sum(x => x.TOTSaledAmt),
                    //                  ClosingQuantity = (decimal)g.Sum(x => x.BalaTot),
                    //                  ClosingValue = (decimal)g.Sum(x => x.BalaTotamt),
                    //                  RelativeNo = g.Max(x => x.RelativeNo).ToString(),
                    //                  PartNo = g.Max(x => x.PartNo),
                    //                  ItemName = g.Max(x => x.ItemName),
                    //                  StockRate = (decimal)g.Max(x => x.StockRate)
                    //              }).ToList();

                    var result = (from im in query
                                  join sr in stock on im.ItemMasterItemId equals (long)sr.StockRegisterMaterialID.GetValueOrDefault() into gimV1
                                  from y in gimV1.DefaultIfEmpty()
                                  where im.ItemMasterItemType == "A"
                                  && (sFilter.LocationId == null || y.StockRegisterLocationID == sFilter.LocationId)
                                  && (sFilter.PartNo == null || im.ItemMasterPartNo == sFilter.PartNo)
                                  select new
                                  {
                                      im.ItemMasterItemId,
                                      im.ItemMasterItemName,
                                      y.StockRegisterSIN,
                                      y.StockRegisterSout,
                                      y.StockRegisterVoucherDate,
                                      y.StockRegisterAmount,
                                      im.ItemMasterPartNo,
                                      im.ItemMasterAccountNo,
                                  })
                                   .AsEnumerable().Select(x => new
                                   {
                                       Item_Id = x.ItemMasterItemId,
                                       Item_Name = x.ItemMasterItemName,
                                       StockIn = (x.StockRegisterSIN != null && x.StockRegisterSIN > 0 && x.StockRegisterVoucherDate >= sFilter.fromDate && x.StockRegisterVoucherDate <= sFilter.toDate) ? x.StockRegisterSIN : 0,
                                       StockInAmount = (x.StockRegisterSIN != null && x.StockRegisterSIN > 0 && x.StockRegisterVoucherDate >= sFilter.fromDate && x.StockRegisterVoucherDate <= sFilter.toDate) ? x.StockRegisterAmount ?? 0 : 0,
                                       StockOut = (x.StockRegisterSout != null && x.StockRegisterSout > 0 && x.StockRegisterVoucherDate >= sFilter.fromDate && x.StockRegisterVoucherDate <= sFilter.toDate) ? x.StockRegisterSout ?? 0 : 0,
                                       StockOutAmount = (x.StockRegisterSout != null && x.StockRegisterSout > 0 && x.StockRegisterVoucherDate >= sFilter.fromDate && x.StockRegisterVoucherDate <= sFilter.toDate) ? x.StockRegisterAmount ?? 0 : 0,
                                       OpenQty = (x.StockRegisterSIN != null && x.StockRegisterSIN > 0 && x.StockRegisterVoucherDate >= sFilter.fromDate && x.StockRegisterVoucherDate <= sFilter.toDate) ? (x.StockRegisterSIN ?? 0) - (x.StockRegisterSout ?? 0) : 0,
                                       OpenQtyAmount = (x.StockRegisterSIN != null && x.StockRegisterSIN > 0 && x.StockRegisterVoucherDate >= sFilter.fromDate && x.StockRegisterVoucherDate <= sFilter.toDate) ? (x.StockRegisterAmount ?? 0) - (x.StockRegisterAmount ?? 0) : 0,
                                       TotalBal = 0,
                                       TotalBalAmount = 0,
                                       ItemMasterPartNo = x.ItemMasterPartNo,
                                       ItemMasterAccountNo = x.ItemMasterAccountNo,
                                   }).GroupBy(
                         m => new { m.Item_Id, m.Item_Name },
                         (k, g) => new StockLedgerResponse()
                         {
                             ItemId = k.Item_Id.ToString(),
                             OpenQuantity = 0,
                             OpenValue = 0,
                             ReceivedQuantity = (decimal)g.Sum(x => x.StockIn),
                             ReceivedValue = (decimal)g.Sum(x => x.StockInAmount),
                             SaledQuantity = (decimal)g.Sum(x => x.StockOut),
                             SaledValue = (decimal)g.Sum(x => x.StockOutAmount),
                             ClosingQuantity = (decimal)g.Sum(a => a.StockIn) - g.Sum(a => a.StockOut),
                             ClosingValue = g.Sum(a => a.StockInAmount) - g.Sum(a => a.StockOutAmount),
                             RelativeNo = g.Max(x => x.ItemMasterPartNo),
                             PartNo = g.Max(x => x.ItemMasterAccountNo.ToString()),
                             ItemName = g.Max(x => x.Item_Name),
                             StockRate = (decimal)g.Max(x => x.StockInAmount),
                         }).ToList();

                    var openValues = getOpenValues(sFilter.fromDate, (int?)sFilter.ItemId);
                    if (openValues.Count > 0)
                    {
                        foreach (var openValue in openValues)
                        {
                            if (openValue.ItemId != null && openValue.ItemId != 0)
                            {
                                var matchedResult = result.FirstOrDefault(r => r.ItemId.ToString() == openValue.ItemId.ToString());

                                if (matchedResult != null)
                                {
                                    matchedResult.OpenQuantity = openValue.OpenQty.HasValue ? Convert.ToDecimal(openValue.OpenQty.Value) : 0;
                                    matchedResult.OpenValue = openValue.OpenValue.HasValue ? Convert.ToDecimal(openValue.OpenValue.Value) : 0;
                                }
                                
                            }
                        }
                    }

                    //if (openValues.Count > 0)
                    //{
                    //    var openValuesDict = openValues.ToDictionary(ov => (int?)ov.ItemId); // Convert to int? if necessary

                    //    for (int i = 0; i < result.Count; i++)
                    //    {
                    //        try
                    //        {
                    //            // Convert the ItemId from string to nullable integer
                    //            if (int.TryParse(result[i].ItemId, out var itemId))
                    //            {
                    //                if (openValuesDict.TryGetValue(itemId, out var openValue))
                    //                {
                    //                    result[i].OpenQuantity = openValue.OpenQty ?? 0;
                    //                    result[i].OpenValue = openValue.OpenValue ?? 0;
                    //                }
                    //                else
                    //                {
                    //                    result[i].OpenQuantity = 0;
                    //                    result[i].OpenValue = 0;
                    //                }
                    //            }
                    //            else
                    //            {
                    //                // If parsing fails, set default values
                    //                result[i].OpenQuantity = 0;
                    //                result[i].OpenValue = 0;
                    //            }
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            // Log the exception if needed
                    //            Console.WriteLine($"Error processing item {result[i].ItemId}: {ex.Message}");
                    //            result[i].OpenQuantity = 0;
                    //            result[i].OpenValue = 0;
                    //        }
                    //    }
                    //}

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

                    var query = _iMster.GetAsQueryable();

                    if (sFilter.ItemId > 0)
                    {
                        query = query.Where(im => im.ItemMasterItemId == sFilter.ItemId && im.ItemMasterDelStatus != true);
                    }
                    if (sFilter.ItemGroupId > 0)
                    {
                        query = query.Where(im => im.ItemMasterAccountNo == sFilter.ItemGroupId && im.ItemMasterItemType == "A" && im.ItemMasterDelStatus != true)
                                     .OrderBy(im => im.ItemMasterItemId);
                    }
                    else
                    {
                        query = query.Where(im => im.ItemMasterItemType == "A" && im.ItemMasterDelStatus != true)
                                     .OrderBy(im => im.ItemMasterItemId);
                    }
                    var stock = _sRegister.GetAsQueryable();


                    //        var otherValues = (from im in _iMster.GetAll()
                    //.Where(x => x.ItemMasterItemType == "A" && x.ItemMasterDelStatus != true)
                    //            join sr in _sRegister.GetAll()
                    //            on im.ItemMasterItemId equals (long?)sr.StockRegisterMaterialID into stocks
                    //            from sr in stocks.DefaultIfEmpty()
                    //                where im.ItemMasterItemType == "A"
                    //                    && (sFilter.ItemId == null || im.ItemMasterItemId == sFilter.ItemId)
                    //                    && (sFilter.LocationId == null || sr.StockRegisterLocationID == sFilter.LocationId)
                    //                    && (sFilter.PartNo == null || im.ItemMasterPartNo == sFilter.PartNo)
                    //                    && (sFilter.ItemGroupId == null || im.ItemMasterAccountNo == sFilter.ItemGroupId)
                    //                    && (sr == null || sr.StockRegisterAssignedDate > minFDate) // Include items with no StockRegister or valid StockRegister date
                    //                group new { im, sr } by im.ItemMasterItemId into g
                    //                select new
                    //                {
                    //                    ItemId = (long)g.Key,
                    //                    TotRevd = g.Sum(x => x.sr != null ? x.sr.StockRegisterSIN : 0),
                    //                    TotRevdAmt = g.Sum(x => x.sr != null ? x.sr.StockRegisterAmount  : 0),
                    //                    TOTSaled = g.Sum(x => x.sr != null ? x.sr.StockRegisterSout - (x.sr.StockRegisterSIN?? 0) : 0),
                    //                    TOTSaledAmt = g.Sum(x => x.sr != null ? (x.sr.StockRegisterAmount  - (x.sr.StockRegisterAmount?? 0)) : 0),
                    //                    TolSin = g.Sum(x => x.sr != null ? x.sr.StockRegisterSIN : 0),
                    //                    TolSOut = g.Sum(x => x.sr != null ? x.sr.StockRegisterSout : 0),
                    //                    StockRate = g.Max(x => x.sr != null ? x.sr.StockRegisterRate : 0)
                    //                }).ToList();

                    var result = (from im in query
                                  join sr in stock on im.ItemMasterItemId equals (long)sr.StockRegisterMaterialID.GetValueOrDefault() into gimV1
                                  from y in gimV1.DefaultIfEmpty()
                                  where im.ItemMasterItemType == "A"
                                  && (sFilter.LocationId == null || y.StockRegisterLocationID == sFilter.LocationId)
                                  && (sFilter.PartNo == null || im.ItemMasterPartNo == sFilter.PartNo)
                                  //&& (y.StockRegisterVoucherDate >= minFDate)
                                  select new
                                  {
                                      im.ItemMasterItemId,
                                      im.ItemMasterItemName,
                                      y.StockRegisterSIN,
                                      y.StockRegisterSout,
                                      y.StockRegisterVoucherDate,
                                      y.StockRegisterAmount,
                                      im.ItemMasterPartNo,
                                      im.ItemMasterAccountNo,
                                  })
                                  .AsEnumerable().Select(x => new
                                  {
                                      Item_Id = x.ItemMasterItemId,
                                      Item_Name = x.ItemMasterItemName,
                                      StockIn = (x.StockRegisterSIN != null && x.StockRegisterSIN > 0) ? x.StockRegisterSIN : 0,
                                      StockInAmount = (x.StockRegisterSIN != null && x.StockRegisterSIN > 0) ? x.StockRegisterAmount ?? 0 : 0,
                                      StockOut = (x.StockRegisterSout != null && x.StockRegisterSout > 0) ? x.StockRegisterSout ?? 0 : 0,
                                      StockOutAmount = (x.StockRegisterSout != null && x.StockRegisterSout > 0) ? x.StockRegisterAmount ?? 0 : 0,
                                      OpenQty = (x.StockRegisterSIN != null && x.StockRegisterSIN > 0) ? (x.StockRegisterSIN ?? 0) - (x.StockRegisterSout ?? 0) : 0,
                                      OpenQtyAmount = (x.StockRegisterSIN != null && x.StockRegisterSIN > 0) ? (x.StockRegisterAmount ?? 0) - (x.StockRegisterAmount ?? 0) : 0,
                                      TotalBal = 0,
                                      TotalBalAmount = 0,
                                      ItemMasterPartNo = x.ItemMasterPartNo,
                                      ItemMasterAccountNo = x.ItemMasterAccountNo,
                                  }).GroupBy(
                        m => new { m.Item_Id, m.Item_Name },
                        (k, g) => new StockLedgerResponse()
                        {
                            ItemId = k.Item_Id.ToString(),
                            OpenQuantity = 0,
                            OpenValue = 0,
                            ReceivedQuantity = (decimal)g.Sum(x => x.StockIn),
                            ReceivedValue = (decimal)g.Sum(x => x.StockInAmount),
                            SaledQuantity = (decimal)g.Sum(x => x.StockOut),
                            SaledValue = (decimal)g.Sum(x => x.StockOutAmount),
                            ClosingQuantity = (decimal)g.Sum(a => a.StockIn) - g.Sum(a => a.StockOut),
                            ClosingValue = g.Sum(a => a.StockInAmount) - g.Sum(a => a.StockOutAmount),
                            RelativeNo = g.Max(x => x.ItemMasterPartNo),
                            PartNo = g.Max(x => x.ItemMasterAccountNo.ToString()),
                            ItemName = g.Max(x => x.Item_Name),
                            StockRate = (decimal)g.Max(x => x.StockInAmount),
                        }).ToList();


                    var openValues = getOpenValues((DateTime)minFDate, (int?)sFilter.ItemId);


                    if (openValues.Count > 0)
                    {
                        foreach (var openValue in openValues)
                        {
                            if (openValue.ItemId != null && openValue.ItemId != 0)
                            {
                                var matchedResult = result.FirstOrDefault(r => r.ItemId.ToString() == openValue.ItemId.ToString());

                                if (matchedResult != null)
                                {
                                    matchedResult.OpenQuantity = openValue.OpenQty.HasValue ? Convert.ToDecimal(openValue.OpenQty.Value) : 0;
                                    matchedResult.OpenValue = openValue.OpenValue.HasValue ? Convert.ToDecimal(openValue.OpenValue.Value) : 0;
                                }                                
                            }
                        }
                    }

                    //var result = (from im in items
                    //              join sr in otherValues on im.ItemMasterItemId equals sr.ItemId
                    //              group new
                    //              {
                    //                  Mat_ID = sr.ItemId,
                    //                  PartNo = im.ItemMasterPartNo,
                    //                  ItemName = im.ItemMasterItemName,

                    //                  OpenQty = 0,
                    //                  OpenQtyAmt = 0,

                    //                  TotRevd = sr.TotRevd,
                    //                  TotRevdAmt = sr.TotRevdAmt,

                    //                  TOTSaled = sr.TOTSaled,
                    //                  TOTSaledAmt = sr.TOTSaledAmt,

                    //                  BalaTot = sr.TolSin - sr.TolSOut,
                    //                  BalaTotamt = (sr.TolSin - sr.TolSOut) * sr.StockRate,


                    //                  RelativeNo = im.ItemMasterAccountNo,
                    //                  StockRate = sr.StockRate
                    //              } by im.ItemMasterItemId into g
                    //              select new StockLedgerResponse()
                    //              {
                    //                  ItemId = g.Key.ToString(),
                    //                  OpenQuantity = (decimal)g.Sum(x => x.OpenQty),
                    //                  OpenValue = (decimal)g.Sum(x => x.OpenQtyAmt),
                    //                  ReceivedQuantity = (decimal)g.Sum(x => x.TotRevd),
                    //                  ReceivedValue = (decimal)g.Sum(x => x.TotRevdAmt),
                    //                  SaledQuantity = (decimal)g.Sum(x => x.TOTSaled),
                    //                  SaledValue = (decimal)g.Sum(x => x.TOTSaledAmt),
                    //                  ClosingQuantity = (decimal)g.Sum(x => x.BalaTot),
                    //                  ClosingValue = (decimal)g.Sum(x => x.BalaTotamt),
                    //                  RelativeNo = g.Max(x => x.RelativeNo).ToString(),
                    //                  PartNo = g.Max(x => x.PartNo),
                    //                  ItemName = g.Max(x => x.ItemName),
                    //                  StockRate = (decimal)g.Max(x => x.StockRate)
                    //              }).ToList();


                    //if (openValues.Count > 0)
                    //{
                    //    var openValuesDict = openValues.ToDictionary(ov => (int?)ov.ItemId); // Convert to int? if necessary

                    //    for (int i = 0; i < result.Count; i++)
                    //    {
                    //        try
                    //        {
                    //            // Convert the ItemId from string to nullable integer
                    //            if (int.TryParse(result[i].ItemId, out var itemId))
                    //            {
                    //                if (openValuesDict.TryGetValue(itemId, out var openValue))
                    //                {
                    //                    result[i].OpenQuantity = openValue.OpenQty ?? 0;
                    //                    result[i].OpenValue = openValue.OpenValue ?? 0;
                    //                }
                    //                else
                    //                {
                    //                    result[i].OpenQuantity = 0;
                    //                    result[i].OpenValue = 0;
                    //                }
                    //            }
                    //            else
                    //            {
                    //                // If parsing fails, set default values
                    //                result[i].OpenQuantity = 0;
                    //                result[i].OpenValue = 0;
                    //            }
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            // Log the exception if needed
                    //            Console.WriteLine($"Error processing item {result[i].ItemId}: {ex.Message}");
                    //            result[i].OpenQuantity = 0;
                    //            result[i].OpenValue = 0;
                    //        }
                    //    }
                    //}

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

        //private List<OpenValues> getOpenValues(DateTime fromdate, int? itemId)
        //{
        //    return _sRegister.GetAsQueryable().Where(a => a.StockRegisterVoucherDate >= fromdate && a.StockRegisterVoucherDate != null)
        //                    .Select(x => new OpenValues()
        //                    {
        //                        ItemId = x.StockRegisterMaterialID,
        //                        OpenQty = x.StockRegisterSIN - x.StockRegisterSout,
        //                        OpenValue = x.StockRegisterSIN * x.StockRegisterRate - x.StockRegisterSout * x.StockRegisterRate,

        //                    }).ToList();

        //}
        private List<OpenValues> getOpenValues(DateTime fromDate, int? itemId)
        {
            // Query to filter and group by ItemId
            var query = _sRegister.GetAsQueryable()
                .Where(a => a.StockRegisterVoucherDate < fromDate);

            // Apply itemId filter only if it's not null
            if (itemId.HasValue)
            {
                query = query.Where(a => a.StockRegisterMaterialID == itemId.Value);
            }

            // Group by ItemId and calculate the aggregated values
            var result = query
                .GroupBy(x => x.StockRegisterMaterialID)
                .Select(g => new OpenValues
                {
                    ItemId = g.Key,
                    OpenQty = g.Sum(x => x.StockRegisterSIN) - g.Sum(x => x.StockRegisterSout),
                    OpenValue = g.Sum(x => (x.StockRegisterSIN * x.StockRegisterRate) - (x.StockRegisterSout * x.StockRegisterRate))
                })
                .ToList();

            return result;
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
