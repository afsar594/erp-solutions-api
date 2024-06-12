using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;
using System.Runtime.CompilerServices;
using Inspire.Erp.Domain.Modals.AccountStatement;
using System.Security.Cryptography;
using Inspire.Erp.Domain.Entities.POS;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Inspire.Erp.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Threading;
using EllipticCurve.Utils;
using Microsoft.VisualBasic;
using System.Globalization;
using Spire.Pdf.Exporting.XPS.Schema;
using Microsoft.VisualBasic.FileIO;
using System.Diagnostics.Eventing.Reader;

namespace Inspire.Erp.Application.Master
{
    public class ItemMasterService : IItemMasterService
    {
        private IRepository<ItemMaster> itemrepository;
        private IRepository<VendorMaster> vendorrepository;
        private IRepository<ItemStockType> itemStockRepository;
        private IRepository<UnitDetails> _unitDetailsRepo;
        private IRepository<UnitMaster> _unitMasterRepo;
        private IRepository<PurchaseVoucherDetails> purchVouchDetailsRepo;
        private IRepository<PurchaseVoucher> purchVouchRepo;
        private IRepository<SalesVoucherDetails> saleVouchDetailsRepo;
        private IRepository<SalesVoucher> saleVouchRepo;
        private IRepository<HappyHour> happyHourRepo;
        private IRepository<HappyHourDeatils> happyHourDetailsRepo;
        private IRepository<BarcodeScale> barCodeRepo;
        private IRepository<POS_WorkPeriod> workPeriodRepo;
        private IRepository<POS_SettlementDetTemp> settDetailsTempRepo;
        private IRepository<POS_SettlementDet> settDetailsRepo;
        private IRepository<TransactionCodes> transCodesRepo;
        private IRepository<POS_SalesVoucher> posSalesVoucherRepo;
        private IRepository<POS_SalesVoucherDetails> posSalesVoucherDetailsRepo;
        private IRepository<POS_Temp_SalesVoucher> posTempSalesVoucherRepo;
        private IRepository<POS_Temp_SalesVoucherDetails> posTempSalesVoucherDetailsRepo;
        private IRepository<POS_SalesTransactionDetails> posSalesTranDetailsRepo;
        private IRepository<POS_Trans_Group_Master> posTranGroupMasterRepo;
        private IRepository<CustomerMaster> customerMasterRepo;
        private IRepository<StockRegister> stockRegisterRepo;
        private IRepository<AccountsTransactions> accTransRepo;
        private IRepository<Inspire.Erp.Domain.Entities.GeneralSettings> gSettingRepo;
        private IRepository<PosStationSettings> stationSettingsRepo;
        public ItemMasterService(IRepository<ItemMaster> _itemrepository, IRepository<ItemStockType> _itemStockRepository, IRepository<UnitDetails> unitDetailsRepo
            , IRepository<VendorMaster> _vendorrepository
            , IRepository<PurchaseVoucherDetails> _purchVouchDetailsRepo
            , IRepository<PurchaseVoucher> _purchVouchRepo
            , IRepository<SalesVoucherDetails> _saleVouchDetailsRepo
            , IRepository<SalesVoucher> _saleVouchRepo
            , IRepository<HappyHour> _happyHourRepo
            , IRepository<HappyHourDeatils> _happyHourDetailsRepo
            , IRepository<BarcodeScale> _barCodeRepo
            , IRepository<POS_WorkPeriod> _workPeriodRepo
            , IRepository<POS_SettlementDetTemp> _settDetailsTempRepo
            , IRepository<POS_SettlementDet> _settDetailsRepo
            ,IRepository<TransactionCodes> _transCodesRepo
             , IRepository<POS_SalesTransactionDetails> _posSalesTranDetailsRepo
            , IRepository<POS_Trans_Group_Master> _posTranGroupMasterRepo
             , IRepository<CustomerMaster> _customerMasterRepo
            , IRepository<StockRegister> _stockRegisterRepo
            , IRepository<AccountsTransactions> _accTransRepo
            , IRepository<Inspire.Erp.Domain.Entities.GeneralSettings> _gSettingRepo
            , IRepository<UnitMaster> unitMasterRepo
            , IRepository<POS_SalesVoucher> _posSalesVoucherRepo
             , IRepository<POS_SalesVoucherDetails> _posSalesVoucherDetailsRepo
             , IRepository<POS_Temp_SalesVoucher> _posTempSalesVoucherRepo
             , IRepository<POS_Temp_SalesVoucherDetails> _posTempSalesVoucherDetailsRepo
            , IRepository<PosStationSettings> _stationSettingsRepo

            )
        {
            itemrepository = _itemrepository;
            vendorrepository = _vendorrepository;
            itemStockRepository = _itemStockRepository;
            _unitDetailsRepo = unitDetailsRepo;
            _unitMasterRepo = unitMasterRepo;
            purchVouchDetailsRepo = _purchVouchDetailsRepo;
            purchVouchRepo = _purchVouchRepo;
            saleVouchDetailsRepo = _saleVouchDetailsRepo;
            saleVouchRepo = _saleVouchRepo;
            happyHourRepo = _happyHourRepo;
            happyHourDetailsRepo = _happyHourDetailsRepo;
            barCodeRepo = _barCodeRepo;
            workPeriodRepo = _workPeriodRepo;
            settDetailsTempRepo = _settDetailsTempRepo;
            settDetailsRepo = _settDetailsRepo;
            transCodesRepo = _transCodesRepo;
            posSalesTranDetailsRepo = _posSalesTranDetailsRepo;
            posTranGroupMasterRepo = _posTranGroupMasterRepo;
            customerMasterRepo = _customerMasterRepo;
            stockRegisterRepo = _stockRegisterRepo;
            accTransRepo = _accTransRepo;
            gSettingRepo = _gSettingRepo;
            posSalesVoucherRepo = _posSalesVoucherRepo;
            posSalesVoucherDetailsRepo = _posSalesVoucherDetailsRepo;
            posTempSalesVoucherRepo = _posTempSalesVoucherRepo;
            posTempSalesVoucherDetailsRepo = _posTempSalesVoucherDetailsRepo;
            stationSettingsRepo = _stationSettingsRepo;
        }
        public IEnumerable<ItemMaster> InsertItem(ItemMaster itemMaster)
        {
            bool valid = false;
            try
            {
                valid = true;
                itemMaster.ItemMasterItemId = Convert.ToInt32(itemrepository.GetAsQueryable()
                                             .Where(x => x.ItemMasterItemId > 0)
                                             .DefaultIfEmpty()
                                             .Max(o => o == null ? 0 : o.ItemMasterItemId)) + 1;

                itemrepository.Insert(itemMaster);
                return this.GetAllItem();
            }
            catch (Exception ex)
            {
                valid = false;
                throw ex;

            }
            finally
            {
                //cityrepository.Dispose();
            }

        }

        public ItemMaster NewItem(ItemMaster itemMaster)
        {
            bool valid = false;
            try
            {
                valid = true;
                itemMaster.ItemMasterItemId = Convert.ToInt32(itemrepository.GetAsQueryable()
                                             .Where(x => x.ItemMasterItemId > 0)
                                             .DefaultIfEmpty()
                                             .Max(o => o == null ? 0 : o.ItemMasterItemId)) + 1;
                itemMaster.ItemMasterMaterialCode = itemMaster.ItemMasterItemId.ToString();

                itemMaster.ItemImages = itemMaster.ItemImages.AsEnumerable().Select(k =>
                {
                    k.ItemImagesItemImageId = null;
                    return k;
                }).ToList();
                itemrepository.Insert(itemMaster);
                return itemMaster;
            }
            catch (Exception ex)
            {
                valid = false;
                throw ex;

            }
            finally
            {
                //cityrepository.Dispose();
            }

        }

        public ItemMaster UpdateNewItem(ItemMaster itemMaster)
        {
            bool valid = false;
            try
            {

                itemMaster.ItemImages = itemMaster.ItemImages.AsEnumerable().Select(k =>
                {
                    k.ItemImagesItemImageId = null;
                    return k;
                }).ToList();

                itemrepository.Update(itemMaster);
                valid = true;
                return itemMaster;

            }
            catch (Exception ex)
            {
                valid = false;
                throw ex;
                // Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException: 'Database operation expected to affect 1 row(s) but actually affected 0 row(s). Data may have been modified or deleted since entities were loaded. See http://go.microsoft.com/fwlink/?LinkId=527962 for information on understanding and handling optimistic concurrency exceptions.'
                return itemMaster;
            }
            finally
            {
                //cityrepository.Dispose();
            }

        }
        public IEnumerable<ItemMaster> UpdateItem(ItemMaster itemMaster)
        {
            bool valid = false;
            try
            {
                itemrepository.Update(itemMaster);
                valid = true;

            }
            catch (Exception ex)
            {
                valid = false;
                throw ex;

            }
            finally
            {
                //cityrepository.Dispose();
            }
            return this.GetAllItem();
        }

        public IEnumerable<ItemStockType> GetAllStockType()
        {
            return itemStockRepository.GetAll();
        }
        public IEnumerable<ItemMaster> DeleteItem(ItemMaster itemMaster)
        {
            bool valid = false;
            try
            {
                itemrepository.Delete(itemMaster);
                valid = true;

            }
            catch (Exception ex)
            {
                valid = false;
                throw ex;

            }
            finally
            {

                //cityrepository.Dispose();
            }
            return this.GetAllItem();
        }

        public List<ItemMaster> GetAllItem()
        {
            List<ItemMaster> itemMaster;
            try
            {
                itemMaster = itemrepository.GetAsQueryable().Where(k => k.ItemMasterAccountNo == 0 && (k.ItemMasterDelStatus == false || k.ItemMasterDelStatus == null)
                && k.ItemMasterItemType == ItemMasterStatus.Group).Select(k => new ItemMaster
                {
                    ItemMasterItemId = k.ItemMasterItemId,
                    ItemMasterItemName = k.ItemMasterItemName
                }).ToList();


            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                //cityrepository.Dispose();
            }
            return itemMaster;
        }

        public List<ItemMaster> GetAllItemNotGroup()
        {
            List<ItemMaster> itemMaster;
            try
            {
                itemMaster = itemrepository.GetAsQueryable().Where(k => k.ItemMasterAccountNo != 0 && (k.ItemMasterDelStatus == false || k.ItemMasterDelStatus == null)
                && k.ItemMasterItemType != ItemMasterStatus.Group).Select(k => new ItemMaster
                {
                    ItemMasterItemId = k.ItemMasterItemId,
                    ItemMasterItemName = k.ItemMasterItemName,
                    ItemMasterPartNo = k.ItemMasterPartNo,
                    ItemMasterPacking = k.ItemMasterPacking,
                    ItemMasterVenderId = k.ItemMasterVenderId,
                    ItemMasterItemSize = k.ItemMasterItemSize,
                    ItemMasterAccountNo = k.ItemMasterAccountNo,
                }).ToList();

                var allUnitDetails = _unitDetailsRepo.GetAll().ToList();
                foreach (var item in itemMaster)
                {
                    item.UnitDetails = allUnitDetails.Where(c => c.UnitDetailsItemId == item.ItemMasterItemId).ToList();

                }

            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                //cityrepository.Dispose();
            }
            return itemMaster;
        }

        public IEnumerable<ItemMaster> GetAllItemById(int id)
        {
            IEnumerable<ItemMaster> itemMaster;
            try
            {
                itemMaster = itemrepository.GetAsQueryable().Where(k => k.ItemMasterAccountNo == id && (k.ItemMasterDelStatus == false || k.ItemMasterDelStatus == null))
                                                            .Include(k => k.ItemImages)
                                                            .Include(k => k.ItemPriceLevelDetails)
                                                            .Include(k => k.UnitDetails).Select(k => k);

            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                //cityrepository.Dispose();
            }
            return itemMaster;

        }


        //GetAllItemMasterById
        public ItemMaster GetAllItemMasterById(int id)
        {
            ItemMaster itemMaster;
            try
            {
                itemMaster = itemrepository.GetAsQueryable().Where(k => k.ItemMasterItemId == id && (k.ItemMasterDelStatus == false || k.ItemMasterDelStatus == null))
                                                            .Include(k => k.ItemImages)
                                                            .Include(k => k.ItemPriceLevelDetails)
                                                            .Include(k => k.UnitDetails).Select(k => k).SingleOrDefault();

            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                //cityrepository.Dispose();
            }
            return itemMaster;

        }


