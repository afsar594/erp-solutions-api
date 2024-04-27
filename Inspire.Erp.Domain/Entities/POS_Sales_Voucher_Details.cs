using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace Inspire.Erp.Domain.Entities
{
    public class POS_Sales_Voucher_Details
    {
        public long SDet_ID { get; set; }
        public string VoucherNo { get; set; }
        public decimal? ItemId { get; set; }
        public string Description { get; set; }
        public string BatchCode { get; set; }
        public decimal? UnitId { get; set; }
        public decimal? Sold_Qty { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? GrossAmt { get; set; }
        public decimal? Discount { get; set; }
        public decimal? NetAmount { get; set;}
        public int? SNO { get; set; }
        public long? DeliveryNote { get; set; }
        public int? LocationID { get; set; }
        public int? CompanyId { get; set; }
        public string SerialNo_V { get; set; }
        public decimal? VatAmount { get; set; }
        public long? StationID { get; set; }
        public decimal? VatableAmt { get; set; }
        public decimal? SVD_Item_VAT { get; set; }
        public int? SerialNoRef_ID { get; set; }
        public int? EditBit { get; set; }
        public string SV_SERIAL_NO_ALL { get; set; }


    }
}
