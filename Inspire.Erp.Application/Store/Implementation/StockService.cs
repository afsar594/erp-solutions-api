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
using System.Linq.Dynamic.Core;
using System.Collections;
using System.Runtime.CompilerServices;
using DinkToPdf;

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

        public async Task<Response<List<StockReportBaseUnitResponse>>> GetStockReportBaseUnitList(StockFilterModel model)
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


                var results = await (from im in _context.ItemMaster.Where(x => x.ItemMasterItemName.ToLower().Contains(model.ItemName.ToLower())
                                     // && x.ItemMasterDefaultLocationName.ToLower().Contains(model.Location.ToLower()) 
                                     && (model.LocationId == null || x.ItemMasterLocationId == model.LocationId)
                                     && (model.ItemId == null || x.ItemMasterItemId == model.ItemId)
                                     && (model.ItemGroupId == null || x.ItemMasterRef1 == model.ItemGroupId.ToString())
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
                                         ItemId = y.StockRegisterMaterialID,
                                         ItemName = im.ItemMasterItemName,
                                         Stock = (int)((y.StockRegisterSIN ?? 0 - y.StockRegisterSout ?? 0) / (decimal)q.ConTyp),
                                         Unit = q.UnitId,
                                         PartNo = n.UnitMasterUnitShortName,
                                         AverageRate = y.StockRegisterRate,
                                         Amount = y.StockRegisterAmount
                                     }).ToListAsync();




                return Response<List<StockReportBaseUnitResponse>>.Success(results, "Data found");
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
                                         Unit = im.Unit,
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

                var GroupItemIds = await _context.ItemMaster.Where(o => o.ItemMasterAccountNo == (long)model.ItemGroupId).Select(o => o.ItemMasterItemId).ToListAsync();
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
                var stockInOutGW = (from im in query
                                    join sr in _context.StockRegister on im.ItemMasterItemId equals sr.StockRegisterMaterialID.GetValueOrDefault() into gim
                                    from y in gim.DefaultIfEmpty()
                                    join ig in _context.ItemMaster on im.ItemMasterAccountNo equals ig.ItemMasterItemId
                                    where im.ItemMasterItemType == "A"
                                     && MatIds.Contains((int)im.ItemMasterItemId)
                                    select new
                                    {
                                        ItemId = (long)im.ItemMasterItemId,
                                        ItemMasterAccountNo = im.ItemMasterAccountNo,
                                        GroupName = ig.ItemMasterItemName,
                                        ItemName = im.ItemMasterItemName,
                                        OpenQuantity = y.StockRegisterSIN - y.StockRegisterSout,
                                        OpenQuantityAmount = (y.StockRegisterSIN - y.StockRegisterSout) * y.StockRegisterRate,

                                        BalanceTotal = y.StockRegisterSIN - y.StockRegisterSout,
                                        BalanceAmount = (y.StockRegisterSIN - y.StockRegisterSout) * y.StockRegisterRate,

                                        PurchaseInQty = y.StockRegisterTransType == "P" ? y.StockRegisterSIN ?? 0 : 0,
                                        PurchaseInAmount = y.StockRegisterTransType == "P" ? (y.StockRegisterSIN ?? 0 * y.StockRegisterRate) : 0,
                                        PurchaseOutQty = y.StockRegisterTransType == "P" ? y.StockRegisterSout ?? 0 : 0,
                                        PurchaseOutAmount = y.StockRegisterTransType == "P" ? (y.StockRegisterSout ?? 0 * y.StockRegisterRate) : 0,

                                        SaleInQty = y.StockRegisterTransType == "S" ? y.StockRegisterSIN ?? 0 : 0,
                                        SaleInAmount = y.StockRegisterTransType == "S" ? (y.StockRegisterSIN ?? 0 * y.StockRegisterRate) : 0,
                                        SaleOutQty = y.StockRegisterTransType == "S" ? y.StockRegisterSout ?? 0 : 0,
                                        SaleOutAmount = y.StockRegisterTransType == "S" ? (y.StockRegisterSout ?? 0 * y.StockRegisterRate) : 0,
                                    }).GroupBy(
                                    m => new { m.ItemId, m.ItemMasterAccountNo, m.GroupName, m.ItemName },
                                    (k, g) => new
                                    {
                                        k.ItemId,
                                        k.ItemMasterAccountNo,
                                        k.GroupName,
                                        k.ItemName,
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
                                .Select(am => new StockMovementSInOutGroupWiseResponse
                                {
                                    ItemId = am.ItemId,
                                    ItemName = am.ItemName,
                                    GroupName = am.GroupName,
                                    ItemMasterAccountNo = am.ItemMasterAccountNo,
                                    OpenQuantity = am.OpenQuantity,
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

                stockMov.StockMovementInOutGroupWise = stockInOutGW;



                //var PurchOrdDetails = (from pod in _context.PurchaseOrderDetails
                //                       join pvd in _context.PurchaseVoucherDetails on pod.PurchaseOrderDetailsPodId equals
                //                       pvd.PurchaseVoucherDetailsPodId
                //                       select new
                //                       {
                //                           pod.PurchaseOrderId,
                //                           pvd.PurchaseVoucherDetailsVoucherNo
                //                       }).AsQueryable();

                var queryInOut = (from sr in _context.StockRegister
                                  join pv in _context.PurchaseVoucher on sr.StockRegisterRefVoucherNo equals pv.PurchaseVoucherVoucherNo into gsr
                                  from srp in gsr.DefaultIfEmpty()

                                  join pv in _context.PurchaseVoucher on sr.StockRegisterRefVoucherNo equals pv.PurchaseVoucherVoucherNo into pvg
                                  from pvgs in pvg.DefaultIfEmpty()

                                  join im in _context.ItemMaster on (long)sr.StockRegisterMaterialID equals im.ItemMasterItemId into gim
                                  from ims in gim.DefaultIfEmpty()

                                  join gm in _context.ItemMaster on ims.ItemMasterItemId equals gm.ItemMasterAccountNo into ggm
                                  from gmi in ggm.DefaultIfEmpty()

                                  join jm in _context.JobMaster on sr.StockRegisterJobID equals jm.JobMasterJobId into gjm
                                  from sjm in gjm.DefaultIfEmpty()

                                  join lm in _context.LocationMaster on sr.StockRegisterLocationID equals lm.LocationMasterLocationId into glm
                                  from lms in glm.DefaultIfEmpty()

                                  where ims.ItemMasterItemType == "A" && sr.StockRegisterMaterialID == model.ItemId
                                 && GroupItemIds.Contains((int)sr.StockRegisterMaterialID)
                                 && sr.StockRegisterFSNO == 1
                                 && (model.fromDate == null || sr.StockRegisterAssignedDate >= model.fromDate)
                                 && (model.toDate == null || sr.StockRegisterAssignedDate <= model.toDate)
                                  // && sr.StockRegisterSout != 0
                                  select new
                                  {
                                      sr.StockRegisterMaterialID,
                                      UnitId = 0,
                                      ShortName = "",
                                      sr.StockRegisterRefVoucherNo,
                                      sr.StockRegisterAssignedDate,
                                      sr.StockRegisterTransType,
                                      DisplayTranstype = sr.StockRegisterTransType == "I" ? "ISSUE" : sr.StockRegisterTransType == "DLV" ? "DELIVERY NOTE" : sr.StockRegisterTransType == "S" ? "SALES" : sr.StockRegisterTransType == "SR" ? "SALES RETURN" : sr.StockRegisterTransType == "P" ? "PURCHASE" : sr.StockRegisterTransType == "PR" ? "PURCHASE RETURN" : sr.StockRegisterTransType == "STOCKTRANS" ? "STOCK TRANSFER" : sr.StockRegisterTransType == "OS" ? "OPENING STOCK" : sr.StockRegisterTransType,
                                      RelativeNo = ims.ItemMasterAccountNo,
                                      GroupName = gmi.ItemMasterItemName ?? "",
                                      JobId = sr.StockRegisterJobID ?? 0,
                                      JobName = sjm.JobMasterJobName ?? "",
                                      LocationId = sr.StockRegisterLocationID ?? 0,
                                      Loca_Name = lms.LocationMasterLocationName ?? "",
                                      QtyIn = sr.StockRegisterSIN ?? 0,
                                      QtyInTotAmt = (sr.StockRegisterSIN * sr.StockRegisterRate),
                                      QtyOut = sr.StockRegisterSout ?? 0,
                                      QtyOutTotAmt = (sr.StockRegisterSout * sr.StockRegisterRate),
                                      Rate = sr.StockRegisterRate,
                                      LPO = pvgs.PurchaseVoucherPONo
                                  }
                            )
                             .Select(am => new StockMovementInOutResponse
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
                                 Rate = am.Rate,
                                 QtyIn = am.QtyIn,
                                 QtyInAmount = am.QtyInTotAmt,
                                 QtyOut = am.QtyOut,
                                 QtyOutAmount = am.QtyOutTotAmt,
                                 LPO = am.LPO
                             }).ToList();

                stockMov.StockMovementInOut = queryInOut;
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

        public async Task<Response<List<VWStockBaseUnitWithValue>>> GetStockReportBaseUnitWithValue(GenericGridViewModel model)
        {
            try
            {
                string query = $" ItemId > 0 {model.Filter}";
                var data = await (from vw in this._context.VWStockBaseUnitWithValue.Where(query)
                                  join i in this._context.ItemMaster on vw.ItemId equals i.ItemMasterItemId
                                  select new VWStockBaseUnitWithValue()
                                  {
                                      ItemId = vw.ItemId,
                                      Stock = (decimal)vw.Stock,
                                      ItemName = (string)vw.ItemName,
                                      RelativeNO = vw.RelativeNO,
                                      UnitId = vw.UnitId,
                                      UnitShortName = (string)vw.UnitShortName,
                                      AVGRATE = vw.AVGRATE,
                                      AMOUNT = vw.AMOUNT,
                                      AVGPURCHASEValue = vw.AVGPURCHASEValue,
                                      ItemNo = i.ItemMasterPartNo
                                  }).ToListAsync();
                return Response<List<VWStockBaseUnitWithValue>>.Success(data, "Data Were Found");
            }
            catch (Exception ex)
            {
                return Response<List<VWStockBaseUnitWithValue>>.Fail(new List<VWStockBaseUnitWithValue>(), "Not Data Were Found");
            }
        }
        //public async Task<Response<StockMovementIN>> GetStockMovementINReport(StockFilterModel model)
        //{
        //    try
        //    {
        //        var stockIn = new StockMovementIN();

        //        var GroupItemIds = await _context.ItemMaster.Where(o => o.ItemMasterAccountNo == (long)model.ItemGroupId).Select(o => o.ItemMasterItemId).ToListAsync();

        //        var PurchOrdDetails = (from pod in _context.PurchaseOrderDetails
        //                               join pvd in _context.PurchaseVoucherDetails on pod.PurchaseOrderDetailsPodId equals
        //                               pvd.PurchaseVoucherDetailsPodId
        //                               select new
        //                               {
        //                                   pod.PurchaseOrderId,
        //                                   pvd.PurchaseVoucherDetailsVoucherNo
        //                               }).AsQueryable();

        //        var queryIn = (from sr in _context.StockRegister
        //                        join pv in _context.PurchaseVoucher on sr.StockRegisterRefVoucherNo equals pv.PurchaseVoucherVoucherNo into gsr
        //                        from srp in gsr.DefaultIfEmpty()

        //                        join pod in PurchOrdDetails on srp.PurchaseVoucherPurID equals (int)pod.PurchaseOrderId into gpod
        //                        from podl in gpod.DefaultIfEmpty()

        //                        join im in _context.ItemMaster on (long)sr.StockRegisterMaterialID equals im.ItemMasterItemId into gim
        //                        from ims in gim.DefaultIfEmpty()

        //                        join gm in _context.ItemMaster on ims.ItemMasterItemId equals gm.ItemMasterAccountNo into ggm
        //                        from gmi in ggm.DefaultIfEmpty()

        //                        join jm in _context.JobMaster on sr.StockRegisterJobID equals jm.JobMasterJobId into gjm
        //                        from sjm in gjm.DefaultIfEmpty()

        //                        join lm in _context.LocationMaster on sr.StockRegisterLocationID equals lm.LocationMasterLocationId into glm
        //                        from lms in glm.DefaultIfEmpty()

        //                        where ims.ItemMasterItemType == "A" && sr.StockRegisterMaterialID == model.ItemId
        //                       && GroupItemIds.Contains((int)sr.StockRegisterMaterialID)
        //                       && sr.StockRegisterFSNO == 1
        //                       && (model.fromDate == null || sr.StockRegisterAssignedDate >= model.fromDate)
        //                       && (model.toDate == null || sr.StockRegisterAssignedDate <= model.toDate)
        //                       && sr.StockRegisterSout != 0
        //                        select new
        //                        {
        //                            sr.StockRegisterMaterialID,
        //                            UnitId = 0,
        //                            ShortName = "",
        //                            sr.StockRegisterRefVoucherNo,
        //                            sr.StockRegisterAssignedDate,
        //                            sr.StockRegisterTransType,
        //                            DisplayTranstype = sr.StockRegisterTransType == "I" ? "ISSUE" : sr.StockRegisterTransType == "DLV" ? "DELIVERY NOTE" : sr.StockRegisterTransType == "S" ? "SALES" : sr.StockRegisterTransType == "SR" ? "SALES RETURN" : sr.StockRegisterTransType == "P" ? "PURCHASE" : sr.StockRegisterTransType == "PR" ? "PURCHASE RETURN" : sr.StockRegisterTransType == "STOCKTRANS" ? "STOCK TRANSFER" : sr.StockRegisterTransType == "OS" ? "OPENING STOCK" : sr.StockRegisterTransType,
        //                            RelativeNo = ims.ItemMasterAccountNo,
        //                            GroupName = gmi.ItemMasterItemName ?? "",
        //                            JobId = sr.StockRegisterJobID ?? 0,
        //                            JobName = sjm.JobMasterJobName ?? "",
        //                            LocationId = sr.StockRegisterLocationID ?? 0,
        //                            Loca_Name = lms.LocationMasterLocationName ?? "",
        //                            Qty = sr.StockRegisterSIN??0,
        //                            Rate = sr.StockRegisterRate,
        //                            TotAmt = (sr.StockRegisterSIN * sr.StockRegisterRate),
        //                            LPO = podl.PurchaseOrderId
        //                        }
        //                    )
        //                     .Select(am => new StockMovementINResponse
        //                     {
        //                         Mat_ID = am.StockRegisterMaterialID,
        //                         UnitId = am.UnitId,
        //                         ShortName = am.ShortName,
        //                         Ref_Vocher_No = am.StockRegisterRefVoucherNo,
        //                         AssingedDate = Convert.ToString(am.StockRegisterAssignedDate),
        //                         TransType = am.DisplayTranstype,
        //                         RelativeNo = am.RelativeNo,
        //                         GroupName = am.GroupName,
        //                         JobId = am.JobId,
        //                         jobname = am.JobName,
        //                         LocationId = am.LocationId,
        //                         Loca_Name = am.Loca_Name,
        //                         Qty = am.Qty,
        //                         Rate = am.Rate,
        //                         TotalAmount = am.TotAmt,
        //                         LPO = am.LPO
        //                     }).ToList();



        //        stockIn.StockInReport = queryIn;
        //         return Response<StockMovementIN>.Success(stockIn, "Data found");
        //       // return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        return Response<StockMovementIN>.Fail(new StockMovementIN(), ex.Message);
        //    }
        //}
        //public async Task<Response<StockMovementOUT>> GetStockMovementOUTReport(StockFilterModel model)
        //{
        //    try
        //    {
        //        var stockout = new StockMovementOUT();
        //        var GroupItemIds = await _context.ItemMaster.Where(o => o.ItemMasterAccountNo == (long)model.ItemGroupId).Select(o => o.ItemMasterItemId).ToListAsync();

        //        var PurchOrdDetails = (from pod in _context.PurchaseOrderDetails
        //                               join pvd in _context.PurchaseVoucherDetails on pod.PurchaseOrderDetailsPodId equals
        //                               pvd.PurchaseVoucherDetailsPodId
        //                               select new
        //                               {
        //                                   pod.PurchaseOrderId,
        //                                   pvd.PurchaseVoucherDetailsVoucherNo
        //                               }).AsQueryable();

        //        var queryOut = (from sr in _context.StockRegister
        //                       join pv in _context.PurchaseVoucher on sr.StockRegisterRefVoucherNo equals pv.PurchaseVoucherVoucherNo into gsr
        //                       from srp in gsr.DefaultIfEmpty()

        //                       join pod in PurchOrdDetails on srp.PurchaseVoucherPurID equals (int)pod.PurchaseOrderId into gpod
        //                       from podl in gpod.DefaultIfEmpty()

        //                        join im in _context.ItemMaster on (long)sr.StockRegisterMaterialID equals im.ItemMasterItemId into gim
        //                        from ims in gim.DefaultIfEmpty()

        //                        join gm in _context.ItemMaster on ims.ItemMasterItemId equals gm.ItemMasterAccountNo into ggm
        //                        from gmi in ggm.DefaultIfEmpty()

        //                        join jm in _context.JobMaster on sr.StockRegisterJobID equals jm.JobMasterJobId into gjm
        //                        from sjm in gjm.DefaultIfEmpty()

        //                        join lm in _context.LocationMaster on sr.StockRegisterLocationID equals lm.LocationMasterLocationId into glm
        //                        from lms in glm.DefaultIfEmpty()

        //                        where ims.ItemMasterItemType == "A" && sr.StockRegisterMaterialID == model.ItemId
        //                       && GroupItemIds.Contains((int)sr.StockRegisterMaterialID)
        //                       && sr.StockRegisterFSNO == 1
        //                       && (model.fromDate == null || sr.StockRegisterAssignedDate >= model.fromDate)
        //                       && (model.toDate == null || sr.StockRegisterAssignedDate <= model.toDate)
        //                       && sr.StockRegisterSout != 0
        //                       select new
        //                       {
        //                           sr.StockRegisterMaterialID,
        //                           UnitId = 0,
        //                           ShortName = "",
        //                           sr.StockRegisterRefVoucherNo,
        //                           sr.StockRegisterAssignedDate,
        //                           sr.StockRegisterTransType,
        //                           DisplayTranstype = sr.StockRegisterTransType == "I" ? "ISSUE" : sr.StockRegisterTransType == "DLV" ? "DELIVERY NOTE" : sr.StockRegisterTransType == "S" ? "SALES" : sr.StockRegisterTransType == "SR" ? "SALES RETURN" : sr.StockRegisterTransType == "P" ? "PURCHASE" : sr.StockRegisterTransType == "PR" ? "PURCHASE RETURN" : sr.StockRegisterTransType == "STOCKTRANS" ? "STOCK TRANSFER" : sr.StockRegisterTransType == "OS" ? "OPENING STOCK" : sr.StockRegisterTransType,
        //                           RelativeNo = ims.ItemMasterAccountNo,
        //                           GroupName = gmi.ItemMasterItemName ?? "",
        //                           JobId = sr.StockRegisterJobID ?? 0,
        //                           JobName = sjm.JobMasterJobName ?? "",
        //                           LocationId = sr.StockRegisterLocationID ?? 0,
        //                           Loca_Name = lms.LocationMasterLocationName ?? "",
        //                           Qty = sr.StockRegisterSout,
        //                           Rate = sr.StockRegisterRate,
        //                           TotAmt = (sr.StockRegisterSout * sr.StockRegisterRate),
        //                           LPO = podl.PurchaseOrderId
        //                       }
        //                    )
        //                     .Select(am => new StockMovementOUTResponse
        //                     {
        //                         Mat_ID = am.StockRegisterMaterialID,
        //                         UnitId = am.UnitId,
        //                         ShortName = am.ShortName,
        //                         Ref_Vocher_No = am.StockRegisterRefVoucherNo,
        //                         AssingedDate = Convert.ToString(am.StockRegisterAssignedDate),
        //                         TransType = am.DisplayTranstype,
        //                         RelativeNo = am.RelativeNo,
        //                         GroupName = am.GroupName,
        //                         JobId = am.JobId,
        //                         jobname = am.JobName,
        //                         LocationId = am.LocationId,
        //                         Loca_Name = am.Loca_Name,
        //                         Qty = am.Qty,
        //                         Rate = am.Rate,
        //                         TotalAmount = am.TotAmt,
        //                         LPO = am.LPO
        //                     }).ToList();

        //        stockout.StockOutReport = queryOut;
        //        return Response<StockMovementOUT>.Success(stockout, "Data found");

        //    }
        //    catch (Exception ex)
        //    {
        //        return Response<StockMovementOUT>.Fail(new StockMovementOUT(), ex.Message);
        //    }
        //}

        public async Task<Response<StockMovementResponse>> GetStockMovement(StockFilterModel model)
        {
            try
            {
                var response = new List<StockReportBaseUnitResponse>();

                var query = _context.ItemMaster.AsQueryable();

                if (model.ItemId > 0)
                {
                    query = query.Where(im => im.ItemMasterItemId == model.ItemId);
                }
                if (model.ItemGroupId > 0)
                {
                    query = query.Where(im => im.ItemMasterAccountNo == model.ItemGroupId && im.ItemMasterItemType == "A")
                                 .OrderBy(im => im.ItemMasterItemId);
                }
                else
                {
                    query = query.Where(im => im.ItemMasterItemType == "A")
                                 .OrderBy(im => im.ItemMasterItemId);
                }


                var subrq = (from s in _context.UnitDetails.Where(x => x.UnitDetailsConversionType == 1 && x.UnitDetailsItemId == model.ItemId)
                             select new
                             {
                                 ItemId = s.UnitDetailsItemId,
                                 UnitId = s.UnitDetailsUnitId
                             });

                var stockValues = (from ss in query
                                   join a in _context.StockRegister on ss.ItemMasterItemId equals (long)a.StockRegisterMaterialID.GetValueOrDefault()
                                   where a.StockRegisterVoucherDate >= model.fromDate && a.StockRegisterVoucherDate <= model.toDate
                                   group a by a.StockRegisterMaterialID into g
                                   select new
                                   {
                                       ItemId = (long)g.Key,
                                       TotalQty = g.Sum(x => x.StockRegisterSIN - x.StockRegisterSout),
                                       TotalAmount = g.Sum(x => (x.StockRegisterSIN - x.StockRegisterSout) * x.StockRegisterRate)
                                   }).ToList();

                //var query1 = query.ToList();



                var result = (from im in query
                              join sr in _context.StockRegister on im.ItemMasterItemId equals (long)sr.StockRegisterMaterialID.GetValueOrDefault() into gimV1
                              from y in gimV1.DefaultIfEmpty()
                              join r in subrq on y.StockRegisterMaterialID.GetValueOrDefault() equals (int)r.ItemId into grj
                              from q in grj.DefaultIfEmpty()
                              join um in _context.UnitMaster on q.UnitId equals um.UnitMasterUnitId into gum
                              from n in gum.DefaultIfEmpty()
                              where im.ItemMasterItemType == "A" && im.ItemMasterItemId == model.ItemId
                                      && (model.LocationId == null || y.StockRegisterLocationID == model.LocationId)
                              select new StockRegisterResponse
                              {
                                  Item_Id = im.ItemMasterItemId,
                                  Item_Name = im.ItemMasterItemName,
                                  Stock_Register_Unit = n.UnitMasterUnitShortName ?? "",
                                  StockIn = y != null && y.StockRegisterSIN > 0 && y.StockRegisterVoucherDate >= model.fromDate && y.StockRegisterVoucherDate <= model.toDate ? y.StockRegisterSIN : 0,
                                  StockInAmount = y != null && y.StockRegisterSIN > 0 && y.StockRegisterVoucherDate >= model.fromDate && y.StockRegisterVoucherDate <= model.toDate ? y.StockRegisterAmount : 0,
                                  StockOut = y != null && y.StockRegisterSout > 0 && y.StockRegisterVoucherDate >= model.fromDate && y.StockRegisterVoucherDate <= model.toDate ? y.StockRegisterSout ?? 0 : 0,
                                  StockOutAmount = y != null && y.StockRegisterSout > 0 && y.StockRegisterVoucherDate >= model.fromDate && y.StockRegisterVoucherDate <= model.toDate ? y.StockRegisterAmount : 0,
                                  OpenQty = y != null && y.StockRegisterSIN > 0 && y.StockRegisterVoucherDate < model.fromDate ? y.StockRegisterSIN ?? 0 - y.StockRegisterSout ?? 0 : 0,
                                  OpenQtyAmount = y != null && y.StockRegisterSIN > 0 && y.StockRegisterVoucherDate < model.fromDate ? y.StockRegisterAmount ?? 0 - y.StockRegisterAmount ?? 0 : 0,
                                  TotalBal = 0,
                                  TotalBalAmount = 0
                              }).ToList();

                var res = result;

                var results = result.Any() ? new List<StockRegisterResponse>
                {
                    new StockRegisterResponse
                    {
                        Item_Id = result[0].Item_Id,
                        Item_Name = result[0].Item_Name,
                        Stock_Register_Unit = result[0].Stock_Register_Unit,
                        StockIn = result.Sum(r => r.StockIn),
                        StockInAmount = result.Sum(r => r.StockInAmount),
                        StockOut = result.Sum(r => r.StockOut),
                        StockOutAmount = result.Sum(r => r.StockOutAmount),
                        OpenQty = result.Sum(r => r.OpenQty),
                        OpenQtyAmount = result.Sum(r => r.OpenQtyAmount),
                        TotalBal =result.Sum(r => r.StockIn)-result.Sum(r => r.StockOut),
                        TotalBalAmount = result.Sum(r => r.StockInAmount)-result.Sum(r => r.StockOutAmount),
                    }
                } : null;

                //var results1 = new List<StockMovementInOut>();
                var results1 = (from im in query
                                join sr in _context.StockRegister on im.ItemMasterItemId equals (long)sr.StockRegisterMaterialID.GetValueOrDefault() into gimV1
                                from y in gimV1.DefaultIfEmpty()
                                join r in subrq on y.StockRegisterMaterialID.GetValueOrDefault() equals (int)r.ItemId into grj
                                from q in grj.DefaultIfEmpty()
                                join um in _context.UnitMaster on q.UnitId equals um.UnitMasterUnitId into gum
                                from n in gum.DefaultIfEmpty()
                                where im.ItemMasterItemType == "A" && im.ItemMasterItemId == model.ItemId
                                      && (model.LocationId == null || y.StockRegisterLocationID == model.LocationId)
                                select new StockMovementInOut
                                {
                                    Ref_Vocher_No = y.StockRegisterRefVoucherNo,
                                    Voucher_Date = y.StockRegisterVoucherDate,
                                    TransType = y.StockRegisterTransType,
                                    Unit = n.UnitMasterUnitShortName ?? "",
                                    Rate = y != null && y.StockRegisterSIN > 0 && y.StockRegisterVoucherDate >= model.fromDate && y.StockRegisterVoucherDate <= model.toDate ? y.StockRegisterFcRate : 0,
                                    Qty = y != null && y.StockRegisterSIN > 0 && y.StockRegisterVoucherDate >= model.fromDate && y.StockRegisterVoucherDate <= model.toDate ? y.StockRegisterSIN : 0,
                                    QtyAmount = y != null && y.StockRegisterSIN > 0 && y.StockRegisterVoucherDate >= model.fromDate && y.StockRegisterVoucherDate <= model.toDate ? y.StockRegisterAmount : 0,
                                }).ToList();
                var results2 = (from im in query
                                join sr in _context.StockRegister on im.ItemMasterItemId equals (long)sr.StockRegisterMaterialID.GetValueOrDefault() into gimV1
                                from y in gimV1.DefaultIfEmpty()
                                join r in subrq on y.StockRegisterMaterialID.GetValueOrDefault() equals (int)r.ItemId into grj
                                from q in grj.DefaultIfEmpty()
                                join um in _context.UnitMaster on q.UnitId equals um.UnitMasterUnitId into gum
                                from n in gum.DefaultIfEmpty()
                                where im.ItemMasterItemType == "A" && im.ItemMasterItemId == model.ItemId
                                      && (model.LocationId == null || y.StockRegisterLocationID == model.LocationId)
                                select new StockMovementInOut
                                {
                                    Ref_Vocher_No = y.StockRegisterRefVoucherNo,
                                    Voucher_Date = y.StockRegisterVoucherDate,
                                    TransType = y.StockRegisterTransType,
                                    Unit = n.UnitMasterUnitShortName ?? "",
                                    Rate = y != null && y.StockRegisterSout > 0 && y.StockRegisterVoucherDate >= model.fromDate && y.StockRegisterVoucherDate <= model.toDate ? y.StockRegisterFcRate : 0,
                                    Qty = y != null && y.StockRegisterSout > 0 && y.StockRegisterVoucherDate >= model.fromDate && y.StockRegisterVoucherDate <= model.toDate ? y.StockRegisterSout : 0,
                                    QtyAmount = y != null && y.StockRegisterSout > 0 && y.StockRegisterVoucherDate >= model.fromDate && y.StockRegisterVoucherDate <= model.toDate ? y.StockRegisterAmount : 0,
                                }).ToList();
                var response1 = new StockMovementResponse
                {
                    StockRegisterResponse = results,
                    StockMovementIn = results1.Where(x => x.Qty > 0).ToList(),
                    StockMovementOut = results2.Where(x => x.Qty > 0).ToList(),
                };
                return Response<StockMovementResponse>.Success(response1, "Data found");
            }
            catch (Exception ex)
            {
                return Response<StockMovementResponse>.Fail(new StockMovementResponse(), ex.Message);
            }

        }
    }
}
