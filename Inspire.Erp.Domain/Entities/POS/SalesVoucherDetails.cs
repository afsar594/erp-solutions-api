using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspire.Erp.Domain.Entities
{
    public partial class POS_SalesVoucherDetails
    {
        public long SDet_ID { get; set; }
        public long V_ID { get; set; }
        public string VoucherNo { get; set; }
        public long ItemId { get; set; }
        public int? Description { get; set; }
        public string BatchCode { get; set; }
        public int? UnitId { get; set; }
        public string Sold_Qty { get; set; }
        public string UnitPrice { get; set; }
        public DateTime? CostPrice { get; set; }
        public DateTime? GrossAmt { get; set; }
        public decimal? Discount { get; set; }
        public decimal? NetAmount { get; set; }
        public decimal? FSNO { get; set; }
        public decimal? SNO { get; set; }
        public decimal? DeliveryNote { get; set; }
        public decimal? LocationID { get; set; }
        public decimal? CompanyId { get; set; }
        public decimal? POD_ID { get; set; }
        public string Del_DetId { get; set; }
        public bool? Del_Id { get; set; }
        public int? Delv_id { get; set; }
        public int? SerialNo_V { get; set; }
        public int? VatAmount { get; set; }
        public int? StationID { get; set; }
        public int? WorkPeriodID { get; set; }
        public int? VatableAmt { get; set; }
        public int? SV_Remarks_V { get; set; }
        public int? SV_Origin_V { get; set; }
        public int? SV_VendorName_V { get; set; }
        public int? SVD_JCD_ID { get; set; }
        public decimal? SVD_Item_VAT { get; set; }
        public bool? SerialNoRef_ID { get; set; }
        public decimal? EditBit { get; set; }
        public bool? SV_SERIAL_NO_ALL { get; set; }
        [NotMapped]
        public virtual POS_SalesVoucher SalesVoucherDetailsNoNavigation { get; set; }
    }
}