        public async Task<Response<List<ItemResponse>>> GetItemsBySearch(string query)
        {
            try
            {

                //var subqueryUnitD = (from s in _Context.CustomerDeliveryNoteDetails
                //                    group s by new { s.CustomerDeliveryNoteDetailsPodId } into g
                //                    select new
                //                    {
                //                        CustomerDeliveryNoteDetailsPodId = g.Key.CustomerDeliveryNoteDetailsPodId,
                //                        CustomerDeliveryNoteDetailsQty = g.Sum(s => s.CustomerDeliveryNoteDetailsQty)
                //                    });



                //var bb = (from pvd in purchVouchDetailsRepo.GetAsQueryable()
                //             join pv in purchVouchRepo.GetAsQueryable() on (long)pvd.PurchaseVoucherDetailsPrId equals pv.PurchaseVoucherPurID

                //           select new 
                //             {
                //                 Rate=pvd.PurchaseVoucherDetailsRate,
                //                 PurchaseDate = pv.PurchaseVoucherPurchaseDate
                //             }).OrderByDescending(c => c.PurchaseDate).FirstOrDefaultAsync();

                var results = await (from ir in itemrepository.GetAsQueryable()
                                     join vr in vendorrepository.GetAsQueryable() on ir.ItemMasterVenderId equals vr.VendorMasterVendorId into irJoin
                                     from im in irJoin.DefaultIfEmpty()
                                     join vd in (
                                   from pod in _unitDetailsRepo.GetAsQueryable()
                                   group pod by pod.UnitDetailsItemId into grp
                                   select new
                                   {
                                       grp.Key,
                                       ConTyp = grp.Max(x => x.UnitDetailsConversionType),

                                   }) on ir.ItemMasterItemId equals vd.Key into vdJoin
                                     from vd in vdJoin.DefaultIfEmpty()
                                     where ir.ItemMasterItemType == "A"
                                && (ir.ItemMasterItemName.Contains(query) || im.VendorMasterVendorName.Contains(query)
                                || ir.ItemMasterPartNo.Contains(query))
                                     select new ItemResponse
                                     {
                                         ItemId = ir.ItemMasterItemId,
                                         ItemName = ir.ItemMasterItemName,
                                         PartNo = ir.ItemMasterPartNo,
                                         MaterialCode = ir.ItemMasterMaterialCode,
                                         VendorName = im.VendorMasterVendorName,
                                         UnitPrice = ir.ItemMasterUnitPrice,
                                         Stock = ir.ItemMasterCurrentStock ?? 0 / vd.ConTyp,
                                         LandingCost = ir.ItemMasterLandingCost,
                                         //LastPurPrice= (from pvd in purchVouchDetailsRepo.GetAsQueryable()
                                         //                    join pv in purchVouchRepo.GetAsQueryable() on (long)pvd.PurchaseVoucherDetailsPrId equals pv.PurchaseVoucherPurID
                                         //                    where pvd.PurchaseVoucherDetailsMaterialId==ir.ItemMasterItemId
                                         //                    select new
                                         //                    {
                                         //                        Rate = pvd.PurchaseVoucherDetailsRate,
                                         //                        PurchaseDate = pv.PurchaseVoucherPurchaseDate
                                         //                    }).OrderByDescending(c => c.PurchaseDate).FirstOrDefault().Rate,
                                         LastPurPrice = (from pvd in purchVouchDetailsRepo.GetAsQueryable()
                                                         join pv in purchVouchRepo.GetAsQueryable() on pvd.PurchaseVoucherDetailsVoucherNo equals pv.PurchaseVoucherVoucherNo
                                                         where pvd.PurchaseVoucherDetailsMaterialId == ir.ItemMasterItemId
                                                         select new
                                                         {
                                                             Rate = pvd.PurchaseVoucherDetailsRate,
                                                             PurchaseDate = pv.PurchaseVoucherPurchaseDate
                                                         }).OrderByDescending(c => c.PurchaseDate).FirstOrDefault().Rate,


                                         LastSalePrice = (from pvd in saleVouchDetailsRepo.GetAsQueryable()
                                                          join pv in saleVouchRepo.GetAsQueryable() on pvd.SalesVoucherDetailsNo equals pv.SalesVoucherNo
                                                          where pvd.SalesVoucherDetailsMatId == ir.ItemMasterItemId
                                                          select new
                                                          {
                                                              Rate = pvd.SalesVoucherDetailsRate,
                                                              PurchaseDate = pv.SalesVoucherDate
                                                          }).OrderByDescending(c => c.PurchaseDate).FirstOrDefault().Rate
                                     })

                       .OrderBy(t => t.ItemName).ThenBy(x => x.MaterialCode).ToListAsync();

                return Response<List<ItemResponse>>.Success(results, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<ItemResponse>>.Fail(new List<ItemResponse>(), ex.Message);
            }
        }
        public ItemMaster GetAllItemMasterByBarCode(string barCode)
        {
            ItemMaster itemMaster;
            try
            {
                itemMaster = itemrepository.GetAsQueryable().Where(k => k.ItemMasterBarcode == barCode && (k.ItemMasterDelStatus == false || k.ItemMasterDelStatus == null))
                                                            .Include(k => k.ItemImages)
                                                            .Include(k => k.ItemPriceLevelDetails)
                                                            .Include(k => k.UnitDetails).Select(k => k).SingleOrDefault();

            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                //cityrepository.Dispose();
            }
            return itemMaster;

        }


        public IEnumerable<ItemMaster> GetItemMastersByName(string name)
        {
            IEnumerable<ItemMaster> itemMasters = itemrepository.GetAsQueryable().Where(k => k.ItemMasterItemName.Contains(name)
            && (k.ItemMasterDelStatus == false || k.ItemMasterDelStatus == null)
            && k.ItemMasterAccountNo != 0 && k.ItemMasterItemType != ItemMasterStatus.Group).Select(k => k);

            return itemMasters;
        }

        public IEnumerable<ItemMaster> GetAllItemSearchFilter(string group, string name)
        {
            IEnumerable<ItemMaster> itemMasters = itemrepository.GetAsQueryable().Where(k => (((name != null && name.Trim() != "")
            || (group != null && group.Trim() != "")) &&
            ((name != null && name.Trim() != "") ? k.ItemMasterItemName.Equals(name) : true
            || (group != null && group.Trim() != "") ? k.ItemMasterItemName.Equals(group) : true))
            && (k.ItemMasterDelStatus == false || k.ItemMasterDelStatus == null))
                .Include(k => k.ItemPriceLevelDetails)
                .Include(x => x.ItemImages)
                .Include(k => k.UnitDetails).Select(k => k);


            return itemMasters;
        }

        public int ItemMasterIdMax()
        {
            return Convert.ToInt32(itemrepository.GetAsQueryable()
                                                        .Where(x => x.ItemMasterItemId > 0)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.ItemMasterItemId)) + 1;
        }

        public IEnumerable<ItemMaster> GetAllItemGroupAndSubGroup()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ItemMaster> GetAllItemMaster()
        {
            throw new NotImplementedException();
        }

        #region POS

