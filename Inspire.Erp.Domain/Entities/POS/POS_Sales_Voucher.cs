//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Text;

//namespace Inspire.Erp.Domain.Entities.POS
//{
//    public class POS_Sales_Voucher
//    {
//        public decimal V_Id { get; set; }
//        public string VoucherNo { get; set; }
//        public long? ShortNo { get; set; }
//        public DateTime? VoucherDate { get; set; }
//        public string Voucher_Type { get; set; }
//        public string CustomerName { get; set; }
//        public decimal? Customer_ID { get; set; }
//        public int? Location { get; set; }
//        public int? Salesman { get; set; }
//        public decimal? Discount { get; set; }
//        public decimal? GrossAmount { get; set; }
//        public decimal? NetAmount { get; set; }
//        public int? CompanyId { get; set; }
//        public string Remarks { get; set; }
//        public long? UserId { get; set; }
//        public string Refrence { get; set; }
//        public decimal? NetDiscount { get; set; }
//        public string CashCustName { get; set; }
//        public decimal? DiscountPer { get; set; }
//        public string InvoiceType { get; set; }
//        public decimal? VatAmount { get; set; }
//        public long? StationID { get; set; }
//        public long? WorkPeriodID { get; set; }
//        public string PaymentMode { get; set; }
//        public decimal? Vat_AMT { get; set; }
//        public decimal? Vat_Per { get; set; }
//        public string Vat_RoundSign { get; set; }
//        public decimal? Vat_RountAmt { get; set; }
//        public long? DeptID { get; set; }
//        public decimal? Vatable_TotAmt { get; set; }
//        public string refno { get; set; }
//        public bool HOLD_B { get; set; }
//        public int Counter_ID_N { get; set; }
//        public int Shift_ID_N { get; set; }
//        public bool? Retail_B { get; set; }
//        public string SV_SM_VoucherNo { get; set; }
//        public decimal? SV_SM_Bal { get; set; }
//        public bool? SV_NoVat_B { get; set; }
//        public decimal? Vat_TaxableAmt { get; set; }

//        [NotMapped]
//        public List<POS_Sales_Voucher_Details> posSalesVoucherDetails { get; set; }
//    }
//}


