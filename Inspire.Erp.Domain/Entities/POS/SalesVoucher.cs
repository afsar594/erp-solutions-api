using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspire.Erp.Domain.Entities
{
    public partial class POS_SalesVoucher
    {
        public POS_SalesVoucher()
        {
            SalesVoucherDetails = new HashSet<POS_SalesVoucherDetails>();
        }
        public long V_ID { get; set; }
        public string VoucherNo { get; set; }
        public long ShortNo { get; set; }
        public string? VoucherDate { get; set; }
        public decimal? Voucher_Type { get; set; }
        public string? CustomerName { get; set; }
        public string? Customer_ID { get; set; }
        public string? Location { get; set; }
        public string? Salesman { get; set; }
        public string? Discount { get; set; }
        public string? GrossAmount { get; set; }
        public DateTime? NetAmount { get; set; }
        public string? Remarks { get; set; }
        public DateTime? UserId { get; set; }
        public string? CurrencyId { get; set; }
        public DateTime? FSNO { get; set; }
        public int? Description { get; set; }
        public int? CONO { get; set; }
        public string? DeliveryNote { get; set; }
        public DateTime? Cust_PONo { get; set; }
        public string? Fc_Rate { get; set; }
        public bool? CompanyId { get; set; }
        public int? Refrence { get; set; }
        public long? JobId { get; set; }
        public int? Currency_ID { get; set; }
        public int? Cust_PODate { get; set; }
        public int? NetDiscount { get; set; }
        public decimal? CashCustName { get; set; }
        public decimal? DiscountPer { get; set; }
        public string? Shipping_Address { get; set; }
        public decimal? PaymentTerms_V { get; set; }
        public decimal? TermsAndConditions_V { get; set; }
        public decimal? InvoiceType { get; set; }
        public decimal? ContractId { get; set; }
        public decimal? InvoiceStatus { get; set; }
        public decimal? SV_QtnID_N { get; set; }
        public decimal? SV_EqpmentID_N { get; set; }
        public string? VatAmount { get; set; }
        public decimal? StationID { get; set; }
        public decimal? WorkPeriodID { get; set; }
        public decimal? PaymentMode { get; set; }
        public string? Vat_AMT { get; set; }
        public string? Vat_Per { get; set; }
        public string? Vat_RoundSign { get; set; }
        public string? Vat_RountAmt { get; set; }
        public string? DeptID { get; set; }
        public string? Vatable_TotAmt { get; set; }
        public string? refno { get; set; }
        public string? Currency_Rate { get; set; }
        public string? SV_VesselId { get; set; }
        public string? HOLD_B { get; set; }
        public string? Counter_ID_N { get; set; }
        public bool? Shift_ID_N { get; set; }
        public long? Retail_B { get; set; }
        public string? SV_DNID_N { get; set; }
        public string? Course_ID { get; set; }
        public string? SV_SM_VoucherNo { get; set; }
        public string? SV_SM_Bal { get; set; }
        public string? SV_NoVat_B { get; set; }
        public string? Vat_TaxableAmt { get; set; }
        public string? LOOP_UPDATE { get; set; }
        public bool? SV_Job_ID { get; set; }
        public long? SV_Job_No { get; set; }
        public string? SV_Job_Date { get; set; }
        public bool? SV_Job_Adv_Amt { get; set; }
        public long? SV_Job_Balnce_Amt { get; set; }

        [NotMapped]
        public virtual ICollection<POS_SalesVoucherDetails> SalesVoucherDetails { get; set; }
    }
}
