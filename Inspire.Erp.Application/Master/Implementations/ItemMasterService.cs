using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inspire.Erp.Application.Master
{
    public class ItemMasterService : IItemMasterService
    {
        private IRepository<ItemMaster> itemrepository;
        private IRepository<ItemStockType> itemStockRepository;
        private IRepository<UnitDetails> _unitDetailsRepo;
        public ItemMasterService(IRepository<ItemMaster> _itemrepository, IRepository<ItemStockType> _itemStockRepository, IRepository<UnitDetails> unitDetailsRepo)
        {
            itemrepository = _itemrepository;
            itemStockRepository = _itemStockRepository;
            _unitDetailsRepo = unitDetailsRepo;
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
                itemrepository.Update(itemMaster);
                valid = true;
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
                    ItemMasterItemName = k.ItemMasterItemName

                }).ToList(); ;
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
    }
}
