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



    }
}
