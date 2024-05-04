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

namespace Inspire.Erp.Application.Master
{
    public class ItemMasterService : IItemMasterService
    {
        private IRepository<ItemMaster> itemrepository;
        private IRepository<VendorMaster> vendorrepository;
        private IRepository<ItemStockType> itemStockRepository;
        private IRepository<UnitDetails> _unitDetailsRepo;
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
        private IRepository<POS_SalesTransactionDetails> posSalesTranDetailsRepo;

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
            )
        {
            itemrepository = _itemrepository;
            vendorrepository = _vendorrepository;
            itemStockRepository = _itemStockRepository;
            _unitDetailsRepo = unitDetailsRepo;
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
            this.transCodesRepo = _transCodesRepo;
            posSalesTranDetailsRepo = _posSalesTranDetailsRepo;

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

                    barqty = String.Format("{0:000}", Convert.ToDouble(barqty) / (double)1000);
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
                        barqty = String.Format("{0:000}", Convert.ToDouble(barqty) / (double)1000);
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
                barqty = String.Format("{0:000}", Convert.ToDouble(barqtys) / (double)1000);
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
                result.VATValue = (double)Math.Round((decimal)result.UnitPriceTaxIncl - (decimal)result.UnitPrice, 2);
            }
            else if (result.SalesTax != 0)
            {
                result.VATValue = (double)Math.Round(((decimal)result.UnitPriceTaxIncl * result.SalesTax) / 100, 2);
            }
            else if (result.SalesTax == 0)
            {
                result.UnitPrice = (double)Math.Round((decimal)result.UnitPriceTaxIncl, 2);
                result.VATValue = (double)Math.Round(0.0, 2);
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
                result.VATValue = (double)Math.Round((decimal)result.UnitPriceTaxIncl - (decimal)result.UnitPrice, 2);
            }
            else if (result.SalesTax != 0)
            {
                result.VATValue = (double)Math.Round(((decimal)result.UnitPriceTaxIncl * result.SalesTax) / 100, 2);
            }
            else if (result.SalesTax == 0)
            {
                result.UnitPrice = (double)Math.Round((decimal)result.UnitPriceTaxIncl, 2);
                result.VATValue = (double)Math.Round(0.0, 2);
            }
            return result;

        }


        public async Task<Response<List<ItemMasterResponse>>> GetItems(string barCode)
        {
            try
            {
                var result = await (from ir in itemrepository.GetAsQueryable().Where(x=>x.ItemMasterItemType=="A")
                                    join ud in _unitDetailsRepo.GetAsQueryable() on ir.ItemMasterItemId equals ud.UnitDetailsItemId
                                    where ir.ItemMasterItemName.Contains(barCode) || ud.UnitDetailsBarcode.Contains(barCode)
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


        public async Task<Response<ItemMasterResponse>> SearchItemByUnitDetailsId(ItemBarCodeSearchFilter model)
        {
            try
            {

                var result = await (from ir in itemrepository.GetAsQueryable()
                                    join ud in _unitDetailsRepo.GetAsQueryable() on ir.ItemMasterItemId equals ud.UnitDetailsItemId
                                    where ud.UnitDetailsId==model.unitDetailsId
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

                if (model.disableVat??false)
                {
                    result.SalesTax = 0;
                }

                if (result.ItemwiseInclusive && result.SalesTax != 0)
                {
                    result.SalesTax1 = (100 + result.SalesTax);
                    result.SalesTax1 = (result.SalesTax1 / (decimal)100);
                    result.UnitPrice = (double)Math.Round((decimal)result.UnitPriceTaxIncl / result.SalesTax1, 2);
                    // 'Unitprice = Math.Round(Val(Val(UpriceTaxIncl) / 1.05), 2)
                    result.VATValue = (double)Math.Round((decimal)result.UnitPriceTaxIncl - (decimal)result.UnitPrice, 2);
                }
                else if (result.SalesTax != 0)
                {
                    result.VATValue = (double)Math.Round(((decimal)result.UnitPriceTaxIncl * result.SalesTax) / 100, 2);
                }
                else if (result.SalesTax == 0)
                {
                    result.UnitPrice = (double)Math.Round((decimal)result.UnitPriceTaxIncl, 2);
                    result.VATValue = (double)Math.Round(0.0, 2);
                }

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
        public async Task<Response<List<SettlementDetailsResponse>>> AddSettlementDetails(SettlementDetailsRequest model)
        {
            using (var trans = new TransactionScope())
            {
                try
                {
                    var salesVoucherNo = getMaxSalesVoucher();

                    //Item Discount
                   
                    if(model.ItemDiscount>0)
                    {

                        var tempSettDetailsDis = settDetailsTempRepo.GetAll().FirstOrDefault(o => o.UserId == model.UserId && o.TransactionCode == "DIS");
                        if (tempSettDetailsDis != null) settDetailsTempRepo.Delete(tempSettDetailsDis);
                        var settDetailsIdDis = getMaxSettlementDetails();
                        var alloModelDis = new POS_SettlementDetTemp()
                        {
                            SettlementDetId = settDetailsIdDis,
                            OrderId = salesVoucherNo,
                            Amount = model.ItemDiscount,
                            Status = "Y",
                            Date = DateTime.Now,//to be changed
                            UserId = model.UserId,
                            TransactionCode = "DIS"

                        };
                        settDetailsTempRepo.Insert(alloModelDis);
                    }

                    //Bill Discount
                    if (model.BillDiscount > 0)
                    {
                        var tempSettDetails = settDetailsTempRepo.GetAll().FirstOrDefault(o => o.UserId == model.UserId && o.TransactionCode == "BIL");
                        if (tempSettDetails != null) settDetailsTempRepo.Delete(tempSettDetails);
                        var settDetailsId = getMaxSettlementDetails();
                        var alloModel = new POS_SettlementDetTemp()
                        {
                            SettlementDetId = settDetailsId,
                            OrderId = salesVoucherNo,
                            Amount = model.BillAmount,
                            Status = "Y",
                            Date = DateTime.Now,//to be changed
                            UserId = model.UserId,
                            TransactionCode = "BIL"

                        };
                        settDetailsTempRepo.Insert(alloModel);

                        tempSettDetails = settDetailsTempRepo.GetAll().FirstOrDefault(o => o.UserId == model.UserId && o.TransactionCode == "DIS");
                        if (tempSettDetails != null) settDetailsTempRepo.Delete(tempSettDetails);
                        settDetailsId = getMaxSettlementDetails();
                        alloModel = new POS_SettlementDetTemp()
                        {
                            SettlementDetId = settDetailsId,
                            OrderId = salesVoucherNo,
                            Amount = model.BillDiscount,
                            Status = "Y",
                            Date = DateTime.Now,//to be changed
                            UserId = model.UserId,
                            TransactionCode = "DIS"

                        };
                        settDetailsTempRepo.Insert(alloModel);

                        tempSettDetails = settDetailsTempRepo.GetAll().FirstOrDefault(o => o.UserId == model.UserId && o.TransactionCode == "VAT");
                        if (tempSettDetails != null) settDetailsTempRepo.Delete(tempSettDetails);
                        settDetailsId = getMaxSettlementDetails();
                        alloModel = new POS_SettlementDetTemp()
                        {
                            SettlementDetId = settDetailsId,
                            OrderId = salesVoucherNo,
                            Amount = model.DiscountVAT,
                            Status = "Y",
                            Date = DateTime.Now,//to be changed
                            UserId = model.UserId,
                            TransactionCode = "VAT"

                        };
                        settDetailsTempRepo.Insert(alloModel);

                    }
                    else
                    {
                        var tempSettDetailsOth = settDetailsTempRepo.GetAll().FirstOrDefault(o => o.UserId == model.UserId && o.TransactionCode == "VAT");
                        if (tempSettDetailsOth != null) settDetailsTempRepo.Delete(tempSettDetailsOth);
                        var settDetailsIdOth = getMaxSettlementDetails();
                        var alloModelOth = new POS_SettlementDetTemp()
                        {
                            SettlementDetId = settDetailsIdOth,
                            OrderId = salesVoucherNo,
                            Amount = model.VAT,
                            Status = "Y",
                            Date = DateTime.Now,//to be changed
                            UserId = model.UserId,
                            TransactionCode = "VAT"

                        };
                        settDetailsTempRepo.Insert(alloModelOth);
                    }

                    var tempSettDetailsVat = settDetailsTempRepo.GetAll().FirstOrDefault(o => o.UserId == model.UserId && o.TransactionCode == "VAMT");
                    if (tempSettDetailsVat != null) settDetailsTempRepo.Delete(tempSettDetailsVat);
                    var settDetailsIdVat = getMaxSettlementDetails();
                    var alloModelVat = new POS_SettlementDetTemp()
                    {
                        SettlementDetId = settDetailsIdVat,
                        OrderId = salesVoucherNo,
                        Amount = model.BillAmount - model.BillDiscount - model.ItemDiscount,
                        Status = "Y",
                        Date = DateTime.Now,//to be changed
                        UserId = model.UserId,
                        TransactionCode = "VAMT"

                    };
                    settDetailsTempRepo.Insert(alloModelVat);

                    Double? netTotal=0;
                    if(model.VAT>0)
                    {
                        netTotal = (model.BillAmount - model.ItemDiscount) - model.BillDiscount + model.DiscountVAT;
                    }
                    else
                    {
                        netTotal = (model.BillAmount - model.ItemDiscount) - model.BillDiscount + model.DiscountVAT;
                    }

                    var tempSettDetailsNet = settDetailsTempRepo.GetAll().FirstOrDefault(o => o.UserId == model.UserId && o.TransactionCode == "NET");
                    if (tempSettDetailsNet != null) settDetailsTempRepo.Delete(tempSettDetailsNet);
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
                        var tempSettDetailsCash = settDetailsTempRepo.GetAll().FirstOrDefault(o => o.UserId == model.UserId && o.TransactionCode == "RCVE");
                        if (tempSettDetailsCash != null) settDetailsTempRepo.Delete(tempSettDetailsCash);
                        var settDetailsIdCash = getMaxSettlementDetails();
                        var alloModelCash = new POS_SettlementDetTemp()
                        {
                            SettlementDetId = settDetailsIdCash,
                            OrderId = salesVoucherNo,
                            Amount = model.CashAmount,
                            Status = "Y",
                            Date = DateTime.Now,//to be changed
                            UserId = model.UserId,
                            TransactionCode = "RCVE"

                        };
                        settDetailsTempRepo.Insert(alloModelCash);

                        tempSettDetailsCash = settDetailsTempRepo.GetAll().FirstOrDefault(o => o.UserId == model.UserId && o.TransactionCode == "CHNG");
                        if (tempSettDetailsCash != null) settDetailsTempRepo.Delete(tempSettDetailsCash);
                        settDetailsIdCash = getMaxSettlementDetails();

                        var cashResult = (from ir in settDetailsTempRepo.GetAsQueryable()
                                            join ud in transCodesRepo.GetAsQueryable() on ir.TransactionCode equals ud.Trans_code
                                            where ir.UserId == 2 && ir.Card == true
                                                group ir by 1 into og
                                            select new 
                                            {
                                                CardAmount =og.Sum(item=>item.Amount)
                                            });

                        Double? cardAmount = cashResult.Count() > 0 ? cashResult.First().CardAmount : 0;
                        Double? netData = 0;
                       tempSettDetailsCash = settDetailsTempRepo.GetAll().FirstOrDefault(o => o.UserId == model.UserId && o.TransactionCode == "NET");
                        if (tempSettDetailsCash != null)
                        {
                            netData = tempSettDetailsCash.Amount;
                        }

                        Double? totalChange = netData-model.CashAmount-cardAmount;
                        if(model.VATRound > 0)
                        {
                            double? VATDAmt = 0;
                            if (model.VATRoundSign == "-")
                            {
                                VATDAmt = 0 - model.VATRoundAmount;
                            }
                            else
                            {
                                VATDAmt = model.VATRoundAmount;
                            }
                            tempSettDetailsCash = settDetailsTempRepo.GetAll().FirstOrDefault(o => o.UserId == model.UserId && o.TransactionCode == "RF");
                            if (tempSettDetailsCash != null) settDetailsTempRepo.Delete(tempSettDetailsCash);
                            alloModelCash = new POS_SettlementDetTemp()
                            {
                                SettlementDetId = settDetailsIdCash,
                                OrderId = salesVoucherNo,
                                Amount = VATDAmt,
                                Status = "Y",
                                Date = DateTime.Now,//to be changed
                                UserId = model.UserId,
                                TransactionCode = "RF"

                            };
                            settDetailsTempRepo.Insert(alloModelCash);
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
                        var tempSettDetailsCash = settDetailsTempRepo.GetAll().FirstOrDefault(o => o.UserId == model.UserId && o.TransactionCode == "RCVE");
                        if (tempSettDetailsCash != null) settDetailsTempRepo.Delete(tempSettDetailsCash);

                        tempSettDetailsCash = settDetailsTempRepo.GetAll().FirstOrDefault(o => o.UserId == model.UserId && o.TransactionCode == "CHNG");
                        if (tempSettDetailsCash != null) settDetailsTempRepo.Delete(tempSettDetailsCash);
                    }

                    #region ADD ACTIVITY LOGS
                    //AddActivityLogViewModel log = new AddActivityLogViewModel()
                    //{
                    //    Page = "Voucher Allocation",
                    //    Section = "Add Voucher Allocation ",
                    //    Entity = "Voucher Allocation ",

                    //};
                    //await _utilityService.AddUserTrackingLog(log);
                    #endregion


                    var result = await (from ir in settDetailsTempRepo.GetAsQueryable()
                                        join ud in transCodesRepo.GetAsQueryable() on ir.TransactionCode equals ud.Trans_code
                                        where ir.UserId == 2 && ud.show_in_inv==true && !ir.TransactionCode.Contains("BIL")
                                        select new SettlementDetailsResponse
                                        {
                                            TransactionDesc = ud.Trans_Description,
                                            Amount = ir.Amount,
                                            SettlementDetailsId = ir.SettlementDetId,
                                            SalesVoucherId = ir.OrderId,
                                            Status = ir.Status,
                                            SettlementDate = ir.Date,
                                            SortOrder = ud.Sort_order,
                                            ShowinInvoice = ud.show_in_inv

                                        }).OrderBy(x=>x.SortOrder).ToListAsync();

                   
                    trans.Complete();
                    return Response<List<SettlementDetailsResponse>>.Success(result, "Settlement Details inserted Successfully.");

                }
                catch (Exception ex)
                {
                    trans.Dispose();
                    return Response<List<SettlementDetailsResponse>>.Fail(new List<SettlementDetailsResponse>(), ex.Message);
                }
            }
        }


        public async Task<Response<CardAmountResponse>> CalcualteCardAmount(SettlementDetailsRequest model)
        {
            using (var trans = new TransactionScope())
            {
                try
                {
                    var cardAmountResponse = new  CardAmountResponse();
                    string billPOSCode = "LA";
                    var salesVoucherNo = getMaxSalesVoucher();
                    string voucherNo = billPOSCode + salesVoucherNo;

                    double? netTotal = 0;
                    var tempSettDetailsDis = settDetailsTempRepo.GetAll().FirstOrDefault(o => o.UserId == model.UserId && o.TransactionCode == "NET");
                    if (tempSettDetailsDis != null) netTotal=tempSettDetailsDis.Amount;

                    double? vat = 0;
                    tempSettDetailsDis = settDetailsTempRepo.GetAll().FirstOrDefault(o => o.UserId == model.UserId && o.TransactionCode == "VAT");
                    if (tempSettDetailsDis != null) vat = tempSettDetailsDis.Amount;

                    

                    double? balance = 0;
                    if (netTotal < 0)
                    {
                        balance = netTotal + model.CashAmount;
                    }
                    else
                    { 
                        balance=netTotal - model.CashAmount;
                    } 

                    double? cash = model.CashAmount;

                    var cashResult = await (from ir in settDetailsTempRepo.GetAsQueryable()
                                      join ud in transCodesRepo.GetAsQueryable() on ir.TransactionCode equals ud.Trans_code
                                      where ir.UserId == 2 && ir.Card == true
                                      group ir by 1 into og
                                      select new
                                      {
                                          CardAmount = og.Sum(item => item.Amount)
                                      }).ToListAsync();

                    double? lastBalance = 0;
                    if (cashResult.Count() > 0)
                    {
                        if (netTotal < 0)
                        {
                            lastBalance = netTotal + model.CashAmount;
                        }
                        else
                        {
                            lastBalance = netTotal - model.CashAmount;
                        }
                    }
                    else
                    {
                        if (netTotal < 0)
                        {
                            lastBalance = netTotal + cashResult.First().CardAmount + model.CashAmount;
                        }
                        else
                        {
                            lastBalance = netTotal  - cashResult.First().CardAmount - model.CashAmount;
                        }
                    }

                    cardAmountResponse.CardAmount = lastBalance;
                    cardAmountResponse.CashAmount = model.CashAmount;
                    cardAmountResponse.NetTotal = netTotal;

                    trans.Complete();
                    return Response<CardAmountResponse>.Success(cardAmountResponse, "Settlement Details inserted Successfully.");

                }
                catch (Exception ex)
                {
                    trans.Dispose();
                    return Response<CardAmountResponse>.Fail(new CardAmountResponse(), ex.Message);
                }
            }
        }

        public async Task<Response<double>> ProcessCard(CardAmountResponse model)
        {
            using (var trans = new TransactionScope())
            {
                try
                {
                    string billPOSCode = "LA";
                    var salesVoucherNo = getMaxSalesVoucher();
                    string voucherNo = billPOSCode + salesVoucherNo;

                    var tempSettDetailsCash = settDetailsRepo.GetAll().FirstOrDefault(o => o.OrderId ==Convert.ToInt64(model.VoucherNo));
                    if (tempSettDetailsCash != null) settDetailsRepo.Delete(tempSettDetailsCash);

                    var tempSalesTrRepo = posSalesTranDetailsRepo.GetAll().FirstOrDefault(o => o.InvoiceNo == model.VoucherNo);
                    if (tempSalesTrRepo != null) posSalesTranDetailsRepo.Delete(tempSalesTrRepo);

                   
                    var tempTransCodes = transCodesRepo.GetAll().FirstOrDefault(o => o.InHouse == true && o.Trans_Description == model.CardType);
                    string transCode = "";
                    string transDesc = "";
                    if (tempTransCodes != null)
                    {
                        transCode = tempTransCodes.Trans_code;
                        transDesc = tempTransCodes.Trans_Description;
                    }

                    var alloModelCash = new POS_SettlementDetTemp()
                    {
                        SettlementDetId = model.BillNoSetDet,
                        OrderId = model.BillnoLoad,
                        Amount = model.CashAmount,
                        Status = "Y",
                        Date = DateTime.Now,//to be changed
                        UserId = model.UserId,
                        TransactionCode = "CAS"

                    };
                    settDetailsTempRepo.Insert(alloModelCash);



                    double? vat = 0;
                    var tempSettDetailsList = settDetailsTempRepo.GetAll().Where(o => o.UserId == model.UserId).ToList();
                    foreach(var item in tempSettDetailsList )
                    {
                        if(item.TransactionCode== "BIL")
                        {
                            SaveTransaction(model.VoucherNo, model.WorkPeriodId,(decimal)model.CardAmount
                                , model.StationName, "BIL", (int)model.UserId, "BILL AMOUNT");
                        }

                        if (item.TransactionCode == "DIS")
                        {
                            SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)model.CardAmount
                                , model.StationName, "DIS", (int)model.UserId, "DISCOUNT");
                        }

                        if (item.TransactionCode == "VAT")
                        {
                            if(item.Amount<0)
                            {
                                SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)model.CardAmount
                                                           , model.StationName, "VAT", (int)model.UserId, "VAT RETURN");
                            }
                            else
                            {
                                SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)model.CardAmount
                                                          , model.StationName, "VAT", (int)model.UserId, "VAT");
                            }
                        }

                        if (item.TransactionCode == "VAMT")
                        {
                            if (item.Amount < 0)
                            {
                                SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)model.CardAmount
                                                           , model.StationName, "VAMTRET", (int)model.UserId, "VATABLE AMOUNT RETURNED");
                            }
                            else
                            {
                                SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)model.CardAmount
                                                          , model.StationName, "VAMT", (int)model.UserId, "VATABLE AMOUNT");
                            }
                        }

                        if (item.TransactionCode == "NET")
                        {
                            SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)model.CardAmount
                                , model.StationName, "NET", (int)model.UserId, "NET AMOUNT");
                        }

                        if (item.TransactionCode == "RCVE")
                        {
                            SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)model.CardAmount
                                , model.StationName, "RCVE", (int)model.UserId, "RECEIVED AMOUNT");
                        }

                        if (item.TransactionCode == "CAS")
                        {
                            if (item.Amount < 0)
                            {
                                SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)model.CardAmount
                                                           , model.StationName, "POTCS", (int)model.UserId, "CASH PAID OUT");
                            }
                            else
                            {
                                SaveTransaction(model.VoucherNo, model.WorkPeriodId, (decimal)model.CardAmount
                                                          , model.StationName, "CAS", (int)model.UserId, "CASH");
                            }
                        }
                    }

                    var settDetails =await (from ir in settDetailsTempRepo.GetAsQueryable()
                                      join ud in transCodesRepo.GetAsQueryable() on ir.TransactionCode equals ud.Trans_code
                                      where ir.UserId == model.UserId && ir.Card == true
                                      select new
                                      {
                                          Amount =ir.Amount,
                                          TransactionCode=ir.TransactionCode
                                      }).ToListAsync();

                    string paymentmode = "CARD";
                    string transtype = "CARD";
                    transDesc = "CARD SALES";

                    foreach(var item in settDetails)
                    {
                        if(item.TransactionCode.Trim()== "CAS")
                        { 
                            paymentmode = "CASH AND CARD";
                            transtype = "SPLIT";
                            transDesc = "CASH AND CARD SALES";
                        }
                    }

                    var settDetailsId = getMaxSettlementDetails();
                    var alloModel = new POS_SettlementDetTemp()
                    {
                        SettlementDetId = settDetailsId,
                        OrderId = salesVoucherNo,
                        Amount = model.CardAmount,
                        Status = "Y",
                        Date = DateTime.Now,//to be changed
                        UserId = model.UserId,
                        TransactionCode =transCode,
                        Card=true,
                        CreditCardNumber=model.CreditCardNumber
                    };
                    settDetailsTempRepo.Insert(alloModelCash);


                    double? lastBalance = 0;
               
                    trans.Complete();
                    return Response<double>.Success((double)lastBalance, "Settlement Details inserted Successfully.");

                }
                catch (Exception ex)
                {
                    trans.Dispose();
                    return Response<double>.Fail(0, ex.Message);
                }
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

                }
                catch (Exception ex)
                {
                   
                    
                }
          
        }


            #endregion

        }
}
