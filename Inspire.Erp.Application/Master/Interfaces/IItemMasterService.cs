using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Modals;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Master
{
    public interface IItemMasterService
    {
        IEnumerable<ItemMaster> GetAllItemMaster();
        public IEnumerable<ItemMaster> InsertItem(ItemMaster itemMaster);
        public ItemMaster NewItem(ItemMaster itemMaster);
        public ItemMaster UpdateNewItem(ItemMaster itemMaster);
        public IEnumerable<ItemMaster> UpdateItem(ItemMaster itemMaster);
        public IEnumerable<ItemMaster> DeleteItem(ItemMaster itemMaster);
        public List<ItemMaster> GetAllItem();
        public IEnumerable<ItemMaster> GetAllItemById(int id);
        public List<ItemMaster> GetAllItemNotGroup();
        public IEnumerable<ItemStockType> GetAllStockType();
        public IEnumerable<ItemMaster> GetItemMastersByName(string name);
        public IEnumerable<ItemMaster> GetAllItemSearchFilter(string group, string name);

        public ItemMaster GetAllItemMasterById(int id);
        public ItemMaster GetAllItemMasterByBarCode(string  barCode);
        //   public IEnumerable<ItemMaster> GetAllItemSearchFilter(string group, string name);
        public IEnumerable<ItemMaster> GetAllItemGroupAndSubGroup();
        public int ItemMasterIdMax();

        Task<Response<List<ItemResponse>>> GetItemsBySearch(string filter);

        public Task<Response<ItemMasterResponse>> SearchItemByBarcodeLink(ItemBarCodeSearchFilter model);
    }
}