using Inspire.Erp.Application.Store.implementations;
using Inspire.Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
namespace Inspire.Erp.Application.Store.Interfaces
{
    public interface IManufactureItems
    {
        public ManufactureItemsMaster GetManufactureItemsById(int? id);
        public ManufactureItemsMaster InsertManufactureItems(ManufactureItemsMaster ManufactureMaster);
        public ManufactureItemsMaster UpdateManufactureItems(ManufactureItemsMaster ManufactureMaster);
        public IEnumerable<ManufactureItemsMaster> DeleteManufactureItems(string Id);
        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate);
        public ManufactureItemsMaster GetManufactureItemVoucherNo(string? id);

    }
}
