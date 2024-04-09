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
        public ItemMasterService(IRepository<ItemMaster> _itemrepository, IRepository<ItemStockType> _itemStockRepository, IRepository<UnitDetails> unitDetailsRepo
            , IRepository<VendorMaster> _vendorrepository
            , IRepository<PurchaseVoucherDetails> _purchVouchDetailsRepo
            , IRepository<PurchaseVoucher> _purchVouchRepo
            , IRepository<SalesVoucherDetails> _saleVouchDetailsRepo
            , IRepository<SalesVoucher> _saleVouchRepo)
        {
            itemrepository = _itemrepository;
            vendorrepository = _vendorrepository;
            itemStockRepository = _itemStockRepository;
            _unitDetailsRepo = unitDetailsRepo;
            purchVouchDetailsRepo = _purchVouchDetailsRepo;
            purchVouchRepo = _purchVouchRepo;
            saleVouchDetailsRepo = _saleVouchDetailsRepo;
            saleVouchRepo = _saleVouchRepo;
        }
        public IEnumerable<ItemMaster> InsertItem(ItemMaster itemMaster)
        {
            bool valid = false;
            try
            {
                valid = true;
                itemMaster.ItemMasterItemId =  Convert.ToInt32(itemrepository.GetAsQueryable()
                                             .Where(x => x.ItemMasterItemId > 0 )
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


                }).ToList() ;

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
                                                            .Include(k =>k.UnitDetails).Select(k => k);

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

                var results =await (from ir in itemrepository.GetAsQueryable()
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
                                    where ir.ItemMasterItemType=="A"
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
                                   Stock=ir.ItemMasterCurrentStock??0/vd.ConTyp,
                                   LandingCost=ir.ItemMasterLandingCost,
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

                       .OrderBy(t => t.ItemName).ThenBy(x=>x.MaterialCode).ToListAsync();

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
            ((name != null && name.Trim() != "")? k.ItemMasterItemName.Equals(name): true
            || (group != null && group.Trim() != "") ? k.ItemMasterItemName.Equals(group) : true))
            && (k.ItemMasterDelStatus == false || k.ItemMasterDelStatus == null))
                .Include(k =>k.ItemPriceLevelDetails)
                .Include(x =>x.ItemImages)
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
    }
}
