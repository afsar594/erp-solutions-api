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
                                            && (model.ItemGroupId==null || x.ItemMasterRef1==model.ItemGroupId)
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





    }
}
