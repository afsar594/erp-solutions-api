using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.DTO.ItemMaster
{
    public class ItemMasterDto
    {
        public int ItemMasterItemId { get; set; }
        public string ItemMasterItemName { get; set; }
        public int ItemMasterVenderId { get; set; }
        public string ItemMasterItemSize { get; set; }
        public string ItemMasterBarcode { get; set; }
    }
}
