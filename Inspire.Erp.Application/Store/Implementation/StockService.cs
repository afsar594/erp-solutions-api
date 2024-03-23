////using System;
////using System.Collections.Generic;
////using System.Text;

////namespace Inspire.Erp.Application.Store.Implementation
////{
////    class OpeningStockService
////    {
////    }
////}
///
using Inspire.Erp.Application.Store.Interfaces;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Inspire.Erp.Infrastructure.Database;
using Inspire.Erp.Application.Account.Implementations;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Modals.Stock;
using Inspire.Erp.Domain.Modals;
using System.Threading.Tasks;
using Spire.Pdf.Exporting.XPS.Schema;
using Inspire.Erp.Domain.Modals.Procurement;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Inspire.Erp.Application.Store.Implementation
{
    public class StockService : IStockService
    {
        private InspireErpDBContext _context;

        private readonly ILogger<StockService> _logger;

        public StockService(InspireErpDBContext context, ILogger<StockService> logger)
        {
            
            _logger = logger;
            _context = context;

        }

        public async Task<Response<List<StockReportBaseUnitResponse>>>   GetStockReportBaseUnitList(StockFilterModel model)
        {
            try
            {
                List<StockReportBaseUnitResponse> response = new List<StockReportBaseUnitResponse>();

                var subrq = (from s in _context.UnitDetails
                             group s by new { s.UnitDetailsItemId, s.UnitDetailsId } into g
                             select new
                             {
                                 ItemId = g.Key.UnitDetailsItemId,
                                 UnitId = g.Key.UnitDetailsId,
                                 ConTyp = g.Max(s => s.UnitDetailsConversionType)
                             });


                       var results = await (from im in _context.ItemMaster.Where(x=>x.ItemMasterItemName.ToLower().Contains(model.ItemName.ToLower())
                                            // && x.ItemMasterDefaultLocationName.ToLower().Contains(model.Location.ToLower()) 
                                            && (model.LocationId == null || x.ItemMasterLocationId == model.LocationId)
                                            && (model.ItemId == null || x.ItemMasterItemId == model.ItemId)
                                            && (model.ItemGroupId==null || x.ItemMasterRef1==model.ItemGroupId.ToString())
                                            )

                            join sr in _context.StockRegister on im.ItemMasterItemId equals sr.StockRegisterMaterialID.GetValueOrDefault() into gim
                            from y in gim.DefaultIfEmpty()
                                  join r in subrq on y.StockRegisterMaterialID.GetValueOrDefault() equals r.ItemId into grj
                                  from q in grj.DefaultIfEmpty()
                                  join um in _context.UnitMaster on q.UnitId equals um.UnitMasterUnitId into gum
                                  from n in gum.DefaultIfEmpty()
                                  where im.ItemMasterItemType == "A"
                                  select new StockReportBaseUnitResponse
                                    {
                                      ItemId=  y.StockRegisterMaterialID,
                                      ItemName= im.ItemMasterItemName,
                                      Stock= (int)((y.StockRegisterSIN??0 - y.StockRegisterSout??0) /(decimal)q.ConTyp),
                                      Unit=   q.UnitId,
                                      PartNo =  n.UnitMasterUnitShortName,
                                      AverageRate=y.StockRegisterRate,
                                      Amount=y.StockRegisterAmount
                                    }).ToListAsync();


               

                          return Response<List< StockReportBaseUnitResponse >>.Success(results, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<StockReportBaseUnitResponse>>.Fail(new List<StockReportBaseUnitResponse>(), ex.Message);
            }
        }
        public async Task<Response<List<StockReportBaseUnitResponse>>> GetStockReportBaseUnit(StockFilterModel model)
        {
            try
            {
                List<StockReportBaseUnitResponse> response = new List<StockReportBaseUnitResponse>();
                var query = _context.ItemMaster.AsQueryable();
                if (model.ItemId > 0)
                {
                    query = query.Where(im => im.ItemMasterItemId == model.ItemId);
                }
                else if (model.ItemGroupId > 0)
                {
                    query = query.Where(im => im.ItemMasterAccountNo == model.ItemGroupId && im.ItemMasterItemType == "A")
                                 .OrderBy(im => im.ItemMasterItemId);
                }
                else
                {
                    query = query.Where(im => im.ItemMasterItemType == "A")
                                 .OrderBy(im => im.ItemMasterItemId);
                }


                var subrq = (from s in _context.UnitDetails.Where(x => x.UnitDetailsConversionType == 1)
                             select new
                             {
                                 ItemId = s.UnitDetailsItemId,
                                 UnitId = s.UnitDetailsId
                             });

                var stockBaseUnits = (from im in query
                                      join sr in _context.StockRegister on im.ItemMasterItemId equals sr.StockRegisterMaterialID.GetValueOrDefault() into gim
                                      from y in gim.DefaultIfEmpty()
                                      join r in subrq on y.StockRegisterMaterialID.GetValueOrDefault() equals r.ItemId into grj
                                      from q in grj.DefaultIfEmpty()
                                      join um in _context.UnitMaster on q.UnitId equals um.UnitMasterUnitId into gum
                                      from n in gum.DefaultIfEmpty()
                                      where im.ItemMasterItemType == "A"
                                      select new StockReportBaseUnitResponse
                                      {
                                          ItemId = (long)im.ItemMasterItemId,
                                          ItemName = im.ItemMasterItemName,
                                          Stock = (int)(y.StockRegisterSIN ?? 0 - y.StockRegisterSout ?? 0),
                                          Unit = q.UnitId,
                                          PartNo = n.UnitMasterUnitShortName,
                                      }).GroupBy(
        m => new { m.ItemId, m.ItemName, m.Unit, m.PartNo },
        (k, g) => new { k.ItemId, k.ItemName, k.Unit, k.PartNo, Stock = g.Sum(x => x.Stock) }
    )
    .Select(am => new
    {
        ItemId = am.ItemId,
        ItemName = am.ItemName,
        Stock = am.Stock,
        Unit = am.Unit,
        PartNo = am.PartNo,
    });

                // dbo].[GetStockValue]
                var stockValues = (from ss in query
                                   join a in _context.StockRegister on ss.ItemMasterItemId equals (long)a.StockRegisterMaterialID.GetValueOrDefault()
                                   group a by a.StockRegisterMaterialID into g
                                   select new
                                   {
                                       ItemId = (long)g.Key,
                                       TotalQty = g.Sum(x => x.StockRegisterSIN),
                                       StockRate = g.Sum(x => x.StockRegisterSIN * x.StockRegisterRate),
                                       StockValue = (decimal)g.Sum(x => x.StockRegisterSIN) * (g.Sum(x => x.StockRegisterSIN) > 0 ? g.Sum(x => x.StockRegisterSIN * x.StockRegisterRate) / g.Sum(x => x.StockRegisterSIN) : 0)
                                   });

                var results = await (from im in stockBaseUnits
                                     join sr in stockValues on im.ItemId equals sr.ItemId into gim
                                     from y in gim.DefaultIfEmpty()
                                      select new StockReportBaseUnitResponse
                                     {
                                         ItemId = im.ItemId,
                                         ItemName = im.ItemName,
                                         Stock = im.Stock,
                                         Unit =im.Unit,
                                         PartNo = im.PartNo,
                                         AverageRate = y.StockRate,
                                         Amount = im.Stock * y.StockRate,
                                         AveragePurchaseValue = y.StockValue,
                                     }).ToListAsync();



                return Response<List<StockReportBaseUnitResponse>>.Success(results, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<StockReportBaseUnitResponse>>.Fail(new List<StockReportBaseUnitResponse>(), ex.Message);
            }
        }
        public async Task<Response<StockMovementReportResponse>> GetStockMovementReport(StockFilterModel model)
        {
            try
            {
                var stockMov = new StockMovementReportResponse();
                var query =   _context.ItemMaster.AsQueryable();
                if (model.ItemId > 0)
                {
                    query = query.Where(im => im.ItemMasterItemId == model.ItemId);
                }
                else if (model.ItemGroupId > 0)
                {
                    query = query.Where(im => im.ItemMasterAccountNo == model.ItemGroupId && im.ItemMasterItemType == "A")
                                 .OrderBy(im => im.ItemMasterItemId);
                }
                else
                {
                    query = query.Where(im => im.ItemMasterItemType == "A")
                                 .OrderBy(im => im.ItemMasterItemId);
                }

                var GroupItemIds =  _context.ItemMaster.Where(o => o.ItemMasterItemId == (long)model.ItemGroupId).Select(o => o.ItemMasterItemId).ToList();

                var MatIds = await _context.StockRegister.Select(o => o.StockRegisterMaterialID).ToListAsync();

                //var subrq = (from s in _context.StockRegister.Where(x => x.StockRegisterMaterialID == model.ItemId
                //             && GroupItemIds.Contains((long)x.StockRegisterMaterialID)
                //             )
                //             select new
                //             {
                //                 ItemId = s.UnitDetailsItemId,
                //                 UnitId = s.UnitDetailsId
                //             });
                //&& GroupItemIds.Contains((long)y.StockRegisterMaterialID)
                var stockInOut = (from im in query
                                      join sr in _context.StockRegister on im.ItemMasterItemId equals sr.StockRegisterMaterialID.GetValueOrDefault() into gim
                                      from y in gim.DefaultIfEmpty()
                                      join ig in _context.ItemMaster on im.ItemMasterAccountNo equals ig.ItemMasterItemId
                                      where im.ItemMasterItemType == "A"   
                                       && MatIds.Contains((int)im.ItemMasterItemId)
                                      select new 
                                      {
                                          ItemId = (long)im.ItemMasterItemId,
                                          ItemMasterAccountNo = im.ItemMasterAccountNo,
                                          GroupName =ig.ItemMasterItemName,
                                          ItemName=im.ItemMasterItemName,
                                          OpenQuantity = y.StockRegisterSIN - y.StockRegisterSout,
                                          OpenQuantityAmount = (y.StockRegisterSIN - y.StockRegisterSout) * y.StockRegisterRate,

                                          BalanceTotal = y.StockRegisterSIN - y.StockRegisterSout,
                                          BalanceAmount = (y.StockRegisterSIN - y.StockRegisterSout) * y.StockRegisterRate,

                                          PurchaseInQty = y.StockRegisterTransType=="P"?y.StockRegisterSIN??0:0,
                                          PurchaseInAmount = y.StockRegisterTransType == "P" ? (y.StockRegisterSIN ?? 0 * y.StockRegisterRate) : 0,
                                          PurchaseOutQty = y.StockRegisterTransType == "P" ? y.StockRegisterSout ?? 0 : 0,
                                          PurchaseOutAmount = y.StockRegisterTransType == "P" ? (y.StockRegisterSout ?? 0 * y.StockRegisterRate) : 0,

                                          SaleInQty = y.StockRegisterTransType == "S" ? y.StockRegisterSIN ?? 0 : 0,
                                          SaleInAmount = y.StockRegisterTransType == "S" ? (y.StockRegisterSIN ?? 0 * y.StockRegisterRate) : 0,
                                          SaleOutQty = y.StockRegisterTransType == "S" ? y.StockRegisterSout ?? 0 : 0,
                                          SaleOutAmount = y.StockRegisterTransType == "S" ? (y.StockRegisterSout ?? 0 * y.StockRegisterRate) : 0,
                                      }).GroupBy(
                                    m => new { m.ItemId,m.ItemMasterAccountNo,m.GroupName, m.ItemName },
                                    (k, g) => new { k.ItemId, k.ItemMasterAccountNo,k.GroupName, k.ItemName,
                                        OpenQuantity = g.Sum(x => x.OpenQuantity),
                                        OpenQuantityAmount = g.Sum(x => x.OpenQuantityAmount),
                                        BalanceTotal = g.Sum(x => x.BalanceTotal),
                                        BalanceAmount = g.Sum(x => x.BalanceAmount),

                                        PurchaseInQty = g.Sum(x => x.PurchaseInQty),
                                        PurchaseInAmount = g.Sum(x => x.PurchaseInAmount),
                                        PurchaseOutQty = g.Sum(x => x.PurchaseOutQty),
                                        PurchaseOutAmount = g.Sum(x => x.PurchaseOutAmount),

                                        SaleInQty = g.Sum(x => x.SaleInQty),
                                        SaleInAmount = g.Sum(x => x.SaleInAmount),
                                        SaleOutQty = g.Sum(x => x.SaleOutQty),
                                        SaleOutAmount = g.Sum(x => x.SaleOutAmount),
                                    }
                                )
                                .Select(am => new StockMovementSINOutResponse
                                {
                                    ItemId = am.ItemId,
                                    ItemName = am.ItemName,
                                    GroupName = am.GroupName,
                                    ItemMasterAccountNo = am.ItemMasterAccountNo,
                                    OpenQuantity =am.OpenQuantity,
                                    OpenQuantityAmount = am.OpenQuantityAmount,
                                    BalanceTotal = am.BalanceTotal,
                                    BalanceAmount = am.BalanceAmount,

                                    PurchaseInQty = am.PurchaseInQty,
                                    PurchaseInAmount = am.PurchaseInAmount,
                                    PurchaseOutQty = am.PurchaseOutQty,
                                    PurchaseOutAmount = am.PurchaseOutAmount,

                                    SaleInQty = am.SaleInQty,
                                    SaleInAmount = am.SaleInAmount,
                                    SaleOutQty = am.SaleOutQty,
                                    SaleOutAmount = am.SaleOutAmount,
                                }).ToList();

                         stockMov.StockMovementInOut = stockInOut;
                // dbo].[GetStockValue]
                //var stockvalues = (from ss in query
                //                   join a in _context.stockregister on ss.itemmasteritemid equals (long)a.stockregistermaterialid.getvalueordefault()
                //                   group a by a.stockregistermaterialid into g
                //                   select new
                //                   {
                //                       itemid = (long)g.key,
                //                       totalqty = g.sum(x => x.stockregistersin),
                //                       stockrate = g.sum(x => x.stockregistersin * x.stockregisterrate),
                //                       stockvalue = (decimal)g.sum(x => x.stockregistersin) * (g.sum(x => x.stockregistersin) > 0 ? g.sum(x => x.stockregistersin * x.stockregisterrate) / g.sum(x => x.stockregistersin) : 0)
                //                   });

                //var results = await (from im in stockbaseunits
                //                     join sr in stockvalues on im.itemid equals sr.itemid into gim
                //                     from y in gim.defaultifempty()
                //                     select new stockreportbaseunitresponse
                //                     {
                //                         itemid = im.itemid,
                //                         itemname = im.itemname,
                //                         stock = im.stock,
                //                         unit = im.unit,
                //                         partno = im.partno,
                //                         averagerate = y.stockrate,
                //                         amount = im.stock * y.stockrate,
                //                         averagepurchasevalue = y.stockvalue,
                //                     }).tolistasync();



                return Response<StockMovementReportResponse>.Success(stockMov, "Data found");
            }
            catch (Exception ex)
            {
                return Response<StockMovementReportResponse>.Fail(new StockMovementReportResponse(), ex.Message);
            }
        }

        private decimal getQuantityValue(long itemId)
        {
            var res = (from d in _context.StockRegister
                        where (new string[] { "P", "OS", "OPENING_STOCK_VOUCHER" }).ToList().Contains(d.StockRegisterTransType)
                         && (int)d.StockRegisterMaterialID == itemId
                       select new
                       {
                           SIN = (decimal)d.StockRegisterSIN,
                           Rate = (decimal)d.StockRegisterRate
                       }).ToList().Sum(x => x.SIN * x.Rate);
            return res;
        }
        public async Task<Response<StockMovementIN>> GetStockMovementINReport(StockFilterModel model)
        {
            try
            {
                var stockIn = new StockMovementIN();
                var query = _context.ItemMaster.AsQueryable();
                if (model.ItemId > 0)
                {
                    query = query.Where(im => im.ItemMasterItemId == model.ItemId);
                }
                else if (model.ItemGroupId > 0)
                {
                    query = query.Where(im => im.ItemMasterAccountNo == model.ItemGroupId && im.ItemMasterItemType == "A")
                                 .OrderBy(im => im.ItemMasterItemId);
                }
                else
                {
                    query = query.Where(im => im.ItemMasterItemType == "A")
                                 .OrderBy(im => im.ItemMasterItemId);
                }
                var GroupItemIds = _context.ItemMaster.Where(o => o.ItemMasterItemId == (long)model.ItemGroupId).Select(o => o.ItemMasterItemId).ToList();

                var MatIds = await _context.StockRegister.Select(o => o.StockRegisterMaterialID).ToListAsync();

                 var queryIn = (from im in query
                               join sr in _context.StockRegister on im.ItemMasterItemId equals sr.StockRegisterMaterialID.GetValueOrDefault() into gim
                               from sr in gim.DefaultIfEmpty()

                               join pv in _context.PurchaseVoucher on Convert.ToInt32(sr.StockRegisterRefVoucherNo) equals pv.PurchaseVoucherPurID into pvGroup
                               from pv in pvGroup.DefaultIfEmpty()

                               join ims in _context.ItemMaster on Convert.ToInt32(sr.StockRegisterMaterialID) equals ims.ItemMasterItemId into imGroup
                               from ims in imGroup.DefaultIfEmpty()

                               join gm in _context.ItemMaster on im.ItemMasterItemId equals gm.ItemMasterItemId into gmGroup
                               from gm in gmGroup.DefaultIfEmpty()

                               join jm in _context.JobMaster on sr.StockRegisterJobID equals jm.JobMasterJobId into jmGroup
                               from jm in jmGroup.DefaultIfEmpty()

                               join lm in _context.LocationMaster on sr.StockRegisterLocationID equals lm.LocationMasterLocationId into lmGroup
                               from lm in lmGroup.DefaultIfEmpty()
                               where im.ItemMasterItemType == "A"
                               && MatIds.Contains((int)im.ItemMasterItemId)
                               select new
                               {
                                   sr.StockRegisterMaterialID,
                                   UnitId = 0,
                                   ShortName = "",
                                   sr.StockRegisterRefVoucherNo,
                                   sr.StockRegisterAssignedDate,
                                   sr.StockRegisterTransType,
                                   DisplayTranstype = sr.StockRegisterTransType == "I" ? "ISSUE" : sr.StockRegisterTransType == "DLV" ? "DELIVERY NOTE" : sr.StockRegisterTransType == "S" ? "SALES" : sr.StockRegisterTransType == "SR" ? "SALES RETURN" : sr.StockRegisterTransType == "P" ? "PURCHASE" : sr.StockRegisterTransType == "PR" ? "PURCHASE RETURN" : sr.StockRegisterTransType == "STOCKTRANS" ? "STOCK TRANSFER" : sr.StockRegisterTransType == "OS" ? "OPENING STOCK" : "",
                                   RelativeNo = 0,
                                   GroupName = gm.ItemMasterAliasName ?? "",
                                   JobId = sr.StockRegisterJobID ?? 0,
                                   JobName = jm.JobMasterJobName ?? "",
                                   LocationId = sr.StockRegisterLocationID ?? 0,
                                   Loca_Name = lm.LocationMasterLocationName ?? "",
                                   Qty = sr.StockRegisterSIN,
                                   sr.StockRegisterRate,
                                   TotAmt = (sr.StockRegisterSIN * sr.StockRegisterRate)
                               }
                            )
                             .Select(am => new StockMovementINResponse
                             {
                                 Mat_ID = am.StockRegisterMaterialID,
                                 UnitId = am.UnitId,
                                 ShortName = am.ShortName,
                                 Ref_Vocher_No = am.StockRegisterRefVoucherNo,
                                 AssingedDate = Convert.ToString(am.StockRegisterAssignedDate),
                                 TransType = am.DisplayTranstype,
                                 RelativeNo = am.RelativeNo,
                                 GroupName = am.GroupName,
                                 JobId = am.JobId,
                                 jobname = am.JobName,
                                 LocationId = am.LocationId,
                                 Loca_Name = am.Loca_Name,
                                 Qty = am.Qty,
                                 Rate = am.StockRegisterRate,
                                 TolAmt = am.TotAmt
                             }).ToList();

                  stockIn.StockInReport = queryIn;
                 return Response<StockMovementIN>.Success(stockIn, "Data found");
               // return null;
            }
            catch (Exception ex)
            {
                return Response<StockMovementIN>.Fail(new StockMovementIN(), ex.Message);
            }
        }

        public async Task<Response<StockMovementOUT>> GetStockMovementOUTReport(StockFilterModel model)
        {
            try
            {
                var stockout = new StockMovementOUT();
                var query = _context.ItemMaster.AsQueryable();
                if (model.ItemId > 0)
                {
                    query = query.Where(im => im.ItemMasterItemId == model.ItemId);
                }
                else if (model.ItemGroupId > 0)
                {
                    query = query.Where(im => im.ItemMasterAccountNo == model.ItemGroupId && im.ItemMasterItemType == "A")
                                 .OrderBy(im => im.ItemMasterItemId);
                }
                else
                {
                    query = query.Where(im => im.ItemMasterItemType == "A")
                                 .OrderBy(im => im.ItemMasterItemId);
                }

                var GroupItemIds = _context.ItemMaster.Where(o => o.ItemMasterItemId == (long)model.ItemGroupId).Select(o => o.ItemMasterItemId).ToList();
                var MatIds = await _context.StockRegister.Select(o => o.StockRegisterMaterialID).ToListAsync();


                var queryOut = (from im in query
                                join sr in _context.StockRegister on im.ItemMasterItemId equals sr.StockRegisterMaterialID.GetValueOrDefault() into gim
                                from sr in gim.DefaultIfEmpty()

                                join pv in _context.PurchaseVoucher on Convert.ToInt32(sr.StockRegisterRefVoucherNo) equals pv.PurchaseVoucherPONo into pvGroup
                                from pv in pvGroup.DefaultIfEmpty()

                                join ims in _context.ItemMaster on Convert.ToInt32(sr.StockRegisterMaterialID) equals ims.ItemMasterItemId into imGroup
                                from ims in imGroup.DefaultIfEmpty()

                                join gm in _context.ItemMaster on im.ItemMasterItemId equals gm.ItemMasterItemId into gmGroup
                                from gm in gmGroup.DefaultIfEmpty()

                                join jm in _context.JobMaster on Convert.ToInt32(sr.StockRegisterJobID) equals jm.JobMasterJobId into jmGroup
                                from jm in jmGroup.DefaultIfEmpty()

                                join lm in _context.LocationMaster on Convert.ToInt32(sr.StockRegisterLocationID) equals lm.LocationMasterLocationId into lmGroup
                                from lm in lmGroup.DefaultIfEmpty()

                                where sr.StockRegisterSIN != null && sr.StockRegisterSIN != 0
                                select new
                                {
                                    sr.StockRegisterMaterialID,
                                    UnitId = 0,
                                    ShortName = "",
                                    sr.StockRegisterRefVoucherNo,
                                    sr.StockRegisterAssignedDate,
                                    sr.StockRegisterTransType,
                                    DisplayTransType = sr.StockRegisterTransType == "I" ? "ISSUE" :
                                                       sr.StockRegisterTransType == "DLV" ? "DELIVERY NOTE" :
                                                       sr.StockRegisterTransType == "S" ? "SALES" :
                                                       sr.StockRegisterTransType == "SR" ? "SALES RETURN" :
                                                       sr.StockRegisterTransType == "P" ? "PURCHASE" :
                                                       sr.StockRegisterTransType == "PR" ? "PURCHASE RETURN" :
                                                       sr.StockRegisterTransType == "STOCKTRANS" ? "STOCK TRANSFER" :
                                                       sr.StockRegisterTransType == "OS" ? "OPENING STOCK" :
                                                       (sr.StockRegisterTransType ?? ""),
                                    RelativeNo = 0,
                                    GroupName = gm.ItemMasterItemName ?? "",
                                    sr.StockRegisterJobID,
                                    jobname = jm.JobMasterJobAdvAccName ?? "",
                                    sr.StockRegisterLocationID,
                                    Loca_Name = lm.LocationMasterLocationName ?? "",
                                    sr.StockRegisterRate,
                                    Qty = sr.StockRegisterSIN,
                                    TotAmt = sr.StockRegisterSIN * sr.StockRegisterRate,
                                    QtyOut = sr.StockRegisterSout,
                                    TotAmtOUT = sr.StockRegisterSout * sr.StockRegisterRate
                                }
                                
                            )
                        .Select(am => new StockMovementOUTResponse
                        {
                             Mat_ID = am.StockRegisterMaterialID,
                             UnitId = am.UnitId,
                             ShortName = am.ShortName,
                             Ref_Vocher_No = am.StockRegisterRefVoucherNo,
                             AssingedDate = Convert.ToString(am.StockRegisterAssignedDate),
                             TransType = am.DisplayTransType,
                             RelativeNo = am.RelativeNo,
                             GroupName = am.GroupName,
                             JobId = am.StockRegisterJobID,
                             jobname = am.jobname,
                             LocationId = am.StockRegisterLocationID,
                             Loca_Name = am.Loca_Name,
                             Qty = am.Qty,
                             Rate = am.StockRegisterRate,
                             QtyOut=am.QtyOut,
                             TotAmtOUT=am.TotAmtOUT
                        }).ToList();

                stockout.StockOutReport = queryOut;
           
                return  Response<StockMovementOUT>.Success(stockout, "Data found");
                
            }
            catch (Exception ex)
            {
                return Response<StockMovementOUT>.Fail(new StockMovementOUT(), ex.Message);
            }
        }
    }
}
