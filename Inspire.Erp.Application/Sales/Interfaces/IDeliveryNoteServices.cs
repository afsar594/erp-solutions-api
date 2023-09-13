using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
namespace Inspire.Erp.Application.Sales.Interfaces
{
    public interface IDeliveryNoteServices
    {
        ////public IEnumerable<DepartmentMaster> SalesVoucher_GetAllDepartmentMaster();
        ////public IEnumerable<CustomerMaster> SalesVoucher_GetAllCustomerMaster();
        ////public IEnumerable<CustomerMaster> SalesVoucher_GetAllSalesManMaster();
        //public IEnumerable<ReportSalesVoucher> SalesVoucher_GetReportSalesVoucher();
        ////public IEnumerable<ItemMaster> SalesVoucher_GetAllItemMaster();
        ////public IEnumerable<UnitMaster> SalesVoucher_GetAllUnitMaster();
        ////public IEnumerable<SuppliersMaster> SalesVoucher_GetAllSuppliersMaster();
        ////public IEnumerable<LocationMaster> SalesVoucher_GetAllLocationMaster();
        //public SalesVoucher InsertSalesVoucher(SalesVoucher salesVoucher,
        //  List<SalesVoucherDetails> salesVoucherDetails);

        public DeliveryNoteModel InsertDeliveryNote(CustomerDeliveryNote deliveryNote,List<CustomerDeliveryNoteDetails> deliveryNoteDetails);
        public DeliveryNoteModel UpdateDeliveryNote(CustomerDeliveryNote deliveryNote, List<CustomerDeliveryNoteDetails> deliveryNoteDetails);
        public int DeleteDeliveryNote(CustomerDeliveryNote deliveryNote,  List<CustomerDeliveryNoteDetails> deliveryNoteDetails);
        public DeliveryNoteModel GetSavedDeliveryNoteDetails(long pvno);
        ////public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate);

        ////public IEnumerable<CustomerDeliveryNote> GetDeliveryNote();
        ////public VouchersNumbers GetVouchersNumbers(long pvno);
    }
}