        public async Task<Response<POS_WorkPeriod>> GetWorkPeriod(WorkPeriodFilter model)
        {
            try
            {
                var workPeriod = await workPeriodRepo.GetAsQueryable().Where(x => x.StationId == model.StationId
                && x.Status==model.Status
                && x.UserId==model.UserId).FirstOrDefaultAsync();

                return Response<POS_WorkPeriod>.Success(workPeriod, "Data found");
            }
            catch (Exception ex)
            {
                return Response<POS_WorkPeriod>.Fail(new POS_WorkPeriod(), ex.Message);
            }
        }
        public async Task<Response<ItemMasterResponse>> SearchItemByBarcode(ItemBarCodeSearchFilter model)
        {
            try
            {

                string prefix = "";
                string br = "";
                string barqty = "0";
                string scaleBarcode = "";
                string barQtyType = "0";
                double? realRate = 0;
                if (model.BarCode.Length == 13)
                {
                    if (model.BarCode.Length >= 3)
                    {
                        prefix = model.BarCode.Substring(0, 2);
                    }
                }
                else
                {
                    if (model.BarCode.Length >= 3)
                    {
                        prefix = model.BarCode.Substring(1, 2);
                    }
                }

                string scaleNo = "";
                int? scaleDigi = 0;
                int? scaleBarOne = 0;
                int? scaleBarTwo = 0;
                int? scaleBarThree = 0;
                int? scaleBarFour = 0;
                if (prefix != "")
                {
                    var barcodeScale = await barCodeRepo.GetAsQueryable().Where(x => x.ScaleNo == Convert.ToInt32(prefix)).FirstOrDefaultAsync();
                    if (barcodeScale != null)
                    {
                        scaleNo = barcodeScale.ScaleNo.ToString();
                        scaleDigi = barcodeScale.ScaleDigit;
                        scaleBarOne = barcodeScale.ScaleOne;
                        scaleBarTwo = barcodeScale.ScaleOne;
                        scaleBarThree = barcodeScale.ScaleOne;
                        scaleBarFour = barcodeScale.ScaleOne;
                    }
                }

                if (scaleNo != null && model.BarCode.Length == 13)
                {
                    scaleNo = model.BarCode.Substring(0, 5);
                    barqty = model.BarCode.Substring(model.BarCode.Length + 2, 6);
                    var unitDetails = await _unitDetailsRepo.GetAsQueryable().Where(x => x.UnitDetailsBarcode == scaleNo).FirstOrDefaultAsync();
                    if (unitDetails != null)
                    {
                        scaleBarcode = unitDetails.UnitDetailsItemId.ToString();
                        realRate = unitDetails.UnitDetailsUnitPrice;
                    }

                    barqty = string.Format("{0:000}", Convert.ToDouble(barqty) / (double)1000);
                    var itemDetails = await itemrepository.GetAsQueryable().Where(x => x.ItemMasterItemId == Convert.ToInt64(scaleBarcode)).FirstOrDefaultAsync();
                    br = itemDetails.ItemMasterItemName;
                }

                else if (scaleNo != null && model.BarCode.Length == 14)
                {
                    scaleBarcode = model.BarCode.Substring(scaleNo.Length + (int)scaleBarFour, (int)scaleBarOne);
                    barQtyType = model.BarCode.Substring(scaleNo.Length, (int)scaleBarFour);
                    barqty = model.BarCode.Substring((scaleNo.Length + (int)scaleBarOne) + (int)scaleBarFour, (int)scaleBarThree);

                    var itemDetails = await itemrepository.GetAsQueryable().Where(x => x.ItemMasterItemId == Convert.ToInt64(scaleBarcode)).FirstOrDefaultAsync();
                    if (itemDetails != null)
                    {
                        br = itemDetails.ItemMasterItemName;
                        realRate = (double)itemDetails.ItemMasterUnitPrice;
                    }

                    if (Convert.ToInt32(barQtyType) == 1)
                    {
                        barqty = string.Format("{0:000}", Convert.ToDouble(barqty) / (double)1000);
                    }
                    else
                    {
                        realRate = (double)realRate / Convert.ToDouble(barqty);
                    }

                }
                else
                {
                    scaleBarcode = model.BarCode;
                    barqty = "1";
                }

                var result = new ItemMasterResponse();
                if (br != null)
                {
                    result = await SeachItemByBarcodeLinkScaleNew(model.BarCode.ToUpper(), (double)realRate, Convert.ToDouble(barqty), model.DisableVat);
                }
                else
                {
                    result = await SearchItemByBarcodeLink(model.BarCode.ToUpper(), model.DisableVat);
                }
                return Response<ItemMasterResponse>.Success(result, "Data found");
            }
            catch (Exception ex)
            {
                return Response<ItemMasterResponse>.Fail(new ItemMasterResponse(), ex.Message);
            }
        }
        public async Task<ItemMasterResponse> SearchItemByBarcodeLink(string barCode, bool disableVat)
        {
            string barqtys;
            string barqty = "0";
            if (barCode.Substring(3, 1) == "990" && barCode.Length == 13)
            {
                barqtys = barCode.Substring(7, 5);
                barqty = string.Format("{0:000}", Convert.ToDouble(barqtys) / (double)1000);
                barCode = barCode.Substring(4, 4);
            }

            var result = await (from ir in itemrepository.GetAsQueryable()
                                join ud in _unitDetailsRepo.GetAsQueryable() on ir.ItemMasterItemId equals ud.UnitDetailsItemId
                                where ud.UnitDetailsBarcode == barCode
                                select new ItemMasterResponse
                                {
                                    BarCode = ud.UnitDetailsBarcode,
                                    ItemName = ir.ItemMasterItemName,
                                    UnitPriceTaxIncl = ud.UnitDetailsUnitPrice,
                                    ItemId = ir.ItemMasterItemId,
                                    SalesTax = ir.ItemMasterVatPercentage ?? 0,
                                    ItemwiseInclusive = ir.ItemMasterVatInclues ?? false,
                                    UnitDetailsId = ud.UnitDetailsId,
                                    OfferPrice = ud.UnitDetailsWrate

                                }).FirstOrDefaultAsync();

            var happyH = await (from hh in happyHourRepo.GetAsQueryable()
                                join hhd in happyHourDetailsRepo.GetAsQueryable() on hh.HappyHourId equals hhd.HappyHourDeatilsHappyHourId
                                where hhd.HappyHourDeatilsItemId == result.ItemId
                                select new
                                {
                                    FromDate = hh.HappyHourFromDate,
                                    ToDate = hh.HappyHourToDate
                                }).FirstOrDefaultAsync();

            if (happyH != null)
                if (happyH.FromDate <= DateTime.Now && happyH.ToDate >= DateTime.Now)
                {
                    result.UnitPriceTaxIncl = result.OfferPrice;
                }


            if (disableVat)
            {
                result.SalesTax = 0;
            }

            if (result.ItemwiseInclusive && result.SalesTax != 0)
            {
                result.SalesTax1 = (100 + result.SalesTax);
                result.SalesTax1 = (result.SalesTax1 / (decimal)100);
                result.UnitPrice = (double)Math.Round((decimal)result.UnitPriceTaxIncl / result.SalesTax1, 2);
                // 'Unitprice = Math.Round(Val(Val(UpriceTaxIncl) / 1.05), 2)
                result.VATValue = (decimal)Math.Round((decimal)result.UnitPriceTaxIncl - (decimal)result.UnitPrice, 2);
            }
            else if (result.SalesTax != 0)
            {
                result.VATValue = (decimal)Math.Round(((decimal)result.UnitPriceTaxIncl * result.SalesTax) / 100, 2);
            }
            else if (result.SalesTax == 0)
            {
                result.UnitPrice = (double)Math.Round((decimal)result.UnitPriceTaxIncl, 2);
                result.VATValue = (decimal)Math.Round(0.0, 2);
            }
            return result;
        }
        public async Task<ItemMasterResponse> SeachItemByBarcodeLinkScaleNew(string barCode, double rate, double barQty, bool disableVat)
        {
            var result = await (from ir in itemrepository.GetAsQueryable()
                                join ud in _unitDetailsRepo.GetAsQueryable() on ir.ItemMasterItemId equals ud.UnitDetailsItemId
                                where ir.ItemMasterItemId == Convert.ToInt64(barCode)
                                select new ItemMasterResponse
                                {
                                    UnitDetailsId = ud.UnitDetailsId,
                                    //BarCode = ud.UnitDetailsBarcode,
                                    ItemName = ir.ItemMasterItemName,
                                    UnitPriceTaxIncl = rate,
                                    ItemId = ir.ItemMasterItemId,
                                    SalesTax = ir.ItemMasterVatPercentage ?? 0,
                                    ItemwiseInclusive = ir.ItemMasterVatInclues ?? false,
                                    OfferPrice = ud.UnitDetailsWrate

                                }).FirstOrDefaultAsync();

            var happyH = await (from hh in happyHourRepo.GetAsQueryable()
                                join hhd in happyHourDetailsRepo.GetAsQueryable() on hh.HappyHourId equals hhd.HappyHourDeatilsHappyHourId
                                where hhd.HappyHourDeatilsItemId == result.ItemId
                                select new
                                {
                                    FromDate = hh.HappyHourFromDate,
                                    ToDate = hh.HappyHourToDate
                                }).FirstOrDefaultAsync();

            if (happyH != null)
                if (happyH.FromDate <= DateTime.Now && happyH.ToDate >= DateTime.Now)
                {
                    result.UnitPriceTaxIncl = result.OfferPrice;
                }
                else
                {
                    result.UnitPriceTaxIncl = rate;
                }

            if (disableVat)
            {
                result.SalesTax = 0;
            }


            if (result.ItemwiseInclusive && result.SalesTax != 0)
            {
                result.SalesTax1 = (100 + result.SalesTax);
                result.SalesTax1 = (result.SalesTax1 / (decimal)100);
                result.UnitPrice = (double)Math.Round((decimal)result.UnitPriceTaxIncl / result.SalesTax1, 2);
                // 'Unitprice = Math.Round(Val(Val(UpriceTaxIncl) / 1.05), 2)
                result.VATValue = (decimal)Math.Round((decimal)result.UnitPriceTaxIncl - (decimal)result.UnitPrice, 2);
            }
            else if (result.SalesTax != 0)
            {
                result.VATValue = (decimal)Math.Round(((decimal)result.UnitPriceTaxIncl * result.SalesTax) / 100, 2);
            }
            else if (result.SalesTax == 0)
            {
                result.UnitPrice = (double)Math.Round((decimal)result.UnitPriceTaxIncl, 2);
                result.VATValue = (decimal)Math.Round(0.0, 2);
            }
            return result;

        }
        public async Task<Response<List<CardType>>> GetCardTypes()
        {
            try
            {
                var result = await (from ir in transCodesRepo.GetAsQueryable().Where(x => x.InHouse == true && x.Trans_group == 3)

                                    select new CardType
                                    {
                                        Id = ir.Id,
                                        TransactionCode = ir.Trans_code.Trim(),
                                        TransactionDescription = ir.Trans_Description,
                                        InHouse = ir.InHouse
                                    }).ToListAsync();
                return Response<List<CardType>>.Success(result, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<CardType>>.Fail(new List<CardType>(), ex.Message);
            }
        }
        public async Task<Response<List<ItemMasterResponse>>> GetItems(string? barCode)
        {
            try
            {
                var result = await (from ir in itemrepository.GetAsQueryable().Where(x=>x.ItemMasterItemType=="A")
                                    join ud in _unitDetailsRepo.GetAsQueryable() on ir.ItemMasterItemId equals ud.UnitDetailsItemId
                                    where (barCode==null || ( ir.ItemMasterItemName.Contains(barCode) || ud.UnitDetailsBarcode.Contains(barCode)))
                                    select new ItemMasterResponse
                                    {
                                        UnitDetailsId = ud.UnitDetailsId,
                                        BarCode = ud.UnitDetailsBarcode,
                                        ItemName = ir.ItemMasterItemName,
                                        UnitPrice = ud.UnitDetailsUnitPrice,
                                        ItemId = ir.ItemMasterItemId,
                                    }).GroupBy(
                                    m => new { m.ItemId, m.ItemName, m.UnitDetailsId, m.UnitPrice,m.BarCode }
                                ).Select(am => new ItemMasterResponse
                                {
                                    UnitDetailsId = am.Key.UnitDetailsId,
                                    BarCode = am.Key.BarCode,
                                    ItemName = am.Key.ItemName,
                                    UnitPrice = am.Key.UnitPrice,
                                    ItemId = am.Key.ItemId,
                                }).ToListAsync();
                 return Response<List<ItemMasterResponse>>.Success(result, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<ItemMasterResponse>>.Fail(new List<ItemMasterResponse>(), ex.Message);
            }
        }
        public async Task<PrintData> PrintReciept(string voucherNo)
        {
            try
            {
                string print = "Rana";
               // 

                string createStringitename;
                string[] getItemName = new string[1];
                string createstringqty;
                string ESC = "\u001b";
                DateTime dateTime = new DateTime();
                DateTimeFormatInfo dateFormat = new DateTimeFormatInfo();
                string strDate;
                int pTOption = 0;
                dateTime = System.DateTime.Now;
                dateFormat.MonthDayPattern = "MMMM";
                strDate = dateTime.ToString("MMMM,dd,yyyy,  HH:mm", dateFormat);

                DateTime filesWritten = System.DateTime.Now;
                int rptWidth = 56;
                string Iname = "Itemname";
                string Qtys = "Qty";
                string Price_Hd = "Price";
                string Tax_hd = "Tax";
                string Disc_hd = "Disc";
                string Total_hd = "Total";
                var stationSett = stationSettingsRepo.GetAsQueryable().FirstOrDefault(o => o.Id == 1);
              
                 System.Diagnostics.Process OSK;
                 string eClear = Strings.Chr(27) + "@";
                  string eCentre = Strings.Chr(27) + Strings.Chr(97) + "1";
                  string eLeft = Strings.Chr(27) + Strings.Chr(97) + "0";
                  string eRight = Strings.Chr(27) + Strings.Chr(97) + "2";
                  string eDrawer = eClear + Strings.Chr(27) + "p" + Strings.Chr(0) + ".}";
                  string eCut = Strings.Chr(27) + "i" + Constants.vbCrLf;
                  string eSmlText = Strings.Chr(27) + "!" + Strings.Chr(1);
                  string eNmlText = Strings.Chr(27) + "!" + Strings.Chr(0);
                  string eInit = eNmlText + Strings.Chr(13) + Strings.Chr(27) + "c6" + Strings.Chr(1) + Strings.Chr(27) + "R3" + Constants.vbCrLf;
                  string eBigCharOn = Strings.Chr(27) + "!" + Strings.Chr(56);
                  string eBigCharOff = Strings.Chr(27) + "!" + Strings.Chr(0);
                 string CreateNewLine;
                  string eBigCharOn1 = Strings.Chr(27) + "!" + Strings.Chr(50);
                  string eBigCharOn2 = Strings.Chr(27) + "!" + Strings.Chr(40);
                  string eBigCharOn3 = Strings.Chr(27) + "!" + Strings.Chr(33);
                var StrRen = new StringBuilder();

                if (stationSett.PrintHeader1 != null)
                    {
                        StrRen.AppendLine(eCentre + eBigCharOn2 + stationSett.PrintHeader1);
                    }
                    if (stationSett.PrintHeader2 != default)
                    {
                        StrRen.AppendLine(eCentre + eBigCharOn3 + stationSett.PrintHeader2);
                    }
                    if (stationSett.PrintHeader3 != default)
                    {
                        StrRen.AppendLine(eCentre + eBigCharOff + stationSett.PrintHeader3);
                    }
                    if (stationSett.PrintHeader4 != default)
                    {
                        StrRen.AppendLine(eCentre + eBigCharOff + stationSett.PrintHeader4);
                    }

                StrRen.AppendLine(eBigCharOff + "------------------------------------------------");
                var DisableVat = false;//to be changed
                if (DisableVat == true)
                {
                    StrRen.AppendLine(eCentre + eBigCharOn2 + "Invoice");
                }
                else
                {
                    StrRen.AppendLine(eCentre + eBigCharOn2 + "Tax Invoice");

                }

                var saleV = posSalesVoucherRepo.GetAsQueryable().FirstOrDefault(o => o.VoucherNo.Trim().ToUpper() == voucherNo.ToUpper());

                if(saleV!=null)
                {
                    StrRen.AppendLine(eBigCharOff + "------------------------------------------------");
                    StrRen.AppendLine(eBigCharOn2 + eLeft + "Bill No: " + voucherNo);
                    StrRen.AppendLine(eCentre + eBigCharOff + "------------------------------------------------");
                    StrRen.AppendLine(eBigCharOff + eLeft + "Cashier   : " + "uname");//to be changd
                    StrRen.AppendLine(eLeft + "Date      : " + strDate);
                    StrRen.AppendLine(eLeft + "Mode      : " + saleV.PaymentMode);
                    StrRen.AppendLine("------------------------------------------------");

                    var cust = customerMasterRepo.GetAsQueryable().FirstOrDefault(o => o.CustomerMasterCustomerNo == saleV.Customer_ID);
                    bool showCustomer = false;
                    if (showCustomer == true)
                    {
                        StrRen.AppendLine(eLeft + "Customer  : " + cust.CustomerMasterCustomerName);
                        StrRen.AppendLine("------------------------------------------------");
                    }
                    else
                    {

                    }


                    StrRen.AppendLine(eLeft + eBigCharOff + Iname.PadRight(1) + Qtys.PadLeft(15 - Strings.Len(Iname)) + Price_Hd.PadLeft(15 - Strings.Len(Iname)) + Disc_hd.PadLeft(14 - Strings.Len(Iname)) + Tax_hd.PadLeft(15 - Strings.Len(Iname)) + Total_hd.PadLeft(17 - Strings.Len(Iname)));
                    StrRen.AppendLine("-----------------------------------------------");


                    var saleVD = posSalesVoucherDetailsRepo.GetAsQueryable().Where(o => o.VoucherNo.Trim().ToUpper() == voucherNo.ToUpper()).ToList();

                    foreach (var vouchD in saleVD)
                    {
                        string unit = vouchD.UnitId.ToString();
                        string itemname = vouchD.ItemId.ToString();
                        string iqty = vouchD.Sold_Qty.ToString();
                        string iprice = Convert.ToDecimal(vouchD.UnitPrice).ToString("0.00");
                        string itax = Convert.ToDecimal(vouchD.VatableAmt).ToString("0.00");
                        string iDisc = Convert.ToDecimal(vouchD.Discount).ToString("0.00");
                        string itotal = Convert.ToDecimal(vouchD.NetAmount - vouchD.Discount + vouchD.VatableAmt).ToString("0.00");
                        StrRen.AppendLine(itemname.PadRight(1) + " " + unit);
                        StrRen.AppendLine(iqty.PadLeft(22 - Strings.Len(Iname)) + iprice.PadLeft(16 - Strings.Len(Iname)) + iDisc.PadLeft(14 - Strings.Len(Iname)) + itax.PadLeft(15 - Strings.Len(Iname)) + itotal.PadLeft(17 - Strings.Len(Iname)));
                    }

                    decimal? NetTotal = saleV.NetAmount;
                    string GrandTotal = "";
                    string bill;
                    string POS = "NORMAL";
                    if (POS == "TOUCH")
                    {
                        GrandTotal = Convert.ToDecimal(NetTotal).ToString("0.00");
                        bill = saleV.ShortNo.ToString();
                    }
                    else if (POS == "NORMAL")
                    {
                        GrandTotal = Convert.ToDecimal(NetTotal).ToString("0.00");
                        bill = saleV.ShortNo.ToString();
                    }

                    var strPaymode = new List<string> { "CHNG", "CAS", "CUS", "DLRY", "POINTS", "NET", "RCVE", "RF", "STFA", "VAMT", "VAT", "CRD", "DIS", "MAS", "MASRES", "VIS", "POTCD", "b", "DINE", "DINRES", "CASR", "BIL", "DISNT" };

                    var result = await (from ir in settDetailsTempRepo.GetAsQueryable()
                                        join ud in transCodesRepo.GetAsQueryable() on ir.TransactionCode equals ud.Trans_code
                                        where ir.UserId == saleV.UserId && ud.show_in_inv == true
                                        && ir.OrderId == saleV.ShortNo && strPaymode.Contains(ud.Trans_code.Trim().ToUpper())
                                        select new SettlementDetailsResponse
                                        {
                                            TransactionDesc = ud.Trans_Description,
                                            Amount = ir.Amount,
                                            SettlementDetailsId = ir.SettlementDetId,
                                            SalesVoucherId = ir.OrderId,
                                            Status = ir.Status,
                                            SettlementDate = ir.Date,
                                            SortOrder = ud.Sort_order,
                                            ShowinInvoice = ud.show_in_inv,
                                            TransactionCode = ud.Trans_code,
                                            UserId = ir.UserId
                                        }).OrderBy(x => x.SortOrder).ToListAsync();

                    foreach (var settD in result)
                    {
                        if (settD.TransactionDesc.Trim().ToUpper() == "BILL WISE DISCOUNT" || settD.TransactionDesc.Trim().ToUpper() == "ROUND OFF")
                            pTOption = 1;
                    }
                    StrRen.AppendLine("-----------------------------------------------");
                    foreach (var settD in result)
                    {
                        if (settD.TransactionDesc.Trim().ToUpper() == "CHANGE" && settD.Amount == 0)
                        { }
                        else
                        {
                            if (pTOption == 0 && settD.TransactionDesc.Trim().ToUpper() == "BILL AMOUNT")
                            {

                            }
                            else
                            {
                                StrRen.AppendLine(eBigCharOff + eLeft + settD.TransactionDesc.PadRight(20) + settD.Amount.ToString().PadLeft(24));
                            }
                        }

                    }

                    StrRen.AppendLine("-----------------------------------------------");

                    string label = "Grand Total: ";
                    string Gtotal = Convert.ToDecimal(GrandTotal).ToString("0.00");
                    StrRen.AppendLine(eBigCharOn2 + eLeft + label.PadRight(13) + Gtotal.PadLeft(9));
                    StrRen.AppendLine(eBigCharOff + "-----------------------------------------------");

                    if (stationSett.PrintFooter1 != "")
                    {
                        StrRen.AppendLine(eCentre + eBigCharOff + stationSett.PrintFooter1);
                    }
                    if (stationSett.PrintFooter2 != "")
                    {
                        StrRen.AppendLine(eCentre + eBigCharOff + stationSett.PrintFooter2);
                    }
                    if (stationSett.PrintFooter3 != "")
                    {
                        StrRen.AppendLine(eCentre + eBigCharOff + stationSett.PrintFooter3);
                    }
                    if (stationSett.PrintFooter4 != "")
                    {
                        StrRen.AppendLine(eCentre + eBigCharOff + stationSett.PrintFooter4);
                    }
                    StrRen.AppendLine("");
                    StrRen.AppendLine("");
                    StrRen.AppendLine("");
                    StrRen.AppendLine("");
                    StrRen.AppendLine("");
                    StrRen.AppendLine(eCut);
                }
               

                return new PrintData { 
                    PrinterName=stationSett.PrinterName,
                    Data=StrRen.ToString()
                };

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Response<ItemMasterResponse>> SearchHoldBill(string voucherNo)
        {
            try
            {
                var result = await (from ir in posTempSalesVoucherDetailsRepo.GetAsQueryable()
                                    join ud in _unitDetailsRepo.GetAsQueryable() on ir.UnitDetailsId equals ud.UnitDetailsId into umJoin
                                    from umd in umJoin.DefaultIfEmpty()
                                    where ir.VoucherNo == voucherNo
                                    select new ItemMasterResponse
                                    {
                                     
                                        BarCode = umd.UnitDetailsBarcode,
                                        ItemName = ir.Description,
                                        UnitPrice = umd.UnitDetailsUnitPrice,
                                        ItemId = ir.ItemId,
                                        Quantity =(int)ir.Sold_Qty,
                                        VATValue = ir.VatAmount,
                                        UnitDetailsId = umd.UnitDetailsId,
                                        Discount = ir.Discount
                                    }).FirstOrDefaultAsync();

                return Response<ItemMasterResponse>.Success(result, "Data found");
            }
            catch (Exception ex)
            {
                return Response<ItemMasterResponse>.Fail(new ItemMasterResponse(), ex.Message);
            }
        }


        public async Task<Response<List<SaleVoucherTempResponse>>> LoadHoldBills(int workPeriodId)
        {
            try
            {
                var result = await (from ir in posTempSalesVoucherRepo.GetAsQueryable().Where(x=>x.WorkPeriodID==workPeriodId)
                                    select new SaleVoucherTempResponse
                                    {

                                        VoucherNo = ir.VoucherNo,
                                        CustomerName = ir.Description,
                                        GrossAmount = ir.GrossAmount,
                                        Discount = ir.Discount,
                                        VAT = ir.VatAmount,
                                        NetAmount =ir.NetAmount,
                                        SaleVoucherDetails=  (from vd in posTempSalesVoucherDetailsRepo.GetAsQueryable()
                                                                   join ud in _unitMasterRepo.GetAsQueryable() on vd.UnitId equals ud.UnitMasterUnitId into umJoin
                                                                   from umd in umJoin.DefaultIfEmpty()
                                                                   where vd.V_ID == ir.V_ID
                                                                   select new SaleVoucherDetailsTempResponse
                                                                   {

                                                                       VoucherNo = vd.VoucherNo,
                                                                       Description = vd.Description,
                                                                       UnitName = umd.UnitMasterUnitShortName,
                                                                       Discount = vd.Discount,
                                                                       Quantity = (int)vd.Sold_Qty,
                                                                       UnitPrice = vd.UnitPrice,
                                                                       VAT = vd.VatAmount,
                                                                       NetAmount=vd.NetAmount,
                                                                       UnitDetailId=vd.UnitDetailsId
                                                                      
                                                                   }).ToList()
            }).ToListAsync();

                return Response<List<SaleVoucherTempResponse>>.Success(result, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<SaleVoucherTempResponse>>.Fail(new List<SaleVoucherTempResponse>(), ex.Message);
            }
        }
        public async Task<Response<ItemMasterResponse>> SearchItemByUnitDetailsId(ItemBarCodeSearchFilter model)
        {
            try
            {
                var result = await (from ir in itemrepository.GetAsQueryable()
                                    join ud in _unitDetailsRepo.GetAsQueryable() on ir.ItemMasterItemId equals ud.UnitDetailsItemId
                                    join um in _unitMasterRepo.GetAsQueryable() on ud.UnitDetailsUnitId equals um.UnitMasterUnitId into umJoin
                                    from umd in umJoin.DefaultIfEmpty()
                                    where ud.UnitDetailsId==model.UnitDetailsId
                                    select new ItemMasterResponse
                                    {
                                        UnitDetailsId = ud.UnitDetailsId,
                                        BarCode = ud.UnitDetailsBarcode,
                                        ItemName = ir.ItemMasterItemName,
                                        UnitPrice = ud.UnitDetailsUnitPrice,
                                        ItemId = ir.ItemMasterItemId,
                                        UnitPriceTaxIncl = ud.UnitDetailsUnitPrice,
                                        SalesTax = ir.ItemMasterVatPercentage ?? 0,
                                        ItemwiseInclusive = ir.ItemMasterVatInclues ?? false,
                                        OfferPrice = ud.UnitDetailsWrate,
                                        UnitName=umd.UnitMasterUnitShortName
                                    }).FirstOrDefaultAsync();

                var happyH = await (from hh in happyHourRepo.GetAsQueryable()
                                    join hhd in happyHourDetailsRepo.GetAsQueryable() on hh.HappyHourId equals hhd.HappyHourDeatilsHappyHourId
                                    where hhd.HappyHourDeatilsItemId == result.ItemId
                                    select new
                                    {
                                        FromDate = hh.HappyHourFromDate,
                                        ToDate = hh.HappyHourToDate
                                    }).FirstOrDefaultAsync();

                if (happyH != null)
                    if (happyH.FromDate <= DateTime.Now && happyH.ToDate >= DateTime.Now)
                    {
                        result.UnitPriceTaxIncl = result.OfferPrice;
                    }

                if (model.DisableVat==false)
                {
                    result.SalesTax = 0;
                }

                if (result.ItemwiseInclusive && result.SalesTax != 0)
                {
                    result.SalesTax1 = (100 + result.SalesTax);
                    result.SalesTax1 = (result.SalesTax1 / (decimal)100);
                    result.UnitPrice = (double)Math.Round((decimal)result.UnitPriceTaxIncl / result.SalesTax1, 2);
                    // 'Unitprice = Math.Round(Val(Val(UpriceTaxIncl) / 1.05), 2)
                    result.VATValue = (decimal)Math.Round((decimal)result.UnitPriceTaxIncl - (decimal)result.UnitPrice, 2);
                }
                else if (result.SalesTax != 0)
                {
                    result.VATValue = (decimal)Math.Round(((decimal)result.UnitPriceTaxIncl * result.SalesTax) / 100, 2);
                }
                else if (result.SalesTax == 0)
                {
                    result.UnitPrice = (double)Math.Round((decimal)result.UnitPriceTaxIncl, 2);
                    result.VATValue = (decimal)Math.Round(0.0, 2);
                }
                result.Quantity = 1;
                return Response<ItemMasterResponse>.Success(result, "Data found");
            }
            catch (Exception ex)
            {
                return Response<ItemMasterResponse>.Fail(new ItemMasterResponse(), ex.Message);
            }
        }
        private long getMaxSalesVoucher()
        {
            try
            {
                long? maxValue = posSalesVoucherRepo.GetAll().Max(x => x.V_ID);
                long? incrementedValue = maxValue.HasValue ? maxValue + 1 : 1;
                return (long)incrementedValue;
            }
            catch
            {
                return 1;
            }
        }
        private long getMaxBillNo()
        {
            try
            {
                long? maxValue = posSalesVoucherRepo.GetAll().Max(x => x.ShortNo);
                long? incrementedValue = maxValue.HasValue ? maxValue + 1 : 1;
                return (long)incrementedValue;
            }
            catch
            {
                return 1;
            }
        }

        private long getMaxTempBillNo()
        {
            try
            {
                long? maxValue = posSalesVoucherRepo.GetAll().Max(x => x.ShortNo);
                long? incrementedValue = maxValue.HasValue ? maxValue + 1 : 1;
                return (long)incrementedValue;
            }
            catch
            {
                return 1;
            }
        }

        private int getMaxSettlementDetails()
        {
            try
            {
                int? maxValue = settDetailsTempRepo.GetAll().Max(x => x.SettlementDetId);
                int? incrementedValue = maxValue.HasValue ? maxValue + 1 : 1;
                return (int)incrementedValue;
            }
            catch
            {
                return 1;
            }
        }

        private decimal getMaxSettDetails()
        {
            try
            {
                decimal? maxValue = settDetailsRepo.GetAll().Max(x => x.SettlementDetId);
                decimal? incrementedValue = maxValue.HasValue ? maxValue + 1 : 1;
                return (decimal)incrementedValue;
            }
            catch
            {
                return 1;
            }
        }


        public async Task<Response<List<SettlementDetailsResponse>>> AddSettlementDetails(SettlementDetailsRequest model)
        {
            using (var trans = new TransactionScope(
                   TransactionScopeOption.RequiresNew,
                   TransactionScopeAsyncFlowOption.Enabled))
            //using (var trans = new TransactionScope())
            {
                try
                {
                    var salesVoucherNo = getMaxBillNo();

                    var tempSettDetBill = settDetailsTempRepo.GetAsQueryable().Where(o => o.UserId == model.UserId && o.TransactionCode.Trim().ToUpper() == "BIL").ToList();
                    if (tempSettDetBill != null) settDetailsTempRepo.DeleteList(tempSettDetBill);
                   // settDetailsTempRepo.SaveChangesAsync();
                    //  var tempSettDetBill = settDetailsTempRepo.GetAll().FirstOrDefault(o => o.UserId == model.UserId && o.TransactionCode.Trim().ToUpper() == "BIL");

                    var settDetailsBId = getMaxSettlementDetails();
                    var billModel = new POS_SettlementDetTemp()
                    {
                        SettlementDetId = settDetailsBId,
                        OrderId = salesVoucherNo,
                        Amount = model.Total + model.VAT,
                        Status = "Y",
                        Date = DateTime.Now,//to be changed
                        UserId = model.UserId,
                        TransactionCode = "BIL"
                    };
                    settDetailsTempRepo.Insert(billModel);

                    decimal? vat = 0;
                    //Bill Discount
                    if (model.BillDiscount > 0)
                    {
                        if (model.BillDiscountPerc == 1)
                        {
                            var tempSettDetDisP = settDetailsTempRepo.GetAsQueryable().Where(o => o.UserId == model.UserId && o.TransactionCode.Trim().ToUpper() == "DISNT").ToList();
                            if (tempSettDetDisP != null) settDetailsTempRepo.DeleteList(tempSettDetDisP);
                            var settDetailsDisPId = getMaxSettlementDetails();
                            var dispModel = new POS_SettlementDetTemp()
                            {
                                SettlementDetId = settDetailsDisPId,
                                OrderId = salesVoucherNo,
                                Amount = model.BillDiscount,
                                Status = "Y",
                                Date = DateTime.Now,//to be changed
                                UserId = model.UserId,
                                TransactionCode = "DISNT"

                            };
                            settDetailsTempRepo.Insert(dispModel);

                            decimal? testTaxable = model.Total - model.BillDiscount;
                            decimal? testvat = (testTaxable * model.SalesTax / 100);
                            vat = testvat;

                            var tempSettDetBillV = settDetailsTempRepo.GetAsQueryable().Where(o => o.UserId == model.UserId && o.TransactionCode.Trim().ToUpper() == "BIL").ToList();
                            if (tempSettDetBillV != null) settDetailsTempRepo.DeleteList(tempSettDetBillV);

                            var settDetailsBVId = getMaxSettlementDetails();
                            var billVModel = new POS_SettlementDetTemp()
                            {
                                SettlementDetId = settDetailsBVId,
                                OrderId = salesVoucherNo,
                                Amount = model.Total,
                                Status = "Y",
                                Date = DateTime.Now,//to be changed
                                UserId = model.UserId,
                                TransactionCode = "BIL"

                            };
                            settDetailsTempRepo.Insert(billVModel);
                        }
                        else
                        {

         
                            var tempSettDetailsDis = settDetailsTempRepo.GetAsQueryable().Where(o => o.UserId == model.UserId && o.TransactionCode.Trim().ToUpper() == "DIS").ToList();
                            if (tempSettDetailsDis != null) settDetailsTempRepo.DeleteList(tempSettDetailsDis);

                            var settDetailsIdDis = getMaxSettlementDetails();
                            var alloModelDis = new POS_SettlementDetTemp()
                            {
                                SettlementDetId = settDetailsIdDis,
                                OrderId = salesVoucherNo,
                                Amount = model.BillDiscount,
                                Status = "Y",
                                Date = DateTime.Now,//to be changed
                                UserId = model.UserId,
                                TransactionCode = "DIS"

                            };
                            settDetailsTempRepo.Insert(alloModelDis);
                        }

                        if (model.BillDiscountPerc == 0)
                        {
                            decimal? testTaxable = model.Total - model.BillDiscount;
                            decimal? testvat = (testTaxable * model.SalesTax / 100);
                            vat = testvat;
                        }

                        var tempSettDetVat = settDetailsTempRepo.GetAsQueryable().Where(o => o.UserId == model.UserId && o.TransactionCode.Trim().ToUpper() == "VAT").ToList();
                        if (tempSettDetVat != null) settDetailsTempRepo.DeleteList(tempSettDetVat);

                        var settDetailsVateId = getMaxSettlementDetails();
                        var billVatModel = new POS_SettlementDetTemp()
                        {
                            SettlementDetId = settDetailsVateId,
                            OrderId = salesVoucherNo,
                            Amount = vat,
                            Status = "Y",
                            Date = DateTime.Now,//to be changed
                            UserId = model.UserId,
                            TransactionCode = "VAT"

                        };
                        settDetailsTempRepo.Insert(billVatModel);
                    }
                    else
                    {
                
                        var tempSettDetailsV = settDetailsTempRepo.GetAsQueryable().Where(o => o.UserId == model.UserId && o.TransactionCode.Trim().ToUpper() == "VAT").ToList();
                        if (tempSettDetailsV != null) settDetailsTempRepo.DeleteList(tempSettDetailsV);


                        var settDetailsVId = getMaxSettlementDetails();
                        var alloModelOth = new POS_SettlementDetTemp()
                        {
                            SettlementDetId = settDetailsVId,
                            OrderId = salesVoucherNo,
                            Amount = model.VAT,
                            Status = "Y",
                            Date = DateTime.Now,//to be changed
                            UserId = model.UserId,
                            TransactionCode = "VAT"

                        };
                        settDetailsTempRepo.Insert(alloModelOth);
                    }


                    var tempSettDetailsVat = settDetailsTempRepo.GetAsQueryable().Where(o => o.UserId == model.UserId && o.TransactionCode.Trim().ToUpper() == "VAMT").ToList();
                    if (tempSettDetailsVat != null) settDetailsTempRepo.DeleteList(tempSettDetailsVat);


                    var settDetailsIdVat = getMaxSettlementDetails();
                    var alloModelVat = new POS_SettlementDetTemp()
                    {
                        SettlementDetId = settDetailsIdVat,
                        OrderId = salesVoucherNo,
                        Amount = model.Total - model.BillDiscount,
                        Status = "Y",
                        Date = DateTime.Now,//to be changed
                        UserId = model.UserId,
                        TransactionCode = "VAMT"

                    };
                    settDetailsTempRepo.Insert(alloModelVat);

                    if (model.VATRound > 0)
                    {
                        decimal? VATDAmt = 0;
                        if (model.VATRoundSign == "-")
                        {
                            VATDAmt = 0 - model.VATRoundAmount;
                        }
                        else
                        {
                            VATDAmt = model.VATRoundAmount;
                        }
                  

                        var tempSettDetailsCASR = settDetailsTempRepo.GetAsQueryable().Where(o => o.UserId == model.UserId && o.TransactionCode.Trim().ToUpper() == "CASR").ToList();
                        if (tempSettDetailsCASR != null) settDetailsTempRepo.DeleteList(tempSettDetailsCASR);

                        var settDetailsIdCASR = getMaxSettlementDetails();
                        var alloModelCASR = new POS_SettlementDetTemp()
                        {
                            SettlementDetId = settDetailsIdCASR,
                            OrderId = salesVoucherNo,
                            Amount = VATDAmt,
                            Status = "Y",
                            Date = DateTime.Now,//to be changed
                            UserId = model.UserId,
                            TransactionCode = "CASR"
                        };
                        settDetailsTempRepo.Insert(alloModelCASR);
                    }


                    decimal? netTotal = model.Total - model.BillDiscount + model.VAT;


                    var tempSettDetailsNet = settDetailsTempRepo.GetAsQueryable().Where(o => o.UserId == model.UserId && o.TransactionCode.Trim().ToUpper() == "NET").ToList();
                    if (tempSettDetailsNet != null) settDetailsTempRepo.DeleteList(tempSettDetailsNet);


                    var settDetailsIdNet = getMaxSettlementDetails();
                    var alloModelNet = new POS_SettlementDetTemp()
                    {
                        SettlementDetId = settDetailsIdNet,
                        OrderId = salesVoucherNo,
                        Amount = netTotal,
                        Status = "Y",
                        Date = DateTime.Now,//to be changed
                        UserId = model.UserId,
                        TransactionCode = "NET"
                    };
                    settDetailsTempRepo.Insert(alloModelNet);

                    //Cash
                    if (model.CashAmount > 0)
                    {
    

                        var tempSettDetailsCash = settDetailsTempRepo.GetAsQueryable().Where(o => o.UserId == model.UserId && o.TransactionCode.Trim().ToUpper() == "RCVE").ToList();
                        if (tempSettDetailsCash != null) settDetailsTempRepo.DeleteList(tempSettDetailsCash);

                        var settDetailsIdCash = getMaxSettlementDetails();
                        var alloModelCash = new POS_SettlementDetTemp()
                        {
                            SettlementDetId = settDetailsIdCash,
                            OrderId = salesVoucherNo,
                            Amount = netTotal < 0 ? -1 * model.CashAmount : model.CashAmount,
                            Status = "Y",
                            Date = DateTime.Now,//to be changed
                            UserId = model.UserId,
                            TransactionCode = "RCVE"

                        };
                        settDetailsTempRepo.Insert(alloModelCash);

                     
                        var tempSettDetailsChng = settDetailsTempRepo.GetAsQueryable().Where(o => o.UserId == model.UserId && o.TransactionCode.Trim().ToUpper() == "CHNG").ToList();
                        if (tempSettDetailsChng != null) settDetailsTempRepo.DeleteList(tempSettDetailsChng);


                        settDetailsIdCash = getMaxSettlementDetails();

                        var cashResult = (from ir in settDetailsTempRepo.GetAsQueryable()
                                          join ud in transCodesRepo.GetAsQueryable() on ir.TransactionCode equals ud.Trans_code
                                          where ir.UserId == model.UserId && ir.Card == true
                                          group ir by 1 into og
                                          select new
                                          {
                                              CardAmount = og.Sum(item => item.Amount)
                                          });

                        decimal? cardAmount = cashResult.Count() > 0 ? cashResult.First().CardAmount : 0;
                        decimal? netData = 0;
                        var tempSettDetailsN = settDetailsTempRepo.GetAll().FirstOrDefault(o => o.UserId == model.UserId && o.TransactionCode.Trim().ToUpper() == "NET");
                        if (tempSettDetailsN != null)
                        {
                            netData = tempSettDetailsN.Amount;
                        }

                        decimal? totalChange = netData - model.CashAmount - cardAmount;

                        if (model.VATRound > 0)
                        {
                            decimal? VATDAmt = 0;
                            if (model.VATRoundSign == "-")
                            {
                                VATDAmt = 0 - model.VATRoundAmount;
                            }
                            else
                            {
                                VATDAmt = model.VATRoundAmount;
                            }
                         

                            var tempSettDetailsRF = settDetailsTempRepo.GetAsQueryable().Where(o => o.UserId == model.UserId && o.TransactionCode.Trim().ToUpper() == "RF").ToList();
                            if (tempSettDetailsRF != null) settDetailsTempRepo.DeleteList(tempSettDetailsRF);

                            var settDetailsIdRF = getMaxSettlementDetails();
                            var alloModelRF = new POS_SettlementDetTemp()
                            {
                                SettlementDetId = settDetailsIdRF,
                                OrderId = salesVoucherNo,
                                Amount = VATDAmt,
                                Status = "Y",
                                Date = DateTime.Now,//to be changed
                                UserId = model.UserId,
                                TransactionCode = "RF"

                            };
                            settDetailsTempRepo.Insert(alloModelRF);
                        }

                        alloModelCash = new POS_SettlementDetTemp()
                        {
                            SettlementDetId = settDetailsIdCash,
                            OrderId = salesVoucherNo,
                            Amount = totalChange,
                            Status = "Y",
                            Date = DateTime.Now,//to be changed
                            UserId = model.UserId,
                            TransactionCode = "CHNG"

                        };
                        settDetailsTempRepo.Insert(alloModelCash);
                    }
                    else
                    {

                        var tempSettDetailsCash = settDetailsTempRepo.GetAsQueryable().Where(o => o.UserId == model.UserId && o.TransactionCode.Trim().ToUpper() == "RCVE").ToList();
                        if (tempSettDetailsCash != null) settDetailsTempRepo.DeleteList(tempSettDetailsCash);

                        tempSettDetailsCash = settDetailsTempRepo.GetAsQueryable().Where(o => o.UserId == model.UserId && o.TransactionCode.Trim().ToUpper() == "CHNG").ToList();
                        if (tempSettDetailsCash != null) settDetailsTempRepo.DeleteList(tempSettDetailsCash);


                        tempSettDetailsCash = settDetailsTempRepo.GetAsQueryable().Where(o => o.UserId == model.UserId && o.TransactionCode.Trim().ToUpper() == "RF").ToList();
                        if (tempSettDetailsCash != null) settDetailsTempRepo.DeleteList(tempSettDetailsCash);

                    }

                    var result = await (from ir in settDetailsTempRepo.GetAsQueryable()
                                        join ud in transCodesRepo.GetAsQueryable() on ir.TransactionCode equals ud.Trans_code
                                        where ir.UserId == model.UserId && ud.show_in_inv == true && !ir.TransactionCode.Contains("BIL")
                                        select new SettlementDetailsResponse
                                        {
                                            TransactionDesc = ud.Trans_Description,
                                            Amount = ir.Amount,
                                            SettlementDetailsId = ir.SettlementDetId,
                                            SalesVoucherId = ir.OrderId,
                                            Status = ir.Status,
                                            SettlementDate = ir.Date,
                                            SortOrder = ud.Sort_order,
                                            ShowinInvoice = ud.show_in_inv,
                                            TransactionCode = ud.Trans_code,
                                            UserId = ir.UserId
                                        }).OrderBy(x => x.SortOrder).ToListAsync();

                    trans.Complete();
                    //trans.Dispose();
                    return Response<List<SettlementDetailsResponse>>.Success(result, "Settlement Details inserted Successfully.");

                }
                catch (Exception ex)
                {
                    trans.Dispose();
                    return Response<List<SettlementDetailsResponse>>.Fail(new List<SettlementDetailsResponse>(), ex.Message);
                }
            }
        }


        public async Task<Response<CardAmountResponse>> CalculateCardAmount(CardAmountRequest model)
        {
            //using (var trans = new TransactionScope())
            using (var trans = new TransactionScope(
                  TransactionScopeOption.RequiresNew,
                  TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var cardAmountResponse = new  CardAmountResponse();
                    //string billPOSCode = "LA";

                    var stationSett = stationSettingsRepo.GetAsQueryable().FirstOrDefault(o => o.Id == (long)model.StationId);//model.StationId
                    string billPOSCode = stationSett.POSStationCode;

                    var salesVoucherNo = getMaxBillNo();
                    string voucherNo = billPOSCode + salesVoucherNo;


                    decimal? netTotal = 0;
                    var tempSettDetailsDis = settDetailsTempRepo.GetAll().FirstOrDefault(o => o.UserId == model.UserId && o.TransactionCode.Trim().ToUpper() == "NET");
                    if (tempSettDetailsDis != null) netTotal=tempSettDetailsDis.Amount;

                    decimal? vat = 0;
                    tempSettDetailsDis = settDetailsTempRepo.GetAll().FirstOrDefault(o => o.UserId == model.UserId && o.TransactionCode.Trim().ToUpper() == "VAT");
                    if (tempSettDetailsDis != null) vat = tempSettDetailsDis.Amount;

                    

                    decimal? balance = 0;
                    if (netTotal < 0)
                    {
                        balance = netTotal + model.Cash;
                    }
                    else
                    { 
                        balance=netTotal - model.Cash;
                    } 

                    decimal? cash = model.Cash;

                    var cashResult = await (from ir in settDetailsTempRepo.GetAsQueryable()
                                      join ud in transCodesRepo.GetAsQueryable() on ir.TransactionCode.Trim().ToUpper() equals ud.Trans_code.Trim().ToUpper()
                                            where ir.UserId == model.UserId && ir.Card == true
                                      group ir by 1 into og
                                      select new
                                      {
                                          CardAmount = og.Sum(item => item.Amount)
                                      }).ToListAsync();

                    decimal? lastBalance = 0;
                    if (cashResult.Count() > 0)
                    {
                        if (netTotal < 0)
                        {
                            lastBalance = netTotal + model.Cash;
                        }
                        else
                        {
                            lastBalance = netTotal - model.Cash;
                        }
                    }
                    else
                    {
                        if (netTotal < 0)
                        {
                            lastBalance = netTotal + cashResult.First().CardAmount + model.Cash;
                        }
                        else
                        {
                            lastBalance = netTotal  - cashResult.First().CardAmount - model.Cash;
                        }
                    }

                   
                    cardAmountResponse.CashAmount = model.Cash;
                    cardAmountResponse.NetTotal = netTotal;
                    cardAmountResponse.Balance = balance;
                    cardAmountResponse.LastBalance = lastBalance;
                    cardAmountResponse.IsFromCash = false;
                     trans.Complete();
                    //trans.Dispose();
                    return Response<CardAmountResponse>.Success(cardAmountResponse, "Amount Returned Successfully.");

                }
                catch (Exception ex)
                {
                    trans.Dispose();
                    return Response<CardAmountResponse>.Fail(new CardAmountResponse(), ex.Message);
                }
            }
        }

        public async Task<Response<bool>> ProcessCard(CardRequest model)
        {
            //using (var trans = new TransactionScope())

            var option = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,

                Timeout = TimeSpan.FromSeconds(300),
                
            };
              using (var trans = new TransactionScope(
          TransactionScopeOption.Required,
          new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted, Timeout = TimeSpan.FromSeconds(300) },
          TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    //string billPOSCode = "LA";
                    //var salesVoucherNo = getMaxBillNo();
                    //string voucherNo = billPOSCode + salesVoucherNo;
                    //model.VoucherNo = voucherNo;
                    var stationSett = stationSettingsRepo.GetAsQueryable().FirstOrDefault(o => o.Id == (long)model.StationId);//model.StationId
                    string billPOSCode = stationSett.POSStationCode;

                    if (model.VoucherNo == "" || model.VoucherNo == null)
                    {
                        model.BillnoLoad = getMaxBillNo();
                        model.VoucherNo = billPOSCode + model.BillnoLoad;
                    }

                    var tempSettDetailsCash = settDetailsRepo.GetAll().FirstOrDefault(o => o.OrderId == Convert.ToInt64(model.BillnoLoad));
                    if (tempSettDetailsCash != null) settDetailsRepo.Delete(tempSettDetailsCash);

                    var tempSalesTrRepo = posSalesTranDetailsRepo.GetAll().FirstOrDefault(o => o.InvoiceNo == model.VoucherNo);
                    if (tempSalesTrRepo != null) posSalesTranDetailsRepo.Delete(tempSalesTrRepo);

                    var tempTransCodes = transCodesRepo.GetAsQueryable().FirstOrDefault(o => o.InHouse == true && o.Trans_Description.Trim().ToUpper() == model.CardType.ToUpper());
                    string transCode = "";
                    string transDesc = "";
                    if (tempTransCodes != null)
                    {
                        transCode = tempTransCodes.Trans_code;
                        transDesc = tempTransCodes.Trans_Description;
                    }

                    if(model.Cash>=model.GrandTotal)
                    {
                        if(model.Cash<=0)
                        {
                            model.CashAmount=-1 * model.Cash;
                        }
                        else
                        {
                            model.CashAmount = model.GrandTotal;
                        }
                    }
                    else if(model.Cash < model.GrandTotal)
                    {
                        model.CashAmount = model.Cash;
                    }
                    var settDetailsId = getMaxSettlementDetails();
                    var alloModelCash = new POS_SettlementDetTemp()
                    {
                        SettlementDetId = settDetailsId,
                        OrderId = model.BillnoLoad,
                        Amount = model.CashAmount,
                        Status = "Y",
                        Date = DateTime.Now,//to be changed
                        UserId = model.UserId,
                        TransactionCode = "CAS"
                    };
                    settDetailsTempRepo.Insert(alloModelCash);
                    //settDetailsTempRepo.SaveChangesAsync();
                    //TRANSACTION
                    double? vat = 0;
                    var tempSettDetailsList = settDetailsTempRepo.GetAll().Where(o => o.UserId == model.UserId).ToList();
                    foreach (var item in tempSettDetailsList)
                    {
                        if (item.TransactionCode.Trim().ToUpper() == "BIL")
                        {
                            SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)item.Amount
                                , model.StationName, "BIL", (int)model.UserId, "BILL AMOUNT");
                        }

                        if (item.TransactionCode.Trim().ToUpper() == "DIS")
                        {
                            SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)item.Amount
                                , model.StationName, "DIS", (int)model.UserId, "DISCOUNT");
                        }

                        if (item.TransactionCode.Trim().ToUpper() == "VAT")
                        {
                            if (item.Amount < 0)
                            {
                                SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)item.Amount
                                                           , model.StationName, "VAT", (int)model.UserId, "VAT RETURN");
                            }
                            else
                            {
                                SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)item.Amount
                                                          , model.StationName, "VAT", (int)model.UserId, "VAT");
                            }
                        }

