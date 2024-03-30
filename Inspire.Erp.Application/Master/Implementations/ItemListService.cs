using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Infrastructure.Database;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Inspire.Erp.Application.Master
{
    public class ItemListService : IItemListService
    {
        private IRepository<ItemMaster> itemrepository;
        private IRepository<ItemStockType> itemStockRepository;
        private IRepository<UnitDetails> _unitDetailsRepo;
        private InspireErpDBContext _dbcontext;
        public ItemListService(IRepository<ItemMaster> _itemrepository, IRepository<ItemStockType> _itemStockRepository, IRepository<UnitDetails> unitDetailsRepo, InspireErpDBContext dbcontext)
        {
            itemrepository = _itemrepository;
            itemStockRepository = _itemStockRepository;
            _unitDetailsRepo = unitDetailsRepo;
            _dbcontext = dbcontext;
        }
        public IEnumerable<ItemListViewModel> GetAllItemListSearch(ItemFilterModel filteModle)
        {
            try
            {
                string qry = string.Empty;

                var query = (from IM in _dbcontext.ItemMaster
                             join IG in _dbcontext.ItemMaster on IM.ItemMasterAccountNo equals IG.ItemMasterItemId into IGJoin
                             from IG in IGJoin.DefaultIfEmpty()

                             join vendor in _dbcontext.VendorMaster on IM.ItemMasterVenderId equals vendor.VendorMasterVendorId into vendorJoin
                             from vendor in vendorJoin.DefaultIfEmpty()

                             join UD in _dbcontext.UnitDetails on IM.ItemMasterItemId equals UD.UnitDetailsItemId into UDJoin
                             from UD in UDJoin.DefaultIfEmpty()

                             join UM in _dbcontext.UnitMaster on UD.UnitDetailsUnitId equals UM.UnitMasterUnitId into UMJoin
                             from UM in UMJoin.DefaultIfEmpty()

                             join S in _dbcontext.Suppliers on IM.ItemMasterSuplierCode equals S.SuppliersGroupNo into SJoin
                             from S in SJoin.DefaultIfEmpty()
                             where IM.ItemMasterItemType == "A"

                             select new ItemListViewModel
                             {
                                 BarcodePrint = "",
                                 ItemId = (long?)IM.ItemMasterItemId ?? 0,
                                 PartNo = IM.ItemMasterPartNo ?? "",
                                 Item_Name = IM.ItemMasterItemName ?? "",
                                 Stock = IM.ItemMasterCurrentStock ?? 0,
                                 Barcode = IM.ItemMasterBarcode ?? "",
                                 GroupName = IG.ItemMasterItemName ?? "ItemMaster",
                                 VendorName = vendor.VendorMasterVendorName ?? "",
                                 Price = IM.ItemMasterUnitPrice ?? 0,
                                 Style = 0,
                                 UD_UNIT = UM.UnitMasterUnitShortName ?? "",
                                 UD_Barcode = UD.UnitDetailsBarcode ?? "",
                                 Unit_Details_ConversionType = UD.UnitDetailsConversionType ?? 0,
                                 Unit_Details_Rate = UD.UnitDetailsRate ?? 0,
                                 Packing = UD.UnitDetailsPacking ?? "",
                                 UD_Discrption = UD.UnitDetailsDescrption ?? "",
                                 UD_UnitCost = UD.UnitDetailsUnitCost ?? 0,
                                 UD_UnitPrice = UD.UnitDetailsUnitPrice ?? 0,
                                 SupplierName = Convert.ToString(S.SuppliersInsName) ?? null,
                                 ItemMasterAccountNo = IM.ItemMasterAccountNo ?? 0
                             }).ToList();

                if (filteModle.IsContaining == true)
                {
                    if (filteModle.Barcode != "" && filteModle.Barcode != "string")
                    {
                        query = query.Where(s => EF.Functions.Like(s.UD_Barcode, "%" + filteModle.Barcode + "%")).ToList();
                    }
                    if (filteModle.PartNo != "" && filteModle.PartNo != "string")
                    {
                        query = query.Where(s => EF.Functions.Like(s.PartNo, "%" + filteModle.PartNo + "%")).ToList();
                    }
                    if (filteModle.ItemName != "" && filteModle.ItemName != "string")
                    {
                        query = query.Where(s => EF.Functions.Like(s.Item_Name, "%" + filteModle.ItemName + "%")).ToList();
                    }
                    if (filteModle.VendorName != "" && filteModle.VendorName != "string")
                    {
                        query = query.Where(s => EF.Functions.Like(s.VendorName, "%" + filteModle.VendorName + "%")).ToList();
                    }
                    if (filteModle.GruopName != "" && filteModle.GruopName != "string")
                    {
                        query = query.Where(s => EF.Functions.Like(s.GroupName, "%" + filteModle.GruopName + "%")).ToList();
                    }
                    if (filteModle.Priceinsname != "" && filteModle.Priceinsname != "string")
                    {
                        query = query.Where(s => EF.Functions.Like(s.SupplierName, "%" + filteModle.Priceinsname + "%")).ToList();
                    }
                }
                else
                {
                    if (filteModle.Barcode != "" && filteModle.Barcode != "string")
                    {
                        query = query.Where(s => EF.Functions.Like(s.UD_Barcode, "%" + filteModle.Barcode + "%")).ToList();
                    }
                    if (filteModle.PartNo != "" && filteModle.PartNo != "string")
                    {
                        query = query.Where(s => EF.Functions.Like(s.PartNo, "%" + filteModle.PartNo + "%")).ToList();
                    }
                    if (filteModle.ItemName != "" && filteModle.ItemName != "string")
                    {
                        query = query.Where(s => EF.Functions.Like(s.Item_Name, "%" + filteModle.ItemName + "%")).ToList();
                    }
                    if (filteModle.VendorName != "" && filteModle.VendorName != "string")
                    {
                        query = query.Where(s => EF.Functions.Like(s.VendorName, "%" + filteModle.VendorName + "%")).ToList();
                    }
                    if (filteModle.GruopName != "" && filteModle.GruopName != "string")
                    {
                        query = query.Where(s => EF.Functions.Like(s.GroupName, "%" + filteModle.GruopName + "%")).ToList();
                    }
                    if (filteModle.Priceinsname != "" && filteModle.Priceinsname != "string")
                    {
                        query = query.Where(s => EF.Functions.Like(s.SupplierName, "%" + filteModle.Priceinsname + "%")).ToList();
                    }

                }

                return query;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

    }
}