                        if (item.TransactionCode.Trim().ToUpper() == "VAMT")
                        {
                            if (item.Amount < 0)
                            {
                                SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)item.Amount
                                                           , model.StationName, "VAMTRET", (int)model.UserId, "VATABLE AMOUNT RETURNED");
                            }
                            else
                            {
                                SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)item.Amount
                                                          , model.StationName, "VAMT", (int)model.UserId, "VATABLE AMOUNT");
                            }
                        }

                        if (item.TransactionCode.Trim().ToUpper() == "NET")
                        {
                            SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)item.Amount
                                , model.StationName, "NET", (int)model.UserId, "NET AMOUNT");
                        }

                        if (item.TransactionCode.Trim().ToUpper() == "RCVE")
                        {
                            SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)item.Amount
                                , model.StationName, "RCVE", (int)model.UserId, "RECEIVED AMOUNT");
                        }

                        if (item.TransactionCode.Trim().ToUpper() == "CAS")
                        {
                            if (item.Amount < 0)
                            {
                                SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)item.Amount
                                                           , model.StationName, "POTCS", (int)model.UserId, "CASH PAID OUT");
                            }
                            else
                            {
                                SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)item.Amount
                                                          , model.StationName, "CAS", (int)model.UserId, "CASH");
                            }
                        }
                    }



                    var settDetailsTr = await (from ir in settDetailsTempRepo.GetAsQueryable()
                                             join ud in transCodesRepo.GetAsQueryable() on ir.TransactionCode.Trim().ToUpper() equals ud.Trans_code.Trim().ToUpper()
                                             where ir.UserId == model.UserId 
                                             select new
                                             {
                                                 Amount = ir.Amount,
                                                 TransactionCode = ir.TransactionCode
                                             }).ToListAsync();

                    string paymentmode = "CARD";
                    string transtype = "CARD";
                    transDesc = "CARD SALES";

                    foreach (var item in settDetailsTr)
                    {
                        if (item.TransactionCode.Trim().ToUpper() == "CAS")
                        { 
                            if (item.Amount > 0)
                            {
                                paymentmode = "CASH AND CARD";
                                transtype = "SPLIT";
                                transDesc = "CASH AND CARD SALES";
                            } 
                          }
                        }
                    

                    var settDetailsIdCard = getMaxSettlementDetails();
                    var alloModel = new POS_SettlementDetTemp()
                    {
                        SettlementDetId = settDetailsIdCard,
                        OrderId = model.BillnoLoad,
                        Amount = model.CardAmount,
                        Status = "Y",
                        Date = DateTime.Now,//to be changed
                        UserId = model.UserId,
                        TransactionCode = transCode,
                        Card = true,
                        CreditCardNumber = model.CreditCardNumber
                    };
                    settDetailsTempRepo.Insert(alloModel);

                    InsertSettlementDetails(model, (long)model.BillnoLoad);

                    var settDetails = await (from ir in settDetailsTempRepo.GetAsQueryable()
                                             join ud in transCodesRepo.GetAsQueryable() on ir.TransactionCode.Trim().ToUpper() equals ud.Trans_code.Trim().ToUpper()
                                             where ir.UserId == model.UserId && ir.Card == true
                                             select new
                                             {
                                                 Amount = ir.Amount,
                                                 TransactionCode = ir.TransactionCode
                                             }).ToListAsync();

                    // settDetailsTempRepo.SaveChangesAsync();
                    SaveVoucher(model, (long)model.BillnoLoad, transtype, transDesc, paymentmode, "CARD CUSTOMER");
                    SavesettlementTrans(model, (long)model.BillnoLoad);


                    var tempSettDet = settDetailsTempRepo.GetAsQueryable().Where(o => o.UserId == model.UserId).ToList();
                    if (tempSettDet != null) settDetailsTempRepo.DeleteList(tempSettDet);

                    trans.Complete();
                    //trans.Dispose();
                    return Response<bool>.Success(true, "Card processed Successfully.");
                }
                catch (Exception ex)
                {
                    trans.Dispose();
                    return Response<bool>.Fail(false, ex.Message);
                }
            }
        }


        public async Task<Response<bool>> Hold(CardRequest model)
        {
           
            var option = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromSeconds(300),
            };

            using (var trans = new TransactionScope(
        TransactionScopeOption.Required,
        new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted, Timeout = TimeSpan.FromSeconds(300) },
        TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    
                    var stationSett = stationSettingsRepo.GetAsQueryable().FirstOrDefault(o => o.Id == (long)model.StationId);//model.StationId
                    string billPOSCode = stationSett.POSStationCode;

                    if (model.VoucherNo == "" || model.VoucherNo == null)
                    {
                        model.BillnoLoad = getMaxTempBillNo();
                        model.VoucherNo = billPOSCode + model.BillnoLoad;
                    }

                    
                    SaveTempVoucher(model, (long)model.BillnoLoad, "CASH SALES", "CASH", "CASH", "HOLD CUSTOMER");
                   

                    trans.Complete();
                    //trans.Dispose();
                    return Response<bool>.Success(true, "Card processed Successfully.");
                }
                catch (Exception ex)
                {
                    trans.Dispose();
                    return Response<bool>.Fail(false, ex.Message);
                }
            }
        }

        public void SaveTempVoucher(CardRequest model, long voucherId, string transType,
    string transDesc, string paymentMode, string customerName)

        {
            try
            {
                string customerAccountNo = null;
                var custDetails = customerMasterRepo.GetAll().FirstOrDefault(o => o.CustomerMasterCustomerNo == 1);
                if (custDetails != null) customerAccountNo = custDetails.CustomerMasterCustomerReffAcNo;

                var tempSalesV = posTempSalesVoucherRepo.GetAll().FirstOrDefault(o => o.VoucherNo == model.VoucherNo);
                if (tempSalesV != null) posTempSalesVoucherRepo.Delete(tempSalesV);

                var tempSalesVD = posTempSalesVoucherDetailsRepo.GetAll().FirstOrDefault(o => o.VoucherNo == model.VoucherNo);
                if (tempSalesVD != null) posTempSalesVoucherDetailsRepo.Delete(tempSalesVD);



                var salesVC = new POS_Temp_SalesVoucher()
                {
                    // V_ID=voucherId,
                    VoucherNo = model.VoucherNo,
                    ShortNo = voucherId,
                    VoucherDate = DateTime.Now,//Format(AppLogDates, "dd/MMM/yyyy hh:mm:ss")
                    Voucher_Type = transType,
                    CustomerName = "HOLD CUSTOMER",
                    Customer_ID = 1,
                    Location = model.LocationId,
                    Salesman = model.SalesManId,
                    Discount = model.ItemDiscount,
                    GrossAmount = model.NetTotal,
                    Vatable_TotAmt = (double)model.NetTotal,
                    VatAmount = model.VAT,
                    NetAmount = model.NetTotal + (model.VAT) - model.BillDiscount,
                    Remarks = model.StationName,
                    UserId = model.UserId,
                    CurrencyId = Convert.ToInt64(1),
                    FSNO = 1,
                    Description = "POS SALES",
                    CompanyId = 1,
                    Refrence = "POS",
                    JobId = 0,
                    Currency_ID = Convert.ToInt64(1),
                    Cust_PODate = DateTime.Now,//Format(AppLogDates, "dd/MMM/yyyy hh:mm:ss"),
                    NetDiscount = model.BillDiscount,
                    CashCustName = custDetails.CustomerMasterCustomerName,
                    Shipping_Address = "",
                    InvoiceType = "POS",
                    InvoiceStatus = "",
                    WorkPeriodID = model.WorkPeriodId,
                    StationID = model.StationId,
                    PaymentMode = paymentMode,
                    refno = model.RefereceNo,
                    DiscountPer = model.BillDiscountPerc
                };

                posTempSalesVoucherRepo.Insert(salesVC);
                // posSalesVoucherRepo.SaveChangesAsync();

                foreach (var item in model.ItemDetails)
                {
                    var salesVDetails = new POS_Temp_SalesVoucherDetails()
                    {
                        V_ID = salesVC.V_ID,
                        VoucherNo = model.VoucherNo,
                        ItemId = item.ItemId,
                        UnitId = item.UnitId,
                        Sold_Qty = item.Quantity,
                        UnitPrice = item.Price,
                        VatableAmt = item.Total,
                        CostPrice = item.Price,//needs to change
                        GrossAmt = item.Total,
                        Discount = item.Discount,
                        NetAmount = item.NetTotal,
                        FSNO = 1,
                        SNO = 1,
                        LocationID = model.LocationId,
                        CompanyId = 1,
                        WorkPeriodID = model.WorkPeriodId,
                        StationID = model.StationId,
                        UnitDetailsId=item.UnitDetailId
                    };

                    posTempSalesVoucherDetailsRepo.Insert(salesVDetails);

                }
                // posSalesVoucherDetailsRepo.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }

        }


        public void SavesettlementTrans(CardRequest model, long salesVoucherNo)
        {
            long orderId = 0;
            var saleV= posSalesVoucherRepo.GetAll().FirstOrDefault(o => o.VoucherNo == model.VoucherNo);
            if (saleV != null) orderId = saleV.ShortNo;

            var list = new List<string> { "VAT", "VAMT", "NET", "BIL" };
            var saleTrnsDetails = posSalesTranDetailsRepo.GetAll().Where(x => x.InvoiceNo == model.VoucherNo
            && list.Contains(x.PaymentMode)).ToList();

            foreach(var saleTD in saleTrnsDetails)
            {
                var settDetails= settDetailsRepo.GetAll().FirstOrDefault(o => o.OrderId == orderId && o.TransactionCode==saleTD.PaymentMode);
                if (settDetails != null)
                {
                    settDetails.Amount = saleTD.Amount;
                   // settDetailsRepo.SaveChangesAsync();
                }
                else
                {
                    var settDetailsId = getMaxSettDetails();
                    var settDetail = new POS_SettlementDet()
                    {
                        SettlementDetId = settDetailsId,
                        OrderId = orderId,
                        Amount = saleTD.Amount,
                        Status = "Y",
                        Date = DateTime.Now,//to be changed
                        UserId = model.UserId,
                        TransactionCode = saleTD.PaymentMode,

                    };
                    settDetailsRepo.Insert(settDetail);
                }
            }


            var settDetilsTemp = settDetailsTempRepo.GetAll().Where(x => x.OrderId ==orderId
         && x.TransactionCode.Trim().ToUpper() == "DISNT").ToList();

            if (settDetilsTemp.Count()>0)
            {
                var setD = settDetailsRepo.GetAll().FirstOrDefault(o => o.OrderId == orderId);
                var setDTemp = settDetailsTempRepo.GetAll().Where(x => x.OrderId == orderId
        && x.TransactionCode.Trim().ToUpper() == "DIS").ToList();

                if(setDTemp.Count()>0)
                {
                    var settDetailsId = getMaxSettDetails();
                    var settDetail = new POS_SettlementDet()
                    {
                        SettlementDetId = settDetailsId,
                        OrderId = orderId,
                        Amount = setD.Amount,
                        Status = "Y",
                        Date = DateTime.Now,//to be changed
                        UserId = model.UserId,
                        TransactionCode = "DISNT",

                    };
                    settDetailsRepo.Insert(settDetail);
                    SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)setD.Amount
                          , model.StationName, "DISNT", (int)model.UserId, "DISCOUNT");
                }




            }

        }
        public void InsertSettlementDetails(CardRequest model,long salesVoucherNo)
        {
            string billPOSCode = "LA";
            //var salesVoucherNo = getMaxSalesVoucher();
            string voucherNo = billPOSCode + salesVoucherNo;
            //Settlementdet
            var settDetailsId = getMaxSettDetails();
            var settDetail = new POS_SettlementDet()
            {
                SettlementDetId = settDetailsId,
                OrderId = salesVoucherNo,
                Amount = model.NetTotal-model.VAT,
                Status = "Y",
                Date = DateTime.Now,//to be changed
                UserId = model.UserId,
                TransactionCode = "BIL",

            };
            settDetailsRepo.Insert(settDetail);

            if (model.ItemDiscount > 0)
            {
                var settDetailsIdDis = getMaxSettDetails();
                var alloModelDis = new POS_SettlementDet()
                {
                    SettlementDetId = settDetailsIdDis,
                    OrderId = salesVoucherNo,
                    Amount = model.ItemDiscount,
                    Status = "Y",
                    Date = DateTime.Now,//to be changed
                    UserId = model.UserId,
                    TransactionCode = "DIS"

                };
                settDetailsRepo.Insert(alloModelDis);
            }

            decimal? disvat = 0;
            decimal? vat  = 0;
            //Bill Discount
            if (model.BillDiscount > 0)
            {
                disvat = (model.BillAmount - model.BillDiscount) * (model.SalesTax / 100);

                settDetailsId = getMaxSettDetails();
                var alloModelDis = new POS_SettlementDet()
                {
                    SettlementDetId = settDetailsId,
                    OrderId = salesVoucherNo,
                    Amount = model.BillDiscount,
                    Status = "Y",
                    Date = DateTime.Now,//to be changed
                    UserId = model.UserId,
                    TransactionCode = "DIS"

                };
                settDetailsRepo.Insert(alloModelDis);

                settDetailsId = getMaxSettDetails();
                var alloModelVat = new POS_SettlementDet()
                {
                    SettlementDetId = settDetailsId,
                    OrderId = salesVoucherNo,
                    Amount = disvat,
                    Status = "Y",
                    Date = DateTime.Now,//to be changed
                    UserId = model.UserId,
                    TransactionCode = "VAT"
                };
                settDetailsRepo.Insert(alloModelVat);
            }
            else
            {
                var settDetailsIdOth = getMaxSettDetails();
                var alloModelOth = new POS_SettlementDet()
                {
                    SettlementDetId = settDetailsIdOth,
                    OrderId = salesVoucherNo,
                    Amount = model.VAT,
                    Status = "Y",
                    Date = DateTime.Now,//to be changed
                    UserId = model.UserId,
                    TransactionCode = "VAT"

                };
                settDetailsRepo.Insert(alloModelOth);
            }

            decimal? netTotal = 0;
            if (model.VAT > 0)
            {
                netTotal = (model.BillAmount - model.ItemDiscount) - model.BillDiscount + model.DiscountVAT;
            }
            else
            {
                netTotal = (model.BillAmount - model.ItemDiscount) - model.BillDiscount + model.DiscountVAT;
            }

            var settDetailsIdNet = getMaxSettDetails();
            var alloModelNet = new POS_SettlementDet()
            {
                SettlementDetId = settDetailsIdNet,
                OrderId = salesVoucherNo,
                Amount = netTotal,
                Status = "Y",
                Date = DateTime.Now,//to be changed
                UserId = model.UserId,
                TransactionCode = "NET"
            };
            settDetailsRepo.Insert(alloModelNet);

            //Cash
            if (model.Cash > 0)
            {
                var settDetailsIdCash = getMaxSettDetails();
                var alloModelDCash = new POS_SettlementDet()
                {
                    SettlementDetId = settDetailsIdCash,
                    OrderId = salesVoucherNo,
                    Amount = model.CashAmount,
                    Status = "Y",
                    Date = DateTime.Now,//to be changed
                    UserId = model.UserId,
                    TransactionCode = "RCVE"

                };
                settDetailsRepo.Insert(alloModelDCash);
               // settDetailsRepo.SaveChangesAsync();
                int cardId = 0;
                var groupMaster = posTranGroupMasterRepo.GetAll().FirstOrDefault(o => o.Trans_GroupName == "CREDIT CARD");
                if (groupMaster != null) cardId = groupMaster.Id;

                settDetailsIdCash = getMaxSettDetails();

                var tempRepResult = (from ir in settDetailsTempRepo.GetAsQueryable()
                                     join ud in transCodesRepo.GetAsQueryable() on ir.TransactionCode equals ud.Trans_code
                                     join tg in posTranGroupMasterRepo.GetAsQueryable() on ud.Trans_group equals tg.Id
                                     where tg.Id == cardId
                                     select new
                                     {
                                         Id = ir.SettlementDetId
                                     });

                if (tempRepResult.Count() <= 0)
                {
                    var settDetId = getMaxSettDetails();
                    var alloModelD = new POS_SettlementDet()
                    {
                        SettlementDetId = settDetId,
                        OrderId = salesVoucherNo,
                        Amount = netTotal - model.Cash,
                        Status = "Y",
                        Date = DateTime.Now,//to be changed
                        UserId = model.UserId,
                        TransactionCode = "CHNG"

                    };
                    settDetailsRepo.Insert(alloModelD);
                    //settDetailsRepo.SaveChangesAsync();
                }
                else
                {
                    var tempSettDetails = settDetailsTempRepo.GetAsQueryable().Where(o => o.UserId == model.UserId && o.TransactionCode.Trim().ToUpper() == "CHNG").ToList();
                   
                    if (tempSettDetails != null)
                    {
                        settDetailsTempRepo.DeleteList(tempSettDetails);
                        //settDetailsTempRepo.SaveChangesAsync();
                    }
                }

               
            }

            //settDetailsRepo.SaveChangesAsync();
            //settDetailsTempRepo.SaveChangesAsync();
        }

        public async Task<Response<CardAmountResponse>> ProcessCashTransaction(CardRequest model)
        {
           
                try
                {

                var cardAmountResponse = new CardAmountResponse();

                cardAmountResponse.IsProcessed = true;
                cardAmountResponse.NetTotal = 0;

                cardAmountResponse.IsFromCash = true;
                cardAmountResponse.CardMessage = "";

                var stationSett = stationSettingsRepo.GetAsQueryable().FirstOrDefault(o => o.Id == (long)model.StationId);//model.StationId
                string payModeCode = "CAS";
                   
                string billPOSCode = stationSett.POSStationCode;

                   if (model.VoucherNo =="" || model.VoucherNo==null)
                    {
                        model.BillnoLoad= getMaxBillNo();
                        model.VoucherNo=billPOSCode + model.BillnoLoad;
                    }
                 
                    
                    var cashResult = (from ir in settDetailsTempRepo.GetAsQueryable()
                                      join ud in transCodesRepo.GetAsQueryable() on ir.TransactionCode equals ud.Trans_code
                                      where ir.UserId == model.UserId && ud.InHouse == true
                                      && ud.CRDR == "D" && ud.Trans_code != "DIS"
                                      group ir by 1 into og
                                      select new
                                      {
                                          CardAmount = og.Sum(item => item.Amount)
                                      });
                    decimal? chngAmount = cashResult.Count() > 0 ? cashResult.First().CardAmount : 0;

                    var tempSettDetailsNet = settDetailsTempRepo.GetAll().FirstOrDefault(o => o.UserId == model.UserId && o.TransactionCode.Trim().ToUpper() == "NET");
                    decimal? netData = 0;
                    decimal? totalChange = 0;
                    if (tempSettDetailsNet != null)
                    {
                        netData = tempSettDetailsNet.Amount;
                    }

                    totalChange = netData - model.Cash;
                    if(totalChange<=0)
                    {
                         var pc=await ProcessCash(totalChange,model);

                        var tempSettDet = settDetailsTempRepo.GetAsQueryable().Where(o => o.UserId == model.UserId).ToList();
                        if (tempSettDet != null) settDetailsTempRepo.DeleteList(tempSettDet);
                     }
                else if (totalChange >0 && model.Cash==0)
                {

                    var cardAmountResult = (from ir in settDetailsTempRepo.GetAsQueryable()
                                      join ud in transCodesRepo.GetAsQueryable() on ir.TransactionCode equals ud.Trans_code
                                      where ir.UserId == model.UserId && ir.Card == true
                                      group ir by 1 into og
                                      select new
                                      {
                                          CardAmount = og.Sum(item => item.Amount)
                                      });

                    model.Cash = model.NetTotal - cardAmountResult.FirstOrDefault().CardAmount;
                    var pc = await ProcessCash(totalChange, model);

                    var tempSettDet = settDetailsTempRepo.GetAsQueryable().Where(o => o.UserId == model.UserId).ToList();
                    if (tempSettDet != null) settDetailsTempRepo.DeleteList(tempSettDet);
                }
                else
                {
                    cardAmountResponse.NetTotal = totalChange;
                    cardAmountResponse.IsProcessed = false;
                    cardAmountResponse.CardMessage = "Collect the Balance through Card Payment";
                }


                return Response<CardAmountResponse>.Success(cardAmountResponse, "Cash processed Successfully.");
                }
                catch (Exception ex)
                {
                   
                    return Response<CardAmountResponse>.Fail(new CardAmountResponse(), ex.Message);
                }
          
        }


        public async Task<Response<bool>> ProcessCash(decimal? totalChange,CardRequest model)
        {
            //using (var trans = new TransactionScope())
            var option = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,

                Timeout = TimeSpan.FromSeconds(300),

            };
            using (var trans = new TransactionScope(
        TransactionScopeOption.Required,
        new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted, Timeout = TimeSpan.FromSeconds(300) },
        TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    //string payModeCode = "CAS";
                    //string billPOSCode = "LA";
                    //var salesVoucherNo = getMaxBillNo();
                    //string voucherNo = billPOSCode + salesVoucherNo;


                    var settDetails = settDetailsRepo.GetAsQueryable().Where(o => o.OrderId == model.BillnoLoad).ToList();
                    if (settDetails != null) settDetailsRepo.DeleteList(settDetails);

                    var tempSalesTrRepo = posSalesTranDetailsRepo.GetAsQueryable().Where(o => o.InvoiceNo == model.VoucherNo).ToList();
                    if (tempSalesTrRepo != null) posSalesTranDetailsRepo.DeleteList(tempSalesTrRepo);



                    decimal? netTotal = 0;
                    var settDetTemp = settDetailsTempRepo.GetAll().FirstOrDefault(o => o.UserId == model.UserId && o.TransactionCode.Trim().ToUpper() == "BIL");
                    if (settDetTemp != null) netTotal = settDetTemp.Amount;

                    decimal? disc = 0;
                    settDetTemp = settDetailsTempRepo.GetAll().FirstOrDefault(o => o.UserId == model.UserId && o.TransactionCode.Trim().ToUpper() == "BIL");
                    if (settDetTemp != null) disc = settDetTemp.Amount;

                    decimal? vat = 0;
                    settDetTemp = settDetailsTempRepo.GetAll().FirstOrDefault(o => o.UserId == model.UserId && o.TransactionCode.Trim().ToUpper() == "BIL");
                    if (settDetTemp != null) vat = settDetTemp.Amount;

                    netTotal = netTotal - disc;


                    var cashResult = (from ir in settDetailsTempRepo.GetAsQueryable()
                                      join ud in transCodesRepo.GetAsQueryable() on ir.TransactionCode.Trim().ToUpper() equals ud.Trans_code.Trim().ToUpper()
                                      where ir.UserId == model.UserId && ud.InHouse == true
                                      && ud.CRDR == "D" && ud.Trans_code.Trim().ToUpper() != "DIS"
                                      group ir by 1 into og
                                      select new
                                      {
                                          CardAmount = og.Sum(item => item.Amount)
                                      });
                    decimal? cardAmount = cashResult.Count() > 0 ? cashResult.First().CardAmount : 0;

                    decimal? payAmount = 0;
                    decimal? cashAmount = 0;
                    decimal? cashAmount1 = 0;

                    if (model.NetTotal < 0)
                    {
                        cashAmount1 = model.NetTotal + model.CardAmount + model.Cash;
                    }
                    else
                    {
                        cashAmount1 = model.NetTotal - model.CardAmount - model.Cash;
                    }

                    if (model.Cash >= model.NetTotal)
                    {
                        if (model.NetTotal < 0)
                        {
                            cashAmount = model.Cash * -1;
                        }
                        else
                        {
                            cashAmount = model.NetTotal;
                        }
                    }
                    else if (model.Cash < model.NetTotal)
                    {
                        cashAmount = model.Cash + cashAmount1;
                    }


     

                    //var tempSettDetailsNet = settDetailsTempRepo.GetAll().FirstOrDefault(o => o.UserId == model.UserId && o.TransactionCode == "NET");
                    //decimal? netData = 0;
                    //decimal? totalChange = 0;
                    //if (tempSettDetailsNet != null)
                    //{
                    //    netData = tempSettDetailsNet.Amount;
                    //}

                    //totalChange = netData - model.Cash;
                    //if (totalChange <= 0)
                    //{

                    //}
           

                    var tempSettDetails = settDetailsTempRepo.GetAsQueryable().Where(o => o.UserId == model.UserId && o.TransactionCode.Trim().ToUpper() == "CAS").ToList();
                    if (tempSettDetails != null) settDetailsTempRepo.DeleteList(tempSettDetails);

                    if (cashAmount1 < 0 && model.SalesRet == true)
                    {
                        if (model.Cash != 0)
                        {
                            if (cashAmount != 0)
                            {
                                var settDetailId = getMaxSettlementDetails();
                                var settDetailModel = new POS_SettlementDetTemp()
                                {
                                    SettlementDetId = settDetailId,
                                    OrderId = model.BillnoLoad,
                                    Amount = cashAmount,
                                    Status = "Y",
                                    Date = DateTime.Now,//to be changed
                                    UserId = model.UserId,
                                    TransactionCode = "CAS"
                                };
                                settDetailsTempRepo.Insert(settDetailModel);
                            }
                            else
                            {
                                var settDetailId = getMaxSettlementDetails();
                                var settDetailModel = new POS_SettlementDetTemp()
                                {
                                    SettlementDetId = settDetailId,
                                    OrderId = model.BillnoLoad,
                                    Amount = model.Cash,
                                    Status = "Y",
                                    Date = DateTime.Now,//to be changed
                                    UserId = model.UserId,
                                    TransactionCode = "CAS"
                                };
                                settDetailsTempRepo.Insert(settDetailModel);
                            }
                        }
                        else
                        {
                            var settDetailId = getMaxSettlementDetails();
                            var settDetailModel = new POS_SettlementDetTemp()
                            {
                                SettlementDetId = settDetailId,
                                OrderId = model.BillnoLoad,
                                Amount = cashAmount1,
                                Status = "Y",
                                Date = DateTime.Now,//to be changed
                                UserId = model.UserId,
                                TransactionCode = "CAS"
                            };
                            settDetailsTempRepo.Insert(settDetailModel);
                        }
                    }
                    else
                    {
                        var settDetailId = getMaxSettlementDetails();
                        var settDetailModel = new POS_SettlementDetTemp()
                        {
                            SettlementDetId = settDetailId,
                            OrderId = model.BillnoLoad,
                            Amount = cashAmount,
                            Status = "Y",
                            Date = DateTime.Now,//to be changed
                            UserId = model.UserId,
                            TransactionCode = "CAS"
                        };
                        settDetailsTempRepo.Insert(settDetailModel);
                    }

                    //TRANSACTION
                    var tempSettDetailsList = settDetailsTempRepo.GetAll().Where(o => o.UserId == model.UserId).ToList();
                    foreach (var item in tempSettDetailsList)
                    {
                        if (item.TransactionCode.Trim().ToUpper() == "BIL")
                        {
                            SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)item.Amount
                                , model.StationName, "BIL", (int)model.UserId, "BILL AMOUNT");
                        }

                        if (item.TransactionCode.Trim().ToUpper() == "DIS")
                        {
                            SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)item.Amount
                                , model.StationName, "DIS", (int)model.UserId, "DISCOUNT");
                        }

                        if (item.TransactionCode.Trim().ToUpper() == "VAT")
                        {
                            if (item.Amount < 0)
                            {
                                SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)item.Amount
                                                           , model.StationName, "VAT", (int)model.UserId, "VAT RETURN");
                            }
                            else
                            {
                                SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)item.Amount
                                                          , model.StationName, "VAT", (int)model.UserId, "VAT");
                            }
                        }

                        if (item.TransactionCode.Trim().ToUpper() == "VAMT")
                        {
                            if (item.Amount < 0)
                            {
                                SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)item.Amount
                                                           , model.StationName, "VAMTRET", (int)model.UserId, "VATABLE AMOUNT RETURNED");
                            }
                            else
                            {
                                SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)item.Amount
                                                          , model.StationName, "VAMT", (int)model.UserId, "VATABLE AMOUNT");
                            }
                        }

                        if (item.TransactionCode.Trim().ToUpper() == "NET")
                        {
                            SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)item.Amount
                                , model.StationName, "NET", (int)model.UserId, "NET AMOUNT");
                        }

                        if (item.TransactionCode.Trim().ToUpper() == "RCVE")
                        {
                            SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)item.Amount
                                , model.StationName, "RCVE", (int)model.UserId, "RECEIVED AMOUNT");
                        }

                        if (item.TransactionCode.Trim().ToUpper() == "CAS")
                        {
                            if (item.Amount < 0 && model.SalesRet == true)
                            {

                                if (model.CashAmount > 0)
                                {
                                    SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)item.Amount
                                                              , model.StationName, "CAS", (int)model.UserId, "CASH");
                                }
                                else
                                {
                                    SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)item.Amount
                                                 , model.StationName, "POTCS", (int)model.UserId, "CASH PAID OUT");
                                }

                            }
                            else
                            {
                                SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)item.Amount
                                                          , model.StationName, "CAS", (int)model.UserId, "CASH");
                            }
                        }


                        if (item.TransactionCode.Trim().ToUpper() == "RF")
                        {
                            SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)item.Amount
                                , model.StationName, "RF", (int)model.UserId, "Round OFF");
                        }
                    }

                    //Card Transactions
                    var settDetailsCard = await (from ir in settDetailsTempRepo.GetAsQueryable()
                                             join ud in transCodesRepo.GetAsQueryable() on ir.TransactionCode.Trim().ToUpper() equals ud.Trans_code.Trim().ToUpper()
                                                 where ir.UserId == model.UserId && ir.Card == true
                                             select new
                                             {
                                                 Amount = ir.Amount,
                                                 TransactionCode = ir.TransactionCode,
                                                 TransDescription = ud.Trans_Description
                                             }).ToListAsync();


                    foreach (var item in settDetailsCard)
                    {
                        //Settlementdet
                        var settDetailsId = getMaxSettDetails();
                        var settDetail = new POS_SettlementDet()
                        {
                            SettlementDetId = settDetailsId,
                            OrderId = model.BillnoLoad,
                            Amount = item.Amount,
                            Status = "Y",
                            Date = DateTime.Now,//to be changed
                            UserId = model.UserId,
                            TransactionCode = item.TransactionCode,

                        };
                        settDetailsRepo.Insert(settDetail);

                        SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)item.Amount
                               , model.StationName, item.TransactionCode, (int)model.UserId, item.TransDescription);

                    }

                    int cardId = 0;
                    var groupMaster = posTranGroupMasterRepo.GetAll().FirstOrDefault(o => o.Trans_GroupName == "CREDIT CARD");
                    if (groupMaster != null) cardId = groupMaster.Id;

                    var tempRepResult = (from ir in settDetailsTempRepo.GetAsQueryable()
                                         join ud in transCodesRepo.GetAsQueryable() on ir.TransactionCode.Trim().ToUpper() equals ud.Trans_code.Trim().ToUpper()
                                         join tg in posTranGroupMasterRepo.GetAsQueryable() on ud.Trans_group equals tg.Id
                                         where tg.Id == cardId
                                         select new
                                         {
                                             Id = ir.SettlementDetId
                                         });

                    string paymentmode = "CASH";
                    string transDesc = "CASH SALES";
                    string transtype = "CASH";

                    if (tempRepResult.Count() > 0)
                    {
                        paymentmode = "CASH AND CARD";
                        transDesc = "CASH AND CARD SALES";
                        transtype = "SPLIT";
                    }
                    SaveVoucher(model,(long)model.BillnoLoad,transtype,transDesc, paymentmode, "CASH CUSTOMER");
                    SavesettlementTrans(model, (long)model.BillnoLoad);
                    trans.Complete();
                    return Response<bool>.Success(true, "Cash processed Successfully.");
                }
                catch (Exception ex)
                {
                    trans.Dispose();
                    return Response<bool>.Fail(false, ex.Message);
                }
            }
        }




        public void SaveVoucher(CardRequest model,long voucherId,string transType,
            string transDesc, string paymentMode,string customerName)
        //  public void SaveVoucher(string voucherNo, int workPeriodId, decimal amount,
        //string stationName, string payMode, int userId, string desc,
        //      string customerName, int customerId
        //      , int saleManId)
        {
            try
            {
                string customerAccountNo = null;
               var custDetails = customerMasterRepo.GetAll().FirstOrDefault(o => o.CustomerMasterCustomerNo == model.CustomerId);
                if (custDetails != null) customerAccountNo = custDetails.CustomerMasterCustomerReffAcNo;

                var tempSalesV = posSalesVoucherRepo.GetAll().FirstOrDefault(o => o.VoucherNo == model.VoucherNo);
                if (tempSalesV != null) posSalesVoucherRepo.Delete(tempSalesV);

                var tempSalesVD = posSalesVoucherDetailsRepo.GetAll().FirstOrDefault(o => o.VoucherNo == model.VoucherNo);
                if (tempSalesVD != null) posSalesVoucherDetailsRepo.Delete(tempSalesVD);

                var tempStockR = stockRegisterRepo.GetAll().FirstOrDefault(o => o.StockRegisterPurchaseID == model.VoucherNo);
                if (tempStockR != null) stockRegisterRepo.Delete(tempStockR);

                var tempAccTrans = accTransRepo.GetAll().FirstOrDefault(o => o.AccountsTransactionsVoucherNo == model.VoucherNo);
                if (tempAccTrans != null) accTransRepo.Delete(tempAccTrans);

                decimal? vatAmount = 0;
                var tempVatD = settDetailsTempRepo.GetAll().FirstOrDefault(o => o.UserId == model.UserId && o.TransactionCode.Trim().ToUpper() == "VAT");
                if (tempVatD != null)
                {
                    if(tempVatD.Amount==null)
                    {
                        vatAmount = 0;
                    }
                    else
                    {
                        vatAmount = tempVatD.Amount;
                    }
                }

                decimal? discount = 0;
                var tempVatDis = settDetailsTempRepo.GetAll().FirstOrDefault(o => o.UserId == model.UserId && o.TransactionCode.Trim().ToUpper() == "DIS");
                if (tempVatDis != null)
                {
                    if (tempVatDis.Amount == null)
                    {
                        discount = 0;
                    }
                    else
                    {
                        discount = tempVatDis.Amount;
                    }
                }


                var currency=gSettingRepo.GetAll().FirstOrDefault(o => o.GeneralSettingsKeyValue.Trim().ToUpper() == "DEFAULTCURRENCY");
                string defCurr = currency.GeneralSettingsTextValue;

                int indexPOS=defCurr.IndexOf("::");
                string defaultCur = "";//UAE Dirham :: 5
                int le = defCurr.Length;
                if (indexPOS>0)
                {
                    defaultCur= defCurr.Substring(indexPOS+2).Trim();
                }
                else
                {
                    defaultCur = null;
                }


                var salesVC = new POS_SalesVoucher()
                {
                   // V_ID=voucherId,
                    VoucherNo = model.VoucherNo,
                    ShortNo=voucherId,
                    VoucherDate = DateTime.Now,//Format(AppLogDates, "dd/MMM/yyyy hh:mm:ss")
                    Voucher_Type =transType,
                    CustomerName= custDetails.CustomerMasterCustomerName,
                    Customer_ID=model.CustomerId,
                    Location=model.LocationId,
                    Salesman=model.SalesManId,
                    Discount=discount,
                    GrossAmount=model.NetTotal,
                    Vatable_TotAmt=(double)model.NetTotal,
                    VatAmount=vatAmount,
                    NetAmount= model.NetTotal + (vatAmount ) - model.BillDiscount,
                    Remarks=model.StationName,
                    UserId=model.UserId,
                    CurrencyId=Convert.ToInt64(defaultCur),
                    FSNO=1,
                    Description= "POS SALES",
                    CompanyId=1,
                    Refrence="POS",
                    JobId=0,
                    Currency_ID=Convert.ToInt64(defaultCur),
                    Cust_PODate= DateTime.Now,//Format(AppLogDates, "dd/MMM/yyyy hh:mm:ss"),
                    NetDiscount =model.BillDiscount,
                    CashCustName= custDetails.CustomerMasterCustomerName,
                    Shipping_Address ="",
                    InvoiceType= "POS",
                    InvoiceStatus="",
                    WorkPeriodID=model.WorkPeriodId,
                    StationID=model.StationId,
                    PaymentMode=paymentMode,
                    refno= model.RefereceNo,
                    DiscountPer=model.BillDiscountPerc
                };

                posSalesVoucherRepo.Insert(salesVC);
               // posSalesVoucherRepo.SaveChangesAsync();

                foreach (var item in model.ItemDetails)
                {
                    var salesVDetails = new POS_SalesVoucherDetails()
                    {
                        V_ID=salesVC.V_ID,
                        VoucherNo = model.VoucherNo,
                        ItemId =item.ItemId,
                        UnitId =item.UnitId,
                        Sold_Qty = item.Quantity,
                        UnitPrice =item.Price,
                        VatableAmt =item.Total,
                        CostPrice = item.Price,//needs to change
                        GrossAmt = item.Total,
                        Discount = item.Discount,
                        NetAmount = item.NetTotal,
                        FSNO = 1,
                        SNO=1,
                        LocationID = model.LocationId,
                        CompanyId = 1,
                        WorkPeriodID = model.WorkPeriodId,
                        StationID = model.StationId
                    };

                    posSalesVoucherDetailsRepo.Insert(salesVDetails);
                  
                }
               // posSalesVoucherDetailsRepo.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }

        }
        public  void SaveTransaction(string voucherNo,int workPeriodId,decimal amount,
            string stationName,string payMode,int userId,string desc)
        {
           
                try
                {
                    

                    var saleTr = new POS_SalesTransactionDetails()
                    {
                        InvoiceNo = voucherNo,
                        InvoiceDate = DateTime.Now,
                        WorkPeriodId = workPeriodId,
                        PaymentMode = payMode,
                        Amount = amount,//to be changed
                        CounterName =stationName,
                        Description =desc,
                        UserId=userId,
                        Status="Y"

                    };
                    posSalesTranDetailsRepo.Insert(saleTr);
                   // posSalesTranDetailsRepo.SaveChangesAsync();
            }
                catch (Exception ex)
                {
                throw ex;
                    
                }
          
        }


            #endregion

        }
}
